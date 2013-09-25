using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace WebSearch.DataMiner.Net.DT
{
    public class TreeNode
    {
        #region Properties

        protected List<TreeNode> _children = null;

        public int ChildrenCount
        {
            get { return _children.Count; }
        }

        protected Attribute _attribute;

        public Attribute Attribute
        {
            get { return _attribute; }
            set { _attribute = value; }
        }

        #endregion

        #region Constructors

        public TreeNode(Attribute attribute)
        {
            if (attribute.Values != null)
            {
                _children = new List<TreeNode>(attribute.Values.Count);
                for (int i = 0; i < attribute.Values.Count; i++)
                    _children.Add(null);
            }
            else
            {
                _children = new List<TreeNode>(1);
                _children.Add(null);
            }
            _attribute = attribute;
        }

        #endregion

        #region Methods

        public void AddChild(TreeNode treeNode, string value)
        {
            int index = _attribute.GetIndex(value);
            _children[index] = treeNode;
        }

        public TreeNode GetChild(int index)
        {
            return _children[index];
        }

        public TreeNode GetChildByBranchName(string branchName)
        {
            int index = _attribute.GetIndex(branchName);
            return _children[index];
        }

        #endregion
    }
}