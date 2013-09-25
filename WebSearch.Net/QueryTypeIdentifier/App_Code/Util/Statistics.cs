using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

namespace SJTU.CS.Apex.WebSearch.Util
{
    public enum StatMethod
    {
        Mean,       // mean method
        Median,     // median method
        Skewness,   // the 3rd central moment normalized by the standard deviation to the order of 3
        Kurtosis    // the 4th central moment normalized by the standard deviation to the order of 4
    }

    /// <summary>
    /// Summary description for Statistics
    /// </summary>
    public static class Statistics
    {
        /// <summary>
        /// The term 'average' is something we have been familiar with from a very early age when
        /// we start analyzing our marks on report cards. We add together all of our test results
        /// and then divide it by the sum of the total number of marks there are. We often call
        /// it the average. However, statistically it's the Mean!
        /// </summary>
        /// <example>
        /// Four tests results: 15, 18, 22, 20
        /// The sum is: 75
        /// Divide 75 by 4: 18.75
        /// The 'Mean' (Average) is 18.75
        /// (Often rounded to 19)
        /// </example>
        /// <param name="values"></param>
        /// <returns></returns>
        public static float Mean(float[] values)
        {
            return (Sum(values) / (float)values.Length);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public static float Mean(int[] values)
        {
            return ((float)Sum(values) / (float)values.Length);
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
        /// Calculate standard deviation
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

        /// <summary>
        /// The Median is the 'middle value' in your list. When the totals of the
        /// list are odd, the median is the middle entry in the list after sorting
        /// the list into increasing order. When the totals of the list are even,
        /// the median is equal to the sum of the two middle (after sorting the
        /// list into increasing order) numbers divided by two. Thus, remember to
        /// line up your values, the middle number is the median! Be sure to remember
        /// the odd and even rule.
        /// </summary>
        /// <example>
        /// Find the Median of: 9, 3, 44, 17, 15 (Odd amount of numbers)
        /// Line up your numbers: 3, 9, 15, 17, 44 (smallest to largest)
        /// The Median is: 15 (The number in the middle)
        /// Find the Median of: 8, 3, 44, 17, 12, 6 (Even amount of numbers)
        /// Line up your numbers: 3, 6, 8, 12, 17, 44
        /// Add the 2 middles numbers and divide by 2: 8 12 = 20/2 = 10
        /// The Median is 10.
        /// </example>
        /// <param name="values"></param>
        /// <returns></returns>
        public static float Median(float[] values)
        {
            Array.Sort<float>(values);
            int len = values.Length, mid = len / 2;
            if (len % 2 == 0)
                return ((values[mid] + values[mid + 1]) / 2);
            else
                return values[mid];
        }

        /// <summary>
        /// Get range around median containing specified percentile of values
        /// </summary>
        /// <param name="values"></param>
        /// <param name="percent"></param>
        /// <returns></returns>
        public static Range GetRange(float[] values, double percent)
        {
            return null;
            //float total = 0, n = values.Length;

            //// for all values
            //for (int i = 0; i < n; i++)
            //    // accumalate total
            //    total += values[i];

            //int min, max, v;
            //int h = (int)(total * (percent + (1 - percent) / 2));

            //// get range min value
            //for (min = 0, v = total; min < n; min++)
            //{
            //    v -= values[min];
            //    if (v < h)
            //        break;
            //}
            //// get range max value
            //for (max = n - 1, v = total; max >= 0; max--)
            //{
            //    v -= values[max];
            //    if (v < h)
            //        break;
            //}
            //return new Range(min, max);
        }

        /// <summary>
        /// Calculate an entropy
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public static float Entropy(float[] values)
        {
            float total = 0;

            for (int i = 0, n = values.Length; i < n; i++)
                total += values[i];

            return Entropy(values, total);
        }

        /// <summary>
        /// Calculate an entropy
        /// </summary>
        /// <param name="values"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        public static float Entropy(float[] values, float total)
        {
            int n = values.Length;
            float entropy = 0, p;

            // for all values
            for (int i = 0; i < n; i++)
            {
                // get item probability
                p = (float)values[i] / total;
                // calculate entropy
                if (p != 0)
                    entropy += (float)(-p * Math.Log(p, 2));
            }
            return entropy;
        }
    }
}