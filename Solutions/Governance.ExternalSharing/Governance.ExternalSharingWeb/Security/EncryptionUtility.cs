using System;
using System.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Contoso.Office365.common
{
    /// <summary>
    /// Utility class that support certificate based encryption/decryption
    /// </summary>
    public static class EncryptionUtility
    {

        //static byte[] aditionalEntropy = { 1, 7, 0, 5, 15 };

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

            byte[] encoded = Encoding.UTF8.GetBytes(stringToEncrypt);
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

            decryptedString = Encoding.UTF8.GetString(decrypted);

            return decryptedString;
        }

        /// <summary>
        /// Encrypts a string using the machine's DPAPI
        /// </summary>
        /// <param name="input">String (SecureString) to encrypt</param>
        /// <returns>Encrypted string</returns>
        public static string EncryptStringWithDPAPI(System.Security.SecureString input)
        {
            byte[] encryptedData = System.Security.Cryptography.ProtectedData.Protect(
                System.Text.Encoding.Unicode.GetBytes(ToInsecureString(input)), null,
                System.Security.Cryptography.DataProtectionScope.LocalMachine);
            return Convert.ToBase64String(encryptedData);
        }

        /// <summary>
        /// Decrypts a DPAPI encryped string
        /// </summary>
        /// <param name="encryptedData">Encrypted string</param>
        /// <returns>Decrypted (SecureString)string</returns>
        public static SecureString DecryptStringWithDPAPI(string encryptedData)
        {
            try
            {
                byte[] decryptedData = System.Security.Cryptography.ProtectedData.Unprotect(
                    Convert.FromBase64String(encryptedData),
                    null,
                    System.Security.Cryptography.DataProtectionScope.LocalMachine);
                return ToSecureString(System.Text.Encoding.Unicode.GetString(decryptedData));
            }
            catch
            {
                return new SecureString();
            }
        }

        /// <summary>
        /// Converts a string to a SecureString
        /// </summary>
        /// <param name="input">String to convert</param>
        /// <returns>SecureString representation of the passed in string</returns>
        public static SecureString ToSecureString(string input)
        {
            if (string.IsNullOrEmpty(input))
                throw new ArgumentException("Input string is empty and cannot be made into a SecureString", "input");

            SecureString secure = new SecureString();
            foreach (char c in input)
            {
                secure.AppendChar(c);
            }
            secure.MakeReadOnly();
            return secure;
        }

        /// <summary>
        /// Converts a SecureString to a "regular" string
        /// </summary>
        /// <param name="input">SecureString to convert</param>
        /// <returns>A "regular" string representation of the passed SecureString</returns>
        public static string ToInsecureString(SecureString input)
        {
            string returnValue = string.Empty;
            IntPtr ptr = System.Runtime.InteropServices.Marshal.SecureStringToBSTR(input);
            try
            {
                returnValue = System.Runtime.InteropServices.Marshal.PtrToStringBSTR(ptr);
            }
            finally
            {
                System.Runtime.InteropServices.Marshal.ZeroFreeBSTR(ptr);
            }
            return returnValue;
        }


    }
}
