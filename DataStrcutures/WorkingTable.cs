using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpecCreator.DataStrcutures
{
    public class WorkingTable
    {
        public string Author { get; set; }
        public DateTime Date { get; set; }
        public string Name { get; set; }
        public string TableName { get; set; }
        public IEnumerable<WorkingColumn> WorkingColumns { get; set; }
    }
}