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

        /// <summary>
        /// 偵測bytes[]是否為Big5編碼
        /// </summary>
        public static bool IsBig5Encoding(this byte[] bytes)
        {
            Encoding big5 = Encoding.GetEncoding(950);

            // 將byte[]轉為string再轉回byte[]看位元數是否有變
            return bytes.Length == big5.GetByteCount(big5.GetString(bytes));
        }

        /// <summary>
        /// 偵測bytes[]是否為UTF-8-BOM編碼
        /// </summary>
        public static bool IsUtf8EncodingWithBom(this byte[] bytes)
        {
            return bytes.Length >= 3 && bytes[0] == 0xEF && bytes[1] == 0xBB && bytes[2] == 0xBF;
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

        /// <summary>
        /// 從這個執行個體擷取子陣列。 子陣列會在指定的位置開始並繼續到結尾。
        /// </summary>
        public static T[] SubArray<T>(this T[] data, int startIndex)
        {
            return SubArray(data, startIndex, data.Length - startIndex);
        }

        /// <summary>
        /// 從這個執行個體擷取子陣列。 子陣列起始於指定的位置，並且具有指定的長度。
        /// </summary>
        public static T[] SubArray<T>(this T[] data, int startIndex, int length)
        {
            T[] result = new T[length];
            Array.Copy(data, startIndex, result, 0, length);
            return result;
        }
    }
}