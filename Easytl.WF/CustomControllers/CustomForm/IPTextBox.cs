using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Easytl.WF.CustomControllers.CustomForm
{
    public partial class IPTextBox : UserControl
    {
        #region 公共属性

        bool _UseAsIP = false;
        /// <summary>
        /// 是否作为IP使用
        /// </summary>
        public bool UseAsIP
        {
            get { return _UseAsIP; }
            set { _UseAsIP = value; }
        }

        public override string Text
        {
            get
            {
                if ((this.Text_IPByte1.Text.Trim() != string.Empty) && (this.Text_IPByte2.Text.Trim() != string.Empty) && (this.Text_IPByte3.Text.Trim() != string.Empty) && (this.Text_IPByte4.Text.Trim() != string.Empty))
                {
                    return this.Text_IPByte1.Text + "." + this.Text_IPByte2.Text + "." + this.Text_IPByte3.Text + "." + this.Text_IPByte4.Text;
                }
                else
                {
                    return string.Empty;
                }
            }
            set
            {
                string[] IPBytes = value.Split('.');
                if (IPBytes.Length == 4)
                {
                    byte IPByte = 0;
                    for (int i = 0; i < IPBytes.Length; i++)
                    {
                        if (byte.TryParse(IPBytes[i], out IPByte))
                        {
                            if ((_UseAsIP) && (IPByte == 255))
                            {
                                this.Controls["Text_IPByte" + (i + 1).ToString()].Text = string.Empty;
                            }
                            else
                            {
                                this.Controls["Text_IPByte" + (i + 1).ToString()].Text = IPByte.ToString();
                            }
                        }
                        else
                        {
                            this.Controls["Text_IPByte" + (i + 1).ToString()].Text = string.Empty;
                        }
                    }
                }
                else
                {
                    this.Text_IPByte1.Text = string.Empty;
                    this.Text_IPByte2.Text = string.Empty;
                    this.Text_IPByte3.Text = string.Empty;
                    this.Text_IPByte4.Text = string.Empty;
                }
            }
        }

        #endregion


        public IPTextBox()
        {
            InitializeComponent();
            this.Text = string.Empty;
            this.Text_IPByte1.KeyPress += new KeyPressEventHandler(Text_IPByte_OnKeyPress);
            this.Text_IPByte2.KeyPress += new KeyPressEventHandler(Text_IPByte_OnKeyPress);
            this.Text_IPByte3.KeyPress += new KeyPressEventHandler(Text_IPByte_OnKeyPress);
            this.Text_IPByte4.KeyPress += new KeyPressEventHandler(Text_IPByte_OnKeyPress);
        }


        /// <summary>
        /// 检验输入格式
        /// </summary>
        protected void Text_IPByte_OnKeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                int kc = (int)e.KeyChar;
                if ((kc < 48 || kc > 57) && kc != 8)
                {
                    e.Handled = true;
                    return;
                }

                TextBox cb = sender as TextBox;
                int IPByteIndex = Convert.ToInt32(cb.Name.Substring(cb.Name.Length - 1));
                string IPByteStr = string.Empty;
                if (kc != 8)
                {
                    IPByteStr = cb.Text.Substring(0, cb.SelectionStart) + e.KeyChar.ToString() + cb.Text.Substring(cb.SelectionStart + cb.SelectionLength);
                    byte IPByte = 0;
                    if (!byte.TryParse(IPByteStr, out IPByte))
                    {
                        MsgBox.ShowDialog(" " + IPByteStr + " 不是一个有效数值，请指定一个介于 0 到 255 之间的数值!", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning, 5);
                        e.Handled = true;
                        return;
                    }

                    if ((_UseAsIP) && (IPByte == 255))
                    {
                        MsgBox.ShowDialog(" " + IPByteStr + " 不是一个有效数值，请指定一个介于 0 到 254 之间的数值!", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning, 5);
                        e.Handled = true;
                        return;
                    }

                    if (IPByteStr.Length >= 3)
                    {
                        if (IPByteIndex < 4)
                        {
                            this.Controls["Text_IPByte" + (IPByteIndex + 1).ToString()].Focus();
                            (this.Controls["Text_IPByte" + (IPByteIndex + 1).ToString()] as TextBox).SelectAll();
                        }
                    }
                }
                else
                {
                    IPByteStr = cb.Text.Substring(0, cb.SelectionStart) + cb.Text.Substring(cb.SelectionStart + cb.SelectionLength);
                    if (IPByteStr.Length <= 0)
                    {
                        if (IPByteIndex > 1)
                        {
                            this.Controls["Text_IPByte" + (IPByteIndex - 1).ToString()].Focus();
                            (this.Controls["Text_IPByte" + (IPByteIndex - 1).ToString()] as TextBox).SelectionStart = this.Controls["Text_IPByte" + (IPByteIndex - 1).ToString()].Text.Length;
                        }
                    }
                }
            }
            catch (Exception)
            { }
        }
    }
}
