using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace TableConvertor
{
    [Value]
    public class TBool : IType
    {
        public eTypeCode TypeCode { get { return eTypeCode.Basic; } }
        public string Name { get { return "bool"; } }
        public IType TKey { get { throw new System.NotImplementedException(); } }
        public IType TValue { get { throw new System.NotImplementedException(); } }

        public Type? Type => typeof(System.Boolean);

        public bool Write(BinaryWriter writer, string val)
        {
            bool result;
            if( false == bool.TryParse(val, out result) )
                return false;

            writer.Write(result);
            return true;
        }
    }
}
