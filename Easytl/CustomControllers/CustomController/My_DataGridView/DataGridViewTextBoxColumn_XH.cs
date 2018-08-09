using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Easytl.CustomControllers.CustomController
{
    /// <summary>
    /// 序号列
    /// </summary>
    public class DataGridViewTextBoxColumn_XH : DataGridViewTextBoxColumn
    {
        public DataGridViewTextBoxColumn_XH()
        {
            this.MinimumWidth = 40;
            this.Width = 40;
            this.HeaderText = "序号";
            this.ReadOnly = true;
            this.SortMode = DataGridViewColumnSortMode.NotSortable;
            this.DefaultCellStyle.BackColor = System.Drawing.SystemColors.Control;
            this.DefaultCellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.Resizable = System.Windows.Forms.DataGridViewTriState.True;
        }
    }
}

