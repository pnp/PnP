using System;
using System.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Governance.AddInSecurity
{
    /// <summary>
    /// Utility class that support certificate based encryption/decryption
    /// </summary>
    public static class EncryptionUtility
    {

        //static byte[] aditionalEntropy = { 1, 7, 0, 5, 15 };       
        

        public static byte[] GetBytes(string str)
        {
            byte[] bytes = new byte[str.Length * sizeof(char)];
            System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }
        public static string EncryptStringWithDPAPI(System.Security.SecureString input, byte[] aditionalEntropy, string scope)
        {     
            byte[] encryptedData = null;
            if (scope == "LocalMachine")
                encryptedData = System.Security.Cryptography.ProtectedData.Protect(
                    System.Text.Encoding.Unicode.GetBytes(ToInsecureString(input)), aditionalEntropy,
                    System.Security.Cryptography.DataProtectionScope.LocalMachine);
            else
                encryptedData = System.Security.Cryptography.ProtectedData.Protect(
                System.Text.Encoding.Unicode.GetBytes(ToInsecureString(input)), aditionalEntropy,
                System.Security.Cryptography.DataProtectionScope.CurrentUser);

            return Convert.ToBase64String(encryptedData);
        }

        /// <summary>
        /// Decrypts a DPAPI encryped string
        /// </summary>
        /// <param name="encryptedData">Encrypted string</param>
        /// <returns>Decrypted (SecureString)string</returns>
        public static SecureString DecryptStringWithDPAPI(string encryptedData, byte[] aditionalEntropy, string scope)
        {
            try
            {
                byte[] decryptedData= null;
                if (scope == "LocalMachine")
                    decryptedData = System.Security.Cryptography.ProtectedData.Unprotect(
                        Convert.FromBase64String(encryptedData),
                        aditionalEntropy,
                        System.Security.Cryptography.DataProtectionScope.LocalMachine);
                else
                    decryptedData = System.Security.Cryptography.ProtectedData.Unprotect(
                    Convert.FromBase64String(encryptedData),
                    aditionalEntropy,
                    System.Security.Cryptography.DataProtectionScope.CurrentUser);

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
            SecureString secure = new SecureString();

            if (!string.IsNullOrEmpty(input))
            {
                //throw new ArgumentException("Input string is empty and cannot be made into a SecureString", "input");

                
                foreach (char c in input)
                {
                    secure.AppendChar(c);
                }
                secure.MakeReadOnly();
            }
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
