using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace TableConvertor
{
    [Value]
    public class TByte : IType
    {
        public eTypeCode TypeCode { get { return eTypeCode.Basic; } }
        public string Name { get { return "byte"; } }
        public IType TKey { get { throw new System.NotImplementedException(); } }
        public IType TValue { get { throw new System.NotImplementedException(); } }
        public Type? Type => typeof(System.Byte);

        public bool Write(BinaryWriter writer, string val)
        {
            byte result;
            if( false == byte.TryParse(val, out result) )
                return false;

            writer.Write(result);
            return true;
        }
    }
}
