using System;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace Contoso.Office365.common
{
    /// <summary>
    /// Supporting class for certificate based operations
    /// </summary>
    public static class X509CertificateUtility
    {
        /// <summary>
        /// Loads a certificate from a given certificate store
        /// </summary>
        /// <param name="storeName">Name of the certificate store</param>
        /// <param name="storeLocation">Location of the certificate store</param>
        /// <param name="thumbprint">Thumbprint of the certificate to load</param>
        /// <returns>An <see cref="X509Certificate2"/> certificate</returns>
        public static X509Certificate2 LoadCertificate(StoreName storeName, StoreLocation storeLocation, string thumbprint)
        {
            // The following code gets the cert from the keystore
            X509Store store = new X509Store(storeName, storeLocation);
            store.Open(OpenFlags.ReadOnly);

            X509Certificate2Collection certCollection =
                    store.Certificates.Find(X509FindType.FindByThumbprint,
                    thumbprint, false);

            X509Certificate2Enumerator enumerator = certCollection.GetEnumerator();

            X509Certificate2 cert = null;

            while (enumerator.MoveNext())
            {
                cert = enumerator.Current;
            }

            return cert;
        }

        /// <summary>
        /// Encrypts data based on the RSACryptoServiceProvider
        /// </summary>
        /// <param name="plainData">Bytes to encrypt</param>
        /// <param name="fOAEP"> true to perform direct System.Security.Cryptography.RSA decryption using OAEP padding</param>
        /// <param name="certificate">Certificate to use</param>
        /// <returns>Encrypted bytes</returns>
        public static byte[] Encrypt(byte[] plainData, bool fOAEP, X509Certificate2 certificate)
        {
            if (plainData == null)
            {
                throw new ArgumentNullException("plainData");
            }

            if (certificate == null)
            {
                throw new ArgumentNullException("certificate");
            }

            using (RSACryptoServiceProvider provider = new RSACryptoServiceProvider())
            {
                provider.FromXmlString(GetPublicKey(certificate));

                // We use the publickey to encrypt.
                return provider.Encrypt(plainData, fOAEP);
            }
        }

        /// <summary>
        /// Decrypts data based on the RSACryptoServiceProvider
        /// </summary>
        /// <param name="encryptedData">Bytes to decrypt</param>
        /// <param name="fOAEP"> true to perform direct System.Security.Cryptography.RSA decryption using OAEP padding</param>
        /// <param name="certificate">Certificate to use</param>
        /// <returns>Decrypted bytes</returns>
        public static byte[] Decrypt(byte[] encryptedData, bool fOAEP, X509Certificate2 certificate)
        {
            if (encryptedData == null)
            {
                throw new ArgumentNullException("encryptedData");
            }

            if (certificate == null)
            {
                throw new ArgumentNullException("certificate");
            }

            using (RSACryptoServiceProvider provider = (RSACryptoServiceProvider)certificate.PrivateKey)
            {
                // We use the private key to decrypt.
                return provider.Decrypt(encryptedData, fOAEP);
            }
        }

        /// <summary>
        /// Returns the certificate public key
        /// </summary>
        /// <param name="certificate">Certificate to operate on</param>
        /// <returns>Public key of the certificate</returns>
        public static string GetPublicKey(X509Certificate2 certificate)
        {
            if (certificate == null)
            {
                throw new ArgumentNullException("certificate");
            }

            return certificate.PublicKey.Key.ToXmlString(false);
        }

    }
}
