using Microsoft.IdentityModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Security.Tokens;
using System.Threading.Tasks;

namespace SharePointPnP.IdentityModel.Extensions.S2S.Tokens
{
    public class JsonWebSecurityToken : System.IdentityModel.Tokens.SecurityToken
    {
        private JsonWebSecurityToken _actorToken;

        private string _audience;

        private System.Collections.Generic.List<JsonWebTokenClaim> _claims;

        private string _id;

        private string _issuer;

        private System.IdentityModel.Tokens.SecurityToken _issuerToken;

        private System.IdentityModel.Tokens.SigningCredentials _signingCredentials;

        private string _sourceData;

        private System.DateTime _validFrom;

        private System.DateTime _validTo;

        public JsonWebSecurityToken ActorToken
        {
            get
            {
                return this._actorToken;
            }
        }

        public string Audience
        {
            get
            {
                return this._audience;
            }
        }

        public virtual bool CanWriteSourceData
        {
            get
            {
                return !string.IsNullOrEmpty(this._sourceData);
            }
        }

        public System.Collections.ObjectModel.ReadOnlyCollection<JsonWebTokenClaim> Claims
        {
            get
            {
                return new System.Collections.ObjectModel.ReadOnlyCollection<JsonWebTokenClaim>(this._claims);
            }
        }

        public override string Id
        {
            get
            {
                return this._id;
            }
        }

        public string Issuer
        {
            get
            {
                return this._issuer;
            }
        }

        public System.IdentityModel.Tokens.SecurityToken IssuerToken
        {
            get
            {
                return this._issuerToken;
            }
        }

        public override System.Collections.ObjectModel.ReadOnlyCollection<System.IdentityModel.Tokens.SecurityKey> SecurityKeys
        {
            get
            {
                return new System.Collections.ObjectModel.ReadOnlyCollection<System.IdentityModel.Tokens.SecurityKey>(new System.Collections.Generic.List<System.IdentityModel.Tokens.SecurityKey>());
            }
        }

        public System.IdentityModel.Tokens.SigningCredentials SigningCredentials
        {
            get
            {
                return this._signingCredentials;
            }
        }

        public override System.DateTime ValidFrom
        {
            get
            {
                return this._validFrom;
            }
        }

        public override System.DateTime ValidTo
        {
            get
            {
                return this._validTo;
            }
        }

        public JsonWebSecurityToken(string issuer, string audience, System.DateTime validFrom, System.DateTime validTo, System.Collections.Generic.IEnumerable<JsonWebTokenClaim> claims, System.IdentityModel.Tokens.SigningCredentials signingCredentials) : this(issuer, audience, validFrom, validTo, claims)
        {
            Utility.VerifyNonNullArgument("signingCredentials", signingCredentials);
            this._signingCredentials = signingCredentials;
        }

        public JsonWebSecurityToken(string issuer, string audience, System.DateTime validFrom, System.DateTime validTo, System.Collections.Generic.IEnumerable<JsonWebTokenClaim> claims, System.IdentityModel.Tokens.SecurityToken issuerToken, JsonWebSecurityToken actorToken) : this(issuer, audience, validFrom, validTo, claims)
        {
            this._issuerToken = issuerToken;
            this._actorToken = actorToken;
        }

        public JsonWebSecurityToken(string issuer, string audience, System.DateTime validFrom, System.DateTime validTo, System.Collections.Generic.IEnumerable<JsonWebTokenClaim> claims)
        {
            Utility.VerifyNonNullOrEmptyStringArgument("issuer", issuer);
            Utility.VerifyNonNullOrEmptyStringArgument("audience", audience);
            Utility.VerifyNonNullArgument("claims", claims);
            this._id = UniqueId.CreateUniqueId();
            this._issuer = issuer;
            this._audience = audience;
            this._validFrom = DateTimeUtil.ToUniversalTime(validFrom);
            this._validTo = DateTimeUtil.ToUniversalTime(validTo);
            this._claims = new System.Collections.Generic.List<JsonWebTokenClaim>(claims);
        }

