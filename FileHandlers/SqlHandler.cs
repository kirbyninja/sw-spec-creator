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
    public class SqlHandler : IFileHandler<string>, IFileHandler
    {
        private static readonly string Template = Properties.Resources.SqlTemplate;

        private enum TableInfo { TableName, Name, Date, Author, }

        public string Description { get { return "SQL Scripts"; } }

        public string Extension { get { return "sql"; } }

        string IFileHandler<string>.ConvertToMeta(WorkingTable table)
        {
            try
            {
                int maxWidthOfColumnName = table.WorkingColumns.Max(c => c.ColumnName.GetWidth());
                int maxWidthOfCaption = table.WorkingColumns.Max(c => c.Caption.GetWidth());
                string s = string.Format(Template,
                    table.Author,
                    table.Date.ToString("yyyy-MM-dd"),
                    table.TableName,
                    GetCreatTableScript(table.TableName, maxWidthOfColumnName, table.WorkingColumns),
                    JoinStringChunksWithNewLine(new[]
                    {
                        GetAddConstaintScriptOfPrimaryKeys(table.TableName, table.WorkingColumns),
                        GetAddConstaintScriptOfUniqueFields(table.TableName, table.WorkingColumns),
                    }),
                    table.Name,
                    string.Join("", table.WorkingColumns.Select(c => GetInsertScriptOfField(c, maxWidthOfColumnName, maxWidthOfCaption))),
                    JoinStringChunksWithNewLine(table.WorkingColumns.Select(c => GetInsertScriptOfOption(c.Option))));

                return s;
            }
            catch (Exception ex)
            {
                throw new ArgumentException("資料格式有誤", ex);
            }
        }

        WorkingTable IFileHandler<string>.ConvertToTable(string sql)
        {
            try
            {
                var table = new WorkingTable();
                table.TableName = GetTableInfo(sql, TableInfo.TableName);
                table.Name = GetTableInfo(sql, TableInfo.Name);
                table.Author = GetTableInfo(sql, TableInfo.Author);
                table.Date = GetTableInfo(sql, TableInfo.Date).ParseDate();

                table.AddColumns(GetWorkingColumns(sql, GetOptions(sql)));

                return table;
            }
            catch (Exception ex)
            {
                throw new FormatException("文件格式有誤", ex);
            }
        }

        string IFileHandler<string>.Load(string fileName)
        {
            try
            {
                string sql;
                using (var reader = new StreamReader(fileName))
                {
                    sql = reader.ReadToEnd();
                }
                return sql;
            }
            catch (Exception ex)
            {
                throw new Exception("檔案讀取失敗", ex);
            }
        }

        void IFileHandler<string>.Save(string sql, string fileName)
        {
            try
            {
                using (var writer = new StreamWriter(fileName, false))
                {
                    writer.WriteLine(sql);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("檔案寫入失敗", ex);
            }
        }

        public WorkingTable Load(string fileName)
        {
            var t = this as IFileHandler<string>;
            return t.ConvertToTable(t.Load(fileName));
        }

        public void Save(WorkingTable table, string fileName)
        {
            var t = this as IFileHandler<string>;
            t.Save(t.ConvertToMeta(table), fileName);
        }

        private static string GetAddConstaintScriptOfPrimaryKeys(string tableName, IEnumerable<WorkingColumn> columns)
        {
            if (!columns.Any(c => c.IsPrimaryKey))
                return string.Empty;

            string s = string.Format(
@"ALTER TABLE [dbo].[{0}] ADD CONSTRAINT [PK_{0}] PRIMARY KEY CLUSTERED
(
    {1}
)
ON [PRIMARY]
GO
",
            tableName,
            string.Join(",", columns.Where(c => c.IsPrimaryKey).Select(c => string.Format("[{0}]", c.ColumnName))));

            return s;
        }

        private static string GetAddConstaintScriptOfUniqueFields(string tableName, IEnumerable<WorkingColumn> columns)
        {
            if (!columns.Any(c => c.IsUnique))
                return string.Empty;

            string s = string.Format(
@"ALTER TABLE [dbo].[{0}] ADD CONSTRAINT [UK_{0}] UNIQUE NONCLUSTERED
(
    {1}
)
ON [PRIMARY]
GO
",
            tableName,
            string.Join(",", columns.Where(c => c.IsUnique).Select(c => string.Format("[{0}]", c.ColumnName))));

            return s;
        }

        private static string GetCreatTableScript(string tableName, int maxWidthOfColumnName, IEnumerable<WorkingColumn> columns)
        {
            string s = string.Format(
@"CREATE TABLE [dbo].[Tmp_{0}] (
{1}
);
",
            tableName,
            string.Join(",\r\n", columns.Select(c => string.Format("\t{0}\t{1} NOT NULL",
                string.Format("[{0}]", c.ColumnName).PadRight(maxWidthOfColumnName + 2),
                GetSqlDataType(c)))));

            return s;
        }

        private static string GetEditorType(WorkingColumn column)
        {
            switch (column.DataType)
            {
                case "INT":
                case "NUMERIC":
                    return "CalcEdit";

                case "SMALLINT":
                    return "LookUpEdit";

                case "DATE":
                case "SMALLDATETIME":
                case "DATETIME":
                    return "DateEdit";

                case "BIT":
                    return "CheckEdit";

                default:
                    return "TextEdit";
            }
        }

        private static string GetInsertScriptOfField(WorkingColumn column, int maxWidthOfColumnName, int maxWidthOfCaption)
        {
            // maxWidth + 2是因為要把兩個單引號的長度也算進去。
            string s = string.Format("INSERT #appTableField SELECT @tbname, {0}, {1}, {2}, {2}, {3}, {3}, 0, '', 0, {4}, 0, '', {5}, {1}, 150, '{6}', @loguser, @dt;\r\n",
                string.Format("'{0}'", column.ColumnName).PadRight(maxWidthOfColumnName + 2),
                column.ColumnNo.ToString().PadLeft(2),
                string.Format("'{0}'", column.Caption).PadRight(maxWidthOfCaption - column.Caption.GetWidth() + column.Caption.Length + 2),
                Convert.ToInt32(column.IsPrimaryKey || column.IsUnique),
                (column.Option == null ? 0 : column.Option.OptionNo).ToString().PadLeft(3),
                Convert.ToInt32(IsVisible(column)),
                GetEditorType(column));

            return s;
        }

        private static string GetInsertScriptOfFieldOption(Option option)
        {
            if (option == null || option.OptionNo <= 0)
                return string.Empty;

            return string.Format("INSERT #appTableFieldo SELECT {0}, '{0}.{1}', 1;\r\n",
                option.OptionNo, option.Text);
        }

        private static string GetInsertScriptOfFieldOptionItem(OptionItem item)
        {
            if (item == null || item.Option == null || item.Option.OptionNo <= 0)
                return string.Empty;

            return string.Format("INSERT #appTableFieldoi SELECT {0}, {1}, '{2}', @loguser, @dt;\r\n",
                    item.Option.OptionNo, item.ItemNo, item.Text);
        }

        private static string GetInsertScriptOfOption(Option option)
        {
            if (option == null || option.OptionNo <= 0 || option.Items.Count() == 0)
                return string.Empty;

            return string.Format(@"{0}{1}",
                GetInsertScriptOfFieldOption(option),
                string.Join("", option.Items.Select(i => GetInsertScriptOfFieldOptionItem(i))));
        }

        private static IList<Option> GetOptions(string input)
        {
            var list = new List<Option>();
            string pattern =
@"INSERT\s+#appTableFieldo\s+SELECT\s+(?<opt>\d+)\s?,\s?'(?:\k<opt>.)?(.*)'\s?,\s?\d+[;\s]+(?:INSERT\s+#appTableFieldoi\s+SELECT\s+\k<opt>\s?,\s?(\d+)\s?,\s?'(.*)'\s?,\s?@loguser\s?,\s?@dt[;\s]+)+";
            foreach (Match m in Regex.Matches(input, pattern, RegexOptions.IgnoreCase))
            {
                var option = new Option(int.Parse(m.Groups[4].Value), GetValidText(m.Groups[1].Value, false));
                for (int i = 0; i < m.Groups[2].Captures.Count; ++i)
                    option.AddItem(int.Parse(m.Groups[2].Captures[i].Value), GetValidText(m.Groups[3].Captures[i].Value, false));
                list.Add(option);
            }
            return list;
        }

        private static string GetSqlDataType(WorkingColumn column)
        {
            switch (column.DataType)
            {
                case "NUMERIC":
                case "CHAR":
                case "NCHAR":
                case "VARCHAR":
                case "NVARCHAR":
                    return string.Format("{0}({1})", column.DataType, column.Length);

                default:
                    return column.DataType;
            }
        }

        private static string GetTableInfo(string input, TableInfo info)
        {
            string pattern = string.Empty;
            switch (info)
            {
                case TableInfo.Author:
                    pattern = @"作者[\t ]*:([^\r\n]*)[\r\n]+";
                    break;

                case TableInfo.Date:
                    pattern = @"日期[\t ]*:([^\r\n]*)[\r\n]+";
                    break;

                case TableInfo.Name:
                    pattern = @"INSERT\s+app_table\s+SELECT\s+@tbname\s*,\s*'(.*)'[;\s]+";
                    break;

                case TableInfo.TableName:
                    pattern = @"SELECT\s+@tbname\s*=\s*'(.+)'";
                    break;

                default:
                    return string.Empty;
            }

            return GetValidText(Regex.Match(input, pattern, RegexOptions.IgnoreCase).Groups[1].Value, false);
        }

        private static string GetValidText(string input, bool isQuoted = true)
        {
            if (isQuoted)
                // 先去掉頭尾的空白再去掉頭尾的單引號，最後再處理字串裡的特殊字元「'」
                return input.Trim().Remove(@"^'|'$").Replace("''", "'");
            else
                // 所有的單引號（不管是不是在頭尾）都是內文之一，而非SQL語法中字串的夾注號
                return input.Trim().Replace("''", "'");
        }

        private static IList<WorkingColumn> GetWorkingColumns(string input, IEnumerable<Option> options)
        {
            var list = new List<WorkingColumn>();
            string pattern = @"INSERT #appTableField SELECT(?:\s*((?(')'.*'|[\w@-]+))\s*,){18} @dt;";
            foreach (Match match in Regex.Matches(input, pattern, RegexOptions.IgnoreCase))
            {
                Group g = match.Groups[1];
                var column = new WorkingColumn();
                column.ColumnName = GetValidText(g.Captures[1].Value);
                column.ColumnNo = int.Parse(GetValidText(g.Captures[2].Value));
                column.Caption = GetValidText(g.Captures[3].Value);
                column.IsPrimaryKey = IsPrimaryKey(column.ColumnName, input);
                column.IsUnique = IsUniqueField(column.ColumnName, input);
                SetDataType(column, input);

                int optNo = int.Parse(GetValidText(g.Captures[10].Value));
                if (column.DataType == "SMALLINT" && optNo > 0)
                    column.Option = options.FirstOrDefault(opt => opt.OptionNo == optNo) ??
                        new Option(optNo, column.Caption);

                list.Add(column);
            }
            return list;
        }

        private static bool IsPrimaryKey(string columnName, string input)
        {
            string pattern = @"ADD\s+CONSTRAINT.+PRIMARY\s+KEY\s+CLUSTERED\s*\(([\s\w\[\],]*)\)\s*ON\s*\[PRIMARY\]";
            string result = Regex.Match(input, pattern, RegexOptions.IgnoreCase | RegexOptions.Singleline).Groups[1].Value.RemoveCtrlChar();
            var keys = result.Split(',').Select(s => s.Trim().Remove(@"^\[|\]$"));
            return keys.Contains(columnName);
        }

        private static bool IsUniqueField(string columnName, string input)
        {
            string pattern = @"ADD\s+CONSTRAINT.+UNIQUE\s+NONCLUSTERED\s*\(([\s\w\[\],]*)\)\s*ON\s*\[PRIMARY\]";
            string result = Regex.Match(input, pattern, RegexOptions.IgnoreCase | RegexOptions.Singleline).Groups[1].Value.RemoveCtrlChar();
            var keys = result.Split(',').Select(s => s.Trim().Remove(@"^\[|\]$"));
            return keys.Contains(columnName);
        }

        private static bool IsVisible(WorkingColumn column)
        {
            switch (column.ColumnName)
            {
                case "lock":
                case "loguser":
                case "logtime":
                case "lockuser":
                case "locktime":
                    return false;

                default:
                    return true;
            }
        }

        private static string JoinStringChunksWithNewLine(IEnumerable<string> stringChunks)
        {
            string newLine = "\r\n";

            if (stringChunks == null)
                return string.Empty;

            stringChunks = stringChunks.Where(chunk => !string.IsNullOrWhiteSpace(chunk));

            if (stringChunks.Count() == 0)
                return string.Empty;

            return string.Concat(newLine, string.Join(newLine, stringChunks));
        }

        private static void SetDataType(WorkingColumn column, string input)
        {
            if (column == null || string.IsNullOrWhiteSpace(column.ColumnName)) return;

            string pattern = string.Format(@"\[{0}\](.+)NOT\s+NULL", column.ColumnName);
            input = Regex.Match(input, pattern, RegexOptions.IgnoreCase).Groups[1].Value.Trim();

            pattern = @"\((.*)\)";
            Match match = Regex.Match(input, pattern);
            if (match.Success)
            {
                string length = match.Groups[1].Value;
                column.Length = string.Join(",", length.Split(',').Select(s => s.Trim().ToUpper()));
                input = input.Remove(pattern).Trim();
            }
            column.DataType = input.ToUpper();
        }
    }
}