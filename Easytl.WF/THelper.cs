using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Easytl.WF
{
    /// <summary>
    /// 各种格式转换类
    /// </summary>
    public static class THelper
    {
        ///// <summary>
        ///// 获取本地IPv4地址
        ///// </summary>
        ///// <returns></returns>
        //public static string GetHostIPv4()
        //{
        //    System.Net.IPAddress[] IPs = System.Net.Dns.GetHostAddresses(System.Net.Dns.GetHostName());
        //    foreach (var item in IPs)
        //    {
        //        if (System.Text.RegularExpressions.Regex.IsMatch(item.ToString(), "^(1\\d{2}|2[0-4]\\d|25[0-5]|[1-9]\\d|[1-9])\\."
        //                                                                        + "(1\\d{2}|2[0-4]\\d|25[0-5]|[1-9]\\d|\\d)\\."
        //                                                                        + "(1\\d{2}|2[0-4]\\d|25[0-5]|[1-9]\\d|\\d)\\."
        //                                                                        + "(1\\d{2}|2[0-4]\\d|25[0-5]|[1-9]\\d|\\d)$"))
        //        {
        //            return item.ToString();
        //        }
        //    }

        //    return string.Empty;
        //}

        /// <summary>
        /// 向用户显示控件
        /// </summary>
        public static void Show(this System.Windows.Forms.Form MyForm, System.Windows.Forms.IWin32Window Owner = null)
        {
            if (MyForm.Visible)
                MyForm.Focus();
            else
            {
                if (Owner == null)
                    MyForm.Show();
                else
                    MyForm.Show(Owner);
            }
        }
    }
}