        public virtual void CaptureSourceData(string tokenString)
        {
            Utility.VerifyNonNullOrEmptyStringArgument("tokenString", tokenString);
            this._sourceData = tokenString;
        }

        internal void SetId(string id)
        {
            this._id = id;
        }

        public override string ToString()
        {
            System.Collections.Generic.IDictionary<string, string> self = this.CreateHeaderClaims();
            System.Collections.Generic.IDictionary<string, string> self2 = this.CreatePayloadClaims();
            return string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0}.{1}", new object[]
            {
                self.EncodeToJson(),
                self2.EncodeToJson()
            });
        }

        public virtual string WriteSourceData()
        {
            if (!this.CanWriteSourceData)
            {
                throw new System.InvalidOperationException("This token's raw data cannot be re-emitted. The token was not deserialized in the first place.");
            }
            return this._sourceData;
        }

        public virtual System.Collections.Generic.IDictionary<string, string> CreatePayloadClaims()
        {
            System.Collections.Generic.Dictionary<string, string> dictionary = new System.Collections.Generic.Dictionary<string, string>(System.StringComparer.Ordinal);
            dictionary.Add("aud", this.Audience);
            dictionary.Add("iss", this.Issuer);
            dictionary.Add("nbf", this.GetTimeInSeconds(this.ValidFrom));
            dictionary.Add("exp", this.GetTimeInSeconds(this.ValidTo));
            foreach (JsonWebTokenClaim current in this.Claims)
            {
                dictionary.Add(current.ClaimType, current.Value);
            }
            return dictionary;
        }

        public virtual System.Collections.Generic.IDictionary<string, string> CreateHeaderClaims()
        {
            System.Collections.Generic.Dictionary<string, string> dictionary = new System.Collections.Generic.Dictionary<string, string>(System.StringComparer.Ordinal);
            dictionary.Add("typ", "JWT");
            if (this.SigningCredentials != null)
            {
                if (System.StringComparer.Ordinal.Compare(this.SigningCredentials.SignatureAlgorithm, "http://www.w3.org/2001/04/xmldsig-more#rsa-sha256") == 0)
                {
                    Microsoft.IdentityModel.SecurityTokenService.X509SigningCredentials x509SigningCredentials = this.SigningCredentials as Microsoft.IdentityModel.SecurityTokenService.X509SigningCredentials;
                    if (x509SigningCredentials == null)
                    {
                        throw new System.InvalidOperationException("JWT token is not valid. RSA signature requires X509SigningCredentials");
                    }
                    dictionary.Add("alg", "RS256");
                    dictionary.Add("x5t", Base64UrlEncoder.Encode(x509SigningCredentials.Certificate.GetCertHash()));
                }
                else if (System.StringComparer.Ordinal.Compare(this.SigningCredentials.SignatureAlgorithm, "http://www.w3.org/2001/04/xmldsig-more#hmac-sha256") == 0)
                {
                    dictionary.Add("alg", "HS256");
                }
            }
            else if (this.IssuerToken != null)
            {
                System.IdentityModel.Tokens.X509SecurityToken x509SecurityToken = this.IssuerToken as System.IdentityModel.Tokens.X509SecurityToken;
                if (x509SecurityToken != null)
                {
                    dictionary.Add("alg", "RS256");
                    dictionary.Add("x5t", Base64UrlEncoder.Encode(x509SecurityToken.Certificate.GetCertHash()));
                }
                else if (this.IssuerToken is BinarySecretSecurityToken)
                {
                    dictionary.Add("alg", "HS256");
                }
            }
            else
            {
                dictionary.Add("alg", "none");
            }
            return dictionary;
        }

        private string GetTimeInSeconds(System.DateTime time)
        {
            return new EpochTime(time).SecondsSinceUnixEpoch.ToString();
        }
    }
}
