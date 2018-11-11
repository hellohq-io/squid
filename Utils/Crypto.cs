using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Q.Squid.Utils
{
    public static class Crypto
    {
        public static string EncryptString(string text, string key)
        {
            if (text == null || text.Length <= 0)
                throw new ArgumentNullException("text is null");
            byte[] encrypted;

            var keyBytes = Encoding.UTF8.GetBytes(key);

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = keyBytes;
                var iv = aesAlg.IV;

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            // write iv
                            msEncrypt.Write(iv, 0, iv.Length);
                            //Write all data to the stream.
                            swEncrypt.Write(text);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }

            return Convert.ToBase64String(encrypted);
        }

        public static string DecryptString(string cipherText, string key)
        {
            var fullCipher = Convert.FromBase64String(cipherText);

            if (fullCipher == null || fullCipher.Length <= 0)
                throw new ArgumentNullException("cipherText is null");

            string plaintext = null;
            var keyBytes = Encoding.UTF8.GetBytes(key);

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = keyBytes;

                using (MemoryStream msDecrypt = new MemoryStream(fullCipher))
                {
                    byte[] iv = new byte[16];
                    msDecrypt.Read(iv, 0, 16);
                    aesAlg.IV = iv;

                    ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }

            return plaintext;
        }
    }
}