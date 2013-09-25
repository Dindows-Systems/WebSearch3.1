using System;
using System.Collections.Generic;
using System.Text;

namespace WebSearch.Maths.Net
{
    /// <summary>
    /// <p>The direction of the fourier transform.</p>
    /// </summary>
    public enum FourierDirection : int
    {
        /// <summary>
        /// Forward direction.  Usually in reference to moving from temporal
        /// representation to frequency representation
        /// </summary>
        Forward = 1,
        /// <summary>
        /// Backward direction. Usually in reference to moving from frequency
        /// representation to temporal representation
        /// </summary>
        Backward = -1,
    }

    /// <summary>
    /// Fourier Transformation
    /// </summary>
    public class FourierTransform
    {
        /// <summary>
        /// One dimensional Discrete Fourier Transform
        /// </summary>
        /// <param name="data"></param>
        /// <param name="direction"></param>
        public static void DFT(Complex[] data, FourierDirection direction)
        {
            int n = data.Length;
            double arg, cos, sin;
            Complex[] dst = new Complex[n];

            // for each destination element
            for (int i = 0; i < n; i++)
            {
                dst[i] = Complex.Zero;
                arg = -(int)direction * 2.0 * System.Math.PI * (double)i / (double)n;

                // sum source elements
                for (int j = 0; j < n; j++)
                {
                    cos = System.Math.Cos(j * arg);
                    sin = System.Math.Sin(j * arg);

                    dst[i].Real += (float)(data[j].Real * cos - data[j].Image * sin);
                    dst[i].Image += (float)(data[j].Real * sin + data[j].Image * cos);
                }
            }

            if (direction == FourierDirection.Forward) // copy elements
            {
                // devide also for forward transform
                for (int i = 0; i < n; i++)
                {
                    data[i].Real = dst[i].Real / n;
                    data[i].Image = dst[i].Image / n;
                }
            }
            else
            {
                for (int i = 0; i < n; i++)
                {
                    data[i].Real = dst[i].Real;
                    data[i].Image = dst[i].Image;
                }
            }
        }

        /// <summary>
        /// Two dimensional Discrete Fourier Transform
        /// </summary>
        /// <param name="data"></param>
        /// <param name="direction"></param>
        public static void DFT2(Complex[,] data, FourierDirection direction)
        {
            int n = data.GetLength(0);	// rows
            int m = data.GetLength(1);	// columns
            double arg, cos, sin;
            Complex[] dst = new Complex[System.Math.Max(n, m)];

            for (int i = 0; i < n; i++) // process rows
            {
                for (int j = 0; j < m; j++)
                {
                    dst[j] = Complex.Zero;

                    arg = -(int)direction * 2.0 * System.Math.PI * (double)j / (double)m;
                    // sum source elements
                    for (int k = 0; k < m; k++)
                    {
                        cos = System.Math.Cos(k * arg);
                        sin = System.Math.Sin(k * arg);

                        dst[j].Real += (float)(data[i, k].Real * cos - data[i, k].Image * sin);
                        dst[j].Image += (float)(data[i, k].Real * sin + data[i, k].Image * cos);
                    }
                }

                if (direction == FourierDirection.Forward) // copy elements
                {
                    // devide also for forward transform
                    for (int j = 0; j < m; j++)
                    {
                        data[i, j].Real = dst[j].Real / m;
                        data[i, j].Image = dst[j].Image / m;
                    }
                }
                else
                {
                    for (int j = 0; j < m; j++)
                    {
                        data[i, j].Real = dst[j].Real;
                        data[i, j].Image = dst[j].Image;
                    }
                }
            }

            for (int j = 0; j < m; j++) // process columns
            {
                for (int i = 0; i < n; i++)
                {
                    dst[i] = Complex.Zero;

                    arg = -(int)direction * 2.0 * System.Math.PI * (double)i / (double)n;

                    // sum source elements
                    for (int k = 0; k < n; k++)
                    {
                        cos = System.Math.Cos(k * arg);
                        sin = System.Math.Sin(k * arg);

                        dst[i].Real += (float)(data[k, j].Real * cos - data[k, j].Image * sin);
                        dst[i].Image += (float)(data[k, j].Real * sin + data[k, j].Image * cos);
                    }
                }
                // copy elements
                if (direction == FourierDirection.Forward)
                {
                    // devide also for forward transform
                    for (int i = 0; i < n; i++)
                    {
                        data[i, j].Real = dst[i].Real / n;
                        data[i, j].Image = dst[i].Image / n;
                    }
                }
                else
                {
                    for (int i = 0; i < n; i++)
                    {
                        data[i, j].Real = dst[i].Real;
                        data[i, j].Image = dst[i].Image;
                    }
                }
            }
        }

