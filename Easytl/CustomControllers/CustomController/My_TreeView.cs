using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;
using System.Data;
using System.Linq;

namespace Easytl.CustomControllers.CustomController
{
    public class My_TreeView : TreeView
    {
        /// <summary>
        /// 节点选中类型
        /// </summary>
        public enum tNodeChecked
        {
            /// <summary>
            /// 未选中
            /// </summary>
            tNoChecked = 0,
            /// <summary>
            /// 选中
            /// </summary>
            tChecked = 1,
            /// <summary>
            /// 子节点选中
            /// </summary>
            tChildChecked = 2
        }

        #region 自定义属性

        string NowSelectNodeName;

        bool showCheckBox = false;

        /// <summary>
        /// 是否显示CheckBox框
        /// </summary>
        [DefaultValue(false)]
        public bool ShowCheckBox
        {
            get { return showCheckBox; }
            set
            {
                showCheckBox = value;
            }
        }

        bool clickDisenable;
        /// <summary>
        /// 启用点击是否有效
        /// </summary>
        public bool ClickDisenable
        {
            get { return clickDisenable; }
            set { clickDisenable = value; }
        }

        #endregion

        #region 自定义事件

        public delegate void NodeCheckAfterEventHandler(NodeCheckAfterEventArgs e);
        /// <summary>
        /// 节点选中选中状态改变事件
        /// </summary>
        public event NodeCheckAfterEventHandler OnNodeCheckAfter;

        private void NodeCheckAfter(TreeNode node, tNodeChecked nodeChecked)
        {
            if (OnNodeCheckAfter != null)
            {
                NodeCheckAfterEventArgs e = new NodeCheckAfterEventArgs();
                e.Node = node;
                e.NodeChecked = nodeChecked;
                OnNodeCheckAfter(e);
            }
        }

        /// <summary>
        /// 节点鼠标点击后事件
        /// </summary>
        public event TreeNodeMouseClickEventHandler OnNodeMouseClickAfter;

        private void NodeMouseClickAfter(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (OnNodeMouseClickAfter != null)
            {
                OnNodeMouseClickAfter(sender, e);
            }
        }

        #endregion

        public My_TreeView()
        {
            NowSelectNodeName = string.Empty;
        }

        

        protected override void OnLayout(LayoutEventArgs levent)
        {
            base.OnLayout(levent);
            if (showCheckBox)
            {
                ImageList = new ImageList();
                ImageList.ImageStream = Easytl.Properties.Resources.ImageStream;
                ImageList.TransparentColor = System.Drawing.Color.Transparent;
                ImageList.Images.SetKeyName(0, "nc.png");
                ImageList.Images.SetKeyName(1, "sc.png");
                ImageList.Images.SetKeyName(2, "tc.png");
            }
        }

        #region 节点选中函数

        protected override void OnBeforeExpand(TreeViewCancelEventArgs e)
        {
            int i = e.Node.ImageIndex;
            base.OnBeforeExpand(e);
            e.Node.SelectedImageIndex = i;
            e.Node.ImageIndex = i;
        }

        protected override void OnBeforeCollapse(TreeViewCancelEventArgs e)
        {
            int i = e.Node.ImageIndex;
            base.OnBeforeCollapse(e);
            e.Node.SelectedImageIndex = i;
            e.Node.ImageIndex = i;
        }

        /// <summary>
        /// 设置节点选定状态：规则.1：检查是否有子节点，需清除所有子节点的选定状态； 
        /// </summary>
        /// <param name="tn"></param>  
        private void SetNodeImg11(TreeNode tn)
        {
            foreach (TreeNode t in tn.Nodes)
            {
                if (t.ImageIndex != 0)
                {
                    t.SelectedImageIndex = 0;
                    t.ImageIndex = 0;

                    //引发选中事件
                    NodeCheckAfter(t, tNodeChecked.tNoChecked);
                }

                if (t.Nodes.Count != 0)
                {
                    SetNodeImg11(t);
                }
            }
        }


        /// <summary>
        /// 设置节点选定状态：规则.2：检查是否有父节点，如有，则根据兄弟节点的选定状态修改父节点的选定状态
        /// </summary>
        /// <param name="tn"></param>  
        private void SetNodeImg12(TreeNode tn)
        {
            if (tn.Parent == null)
                return;
            int Img0Num = 0;
            int Img1Num = 0;
            int Img2Num = 0;
            //统计兄弟节点中选中情况   
            foreach (TreeNode t in tn.Parent.Nodes)
            {
                if (t.ImageIndex == -1)
                    Img0Num++;
                if (t.ImageIndex == 0)
                    Img0Num++;
                if (t.ImageIndex == 1)
                    Img1Num++;
                if (t.ImageIndex == 2)
                    Img2Num++;
            }
            //如果兄弟节点中选中和未选中都有   
            if ((Img2Num != 0) || ((Img0Num != 0) && (Img1Num != 0)))
            {
                if (tn.Parent.ImageIndex != 2)
                {
                    tn.Parent.SelectedImageIndex = 2;
                    tn.Parent.ImageIndex = 2;

                    //引发选中事件
                    NodeCheckAfter(tn.Parent, tNodeChecked.tChildChecked);
                }
            }
            else
            {
                if (tn.Parent.ImageIndex != 0)
                {
                    tn.Parent.StateImageIndex = 0;
                    tn.Parent.ImageIndex = 0;

                    //引发选中事件
                    NodeCheckAfter(tn.Parent, tNodeChecked.tNoChecked);
                }
            }
            //递归   
            SetNodeImg12(tn.Parent);
        }


