using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpecCreator.DataStrcutures;
using SpecCreator.FileHandlers;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace UnitTest
{
    [TestClass]
    public class SqlHandlerUnitTest
    {
        [TestMethod]
        public void TestSqlToTable()
        {
            var expectedTable = CommonFunc.GetTestingTable(new DateTime(2017, 2, 9));
            var actualTable = (new SqlHandler() as IFileHandler<string>).ConvertToTable(Properties.Resources.test_table);

            string message = string.Empty;
            Assert.IsTrue(CommonFunc.CompareTables(expectedTable, actualTable, ref message), message);
        }

        [TestMethod]
        public void TestTableToSql()
        {
            string expectedSql = Properties.Resources.test_table;
            string actualSql = (new SqlHandler() as IFileHandler<string>).ConvertToMeta(CommonFunc.GetTestingTable(new DateTime(2017, 2, 9)));

            string message = string.Empty;
            Assert.IsTrue(CompareStrings(expectedSql, actualSql, ref message), message);
        }

        private static bool CompareStrings(string a, string b, ref string message)
        {
            try
            {
                var regex = new Regex(@"\r\n?|\n");
                string[] linesA = regex.Split(a);
                string[] linesB = regex.Split(b);

                if (linesA.Length != linesB.Length)
                    return false;

                for (int i = 0; i < linesA.Length; ++i)
                {
                    if (linesA[i] != linesB[i])
                    {
                        string diffMessage = string.Format("Line:【{0}】\r\nExpected:【{1}】\r\nActual:【{2}】\r\n",
                            i + 1,
                            linesA[i],
                            linesB[i]);

                        throw new Exception(diffMessage);
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return false;
            }
        }
    }
}