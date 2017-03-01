using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpecCreator.Converting
{
    public struct ConvertInfo
    {
        public bool IsByFolder;
        public FileType SourceType;
        public FileType TargetType;

        public ConvertInfo(FileType sourceType, FileType targetType, bool isByFolder)
        {
            SourceType = sourceType;
            TargetType = targetType;
            IsByFolder = isByFolder;
        }
    }
}