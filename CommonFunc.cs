using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SpecCreator
{
    public static class CommonFunc
    {
        public static int GetWidth(this string s)
        {
            return Encoding.GetEncoding("big5").GetByteCount(s);
        }

        public static DateTime ParseDate(this string date)
        {
            try
            {
                return DateTime.Parse(date);
            }
            catch (FormatException)
            {
                if (string.IsNullOrEmpty(date))
                    return default(DateTime);
                return DateTime.ParseExact(date, "yyyyMMdd", CultureInfo.InvariantCulture);
            }
        }

        public static string Remove(this string input, string pattern, RegexOptions options = RegexOptions.None)
        {
            return Regex.Replace(input, pattern, "", options);
        }

        /// <summary>
        /// 移除特殊控制字元
        /// http://stackoverflow.com/questions/1497885/remove-control-characters-from-php-string
        /// </summary>
        public static string RemoveCtrlChar(this string s)
        {
            return s.Remove(@"[\x00-\x1F\x7F]");
        }
    }
}