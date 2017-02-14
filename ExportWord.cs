using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WORD = Microsoft.Office.Interop.Word;

namespace SpecCreator
{
    internal class ExportWord
    {
        public static DataTable dt;
        public static string TableName;
        public static string TableCName;
        private static string[][] cellContent = new string[6][];
        private static int header = 6;

        public static void ExportSpec(string fileName)
        {
            WORD._Application openWord = new WORD.Application();
            WORD._Document spec = openWord.Documents.Add();

            WORD.Range range = spec.Range();
            WORD.Table table = spec.Content.Tables.Add(range, dt.Rows.Count + 8, dt.Columns.Count - 1);
            SetTableTemplate(ref table);
            WriteContentToTable(ref table);
            object o = (object)fileName;
            spec.SaveAs(ref o);
            openWord.Quit();
        }

        private static void InitCellContent()
        {
            cellContent[0] = new string[] { "文件名稱：" + TableCName, "檔案代碼：" + TableName, "文件代碼：" };
            cellContent[1] = new string[] { "子系統別：", "檔案類型：", "版本編號：" };
            cellContent[2] = new string[] { "設計日期：", "更新日期：", "設 計 者：" };
            cellContent[3] = new string[] { "文件概要" };
            cellContent[4] = new string[] { "Table Field" };
            cellContent[5] = new string[] { "序號", "欄位名稱", "欄位說明", "型態", "長度", "備註" };
        }

        private static void SetTableTemplate(ref WORD.Table table)
        {
            table.Borders.InsideLineStyle = WORD.WdLineStyle.wdLineStyleSingle;
            table.Borders.OutsideLineStyle = WORD.WdLineStyle.wdLineStyleSingle;

            for (int i = 1; i < header; i++)
            {
                if (i < header - 2)
                {
                    for (int j = 1; j <= 3; j++) table.Cell(i, j).Merge(table.Cell(i, j + 1));
                }
                else table.Cell(i, 1).Merge(table.Cell(i, table.Columns.Count));
            }
        }

        private static void WriteContentToTable(ref WORD.Table table)
        {
            InitCellContent();
            for (int i = 0; i < header; i++)
            {
                for (int j = 0; j < table.Rows[i + 1].Cells.Count; j++)
                {
                    table.Cell(i + 1, j + 1).Range.Text = cellContent[i][j];
                }
            }

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                int k = i + header + 1;
                for (int j = 1; j < dt.Columns.Count; j++)
                {
                    table.Cell(k, j).Range.Text = (dt.Rows[i][j] == null) ? string.Empty : dt.Rows[i][j].ToString();
                }

                bool key;
                bool.TryParse(dt.Rows[i][0].ToString(), out key);
                if (key)
                {
                    table.Cell(k, 1).Range.Text = "*" + table.Cell(k, 1).Range.Text;
                    table.Rows[k].Range.Font.Color = WORD.WdColor.wdColorRed;
                }
            }
        }
    }
}