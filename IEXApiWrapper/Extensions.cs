using System;
using System.Collections.Generic;
using System.Text;

namespace IEXApiHandler
{
    using IEXApiHandler.IEXData;
    using IEXApiHandler.IEXData.Stock;

    public static class Extensions
    {
        public static int LimitToMax(this int value, int min = 1, int max = 10)
        {
            return Math.Min(Math.Max(value, min), max);
        }

        public static string ToStringDelimeter(this string[] value, string delimeter = ",")
        {
            return string.Join(delimeter, value);
        }

        public static string ToStringDelimeter(this IEnumerable<IEXDataType> value, string delimeter = ",")
        {
            return string.Join(delimeter, value);
        }
    }
}
