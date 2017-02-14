using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpecCreator.DataStrcutures;
using SpecCreator.FileHandlers;
using System;
using System.Linq;

namespace UnitTest
{
    [TestClass]
    public class SqlHandlerUnitTest
    {
        private static WorkingTable TestingTable = GetTestingTable();

        [TestMethod]
        public void TestTableToSql()
        {
            string expectedSql = Properties.Resources.test_table;
            string actualSql = (new SqlHandler() as IFileHandler<string>).ConvertToMeta(TestingTable);
            Assert.AreEqual<string>(expectedSql, actualSql);
        }

        private static WorkingTable GetTestingTable()
        {
            var table = new WorkingTable()
            {
                Name = "測試資料表",
                TableName = "test_table",
                Author = "Tester",
                Date = new DateTime(2017, 2, 9),
                WorkingColumns = new WorkingColumn[]
                {
                    new WorkingColumn("key1", "測試主鍵1", "VARCHAR") { Length = "20", IsPrimaryKey = true },
                    new WorkingColumn("key2", "測試主鍵2", "INT") { IsPrimaryKey = true },
                    new WorkingColumn("date1", "測試日期1", "DATE"),
                    new WorkingColumn("date2", "測試日期2", "DATETIME"),
                    new WorkingColumn("date3", "測試日期3", "SMALLDATETIME"),
                    new WorkingColumn("char1", "測試字串1", "CHAR") { Length = "10" },
                    new WorkingColumn("char2", "測試字串2", "NCHAR") { Length = "10" },
                    new WorkingColumn("char3", "測試字串3", "VARCHAR") { Length = "10" },
                    new WorkingColumn("char4", "測試字串4", "NVARCHAR") { Length = "10" },
                    new WorkingColumn("char5", "測試字串5", "NVARCHAR") { Length = "MAX" },
                    new WorkingColumn("bit1", "測試二元1", "BIT"),
                    new WorkingColumn("opt1", "測試選項1", "SMALLINT") { OptionNo = 2, OptionItems = new[] {
                            new OptionItem(2, 1, "1.買進"),
                            new OptionItem(2, 2, "2.賣出") } },
                    new WorkingColumn("opt2", "測試選項2", "SMALLINT") { OptionNo = 8, OptionItems = new[] {
                            new OptionItem(8, 1, "1.市價"),
                            new OptionItem(8, 2, "2.限價") } },
                    new WorkingColumn("int1", "測試整數1", "INT"),
                    new WorkingColumn("numeric1", "測試浮點數1", "NUMERIC") { Length = "20,5" },
                }
            };
            for (int i = 0; i < table.WorkingColumns.Count(); ++i)
            {
                var column = table.WorkingColumns.ElementAt(i);
                column.WorkingTable = table;
                column.ColumnNo = i + 1;
            }
            return table;
        }
    }
}