using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace TrueWays.Core.Utilities
{
    public static class DecryptHelper
    {
        public static string Sha512(string text)
        {
            var sha512 = new SHA512CryptoServiceProvider();
            var bytesSha512In = Encoding.Default.GetBytes(text);
            var bytesSha512Out = sha512.ComputeHash(bytesSha512In);
            var strSha512Out = BitConverter.ToString(bytesSha512Out).Replace("-", "").ToLower();
            return strSha512Out;
        }

        public static string SaltAndHash(string rawString, string salt)
        {
            var salted = string.Concat(rawString, salt);
            return Sha512(salted).ToUpper();
        }


        public static string Md5(string text)
        {
            var md5 = new MD5CryptoServiceProvider();

            var data = Encoding.Default.GetBytes(text);
            var md5Data = md5.ComputeHash(data);

            return BitConverter.ToString(md5Data).Replace("-", "").ToLower();
        }

        public static string Hmacsha1(this string encryptText, string encryptKey)
        {
            var myHmacsha1 = new HMACSHA1(Encoding.Default.GetBytes(encryptKey));
            var rstRes = myHmacsha1.ComputeHash(Encoding.Default.GetBytes(encryptText));

            var enText = new StringBuilder();
            foreach (var Byte in rstRes)
            {
                enText.AppendFormat("{0:x2}", Byte);
            }
            return Convert.ToBase64String(myHmacsha1.Hash);
        }

        /// <summary>
        /// 对称密钥解密
        /// </summary>
        /// <typeparam name="TAlgorithm"></typeparam>
        /// <param name="str"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string Decrypt<TAlgorithm>(this string str, string key)
            where TAlgorithm : SymmetricAlgorithm
        {
            return str.Decrypt<TAlgorithm, UTF8Encoding>(key);
        }

        /// <summary>
        /// 对称密钥解密
        /// </summary>
        /// <typeparam name="TAlgorithm">对称加密算法</typeparam>
        /// <typeparam name="TStringEncoding">字符编码</typeparam>
        /// <param name="str"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string Decrypt<TAlgorithm, TStringEncoding>(this string str, string key)
            where TAlgorithm : SymmetricAlgorithm
            where TStringEncoding : Encoding
        {
            var s = new MemoryStream(Convert.FromBase64String(str));
            var decryptedStream = s.Decrypt<TAlgorithm>(key.ToByteArray());
            var bytes = decryptedStream.ToByteArray();
            return Activator.CreateInstance<TStringEncoding>().GetString(bytes);
        }

        public static Stream Decrypt<TAlgorithm>(this Stream stream, byte[] key)
            where TAlgorithm : SymmetricAlgorithm
        {
            var alg = Activator.CreateInstance<TAlgorithm>();
            var pdb = new PasswordDeriveBytes(key, null);
            alg.Key = pdb.GetBytes(alg.KeySize/8);
            alg.GenerateIV();
            alg.IV = pdb.GetBytes(alg.IV.Length);

            var encryptor = alg.CreateDecryptor();
            return new CryptoStream(stream, encryptor, CryptoStreamMode.Read);
        }

        public static string TripleDesc(string key, string str)
        {
            var inputByteArray = Encoding.Default.GetBytes(str);

            var crypto = new TripleDESCryptoServiceProvider();

            var pdb = new PasswordDeriveBytes(key, null);

            crypto.Key = pdb.GetBytes(crypto.KeySize / 8);
            crypto.GenerateIV();
            crypto.IV = pdb.GetBytes(crypto.IV.Length);

            crypto.CreateEncryptor();

            var ms = new MemoryStream();
            var cs = new CryptoStream(ms, crypto.CreateEncryptor(), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();
            return Convert.ToBase64String(ms.ToArray());
        }

        public static byte[] ToByteArray(this Stream stream)
        {
            var buffer = new byte[1000];
            var readLength = 1;
            using (var ms = new MemoryStream())
            {
                while (readLength > 0)
                {
                    readLength = stream.Read(buffer, 0, buffer.Length);
                    if (readLength > 0)
                        ms.Write(buffer, 0, readLength);
                }
                return ms.ToArray();
            }
        }

        public static byte[] ToByteArray(this string str)
        {
            return str.ToByteArray<UTF8Encoding>();
        }

        /// <summary>
        /// 字符串转换为字节数组
        /// </summary>
        /// <typeparam name="TEncoding">编码</typeparam>
        /// <param name="str"></param>
        /// <returns></returns>
        public static byte[] ToByteArray<TEncoding>(this string str) where TEncoding : Encoding
        {
            Encoding enc = Activator.CreateInstance<TEncoding>();
            return enc.GetBytes(str);
        }

        /// <summary>
        /// 不可逆加密
        /// </summary>
        /// <typeparam name="TAlgorithm">加密HASH算法</typeparam>
        /// <param name="str">字符编码</param>
        /// <returns></returns>
        public static string EncryptOneWay<TAlgorithm>(this string str)
            where TAlgorithm : HashAlgorithm
        {
            return str.EncryptOneWay<TAlgorithm, UTF8Encoding>();
        }

        /// <summary>
        /// 不可逆加密
        /// </summary>
        /// <typeparam name="TAlgorithm">加密HASH算法</typeparam>
        /// <typeparam name="TStringEncoding">字符编码</typeparam>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string EncryptOneWay<TAlgorithm, TStringEncoding>(this string str)
            where TAlgorithm : HashAlgorithm
            where TStringEncoding : Encoding
        {
            Encoding enco = Activator.CreateInstance<TStringEncoding>();
            var inputBye = enco.GetBytes(str);
            var bytes = Activator.CreateInstance<TAlgorithm>().ComputeHash(inputBye);
            return System.BitConverter.ToString(bytes).Replace("-", ""); ;
        }
    }
}
