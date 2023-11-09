using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace TableConvertor
{
    [Container]
    public class TMap<_TKey, _TValue> : IType
        where _TKey : IType, new()
        where _TValue : IType, new()
    {
        static _TKey _Tk = new _TKey();
        static _TValue _Tv = new _TValue();
        static string typeName = string.Format("map<{0},{1}>", _Tk.Name, _Tv.Name);

        public eTypeCode TypeCode { get { return eTypeCode.Map; } }
        public string Name { get { return typeName; } }
        public IType TKey { get { return _Tk; } }
        public IType TValue { get { return _Tv; } }
        public Type? Type => Type.GetType(Name);

        public bool Write(BinaryWriter writer, string val)
        {
            string[] mapEntities = val.Split(',');
            writer.Write(mapEntities.Length);

            for (int i = 0; i < mapEntities.Length; ++i)
            {
                if (false == _Tv.Write(writer, mapEntities[i]))
                    return false;
            }
            return true;
        }
    }
}
