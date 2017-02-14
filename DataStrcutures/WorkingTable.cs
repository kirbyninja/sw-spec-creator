using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpecCreator.DataStrcutures
{
    public class WorkingTable
    {
        private readonly List<WorkingColumn> workingColumns = new List<WorkingColumn>();

        public string Author { get; set; }
        public DateTime Date { get; set; }
        public string Name { get; set; }
        public string TableName { get; set; }
        public IEnumerable<WorkingColumn> WorkingColumns { get { return workingColumns.AsReadOnly(); } }

        public void AddColumn(WorkingColumn column)
        {
            column.WorkingTable = this;
            workingColumns.Add(column);
        }

        public void AddColumn(string columnName, string caption, string dataType)
        {
            AddColumn(new WorkingColumn(columnName, caption, dataType));
        }

        public void AddColumns(IEnumerable<WorkingColumn> columns)
        {
            foreach (var column in columns)
                AddColumn(column);
        }

        public override bool Equals(object obj)
        {
            if (obj is WorkingTable)
            {
                var o = obj as WorkingTable;
                return this.GetType().GetProperties().Where(p => p.PropertyType.IsValueType).All(p => p.GetValue(this).Equals(p.GetValue(o)))
                    && this.WorkingColumns.SequenceEqual(o.WorkingColumns);
            }
            else
                return false;
        }

        public override int GetHashCode()
        {
            return (TableName ?? string.Empty).GetHashCode();
        }
    }
}