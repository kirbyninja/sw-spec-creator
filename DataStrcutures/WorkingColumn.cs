using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpecCreator.DataStrcutures
{
    public class WorkingColumn
    {
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
        public string DataType { get; set; }
        public bool IsPrimaryKey { get; set; }
        public bool IsUnique { get; set; }
        public string Length { get; set; }
        public Option Option { get; set; }
        public WorkingTable WorkingTable { get; set; }

        public override bool Equals(object obj)
        {
            if (obj is WorkingColumn)
            {
                var o = obj as WorkingColumn;
                return this.GetType().GetProperties().Where(p => p.PropertyType.IsValueType).All(p => p.GetValue(this).Equals(p.GetValue(o)))
                    && ((this.Option == null && o.Option == null) || this.Option.Equals(o.Option))
                    && ((this.WorkingTable == null && o.WorkingTable == null) || this.WorkingTable.TableName == o.WorkingTable.TableName);
            }
            else
                return false;
        }

        public override int GetHashCode()
        {
            if (WorkingTable == null)
                return ColumnName.GetHashCode();
            else
                return (WorkingTable.TableName ?? string.Empty).GetHashCode() ^ ColumnName.GetHashCode();
        }
    }
}