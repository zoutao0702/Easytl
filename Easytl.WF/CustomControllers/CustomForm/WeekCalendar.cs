using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Easytl.WF.CustomControllers.CustomForm
{
    public partial class WeekCalendar : UserControl
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
        public List<DayOfWeek> WeekList { get; private set; } = new List<DayOfWeek>();

        public WeekCalendar()
        {
            InitializeComponent();

            Array weeks = Enum.GetValues(typeof(DayOfWeek));
            int padding = 3;
            int LBWidth = (this.Width - (8 * padding)) / weeks.Length;

            foreach (int i in weeks)
            {
                DayOfWeek DayOfWeekI = (DayOfWeek)i;

                Label week = new Label()
                {
                    Name = "Week" + i.ToString(),
                    Text = DayOfWeekI.WeekEnToCh(),
                    Location = new Point((LBWidth * i) + (padding * (i + 1)), padding - 1),
                    AutoSize = false,
                    Size = new Size(LBWidth, (this.Height - (2 * padding))),
                    TextAlign = ContentAlignment.MiddleCenter
                };
                week.Click += week_Click;

                this.Controls.Add(week);
            }
        }

        void week_Click(object sender, EventArgs e)
        {
            if (!ReadOnly)
            {
                Label lb = sender as Label;
                int WeekI = Convert.ToInt32(lb.Name.Replace("Week", string.Empty));
                CheckWeek((DayOfWeek)WeekI, !GetState((DayOfWeek)WeekI));
            }
        }

        public bool GetState(DayOfWeek WeekI)
        {
            Label lb = this.Controls["Week" + Convert.ToInt32(WeekI).ToString()] as Label;
            if (lb.BorderStyle == BorderStyle.None)
                return false;
            else
                return true;
        }

        /// <summary>
        /// 选中星期
        /// </summary>
        public void CheckWeek(DayOfWeek WeekI, bool Check)
        {
            Label lb = this.Controls["Week" + Convert.ToInt32(WeekI).ToString()] as Label;
            bool State = GetState(WeekI);
            if (Check)
            {
                if (!State)
                {
                    lb.BorderStyle = BorderStyle.FixedSingle;
                    lb.BackColor = Color.FromArgb(216, 233, 255);

                    WeekList.Add(WeekI);
                }
            }
            else
            {
                if (State)
                {
                    lb.BorderStyle = BorderStyle.None;
                    lb.BackColor = lb.Parent.BackColor;

                    WeekList.Remove(WeekI);
                }
            }

            ValueChanged?.Invoke(this, new EventArgs());
        }

        /// <summary>
        /// 选中星期
        /// </summary>
        public void CheckWeek(DayOfWeek[] Weeks, bool Check)
        {
            foreach (DayOfWeek item in Weeks)
            {
                CheckWeek(item, Check);
            }
        }

        /// <summary>
        /// 清除所有选中星期
        /// </summary>
        public void Clear()
        {
            DayOfWeek[] weeks = new DayOfWeek[WeekList.Count];
            WeekList.CopyTo(weeks);
            foreach (var item in weeks)
            {
                CheckWeek(item, false);
            }
            WeekList.Clear();
        }
    }
}
