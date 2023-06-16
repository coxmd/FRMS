using System;
using System.Security.Cryptography;
using System.Text;

namespace FarmRecordManagementSystem.Utilities
{
    public class ConnectionStringEncryptor
    {
        private static readonly byte[] Key = Encoding.UTF8.GetBytes("S3cureP@ssw0rdStr0ngK3yPhr@se123"); // Replace with your secret key
        private static readonly byte[] IV = Encoding.UTF8.GetBytes("IV1234567890abcd"); // Replace with your initialization vector

        public static string EncryptConnectionString(string connectionString)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = Key;
                aes.IV = IV;

                byte[] encryptedBytes;
                using (var encryptor = aes.CreateEncryptor())
                {
                    byte[] originalBytes = Encoding.UTF8.GetBytes(connectionString);
                    encryptedBytes = encryptor.TransformFinalBlock(originalBytes, 0, originalBytes.Length);
                }

                string encryptedConnectionString = Convert.ToBase64String(encryptedBytes);
                return encryptedConnectionString;
            }
        }

        public static string DecryptConnectionString(string encryptedConnectionString)
        {
            byte[] encryptedBytes = Convert.FromBase64String(encryptedConnectionString);

            using (Aes aes = Aes.Create())
            {
                aes.Key = Key;
                aes.IV = IV;

                byte[] decryptedBytes;
                using (var decryptor = aes.CreateDecryptor())
                {
                    decryptedBytes = decryptor.TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length);
                }

                string decryptedConnectionString = Encoding.UTF8.GetString(decryptedBytes);
                return decryptedConnectionString;
            }
        }
    }
}
