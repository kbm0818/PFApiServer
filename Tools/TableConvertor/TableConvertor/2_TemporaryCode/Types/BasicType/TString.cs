using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace TableConvertor
{
    [Value]
    public class TString : IType
    {
        public eTypeCode TypeCode { get { return eTypeCode.Basic; } }
        public string Name { get { return "string"; } }
        public IType TKey { get { throw new System.NotImplementedException(); } }
        public IType TValue { get { throw new System.NotImplementedException(); } }
        public Type? Type => typeof(System.String);

        public bool Write(BinaryWriter writer, string val)
        {
            // Unity3D UTF - 8 Use
            System.Text.Encoding utf8 = System.Text.Encoding.UTF8;

            byte[] utf8Bytes = utf8.GetBytes(val);
            writer.Write((UInt32)utf8Bytes.Length);
            writer.Write(utf8Bytes, 0, utf8Bytes.Length);
            return true;
        }
    }
}
