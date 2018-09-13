using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using Easytl.WF.CustomClass;

namespace Easytl.WF.CustomControllers.CustomController
{
    /// <summary>
    /// 文本框和下拉列表组合列
    /// </summary>
    public class DataGridViewTextBoxColumn_UnionCombox : DataGridViewTextBoxColumn
    {
        #region 公用属性

        Dictionary<string, string> _comboxitems = new Dictionary<string, string>();
        /// <summary>
        /// 下拉列表项集合
        /// </summary>
        public Dictionary<string, string> ComboxItems
        {
            get { return _comboxitems; }
            set { _comboxitems = value; }
        }

        #endregion

        #region 内部参数

        /// <summary>
        /// 单元格原始值
        /// </summary>
        internal ListItem OldItem;

        /// <summary>
        /// 要编辑的单元格事件类
        /// </summary>
        internal DataGridViewCellEventArgs EditeCellEvent;

        /// <summary>
        /// 是否正在结束编辑
        /// </summary>
        internal bool EndEditCell;

        /// <summary>
        /// 复制后的值
        /// </summary>
        internal ListItem Cell_Value = null;

        /// <summary>
        /// 临时的值
        /// </summary>
        internal ListItem Cell_LS_Value = null;

        #endregion
    }
}
