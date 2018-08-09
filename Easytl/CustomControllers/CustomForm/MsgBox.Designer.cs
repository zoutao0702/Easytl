namespace Easytl.CustomControllers.CustomForm
{
    partial class MsgBox
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
            this.Picture_ShowIcon = new System.Windows.Forms.PictureBox();
            this.lb_MsgShow = new System.Windows.Forms.Label();
            this.Btn_Accept = new System.Windows.Forms.Button();
            this.Timer_CloseForm = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.Picture_ShowIcon)).BeginInit();
            this.SuspendLayout();
            // 
            // Picture_ShowIcon
            // 
            this.Picture_ShowIcon.Location = new System.Drawing.Point(12, 12);
            this.Picture_ShowIcon.Name = "Picture_ShowIcon";
            this.Picture_ShowIcon.Size = new System.Drawing.Size(32, 32);
            this.Picture_ShowIcon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.Picture_ShowIcon.TabIndex = 0;
            this.Picture_ShowIcon.TabStop = false;
            // 
            // lb_MsgShow
            // 
            this.lb_MsgShow.AutoSize = true;
            this.lb_MsgShow.Location = new System.Drawing.Point(64, 15);
            this.lb_MsgShow.MaximumSize = new System.Drawing.Size(700, 0);
            this.lb_MsgShow.MinimumSize = new System.Drawing.Size(0, 26);
            this.lb_MsgShow.Name = "lb_MsgShow";
            this.lb_MsgShow.Size = new System.Drawing.Size(29, 26);
            this.lb_MsgShow.TabIndex = 1;
            this.lb_MsgShow.Text = "成功";
            this.lb_MsgShow.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lb_MsgShow.SizeChanged += new System.EventHandler(this.lb_MsgShow_SizeChanged);
            // 
            // Btn_Accept
            // 
            this.Btn_Accept.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.Btn_Accept.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Btn_Accept.Location = new System.Drawing.Point(22, 54);
            this.Btn_Accept.Name = "Btn_Accept";
            this.Btn_Accept.Size = new System.Drawing.Size(75, 23);
            this.Btn_Accept.TabIndex = 2;
            this.Btn_Accept.Text = "确定（5）";
            this.Btn_Accept.UseVisualStyleBackColor = true;
            this.Btn_Accept.Click += new System.EventHandler(this.Btn_Accept_Click);
            // 
            // Timer_CloseForm
            // 
            this.Timer_CloseForm.Interval = 1000;
            this.Timer_CloseForm.Tick += new System.EventHandler(this.Timer_CloseForm_Tick);
            // 
            // MsgBox
            // 
            this.AcceptButton = this.Btn_Accept;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.Btn_Accept;
            this.ClientSize = new System.Drawing.Size(119, 89);
            this.Controls.Add(this.Btn_Accept);
            this.Controls.Add(this.lb_MsgShow);
            this.Controls.Add(this.Picture_ShowIcon);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(125, 121);
            this.Name = "MsgBox";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Load += new System.EventHandler(this.MsgBox_Load);
            ((System.ComponentModel.ISupportInitialize)(this.Picture_ShowIcon)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox Picture_ShowIcon;
        private System.Windows.Forms.Label lb_MsgShow;
        private System.Windows.Forms.Button Btn_Accept;
        private System.Windows.Forms.Timer Timer_CloseForm;
    }
}