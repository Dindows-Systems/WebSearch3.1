using System;
using System.Collections.Generic;
using System.Text;
using WebSearch.Common.Net;

namespace WebSearch.Maths.Net
{
    public class Angle
    {
        public static readonly int RightAngle = 0;
        
        public static readonly int ObtuseAngle = 1;

        public static readonly int AcuteAngle = 2;

        /// <summary>
        /// 判断给定的角度是一个直角，锐角还是钝角
        /// </summary>
        /// <param name="radian"></param>
        /// <returns></returns>
        public static int Property(float radian)
        {
            if (radian == Math.PI / 2)
                return Angle.RightAngle;
            if (radian > Math.PI / 2 && radian < Math.PI)
                return Angle.ObtuseAngle;
            if (radian < Math.PI / 2 && radian > 0)
                return Angle.AcuteAngle;
            return Const.Invalid;
        }

        /// <summary>
        /// 将给定的弧度转换为角度(0~360)
        /// </summary>
        /// <param name="radian"></param>
        /// <returns></returns>
        public static float ToDegree(float radian)
        {
            if (!Angle.IsStandard(radian))
                Angle.ToStandard(radian);
            // convert radian to degree:
            return 1;
        }

        /// <summary>
        /// 将给定的角度转换为弧度(0~2Pi)
        /// </summary>
        /// <param name="degree"></param>
        /// <returns></returns>
        public static float ToRadian(float degree)
        {
            if (!Angle.IsStandardDegree(degree))
                Angle.ToStandardDegree(degree);
            // convert degree to radian:
            return 1;
        }

        /// <summary>
        /// 将给定的角度值转成标准的0~360的角度
        /// </summary>
        /// <param name="degree"></param>
        /// <returns></returns>
        public static float ToStandardDegree(float degree)
        {
            int n = (int)(degree / 360);
            return degree - (float)(n * 360);
        }

        /// <summary>
        /// 判断给定的角度是否是标准的角度(0~360)
        /// </summary>
        /// <param name="degree"></param>
        /// <returns></returns>
        public static bool IsStandardDegree(float degree)
        {
            return (degree >= 0 && degree < 360);
        }

        /// <summary>
        /// 将给定的弧度转换到给定的弧度区间范围内(+KPi)
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="radian"></param>
        /// <returns></returns>
        public static float Range(float from, float to, float radian)
        {
            return 1;
        }

        /// <summary>
        /// 将给定的弧度角转成标准的0~2Pi的弧度
        /// </summary>
        /// <param name="radian"></param>
        /// <returns></returns>
        public static float ToStandard(float radian)
        {
            int n = (int)(radian / (2 * Math.PI));
            return radian - (float)(n * 2 * Math.PI);
        }

        /// <summary>
        /// 判断给定的弧度是否是标准的弧度(0~2Pi)
        /// </summary>
        /// <param name="radian"></param>
        /// <returns></returns>
        public static bool IsStandard(float radian)
        {
            return (radian >= 0 && radian < 2 * Math.PI);
        }

        /// <summary>
        /// 将给定的弧度角转成标准的0~2Pi的弧度
        /// </summary>
        /// <param name="radian"></param>
        /// <returns></returns>
        public static double ToStandard(double radian)
        {
            int n = (int)(radian / (2 * Math.PI)); 
            return radian - n * 2 * Math.PI;
        }

        /// <summary>
        /// 判断给定的弧度是否是标准的弧度(0~2Pi)
        /// </summary>
        /// <param name="radian"></param>
        /// <returns></returns>
        public static bool IsStandard(double radian)
        {
            return (radian >= 0 && radian < 2 * Math.PI);
        }

        /// <summary>
        /// 比较两弧度大小
        /// </summary>
        /// <param name="radian1"></param>
        /// <param name="radian2"></param>
        /// <returns>
        /// result = 0: radian1 == radian2
        /// result = 1: radian1 > radian2
        /// result = -1: radian1 &lt radian2
        /// </returns>
        public static int Compare(float radian1, float radian2)
        {
            float temp1 = Angle.ToStandard(radian1);
            float temp2 = Angle.ToStandard(radian2);
            if (temp1 == temp2) return 0;
            return (temp1 > temp2) ? 1 : -1;
        }

        /// <summary>
        /// 两弧度相加
        /// </summary>
        /// <param name="radian1"></param>
        /// <param name="radian2"></param>
        /// <returns>(0~2Pi)</returns>
        public static float Add(float radian1, float radian2)
        {
            return Angle.ToStandard(radian1 + radian2);
        }

        /// <summary>
        /// 两弧度相减
        /// </summary>
        /// <param name="radian1"></param>
        /// <param name="radian2"></param>
        /// <returns>(0~2Pi)</returns>
        public static float Minus(float radian1, float radian2)
        {
            return Angle.ToStandard(radian1 - radian2);
        }
    }
}
