using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace TableConvertor
{
    [Value]
    class TULong : IType
    {
        public eTypeCode TypeCode { get { return eTypeCode.Basic; } }
        public string Name { get { return "ulong"; } }
        public IType TKey { get { throw new System.NotImplementedException(); } }
        public IType TValue { get { throw new System.NotImplementedException(); } }
        public Type? Type => typeof(System.UInt64);

        public bool Write(BinaryWriter writer, string val)
        {
            ulong result;
            if (false == ulong.TryParse(val, out result))
                return false;

            writer.Write(result);
            return true;
        }
    }
}
