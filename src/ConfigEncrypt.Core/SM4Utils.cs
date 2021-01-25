using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Org.BouncyCastle.Utilities.Encoders;
namespace ConfigEncrypt.Core
{
    public class SM4Utils
    {
        private const String secretKey = "BF6F8DBEF3244741";
        private const String iv = "BBF9BA86FA6EDBD8";
        public static bool hexString = false;

        public static String Encrypt_ECB(String plainText, string key ="")
        {
            if (string.IsNullOrEmpty(key)) key = secretKey;
            if (string.IsNullOrEmpty(plainText)) return "";
            key = key.ToMD5().Substring(0, 16);

            SM4_Context ctx = new SM4_Context();
            ctx.isPadding = true;
            ctx.mode = SM4.SM4_ENCRYPT;

            byte[] keyBytes;
            if (hexString)
            {
                keyBytes = Hex.Decode(key);
            }
            else
            {
                keyBytes = Encoding.Default.GetBytes(key);
            }

            SM4 sm4 = new SM4();
            sm4.sm4_setkey_enc(ctx, keyBytes);
            byte[] encrypted = sm4.sm4_crypt_ecb(ctx, Encoding.Default.GetBytes(plainText));

            String cipherText = Encoding.Default.GetString(Hex.Encode(encrypted));
            return cipherText;
        }
        public static String Decrypt_ECB(String cipherText, string key = "")
        {
            if (string.IsNullOrEmpty(key)) key = secretKey;
            if (string.IsNullOrEmpty(cipherText)) return "";
            key = key.ToMD5().Substring(0, 16);

            SM4_Context ctx = new SM4_Context();
            ctx.isPadding = true;
            ctx.mode = SM4.SM4_DECRYPT;

            byte[] keyBytes;
            if (hexString)
            {
                keyBytes = Hex.Decode(key);
            }
            else
            {
                keyBytes = Encoding.Default.GetBytes(key);
            }

            SM4 sm4 = new SM4();
            sm4.sm4_setkey_dec(ctx, keyBytes);
            byte[] decrypted = sm4.sm4_crypt_ecb(ctx, Hex.Decode(cipherText));
            return Encoding.Default.GetString(decrypted);
        }

        public static String Encrypt_CBC(String plainText, string key)
        {
            if (string.IsNullOrEmpty(key)) key = secretKey;
            if (string.IsNullOrEmpty(plainText)) return "";

            SM4_Context ctx = new SM4_Context();
            ctx.isPadding = true;
            ctx.mode = SM4.SM4_ENCRYPT;

            byte[] keyBytes;
            byte[] ivBytes;
            if (hexString)
            {
                keyBytes = Hex.Decode(key);
                ivBytes = Hex.Decode(iv);
            }
            else
            {
                keyBytes = Encoding.Default.GetBytes(key);
                ivBytes = Encoding.Default.GetBytes(iv);
            }

            SM4 sm4 = new SM4();
            sm4.sm4_setkey_enc(ctx, keyBytes);
            byte[] encrypted = sm4.sm4_crypt_cbc(ctx, ivBytes, Encoding.Default.GetBytes(plainText));

            String cipherText = Encoding.Default.GetString(Hex.Encode(encrypted));
            return cipherText;
        }
        public static String Decrypt_CBC(String cipherText, string key ="")
        {
            if (string.IsNullOrEmpty(key)) key = secretKey;
            if (string.IsNullOrEmpty(cipherText)) return "";

            SM4_Context ctx = new SM4_Context();
            ctx.isPadding = true;
            ctx.mode = SM4.SM4_DECRYPT;

            byte[] keyBytes;
            byte[] ivBytes;
            if (hexString)
            {
                keyBytes = Hex.Decode(key);
                ivBytes = Hex.Decode(iv);
            }
            else
            {
                keyBytes = Encoding.Default.GetBytes(key);
                ivBytes = Encoding.Default.GetBytes(iv);
            }

            SM4 sm4 = new SM4();
            sm4.sm4_setkey_dec(ctx, keyBytes);
            byte[] decrypted = sm4.sm4_crypt_cbc(ctx, ivBytes, Hex.Decode(cipherText));
            return Encoding.Default.GetString(decrypted);
        }
    }
}
