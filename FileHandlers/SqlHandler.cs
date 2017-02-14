﻿using SpecCreator.DataStrcutures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpecCreator.FileHandlers
{
    public class SqlHandler : IFileHandler<string>, IFileHandler
    {
        private static readonly string Template = Properties.Resources.SqlTemplate;

        public string Description { get { return "SQL Scripts"; } }

        public string Extension { get { return "sql"; } }

        string IFileHandler<string>.ConvertToMeta(WorkingTable table)
        {
            int maxWidthOfColumnName = table.WorkingColumns.Max(c => c.ColumnName.GetWidth());
            int maxWidthOfCaption = table.WorkingColumns.Max(c => c.Caption.GetWidth());
            string s = string.Format(Template,
                table.Author,
                table.Date.ToString("yyyy-MM-dd"),
                table.TableName,
                GetCreatTableScript(table.TableName, maxWidthOfColumnName, table.WorkingColumns),
                GetAddConstaintScriptOfPrimaryKeys(table.TableName, table.WorkingColumns),
                table.Name,
                string.Join("", table.WorkingColumns.Select(c => GetInsertScriptOfField(c, maxWidthOfColumnName, maxWidthOfCaption))),
                string.Join("\r\n", table.WorkingColumns.Where(c => c.OptionNo > 0).Select(c => GetInsertScriptOfOption(c))));

            return s;
        }

        WorkingTable IFileHandler<string>.ConvertToTable(string sql)
        {
            throw new NotImplementedException();
        }

        string IFileHandler<string>.Load(string fileName)
        {
            throw new NotImplementedException();
        }

        void IFileHandler<string>.Save(string sql, string fileName)
        {
            throw new NotImplementedException();
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
                Convert.ToInt32(column.IsPrimaryKey),
                column.OptionNo.ToString().PadLeft(3),
                Convert.ToInt32(IsVisible(column)),
                GetEditorType(column));

            return s;
        }

        private static string GetInsertScriptOfFieldOption(WorkingColumn column)
        {
            if (column.OptionNo <= 0)
                return string.Empty;

            return string.Format("INSERT #appTableFieldo SELECT {0}, '{0}.{1}', 1;\r\n",
                column.OptionNo, column.Caption);
        }

        private static string GetInsertScriptOfFieldOptionItem(OptionItem item)
        {
            if (item.OptionNo <= 0)
                return string.Empty;

            return string.Format("INSERT #appTableFieldoi SELECT {0}, {1}, '{2}', @loguser, @dt;\r\n",
                    item.OptionNo, item.ItemNo, item.Text);
        }

        private static string GetInsertScriptOfOption(WorkingColumn column)
        {
            if (column.OptionNo <= 0)
                return string.Empty;

            return string.Format(@"{0}{1}",
                GetInsertScriptOfFieldOption(column),
                string.Join("", column.OptionItems.Select(i => GetInsertScriptOfFieldOptionItem(i))));
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
    }
}