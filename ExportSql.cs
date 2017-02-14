using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpecCreator
{
    public class ExportSql
    {
        public static string TableName, TableCName, AuthorName;
        public static DataTable WordTable = new DataTable();

        public static int AlignName;
        public static int AlignNote;

        public static string Template;
        private static string columns;
        private static string keys;
        private static string clmDetail;
        private static string option;

        /// <summary>
        /// 回傳 ColumnType, EditorType 跟 OptionNo
        /// </summary>
        private static Tuple<string, string, int> ParseData(DataRow dr)
        {
            string colType = dr["colType"].ToString();
            string editorType;
            int optionNo = 0;
            switch (colType)
            {
                case "NUMERIC":
                    colType += string.Format("({0})", dr["colLength"]);
                    editorType = "CalcEdit";
                    break;

                case "INT":
                    editorType = "CalcEdit";
                    break;

                case "VARCHAR":
                case "CHAR":
                case "NVARCHAR":
                    colType += string.Format("({0})", dr["colLength"]);
                    editorType = "TextEdit";
                    break;

                case "SMALLINT":

                    option += GetOptionSql(dr, ref optionNo);
                    editorType = "LookUpEdit";
                    break;

                case "DATE":
                case "SMALLDATETIME":
                case "DATETIME":
                    editorType = "DateEdit";
                    break;

                case "BIT":
                    editorType = "CheckEdit";
                    break;

                default:
                    editorType = "TextEdit";
                    break;
            }
            return new Tuple<string, string, int>(colType, editorType, optionNo);
        }

        private static void InitParameter()
        {
            columns = string.Empty;
            keys = string.Empty;
            clmDetail = string.Empty;
            option = string.Empty;

            foreach (DataRow dr in WordTable.Rows)
            {
                var data = ParseData(dr);

                columns += string.Format(
                    string.Format("\r\n\t{{0,-{0}}}\t{{1}} NOT NULL,", AlignName),
                    string.Format("[{0}]", dr["colName"]), data.Item1);

                bool key = bool.Parse(dr["colKey"].ToString());
                if (key) keys += string.Format("[{0}],", dr["colName"]);

                bool columVisible;
                switch (dr["colName"].ToString().ToLower())
                {
                    case "lock":
                    case "loguser":
                    case "logtime":
                    case "lockuser":
                    case "locktime":
                        columVisible = false;
                        break;

                    default:
                        columVisible = true;
                        break;
                }

                clmDetail += string.Format(
                    string.Format(
                      "INSERT #appTableField SELECT @tbname, {{0,-{0}}}, {{1,2}}, {{2,-{1}}}, {{2,-{1}}}, {{3}}, {{3}}, 0, '', 0, {{4,3}}, 0, '', {{5}}, {{1,2}}, 150, '{{6}}', @loguser, @dt;\r\n"
                      , AlignName, AlignNote + dr["colNote"].Length() - dr["colNote"].Width())
                    , string.Format("'{0}'", dr["colName"])
                    , dr["colNo"]
                    , string.Format("'{0}'", dr["colNote"])
                    , Convert.ToInt32(key)
                    , data.Item3
                    , Convert.ToInt32(columVisible)
                    , data.Item2);
            }
            columns = columns.RemoveLast();
            keys = keys.RemoveLast();
        }

        public static string GetExportSql()
        {
            InitParameter();

            return string.Format(Template, AuthorName, DateTime.Today.ToString("yyyy-MM-dd"), TableName, columns, keys, TableCName, clmDetail, option);
        }

        private static string GetOptionSql(DataRow dr, ref int opt)
        {
            if (!int.TryParse(dr["colLength"].ToString().RegexFilter(@"[^\d]+"), out opt))
            {
                opt = -1;
                return string.Empty;
            }

            int defaultOption = 99999;
            string[] o = dr["colRemark"].ToString().Split(' ');
            string tmp = "";

            foreach (string s in o)
            {
                int idx = s.IndexOf('.');
                if (idx > 0)
                {
                    int i;
                    int.TryParse(s.Substring(0, idx).RegexFilter(@"[^\d]+"), out i);
                    defaultOption = Math.Min(i, defaultOption);
                    tmp += "\r\n"
                        + string.Format("INSERT #appTableFieldoi SELECT {0}, {1}, '{2}', @loguser, @dt;"
                        , opt, i, s);
                }
            }
            if (defaultOption == 99999) return string.Empty;
            else
            {
                return "\r\n"
                       + string.Format("INSERT #appTableFieldo SELECT {0}, '{0}.{1}', {2};"
                       , opt, dr["colNote"], defaultOption) + tmp + "\r\n";
            }
        }
    }
}