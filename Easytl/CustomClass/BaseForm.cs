using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Easytl.CustomClass
{
    public class BaseForm : Form
    {
        /// <summary>
        /// 显示窗体
        /// </summary>
        public void ShowForm()
        {
            if (this != null)
            {
                if (!this.IsDisposed)
                {
                    if (this.Visible)
                    {
                        if (this.WindowState == FormWindowState.Minimized)
                        {
                            this.WindowState = FormWindowState.Normal;
                        }
                        else
                        {
                            this.Focus();
                        }
                    }
                    else
                    {
                        this.Show();
                    }
                }
            }
        }


        /// <summary>
        /// 显示窗体
        /// </summary>
        /// <param name="owner">窗体拥有者</param>
        public void ShowForm(IWin32Window owner)
        {
            if (this != null)
            {
                if (!this.IsDisposed)
                {
                    if (this.Visible)
                    {
                        if (this.WindowState == FormWindowState.Minimized)
                        {
                            this.WindowState = FormWindowState.Normal;
                        }
                        else
                        {
                            this.Focus();
                        }
                    }
                    else
                    {
                        this.Show(owner);
                    }
                }
            }
        }


        /// <summary>
        /// 窗体加载
        /// </summary>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            this.StartPosition = FormStartPosition.CenterScreen;
        }
    }
}
