using Microsoft.Office.Interop.Word;
using SpecCreator.DataStrcutures;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SpecCreator.FileHandlers
{
    public class WordHandler : IFileHandler<Document>, IFileHandler
    {
        private const string TempFolderName = ".TmpSpec";

        public string Description { get { return "Word Documents"; } }

        public string Extension { get { return "docx|doc"; } }

        Document IFileHandler<Document>.ConvertToMeta(WorkingTable table)
        {
            Application application = new Application();
            Document document = null;
            try
            {
                string fileName = CreateTempFile(Properties.Resources.WordTemplate);
                document = application.Documents.Add(fileName);
                Table t = document.Tables[1];

                InsertTableInfo(t, table);

                for (int i = 0; i < table.WorkingColumns.Count(); ++i)
                    InsertColumnInfo(t.Rows[i + 7], table.WorkingColumns.ElementAt(i));

                return document;
            }
            catch (Exception ex)
            {
                DisposeApplication(document);
                throw new ArgumentException("資料格式有誤", ex);
            }
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
                DisposeApplication(document);
            }
        }

        Document IFileHandler<Document>.Load(string fileName)
        {
            Application application = new Application();
            Document document = null;
            try
            {
                document = application.Documents.Open(fileName, false, true);
                return document;
            }
            catch (Exception ex)
            {
                DisposeApplication(document);
                throw new Exception("檔案讀取失敗", ex);
            }
        }

        void IFileHandler<Document>.Save(Document document, string fileName)
        {
            try
            {
                document.SaveAs2(fileName);
            }
            catch (Exception ex)
            {
                throw new Exception("檔案寫入失敗", ex);
            }
            finally
            {
                DisposeApplication(document);
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

        private static void AppendTextInCell(Cell cell, string text)
        {
            cell.Range.Text = string.Concat(GetCellText(cell), text);
        }

        private static string CreateTempFile(byte[] template)
        {
            var dir = Directory.CreateDirectory(string.Format(@"{0}\{1}", Directory.GetCurrentDirectory(), TempFolderName));
            dir.Attributes = FileAttributes.Hidden;
            string fileName = string.Format(@"{0}\{1}.dotx", dir.FullName, System.Threading.Thread.CurrentThread.ManagedThreadId);
            using (var sr = new FileStream(fileName, FileMode.Create))
            {
                foreach (var b in template)
                    sr.WriteByte(b);
            }
            var fi = new FileInfo(fileName);
            fi.Attributes = FileAttributes.Temporary;

            return fileName;
        }

        private static void DisposeApplication(Document document)
        {
            if (document != null)
            {
                var application = document.Application;

                (document as _Document).Close(WdSaveOptions.wdDoNotSaveChanges);
                document = null;

                (application as _Application).Quit();
                application = null;
            }

            string folderPath = string.Format(@"{0}\{1}", Directory.GetCurrentDirectory(), TempFolderName);

            if (Directory.Exists(folderPath))
                Directory.Delete(folderPath, true);
        }

        private static string GetCellText(Cell cell)
        {
            return cell.Range.Text.RemoveCtrlChar().Trim();
        }

        private static Option GetOption(string text)
        {
            var option = new Option() { OptionNo = -1 };

            // 先用「區隔符號」將字串切開，以確保移除所有特殊字元後不會全黏在一起
            text = string.Join(" ", Regex.Split(text, @"\r\n?|\n|\t| ").Select(s => s.RemoveCtrlChar()).Where(s => s != string.Empty));
            var match = Regex.Match(text, @"^(opt|option) ?(-?\d+) ", RegexOptions.IgnoreCase);
            if (match.Success)
            {
                option.OptionNo = int.Parse(match.Groups[2].Value);
                text = text.Substring(match.Value.Length);
            }

            foreach (Match m in Regex.Matches(text, @" ?((-?\d+).[^ ]+) ?"))
                option.AddItem(int.Parse(m.Groups[2].Value), m.Groups[1].Value);

            return option;
        }

        private static string GetOptionText(Option option)
        {
            if (option == null)
                return string.Empty;

            string itemString = string.Join(" ", option.Items.Select(i =>
            {
                if (i.Text.StartsWith(string.Format("{0}.", i.ItemNo)))
                    return i.Text;
                else
                    return string.Format("{0}.{1}", i.ItemNo, i.Text);
            }));

            if (string.IsNullOrWhiteSpace(itemString))
                return string.Format("Opt {0}", option.OptionNo);
            else
                return string.Format("Opt {0}\n{1}", option.OptionNo, itemString);
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
                if (option.OptionNo <= 0 && !string.IsNullOrEmpty(length))
                    option.OptionNo = int.Parse(length);
                option.Text = column.Caption;
                column.Option = option;
            }
            else if (!string.IsNullOrEmpty(length))
                column.Length = string.Join(",", length.Split(new[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries));
            return column;
        }

        private static void InsertColumnInfo(Row row, WorkingColumn workingColumn)
        {
            var p = row.Parent;
            if (!(p is Table)) return;

            Table table = p as Table;

            // 先將目前的空白行複製一行到後面
            table.Rows.Add(row.Next);

            if (workingColumn.IsPrimaryKey)
            {
                row.Cells[1].Range.Text = string.Format("*{0}", workingColumn.ColumnNo);
                row.Range.Font.Color = WdColor.wdColorRed;
            }
            else
                row.Cells[1].Range.Text = workingColumn.ColumnNo.ToString();

            row.Cells[2].Range.Text = workingColumn.ColumnName;
            row.Cells[3].Range.Text = workingColumn.Caption;
            row.Cells[4].Range.Text = workingColumn.DataType;
            row.Cells[5].Range.Text = workingColumn.Length;
            row.Cells[6].Range.Text = GetOptionText(workingColumn.Option);
        }

        private static void InsertTableInfo(Table table, WorkingTable workingTable)
        {
            AppendTextInCell(table.Cell(1, 1), workingTable.Name);
            AppendTextInCell(table.Cell(1, 2), workingTable.TableName);
            AppendTextInCell(table.Cell(3, 3), workingTable.Author);
        }
    }
}