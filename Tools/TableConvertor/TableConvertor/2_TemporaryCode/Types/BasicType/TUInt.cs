using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace TableConvertor
{
    [Value]
    class TUInt : IType
    {
        public eTypeCode TypeCode { get { return eTypeCode.Basic; } }
        public string Name { get { return "uint"; } }
        public IType TKey { get { throw new System.NotImplementedException(); } }
        public IType TValue { get { throw new System.NotImplementedException(); } }
        public Type? Type => typeof(System.UInt32);

        public bool Write(BinaryWriter writer, string val)
        {
            uint result;
            if (false == uint.TryParse(val, out result))
                return false;

            writer.Write(result);
            return true;
        }
    }
}
