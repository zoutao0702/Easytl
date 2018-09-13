using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Easytl.Web.PayHelper.WeiXin
{
    /// <summary>
    /// 返回类--支付结果通知
    /// </summary>
    public class WXR_paynotify : WXReturn_Success
    {
        /// <summary>
        /// 用户标识
        /// </summary>
        public string openid { get; set; }

        /// <summary>
        /// 是否关注公众账号
        /// </summary>
        public string is_subscribe { get; set; }

        /// <summary>
        /// 交易类型
        /// </summary>
        public string trade_type { get; set; }

        /// <summary>
        /// 付款银行
        /// </summary>
        public string bank_type { get; set; }

        /// <summary>
        /// 总金额
        /// </summary>
        public int total_fee { get; set; }

        /// <summary>
        /// 货币种类
        /// </summary>
        public string fee_type { get; set; }

        /// <summary>
        /// 现金支付金额
        /// </summary>
        public int cash_fee { get; set; }

        /// <summary>
        /// 现金支付货币类型
        /// </summary>
        public string cash_fee_type { get; set; }

        /// <summary>
        /// 代金券金额
        /// </summary>
        public int? coupon_fee { get; set; }

        /// <summary>
        /// 代金券使用数量
        /// </summary>
        public int? coupon_count { get; set; }

        /// <summary>
        /// 代金券ID
        /// </summary>
        public KeyValuePair<int, string>? coupon_id_n { get; set; }

        /// <summary>
        /// 单个代金券支付金额
        /// </summary>
        public KeyValuePair<int, int>? coupon_fee_n { get; set; }

        /// <summary>
        /// 微信支付订单号
        /// </summary>
        public string transaction_id { get; set; }

        /// <summary>
        /// 商户订单号
        /// </summary>
        public string out_trade_no { get; set; }

        /// <summary>
        /// 商家数据包
        /// </summary>
        public string attach { get; set; }

        /// <summary>
        /// 支付完成时间
        /// </summary>
        public string time_end { get; set; }
    }
}
