using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Easytl.WF.CustomControllers.CustomForm
{
    public partial class MsgBox : CustomClass.BaseForm
    {
        /// <summary>
        /// 定时关闭时长
        /// </summary>
        int CloseFormTime = 5;

        private MsgBox(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon Icon, int CloseTime)
        {
            InitializeComponent();
            this.Text = caption;
            this.lb_MsgShow.Text = text;
            switch (Convert.ToInt32(Icon))
            {
                case 0:
                    this.Picture_ShowIcon.Image = null;
                    break;
                case 16:
                    this.Picture_ShowIcon.Image = System.Drawing.SystemIcons.Error.ToBitmap();
                    break;
                case 32:
                    this.Picture_ShowIcon.Image = System.Drawing.SystemIcons.Question.ToBitmap();
                    break;
                case 48:
                    this.Picture_ShowIcon.Image = System.Drawing.SystemIcons.Exclamation.ToBitmap();
                    break;
                case 64:
                    this.Picture_ShowIcon.Image = System.Drawing.SystemIcons.Asterisk.ToBitmap();
                    break;
            }
            CloseFormTime = CloseTime;
        }


        /// <summary>
        /// 窗口加载
        /// </summary>
        private void MsgBox_Load(object sender, EventArgs e)
        {
            if (CloseFormTime != -1)
            {
                this.Timer_CloseForm.Start();
            }
            else
            {
                this.Btn_Accept.Text = "确定";
            }
        }


        /// <summary>
        /// 显示消息框
        /// </summary>
        public static DialogResult Show(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon Icon, int CloseTime)
        {
            MsgBox _MsgBox = new MsgBox(text, caption, buttons, Icon, CloseTime);
            _MsgBox.StartPosition = FormStartPosition.CenterScreen;
            _MsgBox.Show();
            return DialogResult.OK;
        }


        /// <summary>
        /// 显示消息框
        /// </summary>
        public static DialogResult Show(IWin32Window owner, string text, string caption, MessageBoxButtons buttons, MessageBoxIcon Icon, int CloseTime)
        {
            MsgBox _MsgBox = new MsgBox(text, caption, buttons, Icon, CloseTime);
            _MsgBox.StartPosition = FormStartPosition.CenterScreen;
            _MsgBox.Show(owner);
            return DialogResult.OK;
        }


        /// <summary>
        /// 显示消息框
        /// </summary>
        public static DialogResult ShowDialog(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon Icon, int CloseTime)
        {
            MsgBox _MsgBox = new MsgBox(text, caption, buttons, Icon, CloseTime);
            _MsgBox.StartPosition = FormStartPosition.CenterScreen;
            _MsgBox.ShowDialog();
            return DialogResult.OK;
        }


        /// <summary>
        /// 显示消息框
        /// </summary>
        public static DialogResult ShowDialog(IWin32Window owner, string text, string caption, MessageBoxButtons buttons, MessageBoxIcon Icon, int CloseTime)
        {
            MsgBox _MsgBox = new MsgBox(text, caption, buttons, Icon, CloseTime);
            _MsgBox.StartPosition = FormStartPosition.CenterScreen;
            _MsgBox.ShowDialog(owner);
            return DialogResult.OK;
        }


        /// <summary>
        /// 改变文本大小时
        /// </summary>
        private void lb_MsgShow_SizeChanged(object sender, EventArgs e)
        {
            this.Width = this.lb_MsgShow.Width + 76;
            this.Height = this.lb_MsgShow.Height + 92;
            this.Btn_Accept.Left = (this.Width - this.Btn_Accept.Width) / 2;
        }


        /// <summary>
        /// 点击确定关闭窗口
        /// </summary>
        private void Btn_Accept_Click(object sender, EventArgs e)
        {
            this.Timer_CloseForm.Stop();
            this.Close();
        }


        /// <summary>
        /// 定时关闭窗口
        /// </summary>
        private void Timer_CloseForm_Tick(object sender, EventArgs e)
        {
            CloseFormTime--;
            this.Btn_Accept.Text = "确定（" + CloseFormTime.ToString() + "）";
            if (CloseFormTime <= 0)
            {
                this.Timer_CloseForm.Stop();
                this.Close();
            }
        }
    }
}
