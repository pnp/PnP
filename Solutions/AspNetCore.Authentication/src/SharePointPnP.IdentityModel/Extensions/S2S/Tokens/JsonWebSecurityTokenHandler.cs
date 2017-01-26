using Microsoft.IdentityModel.Claims;
using System.ServiceModel.Security.Tokens;
using System.Xml;

namespace SharePointPnP.IdentityModel.Extensions.S2S.Tokens
{
    public class JsonWebSecurityTokenHandler : Microsoft.IdentityModel.Tokens.SecurityTokenHandler
    {
        private const string JsonCompactSerializationRegex = "^[A-Za-z0-9-_]+\\.[A-Za-z0-9-_]+\\.[A-Za-z0-9-_]*$";

        private JsonWebSecurityTokenRequirement _jsonWebSecurityTokenRequirement;

        public override bool CanValidateToken
        {
            get
            {
                return true;
            }
        }

        public override bool CanWriteToken
        {
            get
            {
                return true;
            }
        }

        public JsonWebSecurityTokenRequirement JsonWebSecurityTokenRequirement
        {
            get
            {
                return this._jsonWebSecurityTokenRequirement;
            }
            set
            {
                Utility.VerifyNonNullArgument("jsonWebSecurityTokenRequirement", value);
                this._jsonWebSecurityTokenRequirement = value;
            }
        }

        public override System.Type TokenType
        {
            get
            {
                return typeof(JsonWebSecurityToken);
            }
        }

        public JsonWebSecurityTokenHandler() : this(new JsonWebSecurityTokenRequirement())
        {
        }

        public JsonWebSecurityTokenHandler(JsonWebSecurityTokenRequirement jsonWebSecurityTokenRequirement)
        {
            Utility.VerifyNonNullArgument("jsonWebSecurityTokenRequirement", jsonWebSecurityTokenRequirement);
            this._jsonWebSecurityTokenRequirement = jsonWebSecurityTokenRequirement;
        }

        public override bool CanReadToken(System.Xml.XmlReader reader)
        {
            Utility.VerifyNonNullArgument("reader", reader);
            return this.IsJsonWebSecurityToken(reader);
        }

        public virtual bool CanReadToken(string token)
        {
            Utility.VerifyNonNullOrEmptyStringArgument("token", token);
            return this.IsJsonWebSecurityToken(token);
        }

        public override System.IdentityModel.Tokens.SecurityKeyIdentifierClause CreateSecurityTokenReference(System.IdentityModel.Tokens.SecurityToken token, bool attached)
        {
            return null;
        }

        public override System.IdentityModel.Tokens.SecurityToken CreateToken(Microsoft.IdentityModel.Tokens.SecurityTokenDescriptor tokenDescriptor)
        {
            Utility.VerifyNonNullArgument("tokenDescriptor", tokenDescriptor);
            if (tokenDescriptor.SigningCredentials == null)
            {
                throw new System.ArgumentException("tokenDescriptor.SigningCredentials cannot be null");
            }
            if (tokenDescriptor.Subject == null)
            {
                throw new System.ArgumentException("tokenDescriptor.Subject cannot be null");
            }
            if (string.IsNullOrEmpty(tokenDescriptor.TokenIssuerName))
            {
                throw new System.ArgumentException("tokenDescriptor.TokenIssuerName cannot be null");
            }
            System.DateTime dateTime = System.DateTime.UtcNow;
            System.DateTime validTo = DateTimeUtil.Add(dateTime, System.TimeSpan.FromHours(1.0));
            if (tokenDescriptor.Lifetime != null)
            {
                if (tokenDescriptor.Lifetime.Created.HasValue)
                {
                    dateTime = DateTimeUtil.ToUniversalTime(tokenDescriptor.Lifetime.Created.Value);
                }
                if (tokenDescriptor.Lifetime.Expires.HasValue)
                {
                    validTo = DateTimeUtil.ToUniversalTime(tokenDescriptor.Lifetime.Expires.Value);
                }
            }
            System.Collections.Generic.List<JsonWebTokenClaim> list = new System.Collections.Generic.List<JsonWebTokenClaim>();
            foreach (Claim current in tokenDescriptor.Subject.Claims)
            {
                list.Add(new JsonWebTokenClaim(current.ClaimType, current.Value));
            }
            return new JsonWebSecurityToken(tokenDescriptor.TokenIssuerName, this.GetAppliesTo(tokenDescriptor), dateTime, validTo, list, tokenDescriptor.SigningCredentials);
        }

