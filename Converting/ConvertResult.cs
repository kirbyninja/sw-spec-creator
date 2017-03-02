using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpecCreator.Converting
{
    public struct ConvertResult
    {
        public string FileName;
        public TaskResult TaskResult;
        public string Message;

        public ConvertResult(string fileName, TaskResult taskResult, string message)
        {
            this.FileName = fileName;
            this.TaskResult = taskResult;
            this.Message = message;
        }

        public ConvertResult(string fileName)
            : this(fileName, TaskResult.Failure, string.Empty)
        { }
    }
}