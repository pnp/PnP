using System.Security.Cryptography;

namespace SharePointPnP.IdentityModel.Extensions.S2S.Tokens
{
    internal sealed class RSACryptoServiceProviderProxy : System.IDisposable
    {
        private const int PROV_RSA_AES = 24;

        private bool _disposed;

        private bool _disposeRsa;

        private System.Security.Cryptography.RSACryptoServiceProvider _rsa;

        public RSACryptoServiceProviderProxy(System.Security.Cryptography.RSACryptoServiceProvider rsa)
        {
            Utility.VerifyNonNullArgument("rsa", rsa);
            if (rsa.CspKeyContainerInfo.ProviderType != 24)
            {
                System.Security.Cryptography.CspParameters cspParameters = new System.Security.Cryptography.CspParameters();
                cspParameters.ProviderType = 24;
                cspParameters.KeyContainerName = rsa.CspKeyContainerInfo.KeyContainerName;
                cspParameters.KeyNumber = (int)rsa.CspKeyContainerInfo.KeyNumber;
                if (rsa.CspKeyContainerInfo.MachineKeyStore)
                {
                    cspParameters.Flags = System.Security.Cryptography.CspProviderFlags.UseMachineKeyStore;
                }
                cspParameters.Flags |= System.Security.Cryptography.CspProviderFlags.UseExistingKey;
                this._rsa = new System.Security.Cryptography.RSACryptoServiceProvider(cspParameters);
                this._disposeRsa = true;
                return;
            }
            this._rsa = rsa;
        }

        private void Dispose(bool disposing)
        {
            if (!this._disposed)
            {
                if (disposing && this._disposeRsa && this._rsa != null)
                {
                    this._rsa.Dispose();
                    this._rsa = null;
                }
                this._disposed = true;
            }
        }

        public byte[] SignData(byte[] signingInput, object hash)
        {
            return this._rsa.SignData(signingInput, hash);
        }

        public bool VerifyData(byte[] signingInput, object hash, byte[] signature)
        {
            return this._rsa.VerifyData(signingInput, hash, signature);
        }

        public void Dispose()
        {
            this.Dispose(true);
            System.GC.SuppressFinalize(this);
        }
    }
}