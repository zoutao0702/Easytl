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
        /// �ڵ�ѡ������
        /// </summary>
        public enum tNodeChecked
        {
            /// <summary>
            /// δѡ��
            /// </summary>
            tNoChecked = 0,
            /// <summary>
            /// ѡ��
            /// </summary>
            tChecked = 1,
            /// <summary>
            /// �ӽڵ�ѡ��
            /// </summary>
            tChildChecked = 2
        }

        #region �Զ�������

        string NowSelectNodeName;

        bool showCheckBox = false;

        /// <summary>
        /// �Ƿ���ʾCheckBox��
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
        /// ���õ���Ƿ���Ч
        /// </summary>
        public bool ClickDisenable
        {
            get { return clickDisenable; }
            set { clickDisenable = value; }
        }

        #endregion

        #region �Զ����¼�

        public delegate void NodeCheckAfterEventHandler(NodeCheckAfterEventArgs e);
        /// <summary>
        /// �ڵ�ѡ��ѡ��״̬�ı��¼�
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
        /// �ڵ���������¼�
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

        #region �ڵ�ѡ�к���

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
        /// ���ýڵ�ѡ��״̬������.1������Ƿ����ӽڵ㣬����������ӽڵ��ѡ��״̬�� 
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

                    //����ѡ���¼�
                    NodeCheckAfter(t, tNodeChecked.tNoChecked);
                }

                if (t.Nodes.Count != 0)
                {
                    SetNodeImg11(t);
                }
            }
        }


        /// <summary>
        /// ���ýڵ�ѡ��״̬������.2������Ƿ��и��ڵ㣬���У�������ֵܽڵ��ѡ��״̬�޸ĸ��ڵ��ѡ��״̬
        /// </summary>
        /// <param name="tn"></param>  
        private void SetNodeImg12(TreeNode tn)
        {
            if (tn.Parent == null)
                return;
            int Img0Num = 0;
            int Img1Num = 0;
            int Img2Num = 0;
            //ͳ���ֵܽڵ���ѡ�����   
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
            //����ֵܽڵ���ѡ�к�δѡ�ж���   
            if ((Img2Num != 0) || ((Img0Num != 0) && (Img1Num != 0)))
            {
                if (tn.Parent.ImageIndex != 2)
                {
                    tn.Parent.SelectedImageIndex = 2;
                    tn.Parent.ImageIndex = 2;

                    //����ѡ���¼�
                    NodeCheckAfter(tn.Parent, tNodeChecked.tChildChecked);
                }
            }
            else
            {
                if (tn.Parent.ImageIndex != 0)
                {
                    tn.Parent.StateImageIndex = 0;
                    tn.Parent.ImageIndex = 0;

                    //����ѡ���¼�
                    NodeCheckAfter(tn.Parent, tNodeChecked.tNoChecked);
                }
            }
            //�ݹ�   
            SetNodeImg12(tn.Parent);
        }


        /// <summary>
        /// ���ýڵ�ѡ��״̬������.1������Ƿ����ӽڵ㣬���������ӽڵ�Ϊѡ��״̬
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

                    //����ѡ���¼�
                    NodeCheckAfter(t, tNodeChecked.tChecked);
                }

                if (t.Nodes.Count != 0)
                {
                    SetNodeImg21(t);
                }
            }
        }


        /// <summary>
        /// ���ýڵ�ѡ��״̬������.2������Ƿ��и��ڵ㣬���У�������ֵܽڵ��ѡ��״̬�޸ĸ��ڵ��ѡ��״̬
        /// </summary>
        /// <param name="tn"></param>
        private void SetNodeImg22(TreeNode tn)
        {
            if (tn.Parent == null)
                return;
            int Img0Num = 0;
            int Img1Num = 0;
            int Img2Num = 0;
            //ͳ���ֵܽڵ���ѡ�����   
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

            //����ֵܽڵ���ѡ�к�δѡ�ж���
            if ((Img2Num != 0) || ((Img0Num != 0) && (Img1Num != 0)))
            {
                if (tn.Parent.ImageIndex != 2)
                {
                    tn.Parent.SelectedImageIndex = 2;
                    tn.Parent.ImageIndex = 2;

                    //����ѡ���¼�
                    NodeCheckAfter(tn.Parent, tNodeChecked.tChildChecked);
                }
            }
            else if ((Img1Num == 0) && (Img2Num == 0))
            {
                if (tn.Parent.ImageIndex != 0)
                {
                    tn.Parent.SelectedImageIndex = 0;
                    tn.Parent.ImageIndex = 0;

                    //����ѡ���¼�
                    NodeCheckAfter(tn.Parent, tNodeChecked.tNoChecked);
                }
            }
            else
            {
                if (tn.Parent.ImageIndex != 1)
                {
                    tn.Parent.SelectedImageIndex = 1;
                    tn.Parent.ImageIndex = 1;

                    //����ѡ���¼�
                    NodeCheckAfter(tn.Parent, tNodeChecked.tChecked);
                }
            }
            //�ݹ�   
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
        /// ���ýڵ�ѡ��״̬
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

                    //����ѡ���¼�
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

                    //����ѡ���¼�
                    NodeCheckAfter(node, tNodeChecked.tNoChecked);

                    SetNodeImg11(node);
                    SetNodeImg12(node);
                }
            }
        }

        /// <summary>
        /// TreeView���ַ������Ը��Ľڵ��ѡ��״̬
        /// </summary>
        /// <param name="OPFunctionList">�ַ�����</param>
        public void SetCheck_BindData(string[] OPFunctionList)
        {
            FunctionCheck(OPFunctionList, this);
        }

        /// <summary>
        /// ��ȡѡ�нڵ���ı�ֵ
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
        /// ��ȡѡ�нڵ�����Ƶ��б�
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
        /// ��ȡѡ�нڵ����Ƶ��ַ���ֵ
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
        /// ��ȡ�ڵ�ѡ��״̬
        /// </summary>
        /// <param name="node">�ڵ�</param>
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
    /// �ڵ�ѡ���¼�
    /// </summary>
    public class NodeCheckAfterEventArgs
    {
        TreeNode _node;
        /// <summary>
        /// �����Ľڵ�
        /// </summary>
        public TreeNode Node
        {
            get { return _node; }
            set { _node = value; }
        }

        My_TreeView.tNodeChecked _NodeChecked;
        /// <summary>
        /// �ڵ��ѡ��״̬
        /// </summary>
        public My_TreeView.tNodeChecked NodeChecked
        {
            get { return _NodeChecked; }
            set { _NodeChecked = value; }
        }
    }
}
