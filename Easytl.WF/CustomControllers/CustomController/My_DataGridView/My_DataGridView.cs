using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Design;
using System.ComponentModel;
using Easytl.WF.CustomClass;

namespace Easytl.WF.CustomControllers.CustomController
{
    public class My_DataGridView : DataGridView
    {
        #region 自定义属性

        bool _EditHeadShow = true;
        /// <summary>
        /// 是否可右击列头显示隐藏列
        /// </summary>
        public bool EditHeadShow
        {
            get { return _EditHeadShow; }
            set { _EditHeadShow = value; }
        }

        #endregion

        #region 内部使用变量

        System.Windows.Forms.Timer Timer_TC_Edit = new System.Windows.Forms.Timer();

        #endregion

        #region 事件

        /// <summary>
        /// 当DataGridViewTextBoxColumn_UnionCombox列中单元格结束编辑时引发事件
        /// </summary>
        public event DataGridViewCellEventHandler OnTextUnionCombox_CellEndEdit;

        private void TextUnionCombox_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (OnTextUnionCombox_CellEndEdit != null)
            {
                OnTextUnionCombox_CellEndEdit(this, e);
            }
        }

        #endregion

        public My_DataGridView()
        {
            Timer_TC_Edit.Interval = 100;
            Timer_TC_Edit.Tick += new EventHandler(Timer_TC_Edit_Tick);
        }

        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);
            for (int i = 0; i < this.Columns.Count; i++)
            {
                if (!this.Columns[i].Visible)
                {
                    this.Columns[i].ToolTipText = "Hide";
                }
            }
        }

        protected override void OnColumnHeaderMouseClick(DataGridViewCellMouseEventArgs e)
        {
            try
            {
                switch (this.Columns[e.ColumnIndex].GetType().Name)
                {
                    case "DataGridViewCheckBoxColumn_MultiSelect":
                        if (e.Button == MouseButtons.Left)
                        {
                            bool Checked_ALL = (this.Columns[e.ColumnIndex] as DataGridViewCheckBoxColumn_MultiSelect).Checked_ALL;
                            (this.Columns[e.ColumnIndex] as DataGridViewCheckBoxColumn_MultiSelect).Checked_ALL = !Checked_ALL;
                            for (int i = 0; i < this.Rows.Count; i++)
                            {
                                (this.Rows[i].Cells[e.ColumnIndex] as DataGridViewCheckBoxCell).Value = !Checked_ALL;
                            }
                            if (!Checked_ALL)
                            {
                                (this.Columns[e.ColumnIndex] as DataGridViewCheckBoxColumn_MultiSelect).Checked_Count = this.Rows.Count;
                            }
                            else
                            {
                                (this.Columns[e.ColumnIndex] as DataGridViewCheckBoxColumn_MultiSelect).Checked_Count = 0;
                            }
                        }
                        break;
                }

                base.OnColumnHeaderMouseClick(e);
                if (EditHeadShow)
                {
                    if (e.Button == MouseButtons.Right)
                    {
                        CustomControllers.CustomForm.DataProNameShowBind dpnsb = CustomControllers.CustomForm.DataProNameShowBind.GetInstance(this, true);
                        if (!dpnsb.Visible)
                        {
                            dpnsb.Show(this.FindForm());
                        }
                    }
                }
            }
            catch { }
        }

        protected override void OnColumnSortModeChanged(DataGridViewColumnEventArgs e)
        {
            
            base.OnColumnSortModeChanged(e);
        }

        protected override void OnDataBindingComplete(DataGridViewBindingCompleteEventArgs e)
        {
            for (int i = 0; i < this.Columns.Count; i++)
            {
                switch (this.Columns[i].GetType().Name)
                {
                    case "DataGridViewCheckBoxColumn_MultiSelect":
                        (this.Columns[i] as DataGridViewCheckBoxColumn_MultiSelect).Checked_ALL = false;
                        (this.Columns[i] as DataGridViewCheckBoxColumn_MultiSelect).Checked_Count = 0;
                        break;
                }
            }
            base.OnDataBindingComplete(e);
        }

        protected override void OnCellFormatting(DataGridViewCellFormattingEventArgs e)
        {
            base.OnCellFormatting(e);
            switch (this.Columns[e.ColumnIndex].GetType().Name)
            {
                case "DataGridViewTextBoxColumn_XH":
                    e.Value = e.RowIndex + 1;
                    break;
            }
        }

        protected override void OnCellEnter(DataGridViewCellEventArgs e)
        {
            switch (this.Columns[e.ColumnIndex].GetType().Name)
            {
                case "DataGridViewTextBoxColumn_XH":
                    if (e.RowIndex >= 0)
                    {
                        this.Rows[e.RowIndex].Selected = true;
                    }
                    break;
            }
            base.OnCellEnter(e);
        }

        protected override void OnCellContentClick(DataGridViewCellEventArgs e)
        {
            switch (this.Columns[e.ColumnIndex].GetType().Name)
            {
                case "DataGridViewCheckBoxColumn_OneSelect":
                    if (e.RowIndex > -1)
                    {
                        DataGridViewCheckBoxColumn_OneSelect XZColumn = this.Columns[e.ColumnIndex] as DataGridViewCheckBoxColumn_OneSelect;
                        if (XZColumn.Select_One != null)
                        {
                            XZColumn.Select_One.Value = false;
                        }
                        if (Convert.ToBoolean(this.Rows[e.RowIndex].Cells[e.ColumnIndex].EditedFormattedValue))
                        {
                            XZColumn.Select_One = this.Rows[e.RowIndex].Cells[e.ColumnIndex] as DataGridViewCheckBoxCell;
                        }
                        else
                        {
                            XZColumn.Select_One = null;
                        }
                    }
                    break;
                case "DataGridViewCheckBoxColumn_MultiSelect":
                    if (e.RowIndex > -1)
                    {
                        bool OldChecked = Convert.ToBoolean(this.Rows[e.RowIndex].Cells[e.ColumnIndex].EditedFormattedValue);
                        this.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = !OldChecked;
                        if (!OldChecked)
                        {
                            (this.Columns[e.ColumnIndex] as DataGridViewCheckBoxColumn_MultiSelect).Checked_Count++;
                        }
                        else
                        {
                            (this.Columns[e.ColumnIndex] as DataGridViewCheckBoxColumn_MultiSelect).Checked_Count--;
                        }
                    }
                    break;
            }

            base.OnCellContentClick(e);
        }

        protected override void OnCellDoubleClick(DataGridViewCellEventArgs e)
        {
            base.OnCellDoubleClick(e);

            if (e.RowIndex >= 0)
            {
                if (this.Columns[e.ColumnIndex] is DataGridViewTextBoxColumn_UnionCombox)
                {
                    if (this.Rows[e.RowIndex].Cells[e.ColumnIndex] is DataGridViewTextBoxCell)
                    {
                        try
                        {
                            DataGridViewTextBoxColumn_UnionCombox NowTCColumn = this.Columns[e.ColumnIndex] as DataGridViewTextBoxColumn_UnionCombox;
                            NowTCColumn.EditeCellEvent = e;
                            NowTCColumn.EndEditCell = false;
                            this.Timer_TC_Edit.Start();
                            DataGridViewComboBoxCell cell = new DataGridViewComboBoxCell();
                            cell.DisplayMember = "Text";
                            cell.ValueMember = "Value";
                            DataGridViewTextBoxCell TextCell = this.Rows[e.RowIndex].Cells[e.ColumnIndex] as DataGridViewTextBoxCell;
                            NowTCColumn.OldItem = (TextCell != null) ? ((TextCell.Value is ListItem) ? TextCell.Value as ListItem : null) : null;
                            foreach (string key in NowTCColumn.ComboxItems.Keys)
                            {
                                ListItem item = new ListItem(NowTCColumn.ComboxItems[key], key);
                                cell.Items.Add(item);
                            }
                            cell.Value = (TextCell != null) ? ((TextCell.Value is ListItem) ? (TextCell.Value as ListItem).Value : null) : null;
                            this.Rows[e.RowIndex].Cells[e.ColumnIndex] = cell;
                            this.ReadOnly = false;
                            this.BeginEdit(true);
                        }
                        catch (Exception exception)
                        {
                            throw exception;
                        }
                    }
                }
            }
        }

        private void Timer_TC_Edit_Tick(object sender, EventArgs e)
        {
            for (int i = 0; i < this.Columns.Count; i++)
            {
                if (this.Columns[i] is DataGridViewTextBoxColumn_UnionCombox)
                {
                    if ((this.Columns[i] as DataGridViewTextBoxColumn_UnionCombox).EndEditCell)
                    {
                        TextUnionCombox_CellEndEditFunc((this.Columns[i] as DataGridViewTextBoxColumn_UnionCombox).EditeCellEvent);
                        (this.Columns[i] as DataGridViewTextBoxColumn_UnionCombox).EndEditCell = false;

                        TextUnionCombox_CellEndEdit(this, (this.Columns[i] as DataGridViewTextBoxColumn_UnionCombox).EditeCellEvent);
                    }
                }
            }
        }

        protected override void OnCellEndEdit(DataGridViewCellEventArgs e)
        {
            base.OnCellEndEdit(e);

            if (this.Columns[e.ColumnIndex] is DataGridViewTextBoxColumn_UnionCombox)
            {
                (this.Columns[e.ColumnIndex] as DataGridViewTextBoxColumn_UnionCombox).EndEditCell = true;
            }
        }

        private void TextUnionCombox_CellEndEditFunc(DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                if (this.Columns[e.ColumnIndex] is DataGridViewTextBoxColumn_UnionCombox)
                {
                    if (this.Rows[e.RowIndex].Cells[e.ColumnIndex] is DataGridViewComboBoxCell)
                    {
                        try
                        {
                            DataGridViewTextBoxColumn_UnionCombox NowTCColumn = this.Columns[e.ColumnIndex] as DataGridViewTextBoxColumn_UnionCombox;
                            DataGridViewTextBoxCell cell = new DataGridViewTextBoxCell();
                            DataGridViewComboBoxCell ComCell = this.Rows[e.RowIndex].Cells[e.ColumnIndex] as DataGridViewComboBoxCell;
                            if ((ComCell != null) && (ComCell.Value != null))
                            {
                                cell.Value = new ListItem(NowTCColumn.ComboxItems[ComCell.Value.ToString()], ComCell.Value.ToString());
                            }
                            else
                            {
                                cell.Value = NowTCColumn.OldItem;
                            }
                            this.Rows[e.RowIndex].Cells[e.ColumnIndex] = cell;
                            this.EndEdit();
                            this.ReadOnly = true;
                        }
                        catch (Exception exception)
                        {
                            throw exception;
                        }
                    }
                }
            }
        }

        protected override void OnDataSourceChanged(EventArgs e)
        {
            base.OnDataSourceChanged(e);
            for (int i = 0; i < this.Columns.Count; i++)
            {
                switch (this.Columns[i].GetType().Name)
                {
                    case "DataGridViewCheckBoxColumn_MultiSelect":
                        (this.Columns[i] as DataGridViewCheckBoxColumn_MultiSelect).Checked_Count = 0;
                        break;
                }
            }
        }
    }
}
