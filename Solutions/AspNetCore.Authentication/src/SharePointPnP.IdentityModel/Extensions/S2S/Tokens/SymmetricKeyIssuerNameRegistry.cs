using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Security.Tokens;
using System.Threading.Tasks;

namespace SharePointPnP.IdentityModel.Extensions.S2S.Tokens
{
    public class SymmetricKeyIssuerNameRegistry : Microsoft.IdentityModel.Tokens.IssuerNameRegistry
    {
        private System.Collections.Generic.Dictionary<string, string> _issuerList = new System.Collections.Generic.Dictionary<string, string>();

        public void AddTrustedIssuer(byte[] symmetricKey, string issuerName)
        {
            Utility.VerifyNonNullArgument("symmetricKey", symmetricKey);
            Utility.VerifyNonNullOrEmptyStringArgument("issuerName", issuerName);
            this._issuerList.Add(System.Convert.ToBase64String(symmetricKey), issuerName);
        }

        public override string GetIssuerName(System.IdentityModel.Tokens.SecurityToken securityToken)
        {
            Utility.VerifyNonNullArgument("securityToken", securityToken);
            string result = null;
            BinarySecretSecurityToken binarySecretSecurityToken = securityToken as BinarySecretSecurityToken;
            if (binarySecretSecurityToken != null)
            {
                this._issuerList.TryGetValue(System.Convert.ToBase64String(binarySecretSecurityToken.GetKeyBytes()), out result);
            }
            return result;
        }
    }
}
