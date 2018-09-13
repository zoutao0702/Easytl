using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Easytl.WF.CustomControllers.CustomController
{
    public class My_PictureBox : System.Windows.Forms.PictureBox
    {
        #region 公共属性

        /// <summary>
        /// 加载图片的当前次数
        /// </summary>
        int _loadphoto_nownum = 0;


        /// <summary>
        /// 加载图片的次数
        /// </summary>
        int _loadphoto_num = 1;
        /// <summary>
        /// 加载图片的次数
        /// </summary>
        public int LoadPhoto_Num
        {
            get { return _loadphoto_num; }
            set { _loadphoto_num = value; }
        }


        bool _showmaxphoto = false;
        /// <summary>
        /// 点击是否弹出大图
        /// </summary>
        public bool ShowMaxPhoto
        {
            get { return _showmaxphoto; }
            set { _showmaxphoto = value; }
        }


        int _maxphotowidth = 600;
        /// <summary>
        /// 大图的宽度
        /// </summary>
        public int MaxPhotoWidth
        {
            get { return _maxphotowidth; }
            set { _maxphotowidth = value; }
        }


        int _maxphotoheight = 500;
        /// <summary>
        /// 大图的高度
        /// </summary>
        public int MaxPhotoHeight
        {
            get { return _maxphotoheight; }
            set { _maxphotoheight = value; }
        }


        /// <summary>
        /// 右击是否选择或取消图片
        /// </summary>
        public bool ShowContextMenuStrip
        {
            get 
            {
                if (this.ContextMenuStrip != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            set 
            {
                if (value)
                {
                    this.ContextMenuStrip = ContextMenuStrip_Show;
                }
                else
                {
                    if ((this.ContextMenuStrip != null) && (!this.ContextMenuStrip.IsDisposed))
                    {
                        this.ContextMenuStrip.Dispose();
                    }
                    this.ContextMenuStrip = null;
                }
            }
        }


        System.Drawing.Image _defaultimage;
        /// <summary>
        /// 默认图片
        /// </summary>
        public System.Drawing.Image DefaultImage
        {
            get { return _defaultimage; }
            set { _defaultimage = value; }
        }

        #endregion

        #region 内部使用变量

        #endregion

        #region 内部使用控件

        /// <summary>
        /// 定时加载图片控件
        /// </summary>
        private System.Windows.Forms.Timer Timer_LoadPhoto;

        /// <summary>
        /// 单击弹出的大图窗口
        /// </summary>
        private CustomForm.PhotoBox PhotoBox_Show;

        /// <summary>
        /// 右击弹出的选择或取消图片窗口
        /// </summary>
        private System.Windows.Forms.ContextMenuStrip ContextMenuStrip_Show;

        /// <summary>
        /// 选择图片时的打开文件窗口
        /// </summary>
        private System.Windows.Forms.OpenFileDialog OpenFileDialog_Show;

        #endregion

        /// <summary>
        /// 实例化
        /// </summary>
        public My_PictureBox()
        {
            //定时加载图片控件
            this.Timer_LoadPhoto = new System.Windows.Forms.Timer();
            this.Timer_LoadPhoto.Tick += new System.EventHandler(this.Timer_LoadPhoto_Tick);

            //右击弹出的选择或取消图片窗口
            ContextMenuStrip_Show = new System.Windows.Forms.ContextMenuStrip();
            ContextMenuStrip_Show.Items.Add("选择图片");
            ContextMenuStrip_Show.Items.Add("清除图片");
            ContextMenuStrip_Show.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(ContextMenuStrip_Show_ItemClicked);

            //选择图片时的打开文件窗口
            OpenFileDialog_Show = new System.Windows.Forms.OpenFileDialog();
            OpenFileDialog_Show.Filter = "图片(jpg,png,gif,bmp)|*.jpg;*.png;*.gif;*.bmp";
        }

        /// <summary>
        /// 按规定次数加载图片
        /// </summary>
        public void LoadImageByNum(string url)
        {
            if (url != string.Empty)
            {
                _loadphoto_nownum = 0;
                this.ImageLocation = url;
                this.Timer_LoadPhoto.Start();
            }
            else
            {
                ClearImage();
            }
        }

        /// <summary>
        /// 清除图片显示
        /// </summary>
        public void ClearImage()
        {
            this.ImageLocation = string.Empty;
            this.Image = this.DefaultImage;
        }

        /// <summary>
        /// 按时加载图片
        /// </summary>
        private void Timer_LoadPhoto_Tick(object sender, EventArgs e)
        {
            if (this.Image == this.ErrorImage)
            {
                _loadphoto_nownum++;
                if (_loadphoto_nownum < _loadphoto_num)
                {
                    this.LoadAsync();
                    return;
                }
            }

            this.Timer_LoadPhoto.Stop();
        }

        /// <summary>
        /// 点击图片时
        /// </summary>
        protected override void OnMouseClick(System.Windows.Forms.MouseEventArgs e)
        {
            base.OnMouseClick(e);
            switch (e.Button)
            {
                case System.Windows.Forms.MouseButtons.Left:
                    if (_showmaxphoto)
                    {
                        if (this.Image != this.ErrorImage)
                        {
                            if ((PhotoBox_Show == null) || (PhotoBox_Show.IsDisposed))
                            {
                                PhotoBox_Show = new Easytl.WF.CustomControllers.CustomForm.PhotoBox();
                            }
                            PhotoBox_Show.Width = _maxphotowidth;
                            PhotoBox_Show.Height = _maxphotoheight;
                            PhotoBox_Show.ShowInTaskbar = false;
                            PhotoBox_Show.BackgroundImage = this.Image;
                            PhotoBox_Show.ShowForm();
                        }
                    }
                    break;
                case System.Windows.Forms.MouseButtons.Right:
                    break;
            }
        }

        /// <summary>
        /// 点击快捷菜单时
        /// </summary>
        void ContextMenuStrip_Show_ItemClicked(object sender, System.Windows.Forms.ToolStripItemClickedEventArgs e)
        {
            switch (e.ClickedItem.Text)
            {
                case "选择图片":
                    if (OpenFileDialog_Show.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        this.ImageLocation = OpenFileDialog_Show.FileName;
                    }
                    break;
                case "清除图片":
                    ClearImage();
                    break;
            }
        }
    }
}
