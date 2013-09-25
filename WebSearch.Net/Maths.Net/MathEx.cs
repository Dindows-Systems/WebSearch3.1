using System;
using System.Collections.Generic;
using System.Text;

namespace WebSearch.Maths.Net
{
    public static class MathEx
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj1"></param>
        /// <param name="obj2"></param>
        public static void Swap(ref Object obj1, ref Object obj2)
        {
            Object temp = obj1;
            obj1 = obj2;
            obj2 = temp;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="codes1"></param>
        /// <param name="codes2"></param>
        public static void Swap(ref bool[] codes1, ref bool[] codes2)
        {
            bool[] temp = codes1;
            codes1 = codes2;
            codes2 = temp;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public static void Swap(ref int x, ref int y)
        {
            int temp = x;
            x = y;
            y = temp;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="codes"></param>
        /// <returns></returns>
        public static bool LogicSum(bool[] codes)
        {
            bool result = false;
            foreach (bool instance in codes)
                result = result || instance;
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="codes1"></param>
        /// <param name="codes2"></param>
        /// <returns></returns>
        public static bool[] LogicSum(bool[] codes1, bool[] codes2)
        {
            if (codes1.Length != codes2.Length)
                return null;
            bool[] result = new bool[codes1.Length];
            for (int i = 0; i < codes1.Length; i++)
                result[i] = codes1[i] || codes2[i];
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="codes"></param>
        /// <returns></returns>
        public static bool LogicTimes(bool[] codes)
        {
            bool result = true;
            foreach (bool instance in codes)
                result = result && instance;
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="codes1"></param>
        /// <param name="codes2"></param>
        /// <returns></returns>
        public static bool[] LogicTimes(bool[] codes1, bool[] codes2)
        {
            if (codes1.Length != codes2.Length)
                return null;
            bool[] result = new bool[codes1.Length];
            for (int i = 0; i < codes1.Length; i++)
                result[i] = codes1[i] && codes2[i];
            return result;
        }

        /// <summary>
        /// Calculate the power of 2, exp: [0, 30]
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        public static int Pow2(int exp)
        {
            return ((exp >= 0) && (exp <= 30)) ? (1 << exp) : 0;
        }

        /// <summary>
        /// Check if the integer is a power of 2
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static bool IsPowerOf2(int x)
        {
            return (x & (x - 1)) == 0;
        }

        /// <summary>
        /// Get base  of 2 logarithm (ceiling)
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static int Log2(int x)
        {
            if (x <= 65536)
            {
                if (x <= 256)
                {
                    if (x <= 16)
                    {
                        if (x <= 4)
                        {
                            if (x <= 2)
                            {
                                if (x <= 1)
                                    return 0;
                                return 1;
                            }
                            return 2;
                        }
                        if (x <= 8)
                            return 3;
                        return 4;
                    }
                    if (x <= 64)
                    {
                        if (x <= 32)
                            return 5;
                        return 6;
                    }
                    if (x <= 128)
                        return 7;
                    return 8;
                }
                if (x <= 4096)
                {
                    if (x <= 1024)
                    {
                        if (x <= 512)
                            return 9;
                        return 10;
                    }
                    if (x <= 2048)
                        return 11;
                    return 12;
                }
                if (x <= 16384)
                {
                    if (x <= 8192)
                        return 13;
                    return 14;
                }
                if (x <= 32768)
                    return 15;
                return 16;
            }

            if (x <= 16777216)
            {
                if (x <= 1048576)
                {
                    if (x <= 262144)
                    {
                        if (x <= 131072)
                            return 17;
                        return 18;
                    }
                    if (x <= 524288)
                        return 19;
                    return 20;
                }
                if (x <= 4194304)
                {
                    if (x <= 2097152)
                        return 21;
                    return 22;
                }
                if (x <= 8388608)
                    return 23;
                return 24;
            }
            if (x <= 268435456)
            {
                if (x <= 67108864)
                {
                    if (x <= 33554432)
                        return 25;
                    return 26;
                }
                if (x <= 134217728)
                    return 27;
                return 28;
            }
            if (x <= 1073741824)
            {
                if (x <= 536870912)
                    return 29;
                return 30;
            }
            return 31;
        }
    }
}
