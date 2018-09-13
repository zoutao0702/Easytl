using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Easytl.Web.PayHelper.WeiXin
{
    /// <summary>
    /// 返回类--统一下单
    /// </summary>
    public class WXR_unifiedorder : WXReturn_Success
    {
        /// <summary>
        /// 交易类型
        /// </summary>
        public string trade_type { get; set; }

        /// <summary>
        /// 预支付交易会话标识
        /// </summary>
        public string prepay_id { get; set; }
    }
}
