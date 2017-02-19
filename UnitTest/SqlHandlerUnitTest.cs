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
        [TestMethod]
        public void TestTableToSql()
        {
            string expectedSql = Properties.Resources.test_table;
            string actualSql = (new SqlHandler() as IFileHandler<string>).ConvertToMeta(CommonFunc.GetTestingTable(new DateTime(2017, 2, 9)));
            Assert.AreEqual<string>(expectedSql, actualSql);
        }

        [TestMethod]
        public void TestSqlToTable()
        {
            var expectedTable = CommonFunc.GetTestingTable(new DateTime(2017, 2, 9));
            var actualTable = (new SqlHandler() as IFileHandler<string>).ConvertToTable(Properties.Resources.test_table);

            string message = string.Empty;
            Assert.IsTrue(CommonFunc.CompareTables(expectedTable, actualTable, ref message), message);
        }
    }
}