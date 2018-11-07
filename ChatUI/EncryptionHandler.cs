using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace ChatUI
{

    public class EncryptionHandler
    {
        // The password for all encryption/decryption in the instance of this class.
        protected string instanceKey;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:ChatUI.EncryptionHandler"/> class.
        /// </summary>
        /// <param name="key">Encryption key.</param>
        public EncryptionHandler()
        {

        }
        public void SetKey(string newKey)
        {
            instanceKey = newKey;
        }

        public string Encrypt(string plainMessage)
        {
            string encryptedMessage = null;
            byte[][] keys = GetHashKeys(instanceKey);

            try
            {
                encryptedMessage = EncryptStringToBytes_Aes(plainMessage, keys[0], keys[1]);
            }
            catch (CryptographicException) { }
            catch (ArgumentNullException) { }

            return encryptedMessage;
        }

        public string Decrypt(string secretMessage)
        {
            string decryptedMessage = "";
            byte[][] hashKeys = GetHashKeys(instanceKey);

            try
            {
                decryptedMessage = DecryptStringFromBytes_Aes(secretMessage, hashKeys[0], hashKeys[1]);
            }
            catch (CryptographicException) { }
            catch (ArgumentNullException) { }

            return decryptedMessage;
        }

        // Source: https://docs.microsoft.com/en-us/dotnet/api/system.security.cryptography.aes?view=netframework-4.7.2

        /// <summary>
        /// Gets the hash keys for the cipher password
        /// </summary>
        /// <returns>The hash keys.</returns>
        /// <param name="cipherPassword">Cipher password.</param>
        private static byte[][] GetHashKeys(string cipherPassword)
        {
            byte[][] result = new byte[2][];
            Encoding enc = Encoding.UTF8;

            SHA256 sha2 = new SHA256CryptoServiceProvider();

            byte[] rawKey = enc.GetBytes(cipherPassword);
            byte[] rawIV = enc.GetBytes(cipherPassword);
            byte[] hashKey = sha2.ComputeHash(rawKey);
            byte[] hashIV = sha2.ComputeHash(rawIV);

            Array.Resize(ref hashIV, 16);

            result[0] = hashKey;
            result[1] = hashIV;

            return result;
        }

        private static string EncryptStringToBytes_Aes(string plainText, byte[] Key, byte[] IV)
        {
            if (plainText == null || plainText.Length <= 0)
                throw new ArgumentNullException("plainText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");

            byte[] encrypted;

            using (AesManaged aesAlg = new AesManaged())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt =
                            new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(plainText);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }
            return Convert.ToBase64String(encrypted);
        }

        private static string DecryptStringFromBytes_Aes(string secretMessage, byte[] Key, byte[] IV)
        {
            Console.WriteLine("Intercepted Message: {0}", secretMessage);
            byte[] cipherText = Convert.FromBase64String(secretMessage);

            if (cipherText == null || cipherText.Length <= 0)
                throw new ArgumentNullException("cipherText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");

            string decryptedMessage = null;

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msDecrypt = new MemoryStream(cipherText))
                {
                    using (CryptoStream csDecrypt =
                            new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            decryptedMessage = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
            return decryptedMessage;
        }
    }
}
