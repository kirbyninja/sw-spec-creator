using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpecCreator.Converting
{
    public enum FileType
    {
        Sql,
        Word,
    }

    public enum TaskResult
    {
        Canceled,
        Failure,
        Success,
    }
}