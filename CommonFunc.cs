using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SpecCreator
{
    public static class CommonFunc
    {
        public static string reg(object o, string filter)
        {
            return o.ToString().RegexFilter(filter);
        }

        public static void InitDataTable(ref DataTable dt)
        {
            dt.Reset();
            string[] clmName = new string[7] { "colKey", "colNo", "colName", "colNote", "colType", "colLength", "colRemark" };

            dt.Columns.Add(clmName[0], typeof(bool));
            for (int i = 1; i < 7; i++)
            {
                dt.Columns.Add(clmName[i], typeof(string));
            }
        }
    }

    public static class StringExtension
    {
        public static int GetWidth(this string s)
        {
            return Encoding.GetEncoding("big5").GetByteCount(s);
        }

        public static string[] SplitRemoveEmpty(this string s, char sep)
        {
            string[] r = s.Split(new char[] { sep }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string st in r) st.Trim();
            return r;
        }

        public static string RegexFilter(this string s, string filter)
        {
            return Regex.Replace(s, filter, "");
        }

        public static string Replace(this string s, char[] r, char n)
        {
            foreach (char c in r) s = s.Replace(c, n);
            return s;
        }

        public static string Replace(this string s, string[] r, string[] n)
        {
            for (int i = 0; i < r.Length; i++) s = s.Replace(r[i], n[i]);
            return s;
        }

        public static string Substring(this string s, string subS, string subE)
        {
            int start = s.IndexOf(subS);
            int end = s.IndexOf(subE);
            string r = s.Substring(start + subS.Length, end - start - subS.Length);
            return r.RegexFilter("[\x00-\x1F\x7F]");
        }

        public static string RemoveLast(this string s)
        {
            return s.Remove(s.Length - 1);
        }

        /// <summary>
        /// 移除特殊控制字元
        /// http://stackoverflow.com/questions/1497885/remove-control-characters-from-php-string
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string RemoveCtrlChar(this string s)
        {
            return s.RegexFilter(@"[\x00-\x1F\x7F]");
        }
    }

    public static class ObjectExtension
    {
        public static int Length(this object o)
        {
            return o.ToString().Length;
        }

        public static int Width(this object o)
        {
            return o.ToString().GetWidth();
        }
    }
}