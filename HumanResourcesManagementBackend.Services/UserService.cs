﻿using HumanResourcesManagementBackend.Models;
using HumanResourcesManagementBackend.Repository;
using HumanResourcesManagementBackend.Services.Interface;
using HumanResourcesManagementBackend.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using static HumanResourcesManagementBackend.Models.UserDto;
using HumanResourcesManagementBackend.Repository.Extensions;

namespace HumanResourcesManagementBackend.Services
{
    /// <summary>
    /// 用户管理业务类
    /// </summary>
    public class UserService : IUserService
    {
        public List<UserDto.User> GetUsers(UserDto.Search search)
        {
            using(var db = new HRM())
            {
                var query = from user in db.Users
                            where user.Status != DataStatus.Deleted
                            select user;
                //条件过滤
                if (!string.IsNullOrEmpty(search.LoginName))
                {
                    query = query.Where(u => u.LoginName.Contains(search.LoginName));
                }
                if (search.Status > 0)
                {
                    query = query.Where(u => u.Status == search.Status);
                }
                //分页并将数据库实体映射为dto对象(OrderBy必须调用)
                var list = query.OrderBy(q => q.CreateTime).Pageing(search).MapToList<UserDto.User>();
                //状态处理
                list.ForEach(u =>
                {
                    u.StatusStr = u.Status.Description();
                });
                return list;
            }
        }

        public UserDto.User GetUserById(long id)
        {
            using(var db = new HRM())
            {
                var userR = db.Users.FirstOrDefault(u => u.Id == id && u.Status != DataStatus.Deleted);
                if (userR == null)
                {
                    throw new BusinessException
                    {
                        Status = ResponseStatus.NoData,
                        ErrorMessage = ResponseStatus.NoData.Description()
                    };
                }
                var user = userR.MapTo<UserDto.User>();
                user.StatusStr = user.Status.Description();
                return user;
            }
        }

        public UserDto.User Login(UserDto.Login login)
        {
            using(var db = new HRM())
            {
                //密码加密后比较
                login.Password = login.Password.Encrypt();
                var userR = (from u in db.Users
                            where u.LoginName == login.LoginName && u.Password == login.Password
                            select u).FirstOrDefault();
                //空判断简写  ==  if(user == null) {throw new BusinessException} else {return user.Id}
                if(userR == null)
                {
                    throw new BusinessException
                    {
                        ErrorMessage = "用户名或密码错误",
                        Status = ResponseStatus.NoPermission
                    };
                }
                if(userR.Status != DataStatus.Enable)
                {
                    throw new BusinessException
                    {
                        ErrorMessage = "账号禁止登陆",
                        Status = ResponseStatus.NoPermission
                    };
                }

                var user = userR.MapTo<UserDto.User>();
                user.StatusStr = user.Status.Description();
                return user;
            }
        }

        public void AddUser(UserDto.Save user)
        {
            using(var db = new HRM())
            {
                //安全保障
                user.Id = 0;
                //查询是否存在
                var userEx = db.Users.FirstOrDefault(u => u.LoginName == user.LoginName);
                if(userEx != null)
                {
                    throw new BusinessException
                    {
                        ErrorMessage = "用户已存在",
                        Status = ResponseStatus.ParameterError
                    };
                }
                
                var flag = db.Transaction(() =>
                {
                    //DTO映射为真正的实体，添加到数据库中
                    var userR = user.MapTo<R_User>();
                    userR.Password = userR.Password.Encrypt();
                    userR.CreateTime = DateTime.Now;
                    userR.UpdateTime = DateTime.Now;
                    userR = db.Users.Add(userR);
                    db.SaveChanges();
                    if(userR == null || userR.Id == 0)
                    {
                        return false;
                    }
                    //绑定默认角色
                    var defaultRoles = db.Roles.Where(r => r.IsDefault == YesOrNo.Yes && r.Status == DataStatus.Enable).ToList();
                    if (defaultRoles != null || defaultRoles.Count > 0)
                    {
                        var bindList = defaultRoles.Select(r => new R_UserRoleRef
                        {
                            UserId = userR.Id,
                            RoleId = r.Id,
                            CreateTime = DateTime.Now,
                            UpdateTime = DateTime.Now,
                        }).ToList();
                        var bindResult = db.UserRoleRefs.AddRange(bindList).ToList();
                        db.SaveChanges();
                        if (bindResult == null || bindList.Count != bindResult.Count)
                        {
                            return false;
                        }
                    }

                    return true;
                });

                if (!flag)
                {
                    throw new BusinessException
                    {
                        ErrorMessage = "用户添加失败",
                        Status = ResponseStatus.AddError
                    };
                }
            }
        }

        public void EditUser(UserDto.Save user)
        {
            using (var db = new HRM())
            {
                //查询是否存在
                var userEx = db.Users.FirstOrDefault(u => u.Id == user.Id);
                if (userEx == null)
                {
                    throw new BusinessException
                    {
                        ErrorMessage = "用户不存在",
                        Status = ResponseStatus.ParameterError
                    };
                }
                //在Context里查询到对象才能这样赋值做修改操作，自己new是不行的
                if (string.IsNullOrEmpty(userEx.Password))
                {
                    userEx.Password = user.Password.Encrypt();
                }
                if (string.IsNullOrEmpty(userEx.LoginName))
                {
                    userEx.LoginName = user.LoginName;
                }
                userEx.LoginName = user.LoginName;
                userEx.Password = user.Password.Encrypt();
                userEx.UpdateTime = DateTime.Now;
                userEx.Question = user.Question;
                userEx.Answer = user.Answer;
                userEx.EmployeeId = user.EmployeeId;
                userEx.Status = user.Status;

                if (db.SaveChanges() == 0)
                {
                    throw new BusinessException
                    {
                        ErrorMessage = "用户修改失败",
                        Status = ResponseStatus.AddError
                    };
                }
                AuthService.FlushPermissionCache(user.Id);
            }
        }

