namespace SharePointPnP.IdentityModel.Extensions.S2S.Tokens
{
    internal abstract class SignatureProvider : System.IDisposable
    {
        public static SignatureProvider Create(System.IdentityModel.Tokens.SigningCredentials signingCredentials)
        {
            Utility.VerifyNonNullArgument("signingCredentials", signingCredentials);
            if (System.StringComparer.Ordinal.Compare(signingCredentials.DigestAlgorithm, "http://www.w3.org/2001/04/xmlenc#sha256") != 0)
            {
                throw new System.ArgumentException("signingCredentials.DigestAlgorithm must be SHA-256");
            }
            System.IdentityModel.Tokens.X509AsymmetricSecurityKey x509AsymmetricSecurityKey = signingCredentials.SigningKey as System.IdentityModel.Tokens.X509AsymmetricSecurityKey;
            if (x509AsymmetricSecurityKey != null)
            {
                return new X509AsymmetricSignatureProvider(x509AsymmetricSecurityKey);
            }
            System.IdentityModel.Tokens.SymmetricSecurityKey symmetricSecurityKey = signingCredentials.SigningKey as System.IdentityModel.Tokens.SymmetricSecurityKey;
            if (symmetricSecurityKey != null)
            {
                return new SymmetricSignatureProvider(symmetricSecurityKey);
            }
            throw new System.ArgumentException("signingCredentials.SigningKey must be either X509AsymmetricSecurityKey or SymmetricSecurityKey");
        }

        protected virtual void Dispose(bool disposing)
        {
        }

        public abstract byte[] Sign(byte[] signingInput);

        public abstract bool Verify(byte[] signingInput, byte[] signature);

        public void Dispose()
        {
            this.Dispose(true);
            System.GC.SuppressFinalize(this);
        }
    }
}