using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Easytl.WF.CustomControllers.CustomForm
{
    public partial class WeekCalendar : UserControl
    {
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

            //DayOfWeek DayOfWeekI = DayOfWeek.Sunday;
            //for (int i = 1; i <= 7; i++)
            //{
            //    if (i < 7)
            //        DayOfWeekI = (DayOfWeek)i;
            //    else
            //        DayOfWeekI = DayOfWeek.Sunday;

                
            //}
        }

        void week_Click(object sender, EventArgs e)
        {
            Label lb = sender as Label;
            int WeekI = Convert.ToInt32(lb.Name.Replace("Week", string.Empty));
            CheckWeek((DayOfWeek)WeekI);
        }

        /// <summary>
        /// 选中星期
        /// </summary>
        public void CheckWeek(DayOfWeek WeekI)
        {
            Label lb = this.Controls["Week" + Convert.ToInt32(WeekI).ToString()] as Label;
            if (lb.BorderStyle == BorderStyle.None)
            {
                lb.BorderStyle = BorderStyle.FixedSingle;
                lb.BackColor = Color.FromArgb(216, 233, 255);

                WeekList.Add(WeekI);
            }
            else
            {
                lb.BorderStyle = BorderStyle.None;
                lb.BackColor = lb.Parent.BackColor;

                WeekList.Remove(WeekI);
            }
        }

        /// <summary>
        /// 选中星期
        /// </summary>
        public void CheckWeek(DayOfWeek[] Weeks)
        {
            foreach (DayOfWeek item in Weeks)
            {
                CheckWeek(item);
            }
        }
    }
}