        public void DeleteUser(long userId)
        {
            using (var db = new HRM())
            {
                var user = db.Users.FirstOrDefault(u => u.Id == userId) ?? throw new BusinessException
                {
                    ErrorMessage = "用户不存在",
                    Status = ResponseStatus.ParameterError
                };

                user.Status = DataStatus.Deleted;
                user.UpdateTime = DateTime.Now;
                db.SaveChanges();

                AuthService.FlushPermissionCache(user.Id);
            }
        }

        public void ChangePwd(UserDto.ChangePwd changePwd)
        {
            using (var db = new HRM())
            {
                var user = db.Users.FirstOrDefault(p=>p.Id == changePwd.Id);
                if(user == null)
                {
                    throw new BusinessException
                    {
                        ErrorMessage = "参数错误",
                        Status = ResponseStatus.ParameterError
                    };
                }
                user.Password=user.Password.Decrypt();
                if(user.Password!=changePwd.Password)
                {
                    throw new BusinessException
                    {
                        ErrorMessage = "初始密码输入错误",
                        Status = ResponseStatus.ParameterError
                    };
                }
                user.Password = changePwd.NewPassword.Encrypt();
                if (db.SaveChanges() == 0)
                {
                    throw new BusinessException
                    {
                        ErrorMessage = "修改密码失败,正在维护",
                        Status = ResponseStatus.AddError
                    };
                }
            }
        }

        public void ForgotPassword(UserDto.ChangePwd changePwd)
        {
            using (var db=new HRM())
            {
                if (string.IsNullOrEmpty(changePwd.Password) || string.IsNullOrEmpty(changePwd.NewPassword))
                {
                    throw new BusinessException
                    {
                        ErrorMessage = "密码不能为空",
                        Status = ResponseStatus.ParameterError
                    };
                }
                var user = db.Users.FirstOrDefault(p=>p.LoginName==changePwd.LoginName);
                if (user==null)
                {
                    throw new BusinessException
                    {
                        ErrorMessage ="用户不存在",
                        Status = ResponseStatus.ParameterError
                    };
                }
                else if(user.Answer != changePwd.Answer)
                {
                    throw new BusinessException
                    {
                        ErrorMessage = "答案错误",
                        Status = ResponseStatus.ParameterError
                    };
                }
                user.Password = user.Password.Decrypt();
                if (user.Password == changePwd.NewPassword)
                {
                    throw new BusinessException
                    {
                        ErrorMessage = "新密码不能与旧密码相同",
                        Status = ResponseStatus.ParameterError
                    };
                }
                user.Password=changePwd.NewPassword.Encrypt();
                if (db.SaveChanges() == 0)
                {
                    throw new BusinessException
                    {
                        ErrorMessage = "修改密码失败,正在维护",
                        Status = ResponseStatus.AddError
                    };
                }
            }
        }

        public void ChangeQuestion(UserDto.ChangePwd changeQuestion)
        {
            using (var db=new HRM())
            {
                changeQuestion.Password = changeQuestion.Password.Encrypt();
                var user = db.Users.FirstOrDefault(p=>p.LoginName== changeQuestion.LoginName && p.Password== changeQuestion.Password);
                if(user==null)
                {
                    throw new BusinessException
                    {
                        ErrorMessage = "密码输入错误",
                        Status = ResponseStatus.ParameterError
                    };
                }
                user.Question= changeQuestion.Question;
                user.Answer= changeQuestion.Answer;
                if (db.SaveChanges() == 0)
                {
                    throw new BusinessException
                    {
                        ErrorMessage = "修改密保失败,正在维护",
                        Status = ResponseStatus.AddError
                    };
                }
            }
        }

        public void ChangeStatus(Save user)
        {
            using (var db = new HRM())
            {
                //查询是否存在
                var userEx = db.Users.FirstOrDefault(u => u.Id == user.Id);
                if (userEx == null)
                {
                    throw new BusinessException
                    {
                        ErrorMessage = "用户不存在",
                        Status = ResponseStatus.ParameterError
                    };
                }

                userEx.UpdateTime = DateTime.Now;
                userEx.Status = user.Status;

                if (db.SaveChanges() == 0)
                {
                    throw new BusinessException
                    {
                        ErrorMessage = "用户修改失败",
                        Status = ResponseStatus.UpdateError
                    };
                }

                AuthService.FlushPermissionCache(user.Id);
            }
        }

        public User GetUserByLoginName(string loginName)
        {
            using (var db = new HRM())
            {
                var userR = db.Users.FirstOrDefault(u => u.LoginName == loginName && u.Status != DataStatus.Deleted);
                if (userR == null)
                {
                    throw new BusinessException
                    {
                        Status = ResponseStatus.NoData,
                        ErrorMessage = "没有这个用户"
                    };
                }
                var user = userR.MapTo<UserDto.User>();
                user.StatusStr = user.Status.Description();
                return user;
            }
        }

        public void CheckAnswer(ChangePwd changeAnswer)
        {
            using (var db = new HRM())
            {
                var userR = db.Users.FirstOrDefault(u => u.LoginName == changeAnswer.LoginName && u.Question == changeAnswer.Question && u.Answer == changeAnswer.Answer && u.Status != DataStatus.Deleted);
                if (userR == null)
                {
                    throw new BusinessException
                    {
                        Status = ResponseStatus.NoData,
                        ErrorMessage = "密保问题错误"
                    };
                }
            }
        }
    }
}
