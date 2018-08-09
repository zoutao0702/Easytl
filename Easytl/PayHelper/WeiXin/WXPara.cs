using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Easytl.SafeHelper;

namespace Easytl.PayHelper.WeiXin
{
    /// <summary>
    /// 参数基类
    /// </summary>
    public partial class WXPara : WXConvert
    {
        /// <summary>
        /// 应用ID
        /// </summary>
        internal string appid { get; set; }

        /// <summary>
        /// 商户号
        /// </summary>
        internal string mch_id { get; set; }

        /// <summary>
        /// 设备号
        /// </summary>
        internal string device_info { get; set; }

        /// <summary>
        /// 随机字符串
        /// </summary>
        internal string nonce_str { get; set; }

        /// <summary>
        /// 签名
        /// </summary>
        [SignHelper.Attribute.NoSign]
        internal string sign { get; set; }

        /// <summary>
        /// 签名类型
        /// </summary>
        [SignHelper.Attribute.NoSign]
        internal string sign_type { get; set; }
    }
}
