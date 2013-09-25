using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Globalization;

namespace WebSearch.Common.Net
{
    /// <summary>
    /// http://blog.csdn.net/hxhbluestar/archive/2005/04/21/356601.aspx
    /// </summary>
    public static class EnumHelper
    {
        /// <summary>
        /// Convert string to enum value
        /// </summary>
        /// <param name="type"></param>
        /// <param name="value"></param>
        /// <example>ExampleNormalEnum status = (ExampleNormalEnum)
        /// EnumHelper.StringToEnum( typeof( ExampleNormalEnum ),
        /// "Offline");</example>
        /// <returns></returns>
        public static object StringToEnum(Type type, string value)
        {
            return Enum.Parse(type, value, true);
            //foreach (FieldInfo fi in type.GetFields())
            //    if (fi.Name == value)
            //        // null: enum values are static
            //        return fi.GetValue(null);
            //throw new Exception(string.Format("Can't convert string " + 
            //    "{0} to enum {1}", value, type.ToString()));
        }

        /// <summary>
        /// Convert int to enum value
        /// </summary>
        /// <param name="type"></param>
        /// <param name="value"></param>
        /// <example>ExampleNormalEnum status = (ExampleNormalEnum)
        /// EnumHelper.IntToEnum(typeof( ExampleNormalEnum),1); </example>
        /// <returns></returns>
        public static object IntToEnum(Type type, int value)
        {
            return Enum.Parse(type, Enum.GetName(type, value));
        }

        /// <summary>
        /// Get all int values for the enum
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static int[] GetEnumIntValues(Type type)
        {
            int[] values = new int[Enum.GetValues(type).Length];
            Array.Copy(Enum.GetValues(type), values, Enum.GetValues(type).Length);
            return values;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string EnumToASCIIString(Type type, object value)
        {
            return HexStringToASCIIString(EnumToHexString(type, value));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hexString"></param>
        /// <returns></returns>
        public static string HexStringToASCIIString(string hexString)
        {
            int myInt16 = int.Parse(hexString, NumberStyles.AllowHexSpecifier);
            char myChar = (char)myInt16;
            return myChar.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string EnumToHexString(Type type, object value)
        {
            return Enum.Format(type, Enum.Parse(type,
                 Enum.GetName(type, value)), "X");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="asciiString"></param>
        /// <returns></returns>
        public static object ASCIIStringToEnum(Type type, string asciiString)
        {
            return HexStringToEnum(type, ASCIIStringToHexString(asciiString));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="normalString"></param>
        /// <returns></returns>
        public static string ASCIIStringToHexString(string normalString)
        {
            Encoding enc = Encoding.GetEncoding("ASCII");
            for (int i = 0; i < normalString.Length; ++i)
            {
                byte[] bs = enc.GetBytes(normalString[i].ToString());
                for (int j = 0; j < bs.Length; j++)
                    return bs[j].ToString("X2");
            }
            return "FF";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="hexString"></param>
        /// <returns></returns>
        public static object HexStringToEnum(Type type, string hexString)
        {
            return Enum.Parse(type, Enum.GetName(type, Int16.Parse(
                 hexString, NumberStyles.AllowHexSpecifier)));
        }
    }
}