        protected virtual string GetAppliesTo(Microsoft.IdentityModel.Tokens.SecurityTokenDescriptor tokenDescriptor)
        {
            if (string.IsNullOrEmpty(tokenDescriptor.AppliesToAddress))
            {
                throw new System.ArgumentException("tokenDescriptor.AppliesToAddress cannot be null");
            }
            return tokenDescriptor.AppliesToAddress;
        }

        protected virtual string GetIssuerName(JsonWebSecurityToken token)
        {
            if (token.IssuerToken == null)
            {
                throw new System.IdentityModel.Tokens.SecurityTokenException("JWT tokens must be signed.");
            }
            string issuerName = base.Configuration.IssuerNameRegistry.GetIssuerName(token.IssuerToken, token.Issuer);
            if (string.IsNullOrEmpty(issuerName))
            {
                throw new System.IdentityModel.Tokens.SecurityTokenException("Invalid issuer or signature.");
            }
            return issuerName;
        }

        protected virtual System.IdentityModel.Tokens.SecurityKeyIdentifier GetSigningKeyIdentifier(System.Collections.Generic.IDictionary<string, string> header, System.Collections.Generic.IDictionary<string, string> payload)
        {
            string x;
            if (!header.TryGetValue("alg", out x))
            {
                throw new System.IdentityModel.Tokens.SecurityTokenException("Invalid JWT token. No signature algorithm specified in token header.");
            }
            System.IdentityModel.Tokens.SecurityKeyIdentifierClause securityKeyIdentifierClause;
            if (System.StringComparer.Ordinal.Equals(x, "RS256"))
            {
                string arg;
                if (!header.TryGetValue("x5t", out arg))
                {
                    throw new System.IdentityModel.Tokens.SecurityTokenException("Invalid JWT token. No certificate thumbprint specified in token header.");
                }
                securityKeyIdentifierClause = new System.IdentityModel.Tokens.X509ThumbprintKeyIdentifierClause(Base64UrlEncoder.DecodeBytes(arg));
            }
            else
            {
                if (!System.StringComparer.Ordinal.Equals(x, "HS256"))
                {
                    throw new System.IdentityModel.Tokens.SecurityTokenException("Invalid JWT token. Didn't find a supported signature algorithm in token header.");
                }
                string issuer;
                payload.TryGetValue("iss", out issuer);
                securityKeyIdentifierClause = new SymmetricIssuerKeyIdentifierClause(issuer);
            }
            return new System.IdentityModel.Tokens.SecurityKeyIdentifier(new System.IdentityModel.Tokens.SecurityKeyIdentifierClause[]
            {
                securityKeyIdentifierClause
            });
        }

        public override string[] GetTokenTypeIdentifiers()
        {
            return new string[]
            {
                "http://oauth.net/grant_type/jwt/1.0/bearer"
            };
        }

        private bool IsJsonWebSecurityToken(System.Xml.XmlReader reader)
        {
            return reader.IsStartElement("BinarySecurityToken", "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd") && reader.GetAttribute("ValueType", null) == "http://oauth.net/grant_type/jwt/1.0/bearer";
        }

        private bool IsJsonWebSecurityToken(string token)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(token, "^[A-Za-z0-9-_]+\\.[A-Za-z0-9-_]+\\.[A-Za-z0-9-_]*$");
        }

        private JsonWebSecurityToken ReadActor(System.Collections.Generic.IDictionary<string, string> payload)
        {
            if (!this.JsonWebSecurityTokenRequirement.AllowActorToken)
            {
                return null;
            }
            JsonWebSecurityToken result = null;
            string text;
            payload.TryGetValue("actortoken", out text);
            if (!string.IsNullOrEmpty(text))
            {
                result = (this.ReadTokenCore(text, true) as JsonWebSecurityToken);
                payload.Remove("actortoken");
            }
            return result;
        }

