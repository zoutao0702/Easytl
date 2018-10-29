using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Easytl.WF.CustomControllers.CustomForm
{
    public partial class MonthCalendar : UserControl
    {
        [Browsable(false)]
        public List<int> MonthList { get; private set; } = new List<int>();

        public MonthCalendar()
        {
            InitializeComponent();

            int padding = 3;
            int LBWidth = (this.Width - (8 * padding)) / 7;
            int LBHeight = (this.Height - (6 * padding)) / 5;

            for (int i = 1; i <= 31; i++)
            {
                int xi = (i % 7 > 0) ? (i % 7) : 7;
                int yi = (i % 7 > 0) ? (i / 7) : ((i / 7) - 1);
                Label month = new Label()
                {
                    Name = "Month" + i.ToString(),
                    Text = i.ToString(),
                    Location = new Point((LBWidth * (xi - 1)) + (padding * xi), ((LBHeight + padding) * yi) + padding),
                    AutoSize = false,
                    Size = new Size(LBWidth, LBHeight),
                    TextAlign = ContentAlignment.MiddleCenter
                };
                month.Click += month_Click;

                this.Controls.Add(month);
            }
        }

        void month_Click(object sender, EventArgs e)
        {
            Label lb = sender as Label;
            int Day = Convert.ToInt32(lb.Name.Replace("Month", string.Empty));
            CheckDay(Day);
        }

        /// <summary>
        /// 选中日期
        /// </summary>
        public void CheckDay(int Day)
        {
            string lbname = "Month" + Day.ToString();
            if (this.Controls.ContainsKey(lbname))
            {
                Label lb = this.Controls[lbname] as Label;
                if (lb.BorderStyle == BorderStyle.None)
                {
                    lb.BorderStyle = BorderStyle.FixedSingle;
                    lb.BackColor = Color.FromArgb(216, 233, 255);

                    MonthList.Add(Day);
                }
                else
                {
                    lb.BorderStyle = BorderStyle.None;
                    lb.BackColor = lb.Parent.BackColor;

                    MonthList.Remove(Day);
                }
            }
        }

        /// <summary>
        /// 选中日期
        /// </summary>
        public void CheckDay(int[] Days)
        {
            foreach (int item in Days)
            {
                CheckDay(item);
            }
        }
    }
}
