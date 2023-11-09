using K4os.Compression.LZ4;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace TableConvertor
{
    internal class BytesGenerator : IGenerator
    {
        public void Generate(string fileName, params Sheet[] sheets)
        {
            foreach(var sheet in sheets)
            {
                string filename = string.Format(Const.BYTES_FULL_FILE_NAME, sheet.Name);
               
                using (MemoryStream mstream = new MemoryStream())
                {
                    using (BinaryWriter mwriter = new BinaryWriter(mstream))
                    {
                        sheet.ByteWrite(mwriter);
                        mstream.Flush();
                    }

                    using (FileStream fstream = new FileStream(filename, FileMode.Create))
                    {
                        using (BinaryWriter fwriter = new BinaryWriter(fstream))
                        {
                            var buffer = mstream.GetBuffer();
                            var compressBuffer = LZ4Pickler.Pickle(buffer, LZ4Level.L00_FAST);
                            var encryptBuffer = AES.AESEncrypt256(Const.FILE_CRYPTO_KEY, compressBuffer);

                            fwriter.Write(encryptBuffer);
                            fstream.Flush();
                        }
                    }
                }
            }

            return;
        }
    }
}
