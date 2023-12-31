﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HumanResourcesManagementBackend.Models
{
    /// <summary>
    /// 权限类型
    /// </summary>
    public enum PermissionType
    {
        /// <summary>
        /// API路径
        /// </summary>
        [Description("API路径")]
        Api = 1,
        /// <summary>
        /// 前端菜单
        /// </summary>
        [Description("前端菜单")]
        Menu = 2,
    }
}
