using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SharePointPnP.IdentityModel.Extensions.S2S.Tokens
{
    public class SymmetricIssuerKeyIdentifierClause : System.IdentityModel.Tokens.SecurityKeyIdentifierClause
    {
        private const string SymmetricIssuerClauseType = "SymmetricIssuer";

        private string _issuer;

        public string Issuer
        {
            get
            {
                return this._issuer;
            }
        }

        public SymmetricIssuerKeyIdentifierClause(string issuer) : base("SymmetricIssuer")
        {
            Utility.VerifyNonNullOrEmptyStringArgument("issuer", issuer);
            this._issuer = issuer;
        }
    }
}
