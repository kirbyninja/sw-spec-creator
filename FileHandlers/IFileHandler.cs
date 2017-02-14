using SpecCreator.DataStrcutures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpecCreator.FileHandlers
{
    public interface IFileHandler
    {
        string Description { get; }
        string Extension { get; }

        WorkingTable Load(string fileName);

        void Save(WorkingTable table, string fileName);
    }

    public interface IFileHandler<T> : IFileHandler
    {
        new T Load(string fileName);

        void Save(T meta, string fileName);

        T ConvertToMeta(WorkingTable table);

        WorkingTable ConvertToTable(T meta);
    }
}