        public override System.IdentityModel.Tokens.SecurityToken ReadToken(System.Xml.XmlReader reader)
        {
            if (!this.CanReadToken(reader))
            {
                throw new System.Xml.XmlException("Unsupported security token.");
            }
            string id = null;
            string jsonTokenString = this.GetJsonTokenString(reader, out id);
            JsonWebSecurityToken jsonWebSecurityToken = this.ReadToken(jsonTokenString) as JsonWebSecurityToken;
            if (jsonWebSecurityToken != null)
            {
                jsonWebSecurityToken.SetId(id);
            }
            return jsonWebSecurityToken;
        }

        internal string GetJsonTokenString(System.Xml.XmlReader reader, out string wsuId)
        {
            reader.MoveToContent();
            string @string;
            using (XmlDictionaryReader xmlDictionaryReader = XmlDictionaryReader.CreateDictionaryReader(reader))
            {
                wsuId = xmlDictionaryReader.GetAttribute("Id", "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd");
                string attribute = xmlDictionaryReader.GetAttribute("EncodingType", null);
                if (attribute != null && !(attribute == "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-soap-message-security-1.0#Base64Binary"))
                {
                    throw new System.Xml.XmlException(string.Format(System.Globalization.CultureInfo.InvariantCulture, "Unsupported encoding type: {0}", new object[]
                    {
                        attribute
                    }));
                }
                byte[] bytes = xmlDictionaryReader.ReadElementContentAsBase64();
                @string = Base64UrlEncoder.TextEncoding.GetString(bytes);
            }
            return @string;
        }

        public virtual System.IdentityModel.Tokens.SecurityToken ReadToken(string token)
        {
            return this.ReadTokenCore(token, false);
        }

