namespace Easytl.CustomControllers.CustomForm
{
    partial class IPTextBox
    {
        /// <summary> 
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.Text_IP = new System.Windows.Forms.TextBox();
            this.Text_IPByte1 = new System.Windows.Forms.TextBox();
            this.Text_IPByte2 = new System.Windows.Forms.TextBox();
            this.Text_IPByte3 = new System.Windows.Forms.TextBox();
            this.Text_IPByte4 = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // Text_IP
            // 
            this.Text_IP.BackColor = System.Drawing.Color.White;
            this.Text_IP.Enabled = false;
            this.Text_IP.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Text_IP.Location = new System.Drawing.Point(0, 0);
            this.Text_IP.Margin = new System.Windows.Forms.Padding(0);
            this.Text_IP.Name = "Text_IP";
            this.Text_IP.ReadOnly = true;
            this.Text_IP.Size = new System.Drawing.Size(120, 21);
            this.Text_IP.TabIndex = 2;
            this.Text_IP.Text = "    .    .    .    ";
            // 
            // Text_IPByte1
            // 
            this.Text_IPByte1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.Text_IPByte1.Location = new System.Drawing.Point(6, 4);
            this.Text_IPByte1.Name = "Text_IPByte1";
            this.Text_IPByte1.Size = new System.Drawing.Size(20, 14);
            this.Text_IPByte1.TabIndex = 3;
            this.Text_IPByte1.Text = "255";
            this.Text_IPByte1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // Text_IPByte2
            // 
            this.Text_IPByte2.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.Text_IPByte2.Location = new System.Drawing.Point(34, 4);
            this.Text_IPByte2.Name = "Text_IPByte2";
            this.Text_IPByte2.Size = new System.Drawing.Size(20, 14);
            this.Text_IPByte2.TabIndex = 4;
            this.Text_IPByte2.Text = "255";
            this.Text_IPByte2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // Text_IPByte3
            // 
            this.Text_IPByte3.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.Text_IPByte3.Location = new System.Drawing.Point(64, 4);
            this.Text_IPByte3.Name = "Text_IPByte3";
            this.Text_IPByte3.Size = new System.Drawing.Size(20, 14);
            this.Text_IPByte3.TabIndex = 5;
            this.Text_IPByte3.Text = "255";
            this.Text_IPByte3.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // Text_IPByte4
            // 
            this.Text_IPByte4.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.Text_IPByte4.Location = new System.Drawing.Point(94, 4);
            this.Text_IPByte4.Name = "Text_IPByte4";
            this.Text_IPByte4.Size = new System.Drawing.Size(20, 14);
            this.Text_IPByte4.TabIndex = 6;
            this.Text_IPByte4.Text = "255";
            this.Text_IPByte4.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // IPTextBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.Controls.Add(this.Text_IPByte4);
            this.Controls.Add(this.Text_IPByte3);
            this.Controls.Add(this.Text_IPByte2);
            this.Controls.Add(this.Text_IPByte1);
            this.Controls.Add(this.Text_IP);
            this.Name = "IPTextBox";
            this.Size = new System.Drawing.Size(120, 21);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox Text_IP;
        private System.Windows.Forms.TextBox Text_IPByte1;
        private System.Windows.Forms.TextBox Text_IPByte2;
        private System.Windows.Forms.TextBox Text_IPByte3;
        private System.Windows.Forms.TextBox Text_IPByte4;

    }
}
