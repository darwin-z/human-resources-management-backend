﻿using HumanResourcesManagementBackend.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HumanResourcesManagementBackend.Repository
{
    /// <summary>
    /// 角色
    /// </summary>
    public class R_Role : BaseEntity
    {
        /// <summary>
        /// 角色名称
        /// </summary>
        public string Name { get;set; }
        /// <summary>
        /// 是否是默认角色
        /// </summary>
        public YesOrNo IsDefault { get; set; }
    }
}
