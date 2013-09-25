using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace WebSearch.Maths.Net
{
    /// <summary>
    /// 一次方程 (Simple Equation/Linear Equation)
    /// </summary>
    public class LinearEquation : PolynomialEquation
    {
        #region Indexer
        /// <summary>
        /// 索引器
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public float this[char name]
        {
            get { return ((EquationNode)equationNodes[name]).C; }
            set { ((EquationNode)equationNodes[name]).C = value; }
        }

        #endregion

        #region Constructors
        /// <summary>
        /// 构造函数1，默认构造函数
        /// </summary>
        public LinearEquation() : base()
        {
        }

        /// <summary>
        /// 构造函数2
        /// </summary>
        /// <param name="nodes"></param>
        public LinearEquation(Hashtable nodes) : base(nodes)
        {
            foreach (EquationNode node in nodes)
                // 只可以使常数(p = 0)或一次项(p = 1)
                if (node.P > 1) this.isValid = false;
        }

        /// <summary>
        /// 构造函数3，ax + by + c = 0
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        public LinearEquation(float a, float b, float c) : base()
        {
            this.equationNodes.Add('x', new EquationNode(a, 'x'));
            this.equationNodes.Add('y', new EquationNode(b, 'y'));
            this.equationNodes.Add(Equation.C, new EquationNode(c));
        }

        /// <summary>
        /// 构造函数4，ax + by + cz + d = 0
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <param name="d"></param>
        public LinearEquation(float a, float b, float c, float d) : base()
        {
            this.equationNodes.Add('x', new EquationNode(a, 'x'));
            this.equationNodes.Add('y', new EquationNode(b, 'y'));
            this.equationNodes.Add('z', new EquationNode(c, 'z'));
            this.equationNodes.Add(Equation.C, new EquationNode(d));
        }

        /// <summary>
        /// 当前 Equation 对象的副本
        /// </summary>
        /// <returns></returns>
        public LinearEquation Copy()
        {
            return new LinearEquation(equationNodes);
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// 当前 Equation 加上一个给定的 Equation Node
        /// </summary>
        /// <param name="node"></param>
        public override void Add(EquationNode node)
        {
            node.P = (node.P <= 1) ? node.P : 1;
            // if already has such a node with the same name
            if (this.equationNodes.ContainsKey(node.Name))
                ((EquationNode)this.Nodes[node.Name]).C += node.C;
            else
                this.equationNodes.Add(node.Name, node);
        }

        /// <summary>
        /// 移除指定变元名的 Equation Node
        /// </summary>
        /// <param name="name"></param>
        public void Remove(char name)
        {
            this.equationNodes.Remove(name);
        }

        /// <summary>
        /// 根据给出的变量值求出该方程的结果
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public float Value(Hashtable param)
        {
            float result = 0;
            foreach (EquationNode node in this.equationNodes)
                // 累加每个 Equation Node 的值
                if (param.ContainsKey(node.Name))
                    result += node.Value((float)param[node.Name]);
                // 对于未给出参数值的node，作为0处理
            return result;
        }

        /// <summary>
        /// 运算符+的重载：两一次方程相加
        /// </summary>
        /// <param name="e1"></param>
        /// <param name="e2"></param>
        /// <returns></returns>
        public static LinearEquation operator +(LinearEquation e1, LinearEquation e2)
        {
            LinearEquation result = e1.Copy();
            foreach (EquationNode node in e2.equationNodes)
                result.Add(node);
            return result;
        }
        #endregion
    }
}
