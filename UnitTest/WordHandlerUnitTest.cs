using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpecCreator.DataStrcutures;
using SpecCreator.FileHandlers;
using Microsoft.Office.Interop.Word;
using System;
using System.Text.RegularExpressions;
using System.IO;
using System.Linq;

namespace UnitTest
{
    [TestClass]
    public class WordHandlerUnitTest
    {
        [TestMethod]
        public void TestTableToWord()
        {
            Document expectedDoc = null;
            Document actualDoc = null;
            try
            {
                string testFilePath = string.Concat(
                    Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName,
                    @"\Resources\expected_word.docx");
                expectedDoc = (new Application()).Documents.Open(testFilePath, false, true);
                var expectedArray = ReadTable(expectedDoc.Tables[1]);

                var workingTable = CommonFunc.GetTestingTable(new DateTime(2017, 2, 19));
                actualDoc = (new WordHandler() as IFileHandler<Document>).ConvertToMeta(workingTable);
                ReplaceSaveDate(actualDoc, new DateTime(2017, 2, 19));

                var actualArray = ReadTable(actualDoc.Tables[1]);

                string message = string.Empty;
                Assert.IsTrue(CompareArrays(expectedArray, actualArray, ref message), message);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
            finally
            {
                DisposeApplication(expectedDoc);
                DisposeApplication(actualDoc);
            }
        }

        [TestMethod]
        public void TestWordToTable()
        {
            string textFilePath = string.Format("{0}{1}",
                Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName,
                @"\Resources\test_word.docx");

            var expectedTable = CommonFunc.GetTestingTable(new DateTime(2017, 2, 18));
            var actualTable = (new WordHandler()).Load(textFilePath);

            string message = string.Empty;
            Assert.IsTrue(CommonFunc.CompareTables(expectedTable, actualTable, ref message), message);
        }

        private static bool CompareArrays(string[][] a, string[][] b, ref string message)
        {
            if (a.Length != b.Length)
                return false;

            for (int i = 0; i < a.Length; ++i)
            {
                if (a[i].Length != b[i].Length)
                    return false;

                for (int j = 0; j < a[i].Length; ++j)
                {
                    if (a[i][j] != b[i][j])
                    {
                        string diffMessage = string.Format("Row:【{0}】\r\nColumn:【{1}】\r\nExpected:【{2}】\r\nActual:【{3}】\r\n",
                          i,
                          j,
                          a[i][j] ?? "null",
                          b[i][j] ?? "null");
                        throw new Exception(diffMessage);
                    }
                }
            }
            return true;
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
        }

        private static string GetCellText(Cell cell)
        {
            return Regex.Replace(cell.Range.Text, @"[\x00-\x1F\x7F]", "").Trim();
        }

        private static string[][] ReadTable(Table table)
        {
            string[][] array = new string[table.Rows.Count][];

            for (int i = 0; i < array.Length; ++i)
            {
                string[] innerArray = new string[table.Rows[i + 1].Cells.Count];

                for (int j = 0; j < innerArray.Length; ++j)
                {
                    innerArray[j] = GetCellText(table.Cell(i + 1, j + 1));
                }
                array[i] = innerArray;
            }
            return array;
        }

        private static void ReplaceSaveDate(Document document, DateTime date)
        {
            Cell cell = document.Tables[1].Cell(3, 2);

            cell.Range.Text = Regex.Replace(GetCellText(cell), @"更新日期：\d{4}-\d{2}-\d{2}", m =>
            {
                return string.Format("更新日期：{0}", date.ToString("yyyy-MM-dd"));
            });
        }
    }
}