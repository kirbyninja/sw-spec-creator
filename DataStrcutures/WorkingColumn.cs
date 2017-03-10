using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpecCreator.DataStrcutures
{
    public class WorkingColumn
    {
        private string dataType;

        public WorkingColumn()
        { }

        public WorkingColumn(string columnName, string caption, string dataType)
            : this()
        {
            this.Caption = caption;
            this.ColumnName = columnName;
            this.DataType = dataType;
        }

        public string Caption { get; set; }
        public string ColumnName { get; set; }
        public int ColumnNo { get; set; }
        public string DataType { get { return dataType; } set { dataType = value.ToUpper() == "TINYINT" ? "SMALLINT" : value.ToUpper(); } }
        public bool IsPrimaryKey { get; set; }
        public bool IsUnique { get; set; }
        public string Length { get; set; }
        public Option Option { get; set; }
        public WorkingTable WorkingTable { get; set; }
    }
}