        /// <summary>
        /// 设置节点选定状态：规则.1：检查是否有子节点，设置所有子节点为选定状态
        /// </summary>
        /// <param name="tn"></param>  
        private void SetNodeImg21(TreeNode tn)
        {
            foreach (TreeNode t in tn.Nodes)
            {
                if (t.ImageIndex != 1)
                {
                    t.SelectedImageIndex = 1;
                    t.ImageIndex = 1;

                    //引发选中事件
                    NodeCheckAfter(t, tNodeChecked.tChecked);
                }

                if (t.Nodes.Count != 0)
                {
                    SetNodeImg21(t);
                }
            }
        }


        /// <summary>
        /// 设置节点选定状态：规则.2：检查是否有父节点，如有，则根据兄弟节点的选定状态修改父节点的选定状态
        /// </summary>
        /// <param name="tn"></param>
        private void SetNodeImg22(TreeNode tn)
        {
            if (tn.Parent == null)
                return;
            int Img0Num = 0;
            int Img1Num = 0;
            int Img2Num = 0;
            //统计兄弟节点中选中情况   
            foreach (TreeNode t in tn.Parent.Nodes)
            {
                if (t.ImageIndex == -1)
                    Img0Num++;
                if (t.ImageIndex == 0)
                    Img0Num++;
                if (t.ImageIndex == 1)
                    Img1Num++;
                if (t.ImageIndex == 2)
                    Img2Num++;
            }

            //如果兄弟节点中选中和未选中都有
            if ((Img2Num != 0) || ((Img0Num != 0) && (Img1Num != 0)))
            {
                if (tn.Parent.ImageIndex != 2)
                {
                    tn.Parent.SelectedImageIndex = 2;
                    tn.Parent.ImageIndex = 2;

                    //引发选中事件
                    NodeCheckAfter(tn.Parent, tNodeChecked.tChildChecked);
                }
            }
            else if ((Img1Num == 0) && (Img2Num == 0))
            {
                if (tn.Parent.ImageIndex != 0)
                {
                    tn.Parent.SelectedImageIndex = 0;
                    tn.Parent.ImageIndex = 0;

                    //引发选中事件
                    NodeCheckAfter(tn.Parent, tNodeChecked.tNoChecked);
                }
            }
            else
            {
                if (tn.Parent.ImageIndex != 1)
                {
                    tn.Parent.SelectedImageIndex = 1;
                    tn.Parent.ImageIndex = 1;

                    //引发选中事件
                    NodeCheckAfter(tn.Parent, tNodeChecked.tChecked);
                }
            }
            //递归   
            SetNodeImg22(tn.Parent);
        }

        #endregion

        protected override void OnNodeMouseClick(TreeNodeMouseClickEventArgs e)
        {
            base.OnNodeMouseClick(e);
            if (showCheckBox)
            {
                if (!ClickDisenable)
                {
                    if (e.Button == MouseButtons.Left)
                    {
                        if ((e.X <= (e.Node.Bounds.X + e.Node.Bounds.Width)) && (e.X >= e.Node.Bounds.X - 16))
                        {
                            if (e.Node.ImageIndex <= 0)
                            {
                                SetNodeChecked(e.Node, true);
                            }
                            else
                            {
                                SetNodeChecked(e.Node, false);
                            }
                        }
                    }
                }
            }
            NodeMouseClickAfter(this, e);
        }

        /// <summary>
        /// 设置节点选中状态
        /// </summary>
        /// <param name="node"></param>
        /// <param name="IsCheck"></param>
        public void SetNodeChecked(TreeNode node, bool IsCheck)
        {
            if (IsCheck)
            {
                if (node.ImageIndex != 1)
                {
                    node.SelectedImageIndex = 1;
                    node.ImageIndex = 1;

                    //引发选中事件
                    NodeCheckAfter(node, tNodeChecked.tChecked);

                    SetNodeImg21(node);
                    SetNodeImg22(node);
                }
            }
            else
            {
                if (node.ImageIndex != 0)
                {
                    node.SelectedImageIndex = 0;
                    node.ImageIndex = 0;

                    //引发选中事件
                    NodeCheckAfter(node, tNodeChecked.tNoChecked);

                    SetNodeImg11(node);
                    SetNodeImg12(node);
                }
            }
        }

