using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WORD = Microsoft.Office.Interop.Word;

namespace SpecCreator
{
    public class LoadWord
    {
        public static DataTable dt = new DataTable();

        public static bool LoadSpec(string docName)
        {
            bool initOK;
            WORD._Application openWord = new WORD.Application();
            WORD._Document spec = openWord.Documents.Open(docName);
            if (spec.Tables.Count == 1)
            {
                spec.Activate();
                WORD.Table Table = spec.Tables[1];
                initOK = InitMeta(Table);
                if (initOK) InitDataTable(Table);
                else spec.Close();
            }
            else
            {
                MessageBox.Show("文件格式不符！");
                initOK = false;
            }
            openWord.Quit();
            return initOK;
        }

        private static bool InitMeta(WORD.Table t)
        {
            bool b = IsCellEmpty(t, 1, 2, "尚未填入檔案代碼", ref ExportSql.TableName);
            if (b) return false;

            b = IsCellEmpty(t, 1, 1, "尚未填入文件名稱", ref ExportSql.TableCName);
            if (b) return false;

            b = IsCellEmpty(t, 3, 3, string.Empty, ref ExportSql.AuthorName);
            if (b) ExportSql.AuthorName = Environment.UserName;

            return true;
        }

        private static void InitDataTable(WORD.Table t)
        {
            CommonFunc.InitDataTable(ref dt);

            int header = 6;
            int alignName = 0, alignNote = 0;

            //將 Word 表格中的資料轉成 DataTable
            for (int i = header + 1; i <= t.Rows.Count - 2; i++)
            {
                WORD.Row wdRow = t.Rows[i];
                if (wdRow.Cells.Count < 6) continue;
                string no = wdRow.Cells[1].Range.Text.RemoveCtrlChar();
                if (string.IsNullOrEmpty(no)) continue;

                DataRow dtRow = dt.NewRow();
                bool isKey = t.Rows[i].Range.Font.Color == WORD.WdColor.wdColorRed;

                dtRow["colKey"] = isKey;
                dtRow["colNo"] = no.Contains("*") ? no.Substring(1) : no;//移除星號
                dtRow["colName"] = wdRow.Cells[2].Range.Text.RemoveCtrlChar();
                dtRow["colNote"] = wdRow.Cells[3].Range.Text.RemoveCtrlChar();
                dtRow["colType"] = wdRow.Cells[4].Range.Text.RemoveCtrlChar().ToUpper();

                if (dtRow["colType"].ToString() == "SMALLINT")
                {
                    string remark = wdRow.Cells[6].Range.Text.ToUpper();
                    if (remark.Contains("OPT"))
                        dtRow["colLength"] = remark.Substring("OPT", "\r");
                    else if (remark.Contains("OPTION"))
                        dtRow["colLength"] = remark.Substring("OPTION", "\r");

                    dtRow["colRemark"] =
                        wdRow.Cells[6].Range.Text.Substring(remark.IndexOf((char)13) + 1).RemoveCtrlChar();
                }
                else
                {
                    dtRow["colLength"] = wdRow.Cells[5].Range.Text.RemoveCtrlChar();
                    dtRow["colRemark"] = wdRow.Cells[6].Range.Text.RemoveCtrlChar();
                }

                dt.Rows.Add(dtRow);
                alignName = Math.Max(alignName, dtRow["colName"].ToString().Length);
                alignNote = Math.Max(alignNote, dtRow["colNote"].ToString().Width());
            }
            ExportSql.AlignName = alignName + 2;
            ExportSql.AlignNote = alignNote + 2;
        }

        private static bool IsCellEmpty(WORD.Table t, int i, int j, string errmsg, ref string s)
        {
            s = t.Cell(i, j).Range.Text.RemoveCtrlChar().Trim();
            int k = s.IndexOf('：') + 1;
            bool isEmpty = s.Length == s.IndexOf('：') + 1;
            if (isEmpty && errmsg.Length > 0) MessageBox.Show(errmsg);
            else s = s.Substring(s.IndexOf('：') + 1);
            return isEmpty;
        }
    }
}