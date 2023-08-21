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
using System.Web.Http;

namespace HumanResourcesManagementBackend.Api.Controllers
{
    public class ApplyController : BaseApiController
    {
        private readonly IApplyService _applyService;
        public ApplyController()
        {
            _applyService = new ApplyService();
        }
        /// <summary>
        /// 判断是否有分配账号
        /// </summary>
        long uid;
        public void UserPermissions()
        {
            uid = CurrentUser.EmployeeId;
            if (uid == 0)
            {
                throw new BusinessException
                {
                    ErrorMessage = "没有权限",
                    Status = ResponseStatus.ParameterError
                };
            }
        }

        /// <summary>
        /// 休假申请
        /// </summary>
        /// <param name="VacationApply"></param>
        /// <returns></returns>
        [HttpPost]
        public Response VacationApply(VacationApplyDto vacationApply)
        {
            UserPermissions();
            _applyService.VacationApply(vacationApply,uid);
            return new Response
            {
                Status = ResponseStatus.Success,
                Message = ResponseStatus.Success.Description()
            };  
        }
        /// <summary>
        /// 缺勤申请
        /// </summary>
        /// <param name="absenceApply"></param>
        /// <returns></returns>
        [HttpPost]
        public Response AbsenceApply(AbsenceApplyDto absenceApply)
        {
            UserPermissions();
            _applyService.AbsenceApply(absenceApply,uid);
            return new Response
            {
                Status = ResponseStatus.Success,
                Message = ResponseStatus.Success.Description()
            };

        }
    }
}
