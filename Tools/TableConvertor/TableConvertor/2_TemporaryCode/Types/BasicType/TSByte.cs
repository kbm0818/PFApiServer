using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace TableConvertor
{
    [Value]
    class TSByte : IType
    {
        public eTypeCode TypeCode { get { return eTypeCode.Basic; } }
        public string Name { get { return "sbyte"; } }
        public IType TKey { get { throw new System.NotImplementedException(); } }
        public IType TValue { get { throw new System.NotImplementedException(); } }
        public Type? Type => typeof(System.SByte);

        public bool Write(BinaryWriter writer, string val)
        {
            sbyte result;
            if (false == sbyte.TryParse(val, out result))
                return false;

            writer.Write(result);
            return true;
        }
    }
}
