using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Easytl.WF.CustomClass;

namespace Easytl.WF.CustomControllers.CustomController
{
    public class My_TextBox : TextBox
    {
        ListItem _selectitem;
        /// <summary>
        /// 当前项值
        /// </summary>
        public ListItem SelectItem
        {
            get { return _selectitem; }
            set { _selectitem = value; this.Text = (_selectitem != null) ? _selectitem.Text : string.Empty; }
        }

        bool _OnlyNumber = false;
        /// <summary>
        /// 是否只能输入数字
        /// </summary>
        public bool OnlyNumber
        {
            get { return _OnlyNumber; }
            set { _OnlyNumber = value; }
        }

        bool _AllowPoint = true;
        /// <summary>
        /// 是否允许小数点
        /// </summary>
        public bool AllowPoint
        {
            get { return _AllowPoint; }
            set { _AllowPoint = value; }
        }

        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            base.OnKeyPress(e);

            if (OnlyNumber)
            {
                try
                {
                    int kc = (int)e.KeyChar;
                    if ((kc < 48 || kc > 57) && kc != 8)
                    {
                        if ((kc == 46) && (AllowPoint))                       //小数点   
                        {
                            if (this.Text.Length <= 0)
                            {
                                e.Handled = true;           //小数点不能在第一位
                            }
                            else
                            {
                                float f;
                                float oldf;
                                bool b1 = false, b2 = false;
                                b1 = float.TryParse(this.Text, out oldf);
                                b2 = float.TryParse(this.Text + e.KeyChar.ToString(), out f);
                                if (b2 == false)
                                {
                                    if (b1 == true)
                                    {
                                        e.Handled = true;
                                    }
                                    else
                                    {
                                        e.Handled = false;
                                    }
                                }
                            }
                        }
                        else
                        {
                            e.Handled = true;
                        }
                    }
                }
                catch (Exception)
                { }
            }
        }
    }
}
