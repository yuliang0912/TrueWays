using System;

namespace TrueWays.Core.Common.Extensions
{
    public static class StringExtensions
    {
        public static string CutString(this string str, int length, string additional = "")
        {
            if (string.IsNullOrEmpty(str))
            {
                return str;
            }
            if (length < 0 || str.Length <= length)
            {
                return str;
            }
            if (!string.IsNullOrEmpty(additional))
            {
                return str.Substring(0, length) + additional;
            }
            return str.Substring(0, length);
        }

        public static string GetRandomString()
        {
            return Guid.NewGuid().ToString("N") + Guid.NewGuid().ToString("N");
        }

        public static string FillZeroNumber(int number, int totalLength)
        {
            var numLength = number.ToString().Length;
            if (numLength >= totalLength)
            {
                return number.ToString();
            }
            var tempString = string.Empty;
            while (totalLength-- > numLength)
            {
                tempString += "0";
            }
            return string.Concat(tempString, number);
        }

        public static string FormatSqlLikeString(this string str, int type = 0)
        {
            return string.IsNullOrWhiteSpace(str)
                ? str
                : $"{(type == 0 || type == 1 ? "%" : string.Empty)}{str.Trim()}{(type == 0 || type == 2 ? "%" : string.Empty)}";
        }


        public static string ConvertInterestRate(this decimal d1, decimal d2)
        {
            var difference = d1 - d2;

            return $"{difference / d2:0.00%}";
        }

        public static long GetLongNo()
        {
            System.Threading.Thread.Sleep(1); //保证yyyyMMddHHmmssffff唯一  
            var d = new Random(BitConverter.ToInt32(Guid.NewGuid().ToByteArray(), 0));
            var strUnique = DateTime.Now.ToString("yyyyMMddHHmmss") + d.Next(100, 999);
            return long.Parse(strUnique);
        }
    }
}
