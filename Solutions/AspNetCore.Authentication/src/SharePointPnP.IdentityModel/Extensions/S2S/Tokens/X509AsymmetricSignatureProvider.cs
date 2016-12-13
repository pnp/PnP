using System.Security.Cryptography;

namespace SharePointPnP.IdentityModel.Extensions.S2S.Tokens
{
    internal class X509AsymmetricSignatureProvider : SignatureProvider
    {
        private bool _disposed;

        private RSACryptoServiceProviderProxy _rsaProxy;

        private string _hashAlgorithm;

        public X509AsymmetricSignatureProvider(System.IdentityModel.Tokens.X509AsymmetricSecurityKey x509Key)
        {
            Utility.VerifyNonNullArgument("x509Key", x509Key);
            System.Security.Cryptography.RSACryptoServiceProvider rSACryptoServiceProvider = x509Key.GetAsymmetricAlgorithm("http://www.w3.org/2001/04/xmldsig-more#rsa-sha256", true) as System.Security.Cryptography.RSACryptoServiceProvider;
            if (rSACryptoServiceProvider == null)
            {
                throw new System.InvalidOperationException("Could not get algorithm from X509AsymmetricSecurityKey");
            }
            this.Initialize(rSACryptoServiceProvider);
        }

        public X509AsymmetricSignatureProvider(System.Security.Cryptography.RSACryptoServiceProvider rsa)
        {
            this.Initialize(rsa);
        }

        protected override void Dispose(bool disposing)
        {
            if (!this._disposed)
            {
                if (disposing)
                {
                    if (this._hashAlgorithm != null)
                    {
                        this._hashAlgorithm = null;
                    }
                    if (this._rsaProxy != null)
                    {
                        this._rsaProxy.Dispose();
                        this._rsaProxy = null;
                    }
                }
                this._disposed = true;
            }
        }

        private void Initialize(System.Security.Cryptography.RSACryptoServiceProvider rsa)
        {
            if (Utility.RequiresFipsCompliance)
            {
                System.Security.Cryptography.CryptoConfig.AddOID("2.16.840.1.101.3.4.2.1", new string[]
                {
                    "SHA256CSP"
                });
                System.Security.Cryptography.CryptoConfig.AddAlgorithm(typeof(SHA256CryptoServiceProvider), new string[]
                {
                    "SHA256CSP"
                });
                this._hashAlgorithm = "SHA256CSP";
            }
            else
            {
                this._hashAlgorithm = "SHA256";
            }
            this._rsaProxy = new RSACryptoServiceProviderProxy(rsa);
        }

        public override byte[] Sign(byte[] signingInput)
        {
            Utility.VerifyNonNullArgument("signingInput", signingInput);
            return this._rsaProxy.SignData(signingInput, this._hashAlgorithm);
        }

        public override bool Verify(byte[] signingInput, byte[] signature)
        {
            Utility.VerifyNonNullArgument("signingInput", signingInput);
            Utility.VerifyNonNullArgument("signature", signature);
            return this._rsaProxy.VerifyData(signingInput, this._hashAlgorithm, signature);
        }
    }
}