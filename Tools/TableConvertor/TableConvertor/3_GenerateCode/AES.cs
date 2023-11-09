using System.Text;
using System.Security.Cryptography;
using System.IO;
using System;

namespace TableConvertor
{ /// <summary>
  /// AES 256비트 암호화 처리
  /// </summary>
    public static class AES
    {
        public static readonly byte[] s_solt = Encoding.UTF8.GetBytes("12E76CA4CEDA4B29");

        /// <summary>
        /// input 데이터를 AES 암호화 처리
        /// </summary>
        /// <param name="key">암호화 키</param>
        /// <param name="plain">원본 데이터</param>
        /// <returns>AES 암호화 처리된 데이터</returns>
        public static byte[] AESEncrypt256(string key, byte[] plain)
        {
            byte[] output;

            using (var secretKey = new PasswordDeriveBytes(key, s_solt))
            using (var aes = Aes.Create())
            {
                aes.Key = secretKey.GetBytes(32);
                aes.IV = secretKey.GetBytes(16);

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using (var ms = new MemoryStream())
                using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                {
                    cs.Write(plain, 0, plain.Length);
                    cs.FlushFinalBlock();

                    output = ms.ToArray();
                }
            }

            return output;
        }

        /// <summary>
        /// 암호화된 input 데이터를 복호화한다.
        /// </summary>
        /// <param name="key">복호화 키</param>
        /// <param name="cipher">복호화가 필요한 데이터</param>
        /// <returns>AES 복호화 처리된 데이터</returns>
        public static byte[] AESDecrypt256(string key, byte[] cipher)
        {
            byte[] output;

            using (var secretKey = new PasswordDeriveBytes(key, s_solt))
            using (var aes = Aes.Create())
            {
                aes.Key = secretKey.GetBytes(32);
                aes.IV = secretKey.GetBytes(16);

                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                using (var ms = new MemoryStream())
                using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Write))
                {
                    cs.Write(cipher, 0, cipher.Length);
                    cs.FlushFinalBlock();

                    output = ms.ToArray();
                }

            }

            return output;
        }



        ///// <summary>
        ///// 스트링을 UTF8 바이트로 변환해 암호화한다.
        ///// </summary>
        ///// <param name="key">암호화 키</param>
        ///// <param name="input">대상 문자열</param>
        ///// <returns>암호화 처리된 데이터</returns>
        //public static byte[] AESEncrypt(string key, string input)
        //{
        //    return AESEncrypt256(key, UTF8Encoding.UTF8.GetBytes(input));
        //}


        ///// <summary>
        ///// 암호화된 input 데이터를 복고화해서 UTF8 스트링으로 전달한다.
        ///// </summary>
        ///// <param name="key">복호화 키</param>
        ///// <param name="input">복호화가 필요한 데이터</param>
        ///// <returns>AES 복호화 처리된 데이터</returns>
        //public static string AESDecrypt(string key, byte[] input)
        //{
        //    return UTF8Encoding.UTF8.GetString(AESDecrypt256(key, input));
        //}



        /// <summary>
        /// 암호화 한다.
        /// </summary>
        /// <param name="cryptoKey">암호화 키</param>
        /// <param name="plain">원본 문자열</param>
        /// <returns>암호화된 문자열</returns>
        public static string Encrypt(string cryptoKey, string plain)
        {
            return Convert.ToBase64String(AES.AESEncrypt256(cryptoKey, UTF8Encoding.UTF8.GetBytes(plain)));
        }


        /// <summary>
        /// 복호화 한다.
        /// </summary>
        /// <param name="cryptoKey">암호화 키</param>
        /// <param name="cipher">암호화된 문자열</param>
        /// <returns>복호화된 문자열</returns>
        public static string Decrypt(string cryptoKey, string cipher)
        {
            return UTF8Encoding.UTF8.GetString(AES.AESDecrypt256(cryptoKey, Convert.FromBase64String(cipher)));
        }
    }
}