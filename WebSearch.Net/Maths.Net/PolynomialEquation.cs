using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace WebSearch.Maths.Net
{
    /// <summary>
    /// 多项式方程
    /// </summary>
    public class PolynomialEquation : Equation
    {
        // equation nodes 的列表，key为node的权值，value为node
        protected Hashtable equationNodes = new Hashtable();

        #region Properties
        /// <summary>
        /// 
        /// </summary>
        public Hashtable Nodes
        {
            get { return this.equationNodes; }
            set { this.equationNodes = value; }
        }
        
        /// <summary>
        /// 
        /// </summary>
        public int Length
        {
            get { return this.equationNodes.Count; }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// 构造函数1，默认构造函数
        /// </summary>
        public PolynomialEquation()
        {
        }

        /// <summary>
        /// 构造函数2
        /// </summary>
        /// <param name="nodes"></param>
        public PolynomialEquation(Hashtable nodes)
        {
            this.equationNodes = (Hashtable)nodes.Clone();
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// 当前多项式方程加上一个给定的 Equation Node
        /// </summary>
        /// <param name="node"></param>
        public virtual void Add(EquationNode node)
        {
            this.equationNodes.Add(node.Name, node);
        }

        /// <summary>
        /// 清除当前多项式方程中的数据
        /// </summary>
        public virtual void Clear()
        {
            this.equationNodes.Clear();
        }
        #endregion
    }
}