        private System.IdentityModel.Tokens.SecurityToken ReadTokenCore(string token, bool isActorToken)
        {
            Utility.VerifyNonNullOrEmptyStringArgument("token", token);
            if (base.Configuration == null)
            {
                throw new System.InvalidOperationException("No configuration");
            }
            if (base.Configuration.IssuerTokenResolver == null)
            {
                throw new System.InvalidOperationException("No configured IssuerTokenResolver");
            }
            if (!this.CanReadToken(token))
            {
                throw new System.IdentityModel.Tokens.SecurityTokenException("Unsupported security token.");
            }
            string[] array = token.Split(new char[]
            {
                '.'
            });
            string text = array[0];
            string text2 = array[1];
            string text3 = array[2];
            System.Collections.Generic.Dictionary<string, string> dictionary = new System.Collections.Generic.Dictionary<string, string>(System.StringComparer.Ordinal);
            dictionary.DecodeFromJson(Base64UrlEncoder.Decode(text));
            System.Collections.Generic.Dictionary<string, string> dictionary2 = new System.Collections.Generic.Dictionary<string, string>(System.StringComparer.Ordinal);
            dictionary2.DecodeFromJson(Base64UrlEncoder.Decode(text2));
            string text4;
            dictionary.TryGetValue("alg", out text4);
            System.IdentityModel.Tokens.SecurityToken issuerToken = null;
            if (!System.StringComparer.Ordinal.Equals(text4, "none"))
            {
                if (string.IsNullOrEmpty(text3))
                {
                    throw new System.IdentityModel.Tokens.SecurityTokenException("Missing signature.");
                }
                System.IdentityModel.Tokens.SecurityKeyIdentifier signingKeyIdentifier = this.GetSigningKeyIdentifier(dictionary, dictionary2);
                System.IdentityModel.Tokens.SecurityToken securityToken;
                base.Configuration.IssuerTokenResolver.TryResolveToken(signingKeyIdentifier, out securityToken);
                if (securityToken == null)
                {
                    throw new System.IdentityModel.Tokens.SecurityTokenException("Invalid JWT token. Could not resolve issuer token.");
                }
                issuerToken = this.VerifySignature(string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0}.{1}", new object[]
                {
                    text,
                    text2
                }), text3, text4, securityToken);
            }
            JsonWebSecurityToken actorToken = null;
            if (!isActorToken)
            {
                actorToken = this.ReadActor(dictionary2);
            }
            string text5;
            dictionary2.TryGetValue("iss", out text5);
            if (string.IsNullOrEmpty(text5))
            {
                throw new System.IdentityModel.Tokens.SecurityTokenValidationException("The token being parsed does not have an issuer.");
            }
            string text6;
            dictionary2.TryGetValue("aud", out text6);
            if (string.IsNullOrEmpty(text6))
            {
                throw new System.IdentityModel.Tokens.SecurityTokenValidationException("The token being parsed does not have an audience.");
            }
            string text7;
            dictionary2.TryGetValue("nbf", out text7);
            if (string.IsNullOrEmpty(text7))
            {
                throw new System.IdentityModel.Tokens.SecurityTokenValidationException("The token being parsed does not have an 'not before' claim.");
            }
            System.DateTime dateTimeFromSeconds = this.GetDateTimeFromSeconds(text7);
            text7 = "";
            dictionary2.TryGetValue("exp", out text7);
            if (string.IsNullOrEmpty(text7))
            {
                throw new System.IdentityModel.Tokens.SecurityTokenValidationException("The token being parsed does not have an 'expires at' claim.");
            }
            System.DateTime dateTimeFromSeconds2 = this.GetDateTimeFromSeconds(text7);
            JsonWebSecurityToken jsonWebSecurityToken = new JsonWebSecurityToken(text5, text6, dateTimeFromSeconds, dateTimeFromSeconds2, this.CreateClaims(dictionary2), issuerToken, actorToken);
            jsonWebSecurityToken.CaptureSourceData(token);
            return jsonWebSecurityToken;
        }

        protected virtual string Sign(string signingInput, System.IdentityModel.Tokens.SigningCredentials signingCredentials)
        {
            if (signingCredentials == null)
            {
                return string.Empty;
            }
            string result;
            using (SignatureProvider signatureProvider = SignatureProvider.Create(signingCredentials))
            {
                result = Base64UrlEncoder.Encode(signatureProvider.Sign(Base64UrlEncoder.TextEncoding.GetBytes(signingInput)));
            }
            return result;
        }

        protected virtual ClaimsIdentityCollection ValidateActorToken(JsonWebSecurityToken actorToken)
        {
            return this.ValidateTokenCore(actorToken, true);
        }

        protected virtual void ValidateAudience(JsonWebSecurityToken token)
        {
            if (base.Configuration.AudienceRestriction.AudienceMode == System.IdentityModel.Selectors.AudienceUriMode.Always || base.Configuration.AudienceRestriction.AudienceMode == System.IdentityModel.Selectors.AudienceUriMode.BearerKeyOnly)
            {
                if (string.IsNullOrEmpty(token.Audience))
                {
                    throw new Microsoft.IdentityModel.Tokens.AudienceUriValidationFailedException("Audience URI validation failed. Token audience must be specified.");
                }
                AudienceValidator.ValidateAudiences(base.Configuration.AudienceRestriction.AllowedAudienceUris, new System.Uri[]
                {
                    new System.Uri(token.Audience, System.UriKind.RelativeOrAbsolute)
                });
            }
        }

        protected virtual void ValidateLifetime(JsonWebSecurityToken token)
        {
            System.TimeSpan maxClockSkew = base.Configuration.MaxClockSkew;
            System.DateTime utcNow = System.DateTime.UtcNow;
            if (maxClockSkew < System.TimeSpan.Zero)
            {
                throw new System.InvalidOperationException("No valid ClockSkew configured.");
            }
            if (token.ValidTo < utcNow - maxClockSkew)
            {
                throw new Microsoft.IdentityModel.Tokens.SecurityTokenExpiredException("Invalid JWT token. The token is expired.");
            }
            if (token.ValidFrom > utcNow + maxClockSkew)
            {
                throw new Microsoft.IdentityModel.Tokens.SecurityTokenExpiredException(string.Concat(new object[]
                {
                    "Invalid JWT token. The token is not yet valid. Current time is ",
                    utcNow,
                    " and the token is Valid from ",
                    token.ValidFrom,
                    "."
                }));
            }
        }

        private ClaimsIdentityCollection ValidateTokenCore(System.IdentityModel.Tokens.SecurityToken token, bool isActorToken)
        {
            JsonWebSecurityToken jsonWebSecurityToken = token as JsonWebSecurityToken;
            if (jsonWebSecurityToken == null)
            {
                return base.ValidateToken(token);
            }
            if (base.Configuration == null)
            {
                throw new System.InvalidOperationException("No configuration.");
            }
            if (base.Configuration.IssuerNameRegistry == null)
            {
                throw new System.InvalidOperationException("No issuername registry configured.");
            }
            this.ValidateLifetime(jsonWebSecurityToken);
            this.ValidateAudience(jsonWebSecurityToken);
            System.IdentityModel.Tokens.X509SecurityToken x509SecurityToken = jsonWebSecurityToken.IssuerToken as System.IdentityModel.Tokens.X509SecurityToken;
            if (x509SecurityToken != null)
            {
                base.Configuration.CertificateValidator.Validate(x509SecurityToken.Certificate);
            }
            ClaimsIdentityCollection claimsIdentityCollection = new ClaimsIdentityCollection();
            ClaimsIdentity claimsIdentity = new ClaimsIdentity("Federation");
            if (!isActorToken && jsonWebSecurityToken.ActorToken != null)
            {
                ClaimsIdentityCollection claimsIdentityCollection2 = this.ValidateActorToken(jsonWebSecurityToken.ActorToken);
                if (claimsIdentityCollection2.Count > 1)
                {
                    throw new System.IdentityModel.Tokens.SecurityTokenException("Invalid JWT token. Actor has multiple identities.");
                }
                claimsIdentity.Actor = claimsIdentityCollection2[0];
            }
            string issuerName = this.GetIssuerName(jsonWebSecurityToken);
            foreach (JsonWebTokenClaim current in jsonWebSecurityToken.Claims)
            {
                if (claimsIdentity.Actor == null || !System.StringComparer.Ordinal.Equals("actortoken", current.ClaimType))
                {
                    string text = current.Value;
                    if (text == null)
                    {
                        text = "NULL";
                    }
                    claimsIdentity.Claims.Add(new Claim(current.ClaimType, text, "http://www.w3.org/2001/XMLSchema#string", issuerName));
                }
            }
            if (!isActorToken && base.Configuration.SaveBootstrapTokens)
            {
                claimsIdentity.BootstrapToken = token;
            }
            claimsIdentityCollection.Add(claimsIdentity);
            return claimsIdentityCollection;
        }

        public override ClaimsIdentityCollection ValidateToken(System.IdentityModel.Tokens.SecurityToken token)
        {
            return this.ValidateTokenCore(token, false);
        }

        protected virtual System.IdentityModel.Tokens.SecurityToken VerifySignature(string signingInput, string signature, string algorithm, System.IdentityModel.Tokens.SecurityToken signingToken)
        {
            Utility.VerifyNonNullArgument("signingToken", signingToken);
            bool flag = false;
            System.IdentityModel.Tokens.SecurityToken result = null;
            if (string.Equals(algorithm, "RS256", System.StringComparison.Ordinal))
            {
                System.IdentityModel.Tokens.X509SecurityToken x509SecurityToken = signingToken as System.IdentityModel.Tokens.X509SecurityToken;
                if (x509SecurityToken == null)
                {
                    throw new System.IdentityModel.Tokens.SecurityTokenException("Unsupported issuer token type for asymmetric signature.");
                }
                System.Security.Cryptography.RSACryptoServiceProvider rSACryptoServiceProvider = x509SecurityToken.Certificate.PublicKey.Key as System.Security.Cryptography.RSACryptoServiceProvider;
                if (rSACryptoServiceProvider == null)
                {
                    throw new System.IdentityModel.Tokens.SecurityTokenException("Unsupported asymmetric signing algorithm.");
                }
                using (X509AsymmetricSignatureProvider x509AsymmetricSignatureProvider = new X509AsymmetricSignatureProvider(rSACryptoServiceProvider))
                {
                    flag = x509AsymmetricSignatureProvider.Verify(Base64UrlEncoder.TextEncoding.GetBytes(signingInput), Base64UrlEncoder.DecodeBytes(signature));
                    if (flag)
                    {
                        result = signingToken;
                    }
                    goto IL_133;
                }
            }
            if (string.Equals(algorithm, "HS256", System.StringComparison.Ordinal))
            {
                byte[] bytes = Base64UrlEncoder.TextEncoding.GetBytes(signingInput);
                byte[] signature2 = Base64UrlEncoder.DecodeBytes(signature);
                using (System.Collections.Generic.IEnumerator<System.IdentityModel.Tokens.SecurityKey> enumerator = signingToken.SecurityKeys.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        System.IdentityModel.Tokens.SecurityKey current = enumerator.Current;
                        System.IdentityModel.Tokens.SymmetricSecurityKey symmetricSecurityKey = current as System.IdentityModel.Tokens.SymmetricSecurityKey;
                        if (symmetricSecurityKey != null)
                        {
                            using (SymmetricSignatureProvider symmetricSignatureProvider = new SymmetricSignatureProvider(symmetricSecurityKey))
                            {
                                flag = symmetricSignatureProvider.Verify(bytes, signature2);
                                if (flag)
                                {
                                    result = new BinarySecretSecurityToken(symmetricSecurityKey.GetSymmetricKey());
                                    break;
                                }
                            }
                        }
                    }
                    goto IL_133;
                }
            }
            throw new System.IdentityModel.Tokens.SecurityTokenException("Unsupported signing algorithm.");
            IL_133:
            if (!flag)
            {
                throw new System.IdentityModel.Tokens.SecurityTokenException("Invalid issuer or signature.");
            }
            return result;
        }

        public override void WriteToken(System.Xml.XmlWriter writer, System.IdentityModel.Tokens.SecurityToken token)
        {
            if (!(token is JsonWebSecurityToken))
            {
                base.WriteToken(writer, token);
            }
            Utility.VerifyNonNullArgument("writer", writer);
            byte[] bytes = Base64UrlEncoder.TextEncoding.GetBytes(this.WriteTokenAsString(token));
            writer.WriteStartElement("wsse", "BinarySecurityToken", "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd");
            if (token.Id != null)
            {
                writer.WriteAttributeString("wsu", "Id", "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd", token.Id);
            }
            writer.WriteAttributeString("ValueType", null, "http://oauth.net/grant_type/jwt/1.0/bearer");
            writer.WriteAttributeString("EncodingType", null, "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-soap-message-security-1.0#Base64Binary");
            writer.WriteBase64(bytes, 0, bytes.Length);
            writer.WriteEndElement();
        }

        public virtual string WriteTokenAsString(System.IdentityModel.Tokens.SecurityToken token)
        {
            Utility.VerifyNonNullArgument("token", token);
            JsonWebSecurityToken jsonWebSecurityToken = token as JsonWebSecurityToken;
            if (jsonWebSecurityToken == null)
            {
                throw new System.ArgumentException("Unsupported token type", "token");
            }
            if (jsonWebSecurityToken.CanWriteSourceData)
            {
                return jsonWebSecurityToken.WriteSourceData();
            }
            System.Collections.Generic.IDictionary<string, string> self = jsonWebSecurityToken.CreateHeaderClaims();
            System.Collections.Generic.IDictionary<string, string> self2 = jsonWebSecurityToken.CreatePayloadClaims();
            string text = string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0}.{1}", new object[]
            {
                Base64UrlEncoder.Encode(self.EncodeToJson()),
                Base64UrlEncoder.Encode(self2.EncodeToJson())
            });
            string text2 = this.Sign(text, jsonWebSecurityToken.SigningCredentials);
            return string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0}.{1}", new object[]
            {
                text,
                text2
            });
        }

        private System.DateTime GetDateTimeFromSeconds(string seconds)
        {
            long secondsSinceUnixEpoch = System.Convert.ToInt64(seconds);
            return new EpochTime(secondsSinceUnixEpoch).DateTime;
        }

        private System.Collections.Generic.IEnumerable<JsonWebTokenClaim> CreateClaims(System.Collections.Generic.IDictionary<string, string> payloadClaims)
        {
            System.Collections.Generic.List<JsonWebTokenClaim> list = new System.Collections.Generic.List<JsonWebTokenClaim>();
            foreach (string current in payloadClaims.Keys)
            {
                if (!this.IsReservedClaimType(current))
                {
                    list.Add(new JsonWebTokenClaim(current, payloadClaims[current]));
                }
            }
            return list;
        }

        protected virtual bool IsReservedClaimType(string claimType)
        {
            return System.StringComparer.OrdinalIgnoreCase.Equals(claimType, "aud") || System.StringComparer.OrdinalIgnoreCase.Equals(claimType, "iss") || System.StringComparer.OrdinalIgnoreCase.Equals(claimType, "nbf") || System.StringComparer.OrdinalIgnoreCase.Equals(claimType, "exp");
        }
    }
}
