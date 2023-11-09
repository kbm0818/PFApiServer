using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace TableConvertor
{
    [Value]
    public class TDateTime : IType
    {
        public eTypeCode TypeCode { get { return eTypeCode.Basic; } }
        public string Name { get { return "DateTime"; } }
        public IType TKey { get { throw new System.NotImplementedException(); } }
        public IType TValue { get { throw new System.NotImplementedException(); } }
        public Type? Type => typeof(System.DateTime);

        public bool Write(BinaryWriter writer, string val)
        {
            DateTime result;
            if (false == DateTime.TryParse(val, out result))
                return false;

            writer.Write(result.ToString());
            return true;
        }
    }
}
