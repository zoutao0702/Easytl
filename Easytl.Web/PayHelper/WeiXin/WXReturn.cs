using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Easytl.Web.PayHelper.WeiXin
{
    /// <summary>
    /// 返回基类
    /// </summary>
    public partial class WXReturn : WXConvert
    {
        /// <summary>
        /// 返回状态码
        /// </summary>
        public string return_code { get; set; }

        /// <summary>
        /// 返回信息
        /// </summary>
        public string return_msg { get; set; }

        /// <summary>
        /// 返回状态码（枚举）
        /// </summary>
        [SignHelper.Attribute.NoSign]
        public ReturnCode return_code_enum { get { return THelper.C(return_code, ReturnCode.FAIL); } }
    }

    public partial class WXReturn
    {
        /// <summary>
        /// 返回码
        /// </summary>
        public enum ReturnCode
        {
            /// <summary>
            /// 成功
            /// </summary>
            SUCCESS,
            /// <summary>
            /// 失败
            /// </summary>
            FAIL,
            /// <summary>
            /// 商户无此接口权限
            /// </summary>
            NOAUTH,
            /// <summary>
            /// 余额不足
            /// </summary>
            NOTENOUGH,
            /// <summary>
            /// 商户订单已支付
            /// </summary>
            ORDERPAID,
            /// <summary>
            /// 订单已关闭
            /// </summary>
            ORDERCLOSED,
            /// <summary>
            /// 系统错误
            /// </summary>
            SYSTEMERROR,
            /// <summary>
            /// APPID不存在
            /// </summary>
            APPID_NOT_EXIST,
            /// <summary>
            /// MCHID不存在
            /// </summary>
            MCHID_NOT_EXIST,
            /// <summary>
            /// appid和mch_id不匹配
            /// </summary>
            APPID_MCHID_NOT_MATCH,
            /// <summary>
            /// 缺少参数
            /// </summary>
            LACK_PARAMS,
            /// <summary>
            /// 商户订单号重复
            /// </summary>
            OUT_TRADE_NO_USED,
            /// <summary>
            /// 签名错误
            /// </summary>
            SIGNERROR,
            /// <summary>
            /// XML格式错误
            /// </summary>
            XML_FORMAT_ERROR,
            /// <summary>
            /// 请使用post方法
            /// </summary>
            REQUIRE_POST_METHOD,
            /// <summary>
            /// post数据为空
            /// </summary>
            POST_DATA_EMPTY,
            /// <summary>
            /// 此交易订单号不存在
            /// </summary>
            ORDERNOTEXIST
        }
    }
}
