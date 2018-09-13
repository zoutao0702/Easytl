namespace Easytl.WF.CustomControllers.CustomForm
{
    partial class PhotoBox
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.Timer_BoxMove = new System.Windows.Forms.Timer(this.components);
            this.ToolTip_BoxWarnning = new System.Windows.Forms.ToolTip(this.components);
            this.SuspendLayout();
            // 
            // Timer_BoxMove
            // 
            this.Timer_BoxMove.Interval = 10;
            this.Timer_BoxMove.Tick += new System.EventHandler(this.Timer_BoxMove_Tick);
            // 
            // ToolTip_BoxWarnning
            // 
            this.ToolTip_BoxWarnning.ToolTipTitle = "双击可关闭该图片显示";
            // 
            // PhotoBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(430, 348);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "PhotoBox";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "PhotoBox";
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.PhotoBox_MouseUp);
            this.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.PhotoBox_MouseDoubleClick);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.PhotoBox_MouseDown);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Timer Timer_BoxMove;
        private System.Windows.Forms.ToolTip ToolTip_BoxWarnning;
    }
}