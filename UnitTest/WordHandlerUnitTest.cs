using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpecCreator.DataStrcutures;
using SpecCreator.FileHandlers;
using System;
using System.IO;
using System.Linq;

namespace UnitTest
{
    [TestClass]
    public class WordHandlerUnitTest
    {
        private static WorkingTable TestingTable = GetTestingTable();

        [TestMethod]
        public void TestWordToTable()
        {
            string textFilePath = string.Format("{0}{1}",
                Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName,
                @"\Resources\test_word.docx");

            var actualTable = (new WordHandler()).Load(textFilePath);

            Assert.AreEqual(TestingTable, actualTable);
        }

        private static WorkingTable GetTestingTable()
        {
            var table = new WorkingTable()
            {
                Name = "測試資料表",
                TableName = "test_table",
                Author = "Tester",
                Date = new DateTime(2017, 2, 13),
            };
            table.AddColumns(new WorkingColumn[]
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
                    new WorkingColumn("opt1", "測試選項1", "SMALLINT") { Option = new Option(2, "2.測試選項1", new[] {
                            new OptionItem(1, "1.買進"),
                            new OptionItem(2, "2.賣出") }) },
                    new WorkingColumn("opt2", "測試選項2", "SMALLINT") { Option = new Option(8, "8.測試選項2", new[] {
                            new OptionItem(1, "1.市價"),
                            new OptionItem(2, "2.限價") }) },
                    new WorkingColumn("int1", "測試整數1", "INT"),
                    new WorkingColumn("numeric1", "測試浮點數1", "NUMERIC") { Length = "20, 5" },
                });

            for (int i = 0; i < table.WorkingColumns.Count(); ++i)
                table.WorkingColumns.ElementAt(i).ColumnNo = i + 1;

            return table;
        }
    }
}