using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Collections.Specialized;

namespace Easytl.WebHelper
{
    public class WebHelper
    {
        /// <summary>
        /// 请求方式
        /// </summary>
        public enum RequestType
        {
            /// <summary>
            /// 未知
            /// </summary>
            NoKown = 0,
            /// <summary>
            /// Get请求
            /// </summary>
            Get = 1,
            /// <summary>
            /// Post请求
            /// </summary>
            Post = 2,
            /// <summary>
            /// Put请求
            /// </summary>
            Put = 3,
            /// <summary>
            /// Delete请求
            /// </summary>
            Delete = 4
        }

        /// <summary>
        /// 设置Cookie
        /// </summary>
        /// <param name="CookieKey">Cookie名称</param>
        /// <param name="CookieValues">Cookie值</param>
        /// <param name="Expires">到期时间</param>
        public static void SetCookie(string CookieKey, NameValueCollection CookieValues, DateTime Expires)
        {
            HttpCookie MyCookie = HttpContext.Current.Request.Cookies[CookieKey];
            bool Add = false;
            if (MyCookie == null)
            {
                MyCookie = new HttpCookie(CookieKey);
                Add = true;
            }

            foreach (string ValueKey in CookieValues.Keys)
            {
                MyCookie.Values[ValueKey] = HttpContext.Current.Server.UrlEncode(CookieValues[ValueKey]);
            }
            MyCookie.Expires = Expires;

            if (Add)
                HttpContext.Current.Response.AppendCookie(MyCookie);
            else
                HttpContext.Current.Response.Cookies.Set(MyCookie);
        }

        /// <summary>
        /// 设置Cookie
        /// </summary>
        /// <param name="CookieKey">Cookie名称</param>
        /// <param name="CookieValue">Cookie值</param>
        /// <param name="Expires">到期时间</param>
        public static void SetCookieValue(string CookieKey, string CookieValue, DateTime Expires)
        {
            HttpCookie MyCookie = HttpContext.Current.Request.Cookies[CookieKey];
            bool Add = false;
            if (MyCookie == null)
            {
                MyCookie = new HttpCookie(CookieKey);
                Add = true;
            }

            MyCookie.Value = HttpContext.Current.Server.UrlEncode(CookieValue);
            MyCookie.Expires = Expires;

            if(Add)
                HttpContext.Current.Response.AppendCookie(MyCookie);
            else
                HttpContext.Current.Response.Cookies.Set(MyCookie);
        }

        /// <summary>
        /// 获取Cookie
        /// </summary>
        /// <param name="CookieKey">Cookie名称</param>
        /// <returns>Cookie值</returns>
        public static NameValueCollection GetCookie(string CookieKey)
        {
            HttpCookie MyCookie = HttpContext.Current.Request.Cookies[CookieKey];
            if (MyCookie != null)
            {
                NameValueCollection CookieValues = new NameValueCollection();
                foreach (string name in MyCookie.Values.Keys)
                {
                    CookieValues.Add(name, HttpContext.Current.Server.UrlDecode(MyCookie.Values[name]));
                }
                return CookieValues;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 获取Cookie
        /// </summary>
        /// <param name="CookieKey">Cookie名称</param>
        /// <returns>Cookie值</returns>
        public static string GetCookieValue(string CookieKey)
        {
            HttpCookie MyCookie = HttpContext.Current.Request.Cookies[CookieKey];
            if (MyCookie != null)
            {
                return HttpContext.Current.Server.UrlDecode(MyCookie.Value);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 清理Cookie
        /// </summary>
        /// <param name="CookieKey">Cookie名称</param>
        public static void ClearCookie(string CookieKey)
        {
            HttpCookie MyCookie = HttpContext.Current.Request.Cookies[CookieKey];
            if (MyCookie != null)
            {
                MyCookie.Expires = DateTime.Now.AddDays(-2);
                HttpContext.Current.Response.Cookies.Set(MyCookie);
            }
        }
    }
}
