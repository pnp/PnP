using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SharePointPnP.IdentityModel.Extensions.S2S.Tokens
{
    internal class SymmetricSignatureProvider : SignatureProvider
    {
        private bool _disposed;

        private System.Security.Cryptography.HMACSHA256 _hash;

        public SymmetricSignatureProvider(System.IdentityModel.Tokens.SymmetricSecurityKey symmetricKey)
        {
            Utility.VerifyNonNullArgument("symmetricKey", symmetricKey);
            this._hash = new System.Security.Cryptography.HMACSHA256(symmetricKey.GetSymmetricKey());
        }

        protected override void Dispose(bool disposing)
        {
            if (!this._disposed)
            {
                if (disposing && this._hash != null)
                {
                    this._hash.Dispose();
                    this._hash = null;
                }
                this._disposed = true;
            }
        }

        public override byte[] Sign(byte[] signingInput)
        {
            Utility.VerifyNonNullArgument("signingInput", signingInput);
            return this._hash.ComputeHash(signingInput);
        }

        public override bool Verify(byte[] signingInput, byte[] signature)
        {
            Utility.VerifyNonNullArgument("signingInput", signingInput);
            Utility.VerifyNonNullArgument("signature", signature);
            byte[] b = this._hash.ComputeHash(signingInput);
            return this.AreEqual(signature, b);
        }

        private bool AreEqual(byte[] a, byte[] b)
        {
            if (a == null || b == null)
            {
                return a == null && null == b;
            }
            if (object.ReferenceEquals(a, b))
            {
                return true;
            }
            if (a.Length != b.Length)
            {
                return false;
            }
            for (int i = 0; i < a.Length; i++)
            {
                if (a[i] != b[i])
                {
                    return false;
                }
            }
            return true;
        }
    }
}
