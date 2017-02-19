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
        [TestMethod]
        public void TestWordToTable()
        {
            string textFilePath = string.Format("{0}{1}",
                Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName,
                @"\Resources\test_word.docx");

            var expectedTable = CommonFunc.GetTestingTable(new DateTime(2017, 2, 18));
            var actualTable = (new WordHandler()).Load(textFilePath);

            string diffMessage = string.Empty;
            Assert.IsTrue(CommonFunc.CompareTables(expectedTable, actualTable, ref diffMessage), diffMessage);
        }
    }
}