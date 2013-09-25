using System;
using System.Collections.Generic;
using System.Text;
using WebSearch.Common.Net;

namespace WebSearch.Maths.Net
{
    /// <summary>
    /// 矩阵
    /// </summary>
    public class Matrix
    {
        // matrix dimensions
        private int rows;
        private int cols;
        private float[,] elements;

        /// <summary>
        /// 行数
        /// </summary>
        public int RowNum
        {
            get { return rows; }
            set { rows = value; }
        }

        /// <summary>
        /// 列数
        /// </summary>
        public int ColumnNum
        {
            get { return cols; }
            set { cols = value; }
        }

        /// <summary>
        /// 元素的集合
        /// </summary>
        public float[,] Elements
        {
            get { return elements; }
        }

        /// <summary>
        /// 矩阵的模
        /// </summary>
        public float Modulus
        {
            get 
            {
                if (this.rows != this.cols)  // 只有方阵可以求模
                    return 0;
                // calculate and return the modulus of the matrix
                return this.ModulusValue(elements, this.rows);                
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public Matrix()
        {
            this.rows = this.cols = 2;  // default:2*2
            this.elements = new float[rows, cols];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rowNum"></param>
        /// <param name="colNum"></param>
        public Matrix(int rowNum, int colNum)
        {
            this.rows = rowNum;
            this.cols = colNum;
            this.elements = new float[rows, cols];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="elements"></param>
        /// <param name="rowNum"></param>
        /// <param name="colNum"></param>
        public Matrix(float[,] elements, int rowNum, int colNum)
        {
            this.rows = rowNum;
            this.cols = colNum;
            this.elements = elements;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="a11"></param>
        /// <param name="a12"></param>
        /// <param name="a21"></param>
        /// <param name="a22"></param>
        public Matrix(float a11, float a12, float a21, float a22)
        {
            this.rows = this.cols = 2;
            this.elements = new float[rows, cols];
            this.elements[0, 0] = a11;
            this.elements[0, 1] = a12;
            this.elements[1, 0] = a21;
            this.elements[1, 1] = a22;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="elements"></param>
        /// <param name="size"></param>
        public Matrix(float[,] elements, int size)
        {
            this.rows = this.cols = size;
            this.elements = elements;
        }

        /// <summary>
        /// 创建此Matrix对象的一个精确副本
        /// </summary>
        /// <returns></returns>
        public Matrix Copy()
        {
            Matrix result = new Matrix(this.rows, this.cols);
            // copy the element in the matrix
            for (int i = 0; i < this.rows; i++)
                for (int j = 0; j < this.cols; j++)
                    result.elements[i, j] = this.elements[i, j];
            return result;
        }

        /// <summary>
        /// 测试指定的对象是否是 Matrix 对象以及是否等同于此 Matrix 对象
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public bool Equals(Matrix matrix)
        {
            if (this.rows != matrix.rows || this.cols != matrix.cols)
                return false;
            // otherwise, check the elements
            for (int i = 0; i < this.rows; i++)
            {
                for (int j = 0; j < this.cols; j++)
                    if (this.elements[i, j] != matrix.elements[i, j])
                        return false;
            }
            return true;
        }

        /// <summary>
        /// 平移矩阵
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        public static Matrix TranslationalMatrix(float x, float y, float z)
        {
            float [,] array = {{1, 0, 0, x}, 
                               {0, 1, 0, y}, 
                               {0, 0, 1, z}, 
                               {0, 0, 0, 1}};
            return new Matrix(array, 4);
        }

        /// <summary>
        /// 缩放矩阵
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        public static Matrix ScaleMatrix(float x, float y, float z)
        {
            float[,] array = {{x, 0, 0, 0},
                               {0, y, 0, 0},
                               {0, 0, z, 0},
                               {0, 0, 0, 1}};
            return new Matrix(array, 4);
        }

        /// <summary>
        /// 如果此 Matrix 对象是可逆转的，则逆转该对象
        /// </summary>
        public void Invert()
        {
        }

        /// <summary>
        /// 将此 Matrix 对象与 matrix 参数中指定的矩阵相加
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public bool Add(Matrix matrix)
        {
            // 两矩阵结构不同, 不可进一步相加
            if (this.rows != matrix.rows || this.cols != matrix.cols)
                return false;
            for (int i = 0; i < this.rows; i++)
                for (int j = 0; j < this.cols; j++)
                    elements[i, j] += matrix.elements[i, j];
            return true;
        }

        /// <summary>
        /// 运算符+的重载：用于两矩阵相加
        /// </summary>
        /// <param name="m1"></param>
        /// <param name="m2"></param>
        /// <returns></returns>
        public static Matrix operator +(Matrix m1, Matrix m2)
        {
            Matrix result = m1.Copy();
            if (result.Add(m2))
                return result;
            return null;
        }

        /// <summary>
        /// 将此 Matrix 对象与 matrix 参数中指定的矩阵相减
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public bool Minus(Matrix matrix)
        {
            // 两矩阵结构不同, 不可进一步相加
            if (this.rows != matrix.rows || this.cols != matrix.cols)
                return false;
            for (int i = 0; i < this.rows; i++)
                for (int j = 0; j < this.cols; j++)
                    elements[i, j] -= matrix.elements[i, j];
            return true;
        }

        /// <summary>
        /// 运算符-的重载：用于两矩阵相减
        /// </summary>
        /// <param name="m1"></param>
        /// <param name="m2"></param>
        /// <returns></returns>
        public static Matrix operator -(Matrix m1, Matrix m2)
        {
            Matrix result = m1.Copy();
            if (result.Minus(m2))
                return result;
            return null;
        }

        /// <summary>
        /// 将此 Matrix 对象与 matrix 参数中指定的矩阵相乘
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public bool Multiply(Matrix matrix)
        {
            if (this.cols != matrix.rows)       // 两矩阵结构不同
                return false;                   // 不可进一步相乘
            else
            {
                float[,] result = new float[rows, matrix.cols];
                for (int i = 0; i < this.rows; i++)         // for each row
                    for (int j = 0; j < matrix.cols; j++)   // for each col
                    {
                        float sum = elements[i, 0] * matrix.elements[0, j];
                        for (int k = 1; k < this.cols; k++)
                            sum += elements[i, k] * matrix.elements[k, j];
                        result[i, j] = sum;
                    }
                this.elements = result;
                this.cols = matrix.cols;        // [rows, matrix.cols]
                return true;
            }
        }

        /// <summary>
        /// 运算符*的重载：用于两矩阵相乘
        /// </summary>
        /// <param name="m1">乘法操作数1</param>
        /// <param name="m2">乘法操作数2</param>
        /// <returns></returns>
        public static Matrix operator *(Matrix m1, Matrix m2)
        {
            Matrix result = m1.Copy();
            if (result.Multiply(m2))
                return result;
            return null;
        }

        /// <summary>
        /// 预先计算此 Matrix 对象沿原点和按指定角度的顺时针旋转
        /// </summary>
        /// <param name="angle"></param>
        public void Rotate(float angle)
        {
        }

        /// <summary>
        /// 利用全选主元高斯-约当法求实矩阵的逆
        /// </summary>
        /// <returns></returns>
        public Matrix Reverse()
        {
            if (this.rows != this.cols)
                return null;
            float[,] resultArray = this.ReverseValue(elements, rows);
            if (resultArray == null)
                return null;
            return new Matrix(resultArray, rows);
        }

        /// <summary>
        /// 利用全选主元高斯消去法求矩阵的秩
        /// </summary>
        /// <returns></returns>
        public int Rank()
        {
            int i, j, row, col;
            float max, temp;
            for (int k = 0; k < this.rows; k++) // 找主元
            {
                max = 0; row = col = k;
                for (i = k; i < this.rows; i++)
                    for (j = k; j < this.cols; j++)
                    {
                        temp = Math.Abs(elements[i, j]);
                        if (max < temp) 
                        { max = temp; row = i; col = j; }
                    }
                // 交换行列，将主元调整到k行k列上
                if (max == 0) return k;
                if (row != k)
                {
                    for (j = 0; j < this.cols; j++)
                    { 
                        temp = elements[row, j]; 
                        elements[row, j] = elements[k, j];
                        elements[k, j] = temp; 
                    }
                }
                if (col != k)
                {
                    for (i = 0; i < this.rows; i++)
                    {
                        temp = elements[i, col];
                        elements[i, col] = elements[i, k];
                        elements[i, k] = temp; 
                    }
                }

                for (j = k + 1; j < this.cols; j++) 
                    elements[k, j] /= elements[k, k];
                for (j = k + 1; j < this.cols; j++)
                    for (i = k + 1; i < this.rows; i++)
                        elements[i, j] -= elements[i, k] * elements[k, j];
                for (i = 0; i < k; i++) elements[i, k] = 0;
                elements[k, k] = 1;
            }
            return this.rows;
        }

        /// <summary>
        /// 利用全选主元高斯-约当法求实矩阵的逆的辅助函数
        /// </summary>
        /// <param name="elements"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        private float[,] ReverseValue(float[,] elements, int level)
        {
            int i, j, row, col;
            double max, temp;

            float[,] copyElements = new float[level, level];
            for (i = 0; i < level; i++)
                for (j = 0; j < level; j++)
                    copyElements[i, j] = elements[i, j];
            int[] indexer = new int[level];
            float[,] result = new float[level, level];
            for (i = 0; i < level; i++)
            { indexer[i] = i; result[i, i] = 1; }

            for (int k = 0; k < level; k++) // 找主元
            {
                max = 0; row = col = i;
                for (i = k; i < level; i++)
                {
                    for (j = k; j < level; j++)
                    {
                        temp = Math.Abs(result[i, j]);
                        if (max < temp) 
                        { max = temp; row = i; col = j; }
                    }
                }
                // 交换行列，将主元调整到k行k列上
                if (row != k)
                {
                    for (j = 0; j < level; j++)
                    {
                        temp = copyElements[row, j]; 
                        copyElements[row, j] = copyElements[k, j]; 
                        copyElements[k, j] = (float)temp;
                        temp = result[row, j]; 
                        result[row, j] = result[k, j]; 
                        result[k, j] = (float)temp;
                    }
                }
                if (col != k)
                {
                    for (i = 0; i < level; i++)
                    { 
                        temp = copyElements[i, col]; 
                        copyElements[i, col] = copyElements[i, k]; 
                        copyElements[i, k] = (float)temp;
                    }
                    j = indexer[col]; 
                    indexer[col] = indexer[k];
                    indexer[k] = j;
                }
                // 处理
                for (j = k + 1; j < level; j++) 
                    copyElements[k, j] /= copyElements[k, k];
                for (j = 0; j < level; j++)
                    result[k, j] /= copyElements[k, k];
                copyElements[k, k] = 1;
                for (j = k + 1; j < level; j++)
                {
                    for (i = 0; i < k; i++) 
                        copyElements[i, j] -= copyElements[i, k] * copyElements[k, j];
                    for (i = k + 1; i < level; i++)
                        copyElements[i, j] -= copyElements[i, k] * copyElements[k, j];
                }
                for (j = 0; j < level; j++)
                {
                    for (i = 0; i < k; i++) 
                        result[i, j] -= copyElements[i, k] * result[k, j];
                    for (i = k + 1; i < level; i++) 
                        result[i, j] -= copyElements[i, k] * result[k, j];
                }
                for (i = 0; i < k; i++) copyElements[i, k] = 0;
                copyElements[k, k] = 1;
            }
            // 恢复行列次序
            for (j = 0; j < level; j++)
                for (i = 0; i < level; i++)
                    copyElements[indexer[i], j] = result[i, j];
            return copyElements;
        }

        /// <summary>
        /// 计算方阵的辅助函数
        /// </summary>
        /// <param name="elements"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        private float ModulusValue(float[,] elements, int level)
        {
            // create a copy of the given array: dMatrix
            float[,] copyMatrix = new float[level, level];
            for (int i = 0; i < level; i++)
                for (int j = 0; j < level; j++)
                    copyMatrix[i, j] = elements[i, j];

            int sign = 1, index;
            for (int i = 0, j = 0; i < level && j < level; i++, j++)
            {
                if (copyMatrix[i, j] == 0)
                {
                    for (index = i; index < level && copyMatrix[index, j] == 0; index++) ;
                    if (index == level) return 0;
                    else
                    {
                        // row change between i-row and m-row
                        for (int n = j; n < level; n++)
                        {
                            float temp = copyMatrix[i, n];
                            copyMatrix[i, n] = copyMatrix[index, n];
                            copyMatrix[index, n] = temp;
                        }
                        sign *= (-1);  // change value pre-value
                    }
                }
                // set 0 to the current column in the rows after current row
                for (int s = level - 1; s > i; s--)
                {
                    float temp = copyMatrix[s, j];
                    for (int t = j; t < level; t++)
                        copyMatrix[s, t] -= copyMatrix[i, t] * (temp / copyMatrix[i, j]);
                }
            }
            float sn = 1;
            for (int i = 0; i < level; i++)
            {
                if (copyMatrix[i, i] != 0)
                    sn *= copyMatrix[i, i];
                else
                    return 0;
            }
            return sign * sn;
        }
    }
}
