using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TableConvertor
{
    public enum eTypeCode : byte
    {
        Basic = 0,
        List,
        Map,
        Enum,
        Range,
    }

    public interface IType
    {
        eTypeCode TypeCode { get; }
        string Name { get; }
        IType TKey { get; }
        IType TValue { get; }
        Type? Type { get; }

        bool Write(BinaryWriter stream, string valueString);
    }
}
