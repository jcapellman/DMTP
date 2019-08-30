using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace DMTP.lib.Security
{
    public class AES
    {
        private const string SALT = "DMTPISAWESOMENO";
        private const int ITERATIONS = 128;

        private const int KEYSIZE = 256;

        private static readonly NLog.Logger Log = NLog.LogManager.GetCurrentClassLogger();

        private static Rfc2898DeriveBytes GetKeyBytes(string keyString) => new Rfc2898DeriveBytes(keyString, Encoding.ASCII.GetBytes(SALT), ITERATIONS);

        public static string EncryptString(string data, string keyString)
        {
            try
            {
                var dataBytes = Encoding.UTF8.GetBytes(data);

                using (var keyBytes = GetKeyBytes(keyString))
                {
                    using (var aes = Aes.Create())
                    {
                        if (aes == null)
                        {
                            throw new Exception("Could not create AES object");
                        }

                        aes.Padding = PaddingMode.Zeros;
                        aes.KeySize = KEYSIZE;
                        aes.Key = keyBytes.GetBytes(aes.KeySize / 8);
                        aes.IV = keyBytes.GetBytes(aes.BlockSize / 8);

                        using (var encryptionTransform = aes.CreateEncryptor())
                        {
                            using (var memoryStream = new MemoryStream())
                            {
                                using (var cryptoStream = new CryptoStream(memoryStream, encryptionTransform, CryptoStreamMode.Write))
                                {
                                    cryptoStream.Write(dataBytes, 0, data.Length);
                                    cryptoStream.FlushFinalBlock();
                                    cryptoStream.Close();
                                }

                                return Convert.ToBase64String(memoryStream.ToArray());
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to encrypt {data} due to {ex}");

                return null;
            }
        }

        public static string DecryptString(string data, string keyString)
        {
            var encryptedBytes = Convert.FromBase64String(data);

            using (var keyBytes = GetKeyBytes(keyString))
            {
                using (var aes = Aes.Create())
                {
                    if (aes == null)
                    {
                        throw new Exception("Could not create AES object");
                    }

                    aes.KeySize = KEYSIZE;
                    aes.Padding = PaddingMode.Zeros;
                    aes.Key = keyBytes.GetBytes(aes.KeySize / 8);
                    aes.IV = keyBytes.GetBytes(aes.BlockSize / 8);

                    using (var decryption = aes.CreateDecryptor())
                    {
                        using (var memoryStream = new MemoryStream(encryptedBytes))
                        {
                            using (var cryptoStream = new CryptoStream(memoryStream, decryption, CryptoStreamMode.Read))
                            {
                                using (var srDecrypt = new StreamReader(cryptoStream))
                                {
                                    return srDecrypt.ReadToEnd().TrimEnd('\0');
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}