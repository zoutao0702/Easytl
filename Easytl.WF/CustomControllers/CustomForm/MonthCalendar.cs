using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Easytl.WF.CustomControllers.CustomForm
{
    public partial class MonthCalendar : UserControl
    {
        #region 公共属性

        /// <summary>
        /// 只读
        /// </summary>
        public bool ReadOnly { get; set; } = false;

        #endregion

        #region 事件

        public event EventHandler ValueChanged;

        #endregion

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
            if (!ReadOnly)
            {
                Label lb = sender as Label;
                int Day = Convert.ToInt32(lb.Name.Replace("Month", string.Empty));
                CheckDay(Day, !GetState(Day));
            }
        }

        public bool GetState(int Day)
        {
            Label lb = this.Controls["Month" + Day.ToString()] as Label;
            if (lb.BorderStyle == BorderStyle.None)
                return false;
            else
                return true;
        }

        /// <summary>
        /// 选中日期
        /// </summary>
        public void CheckDay(int Day, bool Check)
        {
            string lbname = "Month" + Day.ToString();
            if (this.Controls.ContainsKey(lbname))
            {
                Label lb = this.Controls[lbname] as Label;
                bool State = GetState(Day);
                if (Check)
                {
                    if (!State)
                    {
                        lb.BorderStyle = BorderStyle.FixedSingle;
                        lb.BackColor = Color.FromArgb(216, 233, 255);

                        MonthList.Add(Day);
                    }
                }
                else
                {
                    if (State)
                    {
                        lb.BorderStyle = BorderStyle.None;
                        lb.BackColor = lb.Parent.BackColor;

                        MonthList.Remove(Day);
                    }
                }

                ValueChanged?.Invoke(this, new EventArgs());
            }
        }

        /// <summary>
        /// 选中日期
        /// </summary>
        public void CheckDay(int[] Days, bool Check)
        {
            foreach (int item in Days)
            {
                CheckDay(item, Check);
            }
        }

        /// <summary>
        /// 清除所有选中日期
        /// </summary>
        public void Clear()
        {
            int[] months = new int[MonthList.Count];
            MonthList.CopyTo(months);
            foreach (var item in months)
            {
                CheckDay(item, false);
            }
            MonthList.Clear();
        }
    }
}
