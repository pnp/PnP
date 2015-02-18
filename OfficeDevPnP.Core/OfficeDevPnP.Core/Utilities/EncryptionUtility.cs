using System;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace OfficeDevPnP.Core.Utilities
{
    /// <summary>
    /// Utility class that support certificate based encryption/decryption
    /// </summary>
    public static class EncryptionUtility
    {

        /// <summary>
        /// Encrypt a piece of text based on a given certificate
        /// </summary>
        /// <param name="stringToEncrypt">Text to encrypt</param>
        /// <param name="thumbPrint">Thumbprint of the certificate to use</param>
        /// <returns>Encrypted text</returns>
        public static string Encrypt(string stringToEncrypt, string thumbPrint)
        {
            string encryptedString = string.Empty;

            X509Certificate2 certificate = X509CertificateUtility.LoadCertificate(StoreName.My, StoreLocation.LocalMachine, thumbPrint);

            if (certificate == null)
            {
                return string.Empty;
            }

            byte[] encoded = UTF8Encoding.UTF8.GetBytes(stringToEncrypt);
            byte[] encrypted;

            try
            {
                encrypted = X509CertificateUtility.Encrypt(encoded, true, certificate);
            }
            catch (Exception)
            {
                return string.Empty;
            }

            encryptedString = Convert.ToBase64String(encrypted);

            return encryptedString;
        }

        /// <summary>
        /// Decrypt a piece of text based on a given certificate
        /// </summary>
        /// <param name="stringToDecrypt">Text to decrypt</param>
        /// <param name="thumbPrint">Thumbprint of the certificate to use</param>
        /// <returns>Decrypted text</returns>
        public static string Decrypt(string stringToDecrypt, string thumbPrint)
        {
            string decryptedString = string.Empty;

            X509Certificate2 certificate = X509CertificateUtility.LoadCertificate(StoreName.My, StoreLocation.LocalMachine, thumbPrint);

            if (certificate == null)
            {
                return string.Empty;
            }

            byte[] encrypted;
            byte[] decrypted;
            encrypted = Convert.FromBase64String(stringToDecrypt);

            try
            {
                decrypted = X509CertificateUtility.Decrypt(encrypted, true, certificate);
            }
            catch (Exception)
            {
                return string.Empty;
            }

            decryptedString = UTF8Encoding.UTF8.GetString(decrypted);

            return decryptedString;
        }

    }
}
