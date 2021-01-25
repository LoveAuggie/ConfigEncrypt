using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ConfigEncrypt.Core
{
    public class DesHelper
    {
        private const string S_KEY = "JQKJDES*";

        #region DES 加密解密
        /// <summary>
        /// DES加密算法
        /// </summary>
        /// <param name="encryptString">要加密的字符串</param>
        /// <param name="sKey">加密码Key</param>
        /// <returns>正确返回加密后的结果，错误返回源字符串</returns>
        public static string ToDESEncrypt(string encryptString, string Key = "")
        {
            try
            {
                if (string.IsNullOrEmpty(Key)) Key = S_KEY;
                if (string.IsNullOrEmpty(encryptString))
                    return "";

                Key = Key.ToMD5().Substring(0, 8);

                byte[] keyBytes = Encoding.UTF8.GetBytes(Key);
                byte[] keyIV = keyBytes;
                byte[] inputByteArray = Encoding.UTF8.GetBytes(encryptString);
                DESCryptoServiceProvider desProvider = new DESCryptoServiceProvider();
                // java 默认的是ECB模式，PKCS5padding；c#默认的CBC模式，PKCS7padding 所以这里我们默认使用ECB方式
                desProvider.Mode = CipherMode.ECB;
                MemoryStream memStream = new MemoryStream();
                CryptoStream crypStream = new CryptoStream(memStream, desProvider.CreateEncryptor(keyBytes, keyIV), CryptoStreamMode.Write);
                crypStream.Write(inputByteArray, 0, inputByteArray.Length);
                crypStream.FlushFinalBlock();
                var msstr = memStream.ToArray();
                memStream.Close();
                crypStream.Close();
                desProvider.Clear();
                return Convert.ToBase64String(msstr);
            }
            catch (Exception ex)
            {
                return encryptString;
            }
        }

        /// <summary>
        /// DES 解密
        /// </summary>
        /// <param name="decryptString"></param>
        /// <param name="Key"></param>
        /// <returns></returns>
        public static string ToDESDecrypt(string decryptString, string Key = "")
        {
            try
            {
                if (string.IsNullOrEmpty(Key)) Key = S_KEY;
                if (string.IsNullOrEmpty(decryptString))
                    return "";
                // 实际使用的Key，为Md5之后的数据
                Key = Key.ToMD5().Substring(0, 8);

                byte[] keyBytes = Encoding.UTF8.GetBytes(Key);
                byte[] keyIV = keyBytes;
                byte[] inputByteArray = Convert.FromBase64String(decryptString);
                DESCryptoServiceProvider desProvider = new DESCryptoServiceProvider();
                // java 默认的是ECB模式，PKCS5padding；c#默认的CBC模式，PKCS7padding 所以这里我们默认使用ECB方式
                desProvider.Mode = CipherMode.ECB;
                MemoryStream memStream = new MemoryStream();
                CryptoStream crypStream = new CryptoStream(memStream, desProvider.CreateDecryptor(keyBytes, keyIV), CryptoStreamMode.Write);
                crypStream.Write(inputByteArray, 0, inputByteArray.Length);
                crypStream.FlushFinalBlock();
                var msstr = memStream.ToArray();
                memStream.Close();
                crypStream.Close();
                desProvider.Clear();
                return Encoding.UTF8.GetString(msstr);
            }
            catch
            {
                return decryptString;
            }
        }
        #endregion
    }
}
