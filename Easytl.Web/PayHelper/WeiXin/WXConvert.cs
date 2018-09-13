using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Easytl.Web.PayHelper.WeiXin
{
    /// <summary>
    /// 微信基础转换类
    /// </summary>
    public class WXConvert
    {
        /// <summary>
        /// 返回结果取值格式{1}为占位符，代表数据
        /// </summary>
        [SignHelper.Attribute.NoSign]
        internal const string WXReturnPattern = @"<![CDATA[{1}]]>";

        static System.Reflection.BindingFlags bindingAttr = System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance;

        /// <summary>
        /// 微信类转化为微信xml格式字符串
        /// </summary>
        /// <param name="Tmodel">要转化的微信类</param>
        /// <param name="CDATA">是否加入CDATA保护</param>
        public static string ToXmlString<T>(T Tmodel, bool CDATA = true) where T : WXConvert
        {
            return Tmodel.ToXmlString<T>(Pattern: WXReturnPattern, bindingAttr: bindingAttr);
        }

        /// <summary>
        /// 微信xml格式字符串转化为微信类
        /// </summary>
        public static T GetWXObject<T>(string XmlStr) where T : WXConvert
        {
            if (!string.IsNullOrEmpty(XmlStr))
            { 
                return XmlStr.XmlToObject<T>(WXReturnPattern, bindingAttr: bindingAttr);
            }
            return default(T);
        }
    }
}
