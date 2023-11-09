using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace TableConvertor
{
    [Value]
    public class TLong : IType
    {
        public eTypeCode TypeCode { get { return eTypeCode.Basic; } }
        public string Name { get { return "long"; } }
        public IType TKey { get { throw new System.NotImplementedException(); } }
        public IType TValue { get { throw new System.NotImplementedException(); } }
        public Type? Type => typeof(System.Int64);

        public bool Write(BinaryWriter writer, string val)
        {
            long result;
            if (false == long.TryParse(val, out result))
                return false;

            writer.Write(result);
            return true;
        }
    }
}