        /// <summary>
        /// One dimensional Fast Fourier Transform
        /// </summary>
        /// <param name="data"></param>
        /// <param name="direction"></param>
        public static void FFT(Complex[] data, FourierDirection direction)
        {
            int n = data.Length;
            int m = MathEx.Log2(n);

            // reorder data first
            ReorderData(data);

            // compute FFT
            int tn = 1, tm;
            for (int k = 1; k <= m; k++)
            {
                Complex[] rotation = FourierTransform.GetComplexRotation(k, direction);

                tm = tn;
                tn <<= 1;

                for (int i = 0; i < tm; i++)
                {
                    Complex t = rotation[i];

                    for (int even = i; even < n; even += tn)
                    {
                        int odd = even + tm;
                        Complex ce = data[even];
                        Complex co = data[odd];

                        float tr = co.Real * t.Real - co.Image * t.Image;
                        float ti = co.Real * t.Image + co.Image * t.Real;

                        data[even].Real += tr;
                        data[even].Image += ti;

                        data[odd].Real = ce.Real - tr;
                        data[odd].Image = ce.Image - ti;
                    }
                }
            }

            if (direction == FourierDirection.Forward)
            {
                for (int i = 0; i < n; i++)
                {
                    data[i].Real /= (float)n;
                    data[i].Image /= (float)n;
                }
            }
        }

        /// <summary>
        /// Two dimensional Fast Fourier Transform
        /// </summary>
        /// <param name="data"></param>
        /// <param name="direction"></param>
        public static void FFT2(Complex[,] data, FourierDirection direction)
        {
            int k = data.GetLength(0);
            int n = data.GetLength(1);

            // check data size
            if ((!MathEx.IsPowerOf2(k)) ||
                (!MathEx.IsPowerOf2(n)) ||
                (k < minLength) || (k > maxLength) ||
                (n < minLength) || (n > maxLength))
                throw new ArgumentException();

            // process rows
            Complex[] row = new Complex[n];

            for (int i = 0; i < k; i++)
            {
                // copy row
                for (int j = 0; j < n; j++)
                    row[j] = data[i, j];
                // transform it
                FourierTransform.FFT(row, direction);
                // copy back
                for (int j = 0; j < n; j++)
                    data[i, j] = row[j];
            }

            Complex[] col = new Complex[k]; // process columns

            for (int j = 0; j < n; j++)
            {
                // copy column
                for (int i = 0; i < k; i++)
                    col[i] = data[i, j];
                // transform it
                FourierTransform.FFT(col, direction);
                // copy back
                for (int i = 0; i < k; i++)
                    data[i, j] = col[i];
            }
        }

        #region Private Region

        private const int minLength = 2;
        private const int maxLength = 16384;
        private const int minBits = 1;
        private const int maxBits = 14;
        private static int[][] reversedBits = new int[maxBits][];
        private static Complex[,][] complexRotation = new Complex[maxBits, 2][];

        /// <summary>
        /// Get array, indicating which data members should be swapped before FFT
        /// </summary>
        /// <param name="numberOfBits"></param>
        /// <returns></returns>
        private static int[] GetReversedBits(int numberOfBits)
        {
            if ((numberOfBits < minBits) || (numberOfBits > maxBits))
                throw new ArgumentOutOfRangeException();

            // check if the array is already calculated
            if (reversedBits[numberOfBits - 1] == null)
            {
                int n = MathEx.Pow2(numberOfBits);
                int[] rBits = new int[n];

                // calculate the array
                for (int i = 0; i < n; i++)
                {
                    int oldBits = i;
                    int newBits = 0;

                    for (int j = 0; j < numberOfBits; j++)
                    {
                        newBits = (newBits << 1) | (oldBits & 1);
                        oldBits = (oldBits >> 1);
                    }
                    rBits[i] = newBits;
                }
                reversedBits[numberOfBits - 1] = rBits;
            }
            return reversedBits[numberOfBits - 1];
        }

        /// <summary>
        /// Get rotation of complex number
        /// </summary>
        /// <param name="numberOfBits"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        private static Complex[] GetComplexRotation(int numberOfBits, FourierDirection direction)
        {
            int directionIndex = (direction == FourierDirection.Forward) ? 0 : 1;

            // check if the array is already calculated
            if (complexRotation[numberOfBits - 1, directionIndex] == null)
            {
                int n = 1 << (numberOfBits - 1);
                float uR = 1.0f;
                float uI = 0.0f;
                double angle = System.Math.PI / n * (int)direction;
                float wR = (float)System.Math.Cos(angle);
                float wI = (float)System.Math.Sin(angle);
                float t;
                Complex[] rotation = new Complex[n];

                for (int i = 0; i < n; i++)
                {
                    rotation[i] = new Complex(uR, uI);
                    t = uR * wI + uI * wR;
                    uR = uR * wR - uI * wI;
                    uI = t;
                }

                complexRotation[numberOfBits - 1, directionIndex] = rotation;
            }
            return complexRotation[numberOfBits - 1, directionIndex];
        }

        /// <summary>
        /// Reorder data for FFT using
        /// </summary>
        /// <param name="data"></param>
        private static void ReorderData(Complex[] data)
        {
            int len = data.Length;

            // check data length
            if ((len < minLength) || (len > maxLength) || (!MathEx.IsPowerOf2(len)))
                throw new ArgumentException();

            int[] rBits = GetReversedBits(MathEx.Log2(len));

            for (int i = 0; i < len; i++)
            {
                int s = rBits[i];

                if (s > i)
                {
                    Complex t = data[i];
                    data[i] = data[s];
                    data[s] = t;
                }
            }
        }
        #endregion
    }
}
