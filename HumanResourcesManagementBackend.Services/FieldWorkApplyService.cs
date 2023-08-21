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
    public class FieldWorkApplyService:IFieldWorkApplyService
    {
        public void FieldWorkApply(FieldWorkApplyDto fieldWorkApply)
        {
            using (var db = new HRM())
            {
                if (fieldWorkApply.EmployeeId == 0)
                {
                    throw new BusinessException
                    {
                        ErrorMessage = "当前无法申请",
                        Status = ResponseStatus.NoPermission
                    };
                }

                if (fieldWorkApply.BeginDate == null || fieldWorkApply.EndDate == null ||
                    fieldWorkApply.Address == "" || fieldWorkApply.Reason == "")
                {
                    throw new BusinessException
                    {
                        ErrorMessage = "请填入具体的参数",
                        Status = ResponseStatus.ParameterError
                    };
                }
                var fieldworkapplyR = fieldWorkApply.MapTo<R_FieldWorkApply>();
                db.FieldWorkApplies.Add(fieldworkapplyR);
                if (db.SaveChanges() == 0)
                {
                    throw new BusinessException
                    {
                        ErrorMessage = "外勤申请失败，正在维护",
                        Status = ResponseStatus.AddError
                    };
                }
            }
        }
    }
}