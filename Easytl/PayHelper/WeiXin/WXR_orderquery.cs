using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Easytl.PayHelper.WeiXin
{
    public class WXR_orderquery : WXReturn_Success
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
        /// 交易状态 
        /// </summary>
        public string trade_state { get; set; }

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
        public string coupon_id_n { get; set; }

        /// <summary>
        /// 单个代金券支付金额   
        /// </summary>
        public int? coupon_fee_n  { get; set; }

        /// <summary>
        /// 微信支付订单号   
        /// </summary>
        public string transaction_id { get; set; }

        /// <summary>
        /// 商户订单号    
        /// </summary>
        public string out_trade_no { get; set; }

        /// <summary>
        /// 附加数据   
        /// </summary>
        public string attach { get; set; }

        /// <summary>
        /// 支付完成时间    
        /// </summary>
        public string time_end { get; set; }

        /// <summary>
        /// 交易状态描述    
        /// </summary>
        public string trade_state_desc { get; set; }
    }
}
