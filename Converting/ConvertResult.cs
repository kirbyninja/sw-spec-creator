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
        public bool IsSuccessful;
        public string Message;

        public ConvertResult(string fileName, bool isSuccessful, string message)
        {
            this.FileName = fileName;
            this.IsSuccessful = isSuccessful;
            this.Message = message;
        }

        public ConvertResult(string fileName)
            : this(fileName, false, string.Empty)
        { }
    }
}