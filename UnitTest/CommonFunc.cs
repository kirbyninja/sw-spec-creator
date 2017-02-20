using SpecCreator.DataStrcutures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTest
{
    internal static class CommonFunc
    {
        private enum InputState { BothNull, BothValid, }

        public static bool CompareTables(WorkingTable a, WorkingTable b, ref string message)
        {
            try
            {
                switch (CheckInput(a, b))
                {
                    case InputState.BothNull:
                        return true;

                    case InputState.BothValid:
                        return ArePropertiesEqual(a, b)
                            && AreSequentialEqual(a.WorkingColumns, b.WorkingColumns, CompareColumns);

                    default:
                        return false;
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return false;
            }
        }

        public static WorkingTable GetTestingTable(DateTime date)
        {
            var table = new WorkingTable()
            {
                Name = "測試資料表",
                TableName = "test_table",
                Author = "Tester",
                Date = date,
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
                    new WorkingColumn("opt1", "測試選項1", "SMALLINT") { Option = new Option(2, "測試選項1", new[] {
                            new OptionItem(1, "1.買進"),
                            new OptionItem(2, "2.賣出") }) },
                    new WorkingColumn("opt2", "測試選項2", "SMALLINT") { Option = new Option(8, "測試選項2", new[] {
                            new OptionItem(1, "1.市價"),
                            new OptionItem(2, "2.限價") }) },
                    new WorkingColumn("int1", "測試整數1", "INT"),
                    new WorkingColumn("numeric1", "測試浮點數1", "NUMERIC") { Length = "20,5" },
                });

            for (int i = 0; i < table.WorkingColumns.Count(); ++i)
                table.WorkingColumns.ElementAt(i).ColumnNo = i + 1;

            return table;
        }

        private static bool ArePropertiesEqual<T>(T a, T b)
        {
            return typeof(T).GetProperties().Where(p => p.PropertyType.IsValueType || p.PropertyType == typeof(string)).All(p =>
               {
                   object propA = p.GetValue(a);
                   object propB = p.GetValue(b);

                   if (Equals(propA, propB))
                       return true;
                   else
                   {
                       string diffMessage = string.Format("【{0}】-【{1}】\r\nExpected:【{2}】\r\nActual: 【{3}】\r\n",
                          typeof(T).Name,
                          p.Name,
                          propA ?? "null",
                          propB ?? "null");
                       throw new Exception(diffMessage);
                   }
               });
        }

        private static bool AreSequentialEqual<T>(IEnumerable<T> a, IEnumerable<T> b, Func<T, T, bool> compareFunc)
        {
            if (a.Count() != b.Count())
            {
                string diffMessage = string.Format("【{0}】\r\nExpected Length:【{1}】\r\nActual Length: 【{2}】\r\n",
                    typeof(T).Name,
                    a.Count(),
                    b.Count());
                throw new Exception(diffMessage);
            }
            else
            {
                bool isEqual = true;
                for (int i = 0; i < a.Count(); ++i)
                    isEqual &= compareFunc(a.ElementAt(i), b.ElementAt(i));
                return isEqual;
            }
        }

        private static InputState CheckInput<T>(T a, T b)
        {
            if (a == null && b == null)
                return InputState.BothNull;
            else if (a == null ^ b == null)
            {
                string diffMessage = string.Format("【{0}】\r\nExpected:【{1}】\r\nActual: 【{2}】\r\n",
                       typeof(T).Name,
                       a == null ? "null" : "not null",
                       b == null ? "null" : "not null");
                throw new Exception(diffMessage);
            }
            else
                return InputState.BothValid;
        }

        private static bool CompareColumns(WorkingColumn a, WorkingColumn b)
        {
            switch (CheckInput(a, b))
            {
                case InputState.BothNull:
                    return true;

                case InputState.BothValid:
                    return ArePropertiesEqual(a, b)
                        && CompareOptions(a.Option, b.Option);

                default:
                    return false;
            }
        }

        private static bool CompareOptionItems(OptionItem a, OptionItem b)
        {
            switch (CheckInput(a, b))
            {
                case InputState.BothNull:
                    return true;

                case InputState.BothValid:
                    return ArePropertiesEqual(a, b);

                default:
                    return false;
            }
        }

        private static bool CompareOptions(Option a, Option b)
        {
            switch (CheckInput(a, b))
            {
                case InputState.BothNull:
                    return true;

                case InputState.BothValid:
                    return ArePropertiesEqual(a, b)
                        && AreSequentialEqual(a.Items, b.Items, CompareOptionItems);

                default:
                    return false;
            }
        }
    }
}