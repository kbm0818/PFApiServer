using K4os.Compression.LZ4;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace TableConvertor
{
    internal class VersionGenerator : IGenerator
    {
        public void Generate(string fileName, params Sheet[] sheets)
        {
            var files = Directory.GetFiles(GenerateOption.FullOutputPath);
            var bytesFiles = files.Where(s => Path.GetExtension(s).Equals(".bytes") == true && s.Equals(Const.VERSION_FULL_FILE_NAME) == false);

            using (MemoryStream mstream = new MemoryStream())
            {
                using (BinaryWriter mwriter = new BinaryWriter(mstream))
                {
                    foreach (var bytesFile in bytesFiles)
                    {
                        var ascii = File.ReadAllBytes(bytesFile);
                        var hash = MD5.Create().ComputeHash(ascii);

                        mwriter.Write(Path.GetFileNameWithoutExtension(bytesFile));
                        mwriter.Write(hash);
                    }

                    mstream.Flush();
                }

                using (FileStream fstream = new FileStream(Const.VERSION_FULL_FILE_NAME, FileMode.Create))
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
            return;
        }
    }
}
