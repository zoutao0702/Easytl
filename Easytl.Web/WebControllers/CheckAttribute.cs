using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Easytl.Web.WebControllers
{
    /// <summary>
    /// 请求前检查是否需要特殊验证
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class CheckAttribute : System.Attribute
    {
        /// <summary>
        /// 是否需要登录验证
        /// </summary>
        public bool CheckLogin { get; set; }

        /// <summary>
        /// 是否需要权限验证
        /// </summary>
        public bool CheckPermissions { get; set; }

    }
}