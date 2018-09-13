using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Easytl.Web.PayHelper.WeiXin
{
    public class WXP_orderquery : WXPara
    {
        /// <summary>
        /// 微信订单号
        /// </summary>
        public string transaction_id { get; set; }

        /// <summary>
        /// 商户订单号
        /// </summary>
        public string out_trade_no { get; set; }
    }
}
