using Microsoft.Office.Interop.Word;
using SpecCreator.DataStrcutures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SpecCreator.FileHandlers
{
    public class WordHandler : IFileHandler<Document>, IFileHandler
    {
        public string Description { get { return "Word Documents"; } }

        public string Extension { get { return "docx|doc"; } }

        Document IFileHandler<Document>.ConvertToMeta(WorkingTable table)
        {
            throw new NotImplementedException();
        }

        WorkingTable IFileHandler<Document>.ConvertToTable(Document document)
        {
            try
            {
                var table = document.Tables[1];
                var workingTable = new WorkingTable();
                var match = new Regex(@"(:|：)(.+)");
                workingTable.Name = match.Match(GetCellText(table.Cell(1, 1))).Groups[2].Value;
                workingTable.TableName = match.Match(GetCellText(table.Cell(1, 2))).Groups[2].Value;
                workingTable.Date = DateTime.Parse(match.Match(GetCellText(table.Cell(3, 2))).Groups[2].Value);
                workingTable.Author = match.Match(GetCellText(table.Cell(3, 3))).Groups[2].Value;
                int rowIndex = 7;
                while (table.Rows[rowIndex].Cells.Count >= 6 && !string.IsNullOrEmpty(GetCellText(table.Cell(rowIndex, 2))))
                    workingTable.AddColumn(GetWorkingColumn(table.Rows[rowIndex++]));

                return workingTable;
            }
            catch (Exception ex)
            {
                throw new FormatException("文件格式有誤", ex);
            }
            finally
            {
                DisposeApplication(document.Application);
            }
        }

        Document IFileHandler<Document>.Load(string fileName)
        {
            Application application = new Application();
            try
            {
                return application.Documents.Open(fileName, false, true);
            }
            catch (Exception ex)
            {
                DisposeApplication(application);
                throw new Exception("檔案讀取失敗", ex);
            }
        }

        void IFileHandler<Document>.Save(Document document, string fileName)
        {
            try
            {
                object name = fileName;
                document.SaveAs2(ref name);
            }
            catch (Exception ex)
            {
                throw new Exception("檔案寫入失敗", ex);
            }
            finally
            {
                DisposeApplication(document.Application);
            }
        }

        public WorkingTable Load(string fileName)
        {
            var t = this as IFileHandler<Document>;
            return t.ConvertToTable(t.Load(fileName));
        }

        public void Save(WorkingTable table, string fileName)
        {
            var t = this as IFileHandler<Document>;
            t.Save(t.ConvertToMeta(table), fileName);
        }

        private static void DisposeApplication(Application application)
        {
            (application as _Application).Quit();
            application = null;
        }

        private static string GetCellText(Cell cell)
        {
            return cell.Range.Text.RemoveCtrlChar().Trim();
        }

        private static Option GetOption(string text)
        {
            var option = new Option();

            // 先用「區隔符號」將字串切開，以確保移除所有特殊字元後不會全黏在一起
            text = string.Join(" ", Regex.Split(text, @"\r\n?|\n|\t| ").Select(s => s.RemoveCtrlChar()).Where(s => s != string.Empty));
            var match = Regex.Match(text, @"^(opt|option) ?(-?\d+) ", RegexOptions.IgnoreCase);
            if (match.Success)
            {
                option.OptionNo = int.Parse(match.Groups[2].Value);
                text = text.Substring(match.Value.Length);
            }

            foreach (Match m in Regex.Matches(text, @" ?(-?\d+).[^ ]+ ?"))
                option.AddItem(int.Parse(m.Groups[1].Value), m.Value);

            return option;
        }

        private static WorkingColumn GetWorkingColumn(Row row)
        {
            var column = new WorkingColumn();
            string columnNo = GetCellText(row.Cells[1]);
            column.IsPrimaryKey = columnNo.Contains("*");
            column.ColumnNo = int.Parse(columnNo.Replace("*", ""));
            column.ColumnName = GetCellText(row.Cells[2]);
            column.Caption = GetCellText(row.Cells[3]);
            column.DataType = GetCellText(row.Cells[4]);
            string length = GetCellText(row.Cells[5]);
            if (column.DataType.ToUpper() == "SMALLINT")
            {
                var option = GetOption(row.Cells[6].Range.Text);
                if (option.OptionNo == 0 && !string.IsNullOrEmpty(length))
                    option.OptionNo = int.Parse(length);
                option.Text = column.Caption;
                column.Option = option;
            }
            else
                column.Length = string.Join(", ", length.Split(new[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries));
            return column;
        }
    }
}