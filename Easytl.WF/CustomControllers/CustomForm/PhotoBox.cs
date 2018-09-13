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
    public partial class PhotoBox : CustomClass.BaseForm
    {
        #region 公共属性

        string _backgroundimagelocation = string.Empty;
        /// <summary>
        /// 背景图片的路径
        /// </summary>
        public string BackgroundImageLocation
        {
            get { return _backgroundimagelocation; }
            set 
            { 
                _backgroundimagelocation = value;
                this.BackgroundImage = Image.FromFile(_backgroundimagelocation);
            }
        }

        #endregion

        /// <summary>
        /// 位移坐标
        /// </summary>
        Point PhotoBox_Location = new Point();


        /// <summary>
        /// 实例化窗口
        /// </summary>
        public PhotoBox()
        {
            InitializeComponent();
        }

        #region 拖动图片

        private void PhotoBox_MouseDown(object sender, MouseEventArgs e)
        {
            PhotoBox_Location = e.Location;
            this.Timer_BoxMove.Start();
        }


        private void Timer_BoxMove_Tick(object sender, EventArgs e)
        {
            this.Left = MousePosition.X - PhotoBox_Location.X;
            this.Top = MousePosition.Y - PhotoBox_Location.Y;
        }


        private void PhotoBox_MouseUp(object sender, MouseEventArgs e)
        {
            this.Timer_BoxMove.Stop();
        }

        #endregion

        /// <summary>
        /// 关闭图片窗口
        /// </summary>
        private void PhotoBox_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.Close();
        }
    }
}
