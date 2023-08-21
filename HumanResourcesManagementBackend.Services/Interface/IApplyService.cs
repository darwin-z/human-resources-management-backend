﻿using HumanResourcesManagementBackend.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HumanResourcesManagementBackend.Services.Interface
{
    public interface IApplyService
    {
        /// <summary>
        /// 休假申请
        /// </summary>
        /// <param name="vacationapply"></param>
        void VacationApply(VacationApplyDto vacationApply, long id);

        /// <summary>
        /// 缺勤申请
        /// </summary>
        /// <param name="vacationapply"></param>
        void AbsenceApply(AbsenceApplyDto absenceApply, long id);

        /// <summary>
        /// 调休申请
        /// </summary>
        /// <param name="CompensatoryApply"></param>
        /// <returns></returns>
        void CompensatoryApply(CompensatoryApplyDto compensatoryApply, long id);

        /// <summary>
        /// 外勤申请
        /// </summary>
        /// <param name="FieldWorkApply"></param>
        /// <returns></returns>
        void FieldWorkApply(FieldWorkApplyDto fieldWorkApply, long id);
    }
}
