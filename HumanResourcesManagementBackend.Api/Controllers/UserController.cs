﻿using HumanResourcesManagementBackend.Api.Extensions;
using HumanResourcesManagementBackend.Common;
using HumanResourcesManagementBackend.Models;
using HumanResourcesManagementBackend.Services;
using HumanResourcesManagementBackend.Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace HumanResourcesManagementBackend.Api.Controllers
{
    /// <summary>
    /// 用户管理控制器
    /// </summary>
    public class UserController : BaseApiController
    {
        private readonly IUserService userService;

        public UserController()
        {
            userService = new UserService();
        }

        /// <summary>
        /// 查询用户列表
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        /// <exception cref="BusinessException"></exception>
        [HttpPost]
        public PageResponse<UserDto.User> QueryUserByPage(UserDto.Search search)
        {
            var users = userService.GetUsers(search);

            //返回响应结果
            return new PageResponse<UserDto.User>()
            {
                RecordCount = search.RecordCount,
                Status = ResponseStatus.Success,
                Message = ResponseStatus.Success.Description(),
                Data = users ?? throw new BusinessException
                {
                    Status = ResponseStatus.NoData
                }
            };
        }

        /// <summary>
        /// 根据用户id获取用户详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public DataResponse<UserDto.User> GetUserById(long id)
        {
            var user = userService.GetUserById(id);
            return new DataResponse<UserDto.User>
            {
                Data = user,
                Status = ResponseStatus.Success,
                Message = ResponseStatus.Success.Description(),
            };
        }

        /// <summary>
        /// 用户登录
        /// </summary>
        /// <param name="login"></param>
        /// <returns></returns>
        [HttpPost, AllowAnonymous]
        public DataResponse<string> Login(UserDto.Login login)
        {
            var user = userService.Login(login);
            //返回Token
            var token = JwtHelper.Publish(user.ToJson());

            return new DataResponse<string>
            {
                Data = token,
                Status = ResponseStatus.Success,
                Message = ResponseStatus.Success.Description()
            };
        }

        /// <summary>
        /// 根据登录名获取用户信息
        /// </summary>
        /// <param name="loginName"></param>
        /// <returns></returns>
        [HttpGet, AllowAnonymous]
        public DataResponse<UserDto.User> GetUserByLoginName(string loginName)
        {
            var user = userService.GetUserByLoginName(loginName);
            return new DataResponse<UserDto.User>
            {
                Data = user,
                Status = ResponseStatus.Success,
                Message = ResponseStatus.Success.Description(),
            };
        }

        /// <summary>
        /// 添加用户
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPost]
        public Response AddUser(UserDto.Save user)
        {
            userService.AddUser(user);
            return new Response 
            {
                Status = ResponseStatus.Success,
                Message = ResponseStatus.Success.Description()
            };
        }

        /// <summary>
        /// 修改用户
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPut]
        public Response EditUser(UserDto.Save user)
        {
            userService.EditUser(user);
            return new Response
            {
                Status = ResponseStatus.Success,
                Message = ResponseStatus.Success.Description()
            };
        }

        /// <summary>
        /// 删除用户
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        public Response DeleteUser(long id)
        {
            userService.DeleteUser(id);
            return new Response
            {
                Status = ResponseStatus.Success,
                Message = ResponseStatus.Success.Description()
            };
        }

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="changepwd"></param>
        /// <returns></returns>
        [HttpPut]
        public Response ChangePwd(UserDto.ChangePwd changePwd)
        {
            changePwd.Id = CurrentUser.Id;
            userService.ChangePwd(changePwd);
            return new Response
            {
                Status = ResponseStatus.Success,
                Message = ResponseStatus.Success.Description()
            };
        }

        /// <summary>
        /// 找回密码
        /// </summary>
        /// <param name="changepwd"></param>
        /// <returns></returns>
        [HttpPut,AllowAnonymous]
        public Response ForgotPassword(UserDto.ChangePwd changePwd)
        {
            userService.ForgotPassword(changePwd);
            return new Response
            {
                Status = ResponseStatus.Success,
                Message = ResponseStatus.Success.Description()
            };
        }

        /// <summary>
        /// 检测密保问题
        /// </summary>
        /// <param name="changepwd"></param>
        /// <returns></returns>
        [HttpPost, AllowAnonymous]
        public Response CheckAnswer(UserDto.ChangePwd changePwd)
        {
            userService.CheckAnswer(changePwd);
            return new Response
            {
                Status = ResponseStatus.Success,
                Message = ResponseStatus.Success.Description()
            };
        }

        /// <summary>
        /// 修改密保
        /// </summary>
        /// <param name="changeQuestion"></param>
        /// <returns></returns>
        [HttpPut]
        public Response ChangeQuestion(UserDto.ChangePwd changeQuestion)
        {
            changeQuestion.LoginName = CurrentUser.LoginName;
            userService.ChangeQuestion(changeQuestion);
            return new Response
            {
                Status = ResponseStatus.Success,
                Message = ResponseStatus.Success.Description()
            };
        }

        /// <summary>
        /// 修改状态
        /// </summary>
        /// <param name="changeQuestion"></param>
        /// <returns></returns>
        [HttpPut]
        public Response ChangeStatus(UserDto.Save user)
        {
            userService.ChangeStatus(user);
            return new Response
            {
                Status = ResponseStatus.Success,
                Message = ResponseStatus.Success.Description()
            };
        }
    }
}
