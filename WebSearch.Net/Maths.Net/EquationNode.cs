using System;
using System.Collections.Generic;
using System.Text;

namespace WebSearch.Maths.Net
{
    public class EquationNode
    {
        private float coefficient = 1;      // 系数
        private int power = 0;              // 权值
        private char metaVariable;          // 元变量名

        private bool isValid = true;

        public float C
        {
            get { return this.coefficient; }
            set { this.coefficient = value; }
        }
        public int P
        {
            get { return this.power; }
            set { this.power = value; }
        }
        public char Name
        {
            get { return this.metaVariable; }
            set { this.metaVariable = value; }
        }
        public bool IsValid
        {
            get { return this.isValid; }
            set { this.isValid = value; }
        }
        public bool IsConstant
        {
            get { return this.power == 0 || this.IsZero; }
        }
        public bool IsZero
        {
            get { return this.coefficient == 0; }
        }

        /// <summary>
        /// 初始化为系数为1的一次变元 (如 x)
        /// </summary>
        /// <param name="name"></param>
        public EquationNode(char name)
        {
            //this.coefficient = 1;
            this.metaVariable = name;
            this.power = 1;
        }

        /// <summary>
        /// 初始化为一常数 (如 2)
        /// </summary>
        /// <param name="c"></param>
        public EquationNode(float c)
        {
            this.metaVariable = Equation.C;
            this.coefficient = c;
            //this.power = 0;
        }

        /// <summary>
        /// 初始化为给定系数的一次变元 (如 2x)
        /// </summary>
        /// <param name="c"></param>
        /// <param name="name"></param>
        public EquationNode(float c, char name)
        {
            if (name == Equation.C)
                throw new ArgumentException("Invalid constant equation node");
            this.coefficient = c;
            this.metaVariable = name;
            this.power = 1;
        }

        /// <summary>
        /// 初始化为系数为1的node (如 x^2)
        /// </summary>
        /// <param name="name"></param>
        /// <param name="p"></param>
        public EquationNode(char name, int p) 
        {
            if (name == Equation.C && p != 0)
                throw new ArgumentException("Invalid constant equation node");
            this.metaVariable = name;
            this.power = p;
        }

        /// <summary>
        /// 初始化为给定系数的p次变元 (如 2x^2)
        /// </summary>
        /// <param name="c"></param>
        /// <param name="name"></param>
        /// <param name="p"></param>
        public EquationNode(float c, char name, int p)
        {
            if (name == Equation.C && p != 0)
                throw new ArgumentException("Invalid constant equation node");
            this.coefficient = c;
            this.metaVariable = name;
            this.power = p;
        }

        /// <summary>
        /// 根据给出的变量值求出该node的值
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public float Value(float param)
        {
            return coefficient * (float)Math.Pow(param, power);
        }
    }
}
