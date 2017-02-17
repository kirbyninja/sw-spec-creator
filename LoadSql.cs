using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SpecCreator
{
    public class LoadSql
    {
        public static DataTable dt = new DataTable();
        public static string sql;

        public static bool LoadSpec(string sqlName)
        {
            try
            {
                using (StreamReader sr = new StreamReader(sqlName, Encoding.Default))
                {
                    if ((sql = sr.ReadToEnd()) != null)
                    {
                        InitMeta();
                        InitDataTable();
                    }
                }
                return true;
            }
            catch (Exception e)
            {
                MessageBox.Show("檔案無法讀取！\r\n" + e.Message);
                return false;
            }
        }

        /// <summary>
        /// 抓 Word 應該放的欄位名稱
        /// </summary>
        private static void InitMeta()
        {
            ExportWord.TableName = sql.Substring(@"SELECT @tbname = '", @"', @loguser = USER, @dt = ROUND(CONVERT(FLOAT, GETDATE()), 0, 1);");
            ExportWord.TableCName = sql.Substring("INSERT app_table SELECT @tbname, '", "\r\nSELECT * INTO #appTableField FROM app_table_field WHERE 1 = 0;").Replace("';", string.Empty).Trim();
        }

        private static void InitDataTable()
        {
            CommonFunc.InitDataTable(ref dt);

            string colDetail = sql.Substring("SELECT * INTO #appTableField FROM app_table_field WHERE 1 = 0;", "IF @owfield=1");
            string[] col = colDetail.SplitRemoveEmpty(';');
            foreach (string c in col)
            {
                DataRow dr = dt.NewRow();
                string[] detail = c.SplitRemoveEmpty(',');
                for (int i = 0; i < detail.Length; i++) detail[i] = detail[i].Remove(@"[\']").Trim();
                dr["colKey"] = false;
                dr["colNo"] = detail[2];
                dr["colName"] = detail[1];
                dr["colNote"] = detail[3];
                dr["colType"] = string.Empty;
                string o = detail[10].ToString();
                if (Convert.ToInt32(o) == 0) dr["colLength"] = dr["colRemark"] = string.Empty;
                else
                {
                    dr["colLength"] = o;
                    dr["colRemark"] = "Option " + o + "\r\n";
                }
                dt.Rows.Add(dr);
            }
            SetKey();
            SetType();
            SetOption();
        }

        private static void SetKey()
        {
            string colKey = sql.Substring("PRIMARY KEY CLUSTERED", "ON [PRIMARY]");
            colKey = colKey.Replace(new char[] { '(', ')', '[', ']' }, ' ').Trim();
            string[] keys = colKey.SplitRemoveEmpty(',');
            foreach (DataRow dr in dt.Rows)
            {
                dr["colKey"] = keys.Contains(dr["colName"].ToString());
            }
        }

        private static void SetType()
        {
            string colType = sql.Substring("] (", ");").Replace(" NOT NULL,", "%").Replace(" NOT NULL", "");
            colType = colType.Replace(new string[] { "[", "]", " " }, new string[] { string.Empty, ";", string.Empty });

            Dictionary<string, string> col = colType.SplitRemoveEmpty('%').ToDictionary(c => c.Split(';')[0], c => c.Split(';')[1]);
            foreach (DataRow dr in dt.Rows)
            {
                string[] v = col[dr["colName"].ToString()].Split('(');
                dr["colType"] = v[0].ToUpper();
                if (v.Length == 2) dr["colLength"] = v[1].RemoveLast();
            }
        }

        private static void SetOption()
        {
            string colOption = sql.Substring("SELECT * INTO #appTableFieldoi FROM app_table_field_option_item WHERE 1 = 0;", "IF @owoption=1").RemoveCtrlChar().Trim();
            string[] optSql = colOption.Split(new string[] { "INSERT #appTableFieldo SELECT" }, StringSplitOptions.RemoveEmptyEntries);

            Dictionary<string, string> option = new Dictionary<string, string>();

            for (int i = 0; i < optSql.Length; i++)
            {
                string[] n = optSql[i].SplitRemoveEmpty(';');
                string k = n[0].SplitRemoveEmpty(',')[0].Trim();
                string v = string.Empty;
                for (int j = 1; j < n.Length; j++) v += n[j].SplitRemoveEmpty(',')[2].Replace("'", "");
                option.Add(k, v.Trim());
            }

            foreach (DataRow dr in dt.Rows)
            {
                if (dr["colType"].ToString() == "SMALLINT")
                {
                    dr["colRemark"] += option[dr["colLength"].ToString()];
                    dr["colLength"] = string.Empty;
                }
            }
        }
    }
}