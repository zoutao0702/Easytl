using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Easytl.CustomControllers.CustomController
{
    /// <summary>
    /// 单选列
    /// </summary>
    public class DataGridViewCheckBoxColumn_OneSelect : DataGridViewCheckBoxColumn
    {
        DataGridViewCheckBoxCell _Select_One;
        /// <summary>
        /// 当前单选项
        /// </summary>
        public DataGridViewCheckBoxCell Select_One
        {
            get { return _Select_One; }
            set { _Select_One = value; }
        }

        public DataGridViewCheckBoxColumn_OneSelect()
        {
            this.ReadOnly = true;
            this.MinimumWidth = 40;
            this.Width = 40;
            this.HeaderText = "选择";
        }
    }
}

