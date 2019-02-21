using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Easytl.WF
{
    /// <summary>
    /// 各种格式转换类
    /// </summary>
    public static class THelper
    {
        /// <summary>
        /// 向用户显示控件
        /// </summary>
        public static void Show(this Form MyForm, IWin32Window Owner = null)
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

        /// <summary>
        /// 执行方法
        /// </summary>
        public static void InvokeT(this Form MyForm, Action action)
        {
            if (MyForm.InvokeRequired)
                MyForm.Invoke(action);
            else
                action.Invoke();
        }
    }
}
