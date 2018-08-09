using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Easytl.PayHelper.WeiXin
{
    /// <summary>
    /// APP支付请求类
    /// </summary>
    public class WXP_apprequest
    {
        /// <summary>
        /// 应用ID
        /// </summary>
        internal string appid { get; set; }

        /// <summary>
        /// 商户号
        /// </summary>
        internal string partnerid { get; set; }

        /// <summary>
        /// 预支付交易会话ID
        /// </summary>
        public string prepayid { get; set; }

        /// <summary>
        /// 扩展字段
        /// </summary>
        public string package { get; set; }

        /// <summary>
        /// 随机字符串
        /// </summary>
        public string noncestr { get; set; }

        /// <summary>
        /// 时间戳
        /// </summary>
        public string timestamp { get; set; }

        /// <summary>
        /// 签名
        /// </summary>
        public string sign { get; set; }
    }
}