        /// <summary>
        /// TreeView绑定字符串组以更改节点的选中状态
        /// </summary>
        /// <param name="OPFunctionList">字符串组</param>
        public void SetCheck_BindData(string[] OPFunctionList)
        {
            FunctionCheck(OPFunctionList, this);
        }

        /// <summary>
        /// 获取选中节点的文本值
        /// </summary>
        /// <param name="TreeSelectItemsTextList"></param>
        /// <param name="obj"></param>
        public void GetCheckedNodeText(ref List<string> CheckedNodeTextList, object TreeOrNode)
        {
            TreeNodeCollection nodes;
            if (TreeOrNode is TreeView)
            {
                nodes = (TreeOrNode as TreeView).Nodes;
                CheckedNodeTextList.Clear();
            }
            else
            {
                nodes = (TreeOrNode as TreeNode).Nodes;
            }
            for (int i = 0; i < nodes.Count; i++)
            {
                if ((nodes[i].ImageIndex == 1) || (nodes[i].ImageIndex == 2))
                {
                    CheckedNodeTextList.Add(nodes[i].Text);
                }
                if (nodes[i].Nodes.Count > 0)
                {
                    GetCheckedNodeText(ref CheckedNodeTextList, nodes[i]);
                }
            }
        }

        /// <summary>
        /// 获取选中节点的名称的列表
        /// </summary>
        /// <param name="TreeSelectItemsNameList"></param>
        /// <param name="obj"></param>
        public void GetCheckedNodeName(ref List<string> CheckedNodeNameList, object TreeOrNode)
        {
            TreeNodeCollection nodes;
            if (TreeOrNode is TreeView)
            {
                nodes = (TreeOrNode as TreeView).Nodes;
                CheckedNodeNameList.Clear();
            }
            else
            {
                nodes = (TreeOrNode as TreeNode).Nodes;
            }
            for (int i = 0; i < nodes.Count; i++)
            {
                if ((nodes[i].ImageIndex == 1) || (nodes[i].ImageIndex == 2))
                {
                    CheckedNodeNameList.Add(nodes[i].Name);
                }
                if (nodes[i].Nodes.Count > 0)
                {
                    GetCheckedNodeName(ref CheckedNodeNameList, nodes[i]);
                }
            }
        }

        /// <summary>
        /// 获取选中节点名称的字符串值
        /// </summary>
        /// <returns></returns>
        public void GetCheckedNodeName(ref string CheckedNodeName, object TreeOrNode)
        {
            TreeNodeCollection nodes;
            if (TreeOrNode is TreeView)
            {
                nodes = (TreeOrNode as TreeView).Nodes;
                CheckedNodeName = string.Empty;
            }
            else
            {
                nodes = (TreeOrNode as TreeNode).Nodes;
            }
            for (int i = 0; i < nodes.Count; i++)
            {
                if ((nodes[i].ImageIndex == 1) || (nodes[i].ImageIndex == 2))
                {
                    if (CheckedNodeName != string.Empty)
                    {
                        CheckedNodeName += ",";
                    }
                    CheckedNodeName += nodes[i].Name;
                }
                if (nodes[i].Nodes.Count > 0)
                {
                    GetCheckedNodeName(ref CheckedNodeName, nodes[i]);
                }
            }
        }

        private void FunctionCheck(string[] OPFunctionList, object obj)
        {
            TreeNodeCollection nodes;
            if (obj is TreeView)
            {
                nodes = (obj as TreeView).Nodes;
            }
            else
            {
                nodes = (obj as TreeNode).Nodes;
            }
            for (int i = 0; i < nodes.Count; i++)
            {
                if (OPFunctionList.Contains(nodes[i].Name))
                {
                    SetNodeChecked(nodes[i], true);
                }
                else
                {
                    SetNodeChecked(nodes[i], false);
                }
                if (nodes[i].Nodes.Count > 0)
                {
                    FunctionCheck(OPFunctionList, nodes[i]);
                }
            }
        }

        /// <summary>
        /// 获取节点选中状态
        /// </summary>
        /// <param name="node">节点</param>
        public tNodeChecked GetNodeCheckState(TreeNode node)
        {
            if (node.ImageIndex >= 0)
            {
                return (tNodeChecked)node.ImageIndex;
            }
            else
            {
                return tNodeChecked.tNoChecked;
            }
        }
    }

    /// <summary>
    /// 节点选中事件
    /// </summary>
    public class NodeCheckAfterEventArgs
    {
        TreeNode _node;
        /// <summary>
        /// 触发的节点
        /// </summary>
        public TreeNode Node
        {
            get { return _node; }
            set { _node = value; }
        }

        My_TreeView.tNodeChecked _NodeChecked;
        /// <summary>
        /// 节点的选中状态
        /// </summary>
        public My_TreeView.tNodeChecked NodeChecked
        {
            get { return _NodeChecked; }
            set { _NodeChecked = value; }
        }
    }
}
