using System;
using System.Collections.Generic;
using System.Text;
using WebSearch.Common.Net;

namespace WebSearch.Maths.Net
{
    /// <summary>
    /// 向量
    /// </summary>
    public class Vector
    {
        private int size;
        private float[] elements;

        /// <summary>
        /// The size of the vector
        /// </summary>
        public int Size
        {
            get { return size; }
            set { size = value; }
        }

        /// <summary>
        /// The elment array of the vector
        /// </summary>
        public float[] Elements
        {
            get { return elements; }
        }

        /// <summary>
        /// Whether all the elements in this vector is 0
        /// </summary>
        public bool IsZero
        {
            get
            {
                for (int i = 0; i < this.size; i++)
                {
                    if (elements[i] != 0)
                        return false;
                }
                return true;
            }
        }

        public Vector()
        {
            this.size = 2;
            this.elements = new float[size];
        }

        public Vector(int size)
        {
            this.size = size;
            this.elements = new float[size];
        }

        public Vector(float x, float y)
        {
            this.size = 2;
            this.elements = new float[size];
            this.elements[0] = x;
            this.elements[1] = y;
        }

        public Vector(float x, float y, float z)
        {
            this.size = 3;
            this.elements = new float[size];
            this.elements[0] = x;
            this.elements[1] = y;
            this.elements[2] = z;
        }

        public Vector Copy()
        {
            Vector result = new Vector(this.size);
            for (int i = 0; i < this.size; i++)
                result.elements[i] = this.elements[i];
            return result;
        }

        /// <summary>
        /// 测试指定的对象是否是 Vector 对象以及是否等同于此 Vector 对象
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public bool Equals(Vector vector)
        {
            if (this.size != vector.size)
                return false;
            // otherwise, check the elements
            for (int i = 0; i < this.size; i++)
            {
                if (this.elements[i] != vector.elements[i])
                    return false;
            }
            return true;
        }

        /// <summary>
        /// 将此 Vector 对象与 vector 参数中指定的向量相加
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public bool Add(Vector vector)
        {
            // 两向量结构不同, 不可进一步相加
            if (this.size != vector.size)
                return false;
            for (int i = 0; i < this.size; i++)
                elements[i] += vector.elements[i];
            return true;
        }

        /// <summary>
        /// 运算符+的重载：两向量相加
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        public static Vector operator +(Vector v1, Vector v2)
        {
            Vector result = v1.Copy();
            if (result.Add(v2))
                return result;
            return null;
        }

        /// <summary>
        /// 将此 Vector 对象与 vector 参数中指定的向量相减
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public bool Minus(Vector vector)
        {
            // 两向量结构不同, 不可进一步相减
            if (this.size != vector.size)
                return false;
            for (int i = 0; i < this.size; i++)
                elements[i] -= vector.elements[i];
            return true;
        }

        /// <summary>
        /// 运算符-的重载：两向量相减
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        public static Vector operator -(Vector v1, Vector v2)
        {
            Vector result = v1.Copy();
            if (result.Minus(v2))
                return result;
            return null;
        }

        /// <summary>
        /// 用数字乘以向量
        /// </summary>
        /// <param name="multiplier"></param>
        /// <returns></returns>
        public void Multiply(float multiplier)
        {
            for (int i = 0; i < this.size; i++)
                elements[i] *= multiplier;
        }

        /// <summary>
        /// 运算符*的重载：用数字乘以向量
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="multiplier"></param>
        /// <returns></returns>
        public static Vector operator *(Vector v1, float multiplier)
        {
            Vector result = v1.Copy();
            result.Multiply(multiplier);
            return result;
        }

        /// <summary>
        /// 运算符*的重载：用数字乘以向量
        /// </summary>
        /// <param name="multiplier"></param>
        /// <param name="v1"></param>
        /// <returns></returns>
        public static Vector operator *(float multiplier, Vector v1)
        {
            Vector result = v1.Copy();
            result.Multiply(multiplier);
            return result;
        }

        /// <summary>
        /// 将此 Vector 对象与 vector 参数中指定的向量相点积
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public float DotProduct(Vector vector)
        {
            if (this.size != vector.size)
                return Const.MaximumValue;
            float result = 0;
            for (int i = 0; i < this.size; i++)
                result += (elements[i] * vector.elements[i]);
            return result;
        }

        /// <summary>
        /// 将此 Vector 对象与 vector 参数中指定的向量相叉积
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public bool CrossProduct(Vector vector)
        {
            Vector v1 = this.ToStandard();
            if (v1 == null) return false;
            Vector v2 = vector.ToStandard();
            if (v2 == null) return false;

            Matrix x = new Matrix(v1.elements[1], v1.elements[2], v2.elements[1], v2.elements[2]);
            Matrix y = new Matrix(v1.elements[0], v1.elements[2], v2.elements[0], v2.elements[2]);
            Matrix z = new Matrix(v1.elements[0], v1.elements[1], v2.elements[0], v2.elements[1]);

            this.elements = new float[3]{x.Modulus, -y.Modulus, z.Modulus};
            this.size = 3;
            return true;
        }

        /// <summary>
        /// 运算符*的重载：用于两向量的叉击
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        public static Vector operator *(Vector v1, Vector v2)
        {
            Vector result = v1.Copy();
            if (result.CrossProduct(v2))
                return result;
            return null;
        }

        /// <summary>
        /// 有当前向量创建一个标准的向量副本(xi, yj, zk)
        /// </summary>
        /// <returns></returns>
        public Vector ToStandard()
        {
            if (this.size > 3)
                return null;
            Vector result = new Vector(3);
            switch (this.size)
            {
                case 1:
                    result.elements[0] = elements[0];
                    result.elements[1] = 0;
                    result.elements[2] = 0;
                    return result;
                case 2:
                    result.elements[0] = elements[0];
                    result.elements[1] = elements[1];
                    result.elements[2] = 0;
                    return result;
                case 3:
                    result.elements[0] = elements[0];
                    result.elements[1] = elements[1];
                    result.elements[2] = elements[2];
                    return result;
                default:
                    return null;
            }
        }
    }
}
