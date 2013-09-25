using System;
using System.Collections.Generic;
using System.Text;
using WebSearch.Common.Net;

namespace WebSearch.Maths.Net
{
    /// <summary>
    /// Set of statistics functions
    /// </summary>
    public static class Statistics
    {
        /// <summary>
        /// Get the min value
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public static float Min(float[] values)
        {
            float min = float.MaxValue;
            foreach (float v in values)
                min = (v < min) ? v : min;
            return min;
        }

        /// <summary>
        /// Get the max value
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public static float Max(float[] values)
        {
            float max = float.MinValue;
            foreach (float v in values)
                max = (v > max) ? v : max;
            return max;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public static float Sum(float[] values)
        {
            float sum = 0;
            for (int i = 0; i < values.Length; i++)
                sum += values[i];
            return sum;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public static int Sum(int[] values)
        {
            int sum = 0;
            for (int i = 0; i < values.Length; i++)
                sum += values[i];
            return sum;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public static float SquareSum(float[] values)
        {
            float sum = 0;
            for (int i = 0; i < values.Length; i++)
                sum += (values[i] * values[i]);
            return sum;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public static int SquareSum(int[] values)
        {
            int sum = 0;
            for (int i = 0; i < values.Length; i++)
                sum += (values[i] * values[i]);
            return sum;
        }

        /// <summary>
        /// Calculate mean value
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public static float Mean(int[] values)
        {
            float total = 0.0F;
            for (int i = 0; i < values.Length; i++)
                total += values[i];
            return (float)total / (float)values.Length;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public static float Mean(float[] values)
        {
            float total = 0;
            for (int i = 0; i < values.Length; i++)
                total += values[i];
            return total / (float)values.Length;
        }

        /// <summary>
        /// Calculate standard deviation
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public static double StdDev(int[] values)
        {
            float mean = Statistics.Mean(values);
            float stddev = 0, t;
            int v, total = 0;

            // for all values
            for (int i = 0, n = values.Length; i < n; i++)
            {
                v = values[i];
                t = (float)i - mean;

                // accumulate mean
                stddev += t * t * v;
                // accumalate total
                total += v;
            }
            return Math.Sqrt(stddev / total);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public static float StdDev(float[] values)
        {
            float mean = Statistics.Mean(values);
            float stddev = 0, t;
            float v, total = 0;

            // for all values
            for (int i = 0, n = values.Length; i < n; i++)
            {
                v = values[i];
                t = (float)i - mean;

                // accumulate mean
                stddev += t * t * v;
                // accumalate total
                total += v;
            }
            return (float)Math.Sqrt(stddev / total);
        }

        /// <summary>
        /// Calculate median value
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public static float Median(int[] values)
        {
            // find the median for the values
            Array.Sort<int>(values);
            // get the median value
            int size = values.Length;
            int mid = size / 2;
            return (size % 2 != 0) ? (float)values[mid] :
                (float)(values[mid] + values[mid - 1]) / 2.0F;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public static float Median(float[] values)
        {
            // find the median for the values
            Array.Sort<float>(values);
            // get the median value
            int size = values.Length;
            int mid = size / 2;
            return (size % 2 != 0) ? values[mid] :
                (values[mid] + values[mid - 1]) / 2.0F;
        }

        /// <summary>
        /// Get range around median containing specified percentile of values
        /// </summary>
        /// <param name="values"></param>
        /// <param name="percent"></param>
        /// <returns></returns>
        public static Range GetRange(int[] values, double percent)
        {
            int total = 0, n = values.Length;

            // for all values
            for (int i = 0; i < n; i++)
                // accumalate total
                total += values[i];

            int min, max, v;
            int h = (int)(total * (percent + (1 - percent) / 2));

            // get range min value
            for (min = 0, v = total; min < n; min++)
            {
                v -= values[min];
                if (v < h)
                    break;
            }
            // get range max value
            for (max = n - 1, v = total; max >= 0; max--)
            {
                v -= values[max];
                if (v < h)
                    break;
            }
            return new Range(min, max);
        }

        /// <summary>
        /// Calculate an entropy
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public static double Entropy(int[] values)
        {
            int total = 0;

            for (int i = 0, n = values.Length; i < n; i++)
                total += values[i];
            
            return Entropy(values, total);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="positives"></param>
        /// <param name="negatives"></param>
        /// <returns></returns>
        public static double Entropy(int positives, int negatives)
        {
            int total = positives + negatives;
            double ratioPositive = (double)positives / total;
            double ratioNegative = (double)negatives / total;

            if (ratioPositive != 0)
                ratioPositive = -(ratioPositive) * System.Math.Log(ratioPositive, 2);
            if (ratioNegative != 0)
                ratioNegative = -(ratioNegative) * System.Math.Log(ratioNegative, 2);

            return ratioPositive + ratioNegative;
        }

        /// <summary>
        /// Calculate an entropy
        /// </summary>
        /// <param name="values"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        public static double Entropy(int[] values, int total)
        {
            int n = values.Length;
            double entropy = 0, p;

            // for all values
            for (int i = 0; i < n; i++)
            {
                // get item probability
                p = (double)values[i] / total;
                // calculate entropy
                if (p != 0)
                    entropy += (-p * Math.Log(p, 2));
            }
            return entropy;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public static float Variance(float[] values)
        {
            float mean = Mean(values);
            return (SquareSum(values) / values.Length - mean * mean);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public static float Variance(int[] values)
        {
            float mean = Mean(values);
            return ((float)SquareSum(values) / (float)values.Length - mean * mean);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targets"></param>
        /// <returns></returns>
        public static float Variance(float value, float[] targets)
        {
            Array.Sort<float>(targets);
            int maxIdex = targets.Length - 1;
            float result = 0.9F * Variance(new float[] { value, targets[maxIdex] });
            targets[maxIdex] = value;
            result += 0.1F * Variance(targets);
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targets"></param>
        /// <returns></returns>
        public static float Variance(int value, int[] targets)
        {
            Array.Sort<int>(targets);
            int maxIdex = targets.Length - 1;
            float result = 0.9F * Variance(new int[] { value, targets[maxIdex] });
            targets[maxIdex] = value;
            result += 0.1F * Variance(targets);
            return result;
        }
    }
}
