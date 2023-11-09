using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace TableConvertor
{
    [Container]
    public class TList<_TValue> : IType
        where _TValue : IType, new()
    {
        static IType _Tv = new _TValue();
        static string typeName = string.Format("List<{0}>", _Tv.Name);

        public eTypeCode TypeCode { get { return eTypeCode.List; } }
        public string Name { get { return typeName; } }
        public IType TKey { get { throw new System.NotImplementedException("List can't use key."); } }
        public IType TValue { get { return _Tv; } }
        public Type? Type => null;

        public bool Write(BinaryWriter writer, string val)
        {
            int length = 0;
            string[] strValues = val.Split(',');
            if (val == "")
            {
                writer.Write((int)0);
                return true;
            }
            else
            {
                length = (int)strValues.Length;
                writer.Write((int)strValues.Length);
            }

            for (int i = 0; i < length; ++i)
            {
                string strValue = strValues[i];
                if (false == _Tv.Write(writer, strValue))
                    return false;
            }
            return true;
        }
    }
}
