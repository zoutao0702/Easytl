using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace Easytl.CustomControllers.CustomController
{
    public class My_ComboBox : ComboBox
    {
        public My_ComboBox()
        { }

        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);
            if (this.DropDownStyle == ComboBoxStyle.DropDownList)
            {
                if ((this.Items.Count > 0) && (this.SelectedIndex < 0))
                {
                    this.SelectedIndex = 0;
                }
            }
        }

        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            base.OnDrawItem(e);
            if (e.Index < 0)
            {
                return;
            }
            e.DrawBackground();
            e.DrawFocusRectangle();
            e.Graphics.DrawString(this.Items[e.Index].ToString(), e.Font, new SolidBrush(e.ForeColor), e.Bounds.X, e.Bounds.Y + 3);
        }
    }
}
