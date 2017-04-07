using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpecCreator.Converting
{
    public class ConvertResult
    {
        public ConvertResult(string fileName, TaskResult taskResult, Exception exception)
        {
            this.FileName = fileName;
            this.TaskResult = taskResult;
            this.Exception = exception;
        }

        public ConvertResult(string fileName, TaskResult taskResult)
            : this(fileName, taskResult, null)
        { }

        public ConvertResult(string fileName)
            : this(fileName, TaskResult.Failure, null)
        { }

        public Exception Exception { get; private set; }

        public string ExceptionMessage
        {
            get
            {
                return GetExceptionMessage(Exception);
            }
        }

        public string FileName { get; private set; }

        public TaskResult TaskResult { get; private set; }

        private static string GetExceptionMessage(Exception exception)
        {
            if (exception == null)
                return string.Empty;
            else if (exception.InnerException == null)
                return exception.Message;
            else
                return string.Concat(exception.Message, "\r\n ↳", GetExceptionMessage(exception.InnerException));
        }
    }
}