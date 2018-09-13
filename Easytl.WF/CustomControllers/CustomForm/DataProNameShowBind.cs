using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Easytl.WF.CustomControllers.CustomController;

namespace Easytl.WF.CustomControllers.CustomForm
{
    internal partial class DataProNameShowBind : Form
    {
        My_DataGridView datagrid;

        private DataProNameShowBind(My_DataGridView DataGrid)
        {
            InitializeComponent();
            datagrid = DataGrid;
        }

        private static DataProNameShowBind instance;

        private static object _lock = new object();

        /// <summary>
        /// ´´½¨DataProNameShowBindÊµÀý
        /// </summary>
        /// <returns></returns>
        public static DataProNameShowBind GetInstance(My_DataGridView DataGrid, bool IsCreate)
        {
            if (instance == null || instance.IsDisposed)
            {
                lock (_lock)
                {
                    if (instance == null || instance.IsDisposed)
                    {
                        if (IsCreate)
                        {
                            instance = new DataProNameShowBind(DataGrid);
                        }
                        else
                        {
                            return null;
                        }
                    }
                }
            }
            instance.Location = MousePosition;
            return instance;
        }

        private void DataProNameShowBind_Load(object sender, EventArgs e)
        {
            this.checkedListBox1.Items.Clear();
            this.Location = MousePosition;
            for (int i = 0; i < datagrid.Columns.Count; i++)
            {
                if (datagrid.Columns[i] is DataGridViewTextBoxColumn)
                {
                    if ((datagrid.Columns[i].ToolTipText != "Hide") && !(datagrid.Columns[i] is DataGridViewTextBoxColumn_XH))
                    {
                        int selectindex = this.checkedListBox1.Items.Add(datagrid.Columns[i].HeaderText.Trim());
                        if (datagrid.Columns[i].Visible)
                        {
                            this.checkedListBox1.SetItemChecked(selectindex, true);
                        }
                        else
                        {
                            this.checkedListBox1.SetItemChecked(selectindex, false);
                        }
                    }
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < datagrid.Columns.Count; i++)
            {
                if (datagrid.Columns[i] is DataGridViewTextBoxColumn)
                {
                    if ((datagrid.Columns[i].ToolTipText != "Hide") && !(datagrid.Columns[i] is DataGridViewTextBoxColumn_XH))
                    {
                        if (this.checkedListBox1.CheckedItems.Contains(datagrid.Columns[i].HeaderText.Trim()))
                        {
                            datagrid.Columns[i].Visible = true;
                        }
                        else
                        {
                            datagrid.Columns[i].Visible = false;
                        }
                    }
                }
            }
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

    }
}