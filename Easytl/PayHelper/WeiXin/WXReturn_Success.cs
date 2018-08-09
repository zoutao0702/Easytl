using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Easytl.PayHelper.WeiXin
{
    /// <summary>
    /// 返回成功基类
    /// </summary>
    public class WXReturn_Success : WXReturn
    {
        /// <summary>
        /// 应用APPID
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
        /// 业务结果
        /// </summary>
        public string result_code { get; set; }

        /// <summary>
        /// 业务结果（枚举）
        /// </summary>
        [SignHelper.Attribute.NoSign]
        public ReturnCode result_code_enum { get { return THelper.C(result_code, ReturnCode.FAIL); } }

        /// <summary>
        /// 错误代码
        /// </summary>
        public string err_code { get; set; }

        /// <summary>
        /// 错误代码（枚举）
        /// </summary>
        [SignHelper.Attribute.NoSign]
        public ReturnCode err_code_enum { get { return THelper.C(err_code, ReturnCode.FAIL); } }

        /// <summary>
        /// 错误代码描述
        /// </summary>
        public string err_code_des { get; set; }
    }
}
