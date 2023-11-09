using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace TableConvertor
{
    [Value]
    public class TFloat : IType
    {
        public eTypeCode TypeCode { get { return eTypeCode.Basic; } }
        public string Name { get { return "float"; } }
        public IType TKey { get { throw new System.NotImplementedException(); } }
        public IType TValue { get { throw new System.NotImplementedException(); } }
        public Type? Type => typeof(System.Single);

        public bool Write(BinaryWriter writer, string val)
        {
            float result;
            if (false == float.TryParse(val, out result))
                return false;

            writer.Write(result);
            return true;
        }
    }
}
