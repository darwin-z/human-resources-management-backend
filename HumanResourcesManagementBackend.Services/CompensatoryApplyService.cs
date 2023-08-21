﻿using HumanResourcesManagementBackend.Common;
using HumanResourcesManagementBackend.Models;
using HumanResourcesManagementBackend.Repository;
using HumanResourcesManagementBackend.Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HumanResourcesManagementBackend.Services
{
    public class CompensatoryApplyService:ICompensatoryApplyService
    {
        public void CompensatoryApply(CompensatoryApplyDto compensatoryApply)
        {
            using (var db = new HRM())
            {
                if (compensatoryApply.EmployeeId == 0)
                {
                    throw new BusinessException
                    {
                        ErrorMessage = "当前无法申请",
                        Status = ResponseStatus.NoPermission
                    };
                }

                if (compensatoryApply.WorkDate == null || compensatoryApply.RestDate == null || compensatoryApply.WorkPlan == "")
                {
                    throw new BusinessException
                    {
                        ErrorMessage = "请填入具体的参数",
                        Status = ResponseStatus.ParameterError
                    };
                }
                var compensatoryapplyR = compensatoryApply.MapTo<R_CompensatoryApply>();
                db.CompensatoryApplies.Add(compensatoryapplyR);
                if (db.SaveChanges() == 0)
                {
                    throw new BusinessException
                    {
                        ErrorMessage = "调休申请失败，正在维护",
                        Status = ResponseStatus.AddError
                    };
                }
            }
        }
    }
}