using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Easytl.CustomControllers.CustomController
{
    /// <summary>
    /// 可全选列
    /// </summary>
    public class DataGridViewCheckBoxColumn_MultiSelect : DataGridViewCheckBoxColumn
    {
        bool _Checked_ALL = false;
        /// <summary>
        /// 是否全选
        /// </summary>
        public bool Checked_ALL
        {
            get { return _Checked_ALL; }
            set { _Checked_ALL = value; }
        }

        int _Checked_Count = 0;
        /// <summary>
        /// 当前选中的行数
        /// </summary>
        public int Checked_Count
        {
            get { return _Checked_Count; }
            set { _Checked_Count = value; }
        }

        public DataGridViewCheckBoxColumn_MultiSelect()
        {
            this.ReadOnly = true;
            this.MinimumWidth = 40;
            this.Width = 40;
            this.HeaderText = "选择";
        }
    }
}
