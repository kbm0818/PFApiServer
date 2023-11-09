using System.Data;

namespace TableConvertor
{
    internal interface IReader
    {
        void Read(string path, out List<DataTable> table);
    }
}
