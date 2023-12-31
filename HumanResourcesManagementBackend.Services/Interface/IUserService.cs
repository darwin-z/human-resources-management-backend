﻿using HumanResourcesManagementBackend.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HumanResourcesManagementBackend.Services.Interface
{
    public interface IUserService
    {
        /// <summary>
        /// 获取用户列表
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        List<UserDto.User> GetUsers(UserDto.Search search);

        /// <summary>
        /// 根据ID获取用户
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        UserDto.User GetUserById(long id);

        /// <summary>
        /// 根据登录名获取登录信息
        /// </summary>
        /// <param name="loginName"></param>
        /// <returns></returns>
        UserDto.User GetUserByLoginName(string loginName);

        /// <summary>
        /// 用户登录
        /// </summary>
        /// <param name="login"></param>
        /// <returns>用户对象</returns>
        UserDto.User Login(UserDto.Login login);

        /// <summary>
        /// 添加用户
        /// </summary>
        /// <param name="user"></param>
        void AddUser(UserDto.Save user);

        /// <summary>
        /// 编辑用户
        /// </summary>
        /// <param name="user"></param>
        void EditUser(UserDto.Save user);

        /// <summary>
        /// 修改状态
        /// </summary>
        /// <param name="user"></param>
        void ChangeStatus(UserDto.Save user);

        /// <summary>
        /// 删除用户
        /// </summary>
        /// <param name="userId"></param>
        void DeleteUser(long userId);

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="pwd"></param>
        void ChangePwd(UserDto.ChangePwd changePwd);

        /// <summary>
        /// 忘记密码
        /// </summary>
        /// <param name="pwd"></param>
        void ForgotPassword(UserDto.ChangePwd changePwd);

        /// <summary>
        /// 修改密保问题
        /// </summary>
        /// <param name="question"></param>
        void ChangeQuestion(UserDto.ChangePwd changeQuestion);

        /// <summary>
        /// 检查密保问题
        /// </summary>
        /// <param name="changeAnswer"></param>
        void CheckAnswer(UserDto.ChangePwd changeAnswer);
    }
}
