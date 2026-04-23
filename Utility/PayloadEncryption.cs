using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace OTC.Api.Utility
{
    public static class PayloadEncryption
    {
        // 16 bytes key and IV for simplicity (128-bit AES)
        private const string KeyString = "OTC_SECURE_PAY_K"; 
        private const string IVString = "OTC_SECURE_PAY_I";

        public static string Decrypt(string cipherText)
        {
            if (string.IsNullOrEmpty(cipherText))
                return cipherText;

            byte[] key = Encoding.UTF8.GetBytes(KeyString);
            byte[] iv = Encoding.UTF8.GetBytes(IVString);

            using (Aes aes = Aes.Create())
            {
                aes.Key = key;
                aes.IV = iv;
                aes.Padding = PaddingMode.PKCS7;
                aes.Mode = CipherMode.CBC;

                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                using (MemoryStream msDecrypt = new MemoryStream(Convert.FromBase64String(cipherText)))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            return srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
        }
    }
}
