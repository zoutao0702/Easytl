using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Easytl.Web.PayHelper.WeiXin
{
    /// <summary>
    /// 参数类--统一下单
    /// </summary>
    public class WXP_unifiedorder : WXPara
    {
        /// <summary>
        /// 商品描述
        /// </summary>
        public string body { get; set; }

        /// <summary>
        /// 商品详情
        /// </summary>
        public string detail { get; set; }

        /// <summary>
        /// 附加数据
        /// </summary>
        public string attach { get; set; }

        /// <summary>
        /// 商户订单号
        /// </summary>
        public string out_trade_no { get; set; }

        /// <summary>
        /// 货币类型
        /// </summary>
        public string fee_type { get; set; }

        /// <summary>
        /// 总金额
        /// </summary>
        public int total_fee { get; set; }

        /// <summary>
        /// 终端IP
        /// </summary>
        public string spbill_create_ip { get; set; }

        /// <summary>
        /// 交易起始时间
        /// </summary>
        public string time_start { get; set; }

        /// <summary>
        /// 交易结束时间
        /// </summary>
        public string time_expire { get; set; }

        /// <summary>
        /// 商品标记
        /// </summary>
        public string goods_tag { get; set; }

        /// <summary>
        /// 通知地址
        /// </summary>
        public string notify_url { get; set; }

        /// <summary>
        /// 交易类型
        /// </summary>
        public string trade_type { get; set; }

        /// <summary>
        /// 指定支付方式
        /// </summary>
        public string limit_pay { get; set; }
    }
}
