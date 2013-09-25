using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using WebSearch.Common.Net;

namespace WebSearch.Maths.Net
{
    /// <summary>
    /// 一元方程 (Equation in one unknown)
    /// </summary>
    public class OneUnknownEquation : PolynomialEquation
    {
        // 元变量名称 (如：x, y)
        private char metaVariable;

        public char MetaVar
        {
            get { return this.metaVariable; }
            set { this.metaVariable = value; }
        }
        public EquationNode Highest
        {
            get
            {
                if (this.equationNodes.Count == 0) // empty
                    return null;
                float resultIndex = Const.MinimumValue;
                // find the largest key in this equation
                foreach (int key in this.equationNodes.Keys)
                    resultIndex = Math.Max(resultIndex, key);
                // return the equation node
                return (EquationNode) this.equationNodes[resultIndex];
            }
        }

        public OneUnknownEquation(char name)
        {
            this.metaVariable = name;
        }
        public OneUnknownEquation(char name, float c, int p)
        {
            this.metaVariable = name;
            this.equationNodes.Add(p, new EquationNode(c, name, p));
        }
        public OneUnknownEquation(char name, Hashtable nodes)
        {
            this.metaVariable = name;
            this.equationNodes = (Hashtable)nodes.Clone();
        }

        /// <summary>
        /// 当前 Equation 对象的副本
        /// </summary>
        /// <returns></returns>
        public OneUnknownEquation Copy()
        {
            return new OneUnknownEquation(metaVariable, equationNodes);
        }

        /// <summary>
        /// 当前 Equation 加上一个给定的 Equation Node
        /// </summary>
        /// <param name="node"></param>
        public override void Add(EquationNode node)
        {
            node.Name = this.metaVariable;  // ensure the name
            // if already has such a node with the same power
            if (this.equationNodes.ContainsKey(node.P))
                ((EquationNode)this.Nodes[node.P]).C += node.C;
            else
                this.equationNodes.Add(node.P, node);
        }

        /// <summary>
        /// 移除指定权值的 Equation Node
        /// </summary>
        /// <param name="power"></param>
        public void Remove(int power)
        {
            this.equationNodes.Remove(power);
        }

        /// <summary>
        /// 根据给出的变量值求出该方程的结果
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public float Value(float param)
        {
            float result = 0;
            foreach (EquationNode node in this.equationNodes)
                // 累加每个 Equation Node 的值
                result += node.Value(param);
            return result;
        }

        /// <summary>
        /// 运算符+的重载：两一元方程相加
        /// </summary>
        /// <param name="e1"></param>
        /// <param name="e2"></param>
        /// <returns></returns>
        public static OneUnknownEquation operator +(OneUnknownEquation e1, OneUnknownEquation e2)
        {
            if (e1.MetaVar != e2.MetaVar)  // ensure they have the same metaname
                return null;
            OneUnknownEquation result = e1.Copy();
            foreach (EquationNode node in e2.equationNodes)
                result.Add(node);
            return result;
        }
    }
}
