using SharePointPnP.IdentityModel.Extensions.S2S.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SharePointPnP.IdentityModel.Extensions.S2S.Protocols.OAuth2
{
    public static class OAuth2MessageFactory
    {
        public static OAuth2Message CreateFromEncodedResponse(System.IO.StreamReader reader)
        {
            return OAuth2MessageFactory.CreateFromEncodedResponse(reader.ReadToEnd());
        }

        public static OAuth2Message CreateFromEncodedResponse(string responseString)
        {
            if (responseString.StartsWith("{\"error"))
            {
                return OAuth2ErrorResponse.CreateFromEncodedResponse(responseString);
            }
            return OAuth2AccessTokenResponse.Read(responseString);
        }

        public static OAuth2AccessTokenRequest CreateAccessTokenRequestWithAuthorizationCode(string clientId, string clientSecret, string authorizationCode, System.Uri redirectUri, string resource)
        {
            OAuth2AccessTokenRequest oAuth2AccessTokenRequest = new OAuth2AccessTokenRequest();
            oAuth2AccessTokenRequest.GrantType = "authorization_code";
            oAuth2AccessTokenRequest.ClientId = clientId;
            oAuth2AccessTokenRequest.ClientSecret = clientSecret;
            oAuth2AccessTokenRequest.Code = authorizationCode;
            if (redirectUri != null)
            {
                oAuth2AccessTokenRequest.RedirectUri = redirectUri.AbsoluteUri;
            }
            oAuth2AccessTokenRequest.Resource = resource;
            return oAuth2AccessTokenRequest;
        }

        public static OAuth2AccessTokenRequest CreateAccessTokenRequestWithAuthorizationCode(string clientId, string clientSecret, string authorizationCode, string resource)
        {
            return new OAuth2AccessTokenRequest
            {
                GrantType = "authorization_code",
                ClientId = clientId,
                ClientSecret = clientSecret,
                Code = authorizationCode,
                Resource = resource
            };
        }

        public static OAuth2AccessTokenRequest CreateAccessTokenRequestWithRefreshToken(string clientId, string clientSecret, string refreshToken, string resource)
        {
            return new OAuth2AccessTokenRequest
            {
                GrantType = "refresh_token",
                ClientId = clientId,
                ClientSecret = clientSecret,
                RefreshToken = refreshToken,
                Resource = resource
            };
        }

        public static OAuth2AccessTokenRequest CreateAccessTokenRequestWithClientCredentials(string clientId, string clientSecret, string scope)
        {
            return new OAuth2AccessTokenRequest
            {
                GrantType = "client_credentials",
                ClientId = clientId,
                ClientSecret = clientSecret,
                Scope = scope
            };
        }

        public static OAuth2AccessTokenRequest CreateAccessTokenRequestWithAssertion(System.IdentityModel.Tokens.SecurityToken token, string resource)
        {
            Utility.VerifyNonNullArgument("token", token);
            Microsoft.IdentityModel.Tokens.SecurityTokenHandlerCollection securityTokenHandlerCollection = Microsoft.IdentityModel.Tokens.SecurityTokenHandlerCollection.CreateDefaultSecurityTokenHandlerCollection();
            securityTokenHandlerCollection.Add(new JsonWebSecurityTokenHandler());
            return OAuth2MessageFactory.CreateAccessTokenRequestWithAssertion(token, securityTokenHandlerCollection, resource);
        }

        public static OAuth2AccessTokenRequest CreateAccessTokenRequestWithAssertion(System.IdentityModel.Tokens.SecurityToken token, Microsoft.IdentityModel.Tokens.SecurityTokenHandlerCollection securityTokenHandlers, string resource)
        {
            Utility.VerifyNonNullArgument("token", token);
            if (token is JsonWebSecurityToken)
            {
                return OAuth2MessageFactory.CreateAccessTokenRequestWithAssertion((JsonWebSecurityToken)token, securityTokenHandlers, resource);
            }
            if (token is System.IdentityModel.Tokens.GenericXmlSecurityToken)
            {
                return OAuth2MessageFactory.CreateAccessTokenRequestWithAssertion((System.IdentityModel.Tokens.GenericXmlSecurityToken)token, resource);
            }
            if (token is System.IdentityModel.Tokens.SamlSecurityToken || token is Microsoft.IdentityModel.Tokens.Saml2.Saml2SecurityToken)
            {
                return OAuth2MessageFactory.CreateAccessTokenRequestWithAssertionForSamlSecurityTokens(token, securityTokenHandlers, resource);
            }
            throw new System.ArgumentException("Unsupported SecurityToken");
        }

        private static OAuth2AccessTokenRequest CreateAccessTokenRequestWithAssertion(System.IdentityModel.Tokens.GenericXmlSecurityToken token, string resource)
        {
            Utility.VerifyNonNullArgument("token", token);
            OAuth2AccessTokenRequest oAuth2AccessTokenRequest = new OAuth2AccessTokenRequest();
            JsonWebSecurityTokenHandler jsonWebSecurityTokenHandler = new JsonWebSecurityTokenHandler();
            System.Xml.XmlReader reader = new System.Xml.XmlNodeReader(token.TokenXml);
            string text;
            string jsonTokenString = jsonWebSecurityTokenHandler.GetJsonTokenString(reader, out text);
            oAuth2AccessTokenRequest.GrantType = OAuth2MessageFactory.GetTokenType(token);
            oAuth2AccessTokenRequest.Assertion = jsonTokenString;
            oAuth2AccessTokenRequest.Resource = resource;
            return oAuth2AccessTokenRequest;
        }

        private static OAuth2AccessTokenRequest CreateAccessTokenRequestWithAssertionForSamlSecurityTokens(System.IdentityModel.Tokens.SecurityToken token, Microsoft.IdentityModel.Tokens.SecurityTokenHandlerCollection securityTokenHandlers, string resource)
        {
            Utility.VerifyNonNullArgument("securityTokenHandlers", securityTokenHandlers);
            OAuth2AccessTokenRequest oAuth2AccessTokenRequest = new OAuth2AccessTokenRequest();
            if (token is System.IdentityModel.Tokens.SamlSecurityToken)
            {
                oAuth2AccessTokenRequest.GrantType = "urn:oasis:names:tc:SAML:1.0:assertion";
            }
            else
            {
                oAuth2AccessTokenRequest.GrantType = "urn:oasis:names:tc:SAML:2.0:assertion";
            }
            System.Xml.XmlWriterSettings xmlWriterSettings = new System.Xml.XmlWriterSettings();
            System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder();
            xmlWriterSettings.OmitXmlDeclaration = true;
            using (System.Xml.XmlWriter xmlWriter = System.Xml.XmlWriter.Create(stringBuilder, xmlWriterSettings))
            {
                securityTokenHandlers.WriteToken(xmlWriter, token);
                oAuth2AccessTokenRequest.Assertion = stringBuilder.ToString();
            }
            oAuth2AccessTokenRequest.Resource = resource;
            return oAuth2AccessTokenRequest;
        }

        private static OAuth2AccessTokenRequest CreateAccessTokenRequestWithAssertion(JsonWebSecurityToken token, Microsoft.IdentityModel.Tokens.SecurityTokenHandlerCollection securityTokenHandlers, string resource)
        {
            Utility.VerifyNonNullArgument("token", token);
            Utility.VerifyNonNullArgument("securityTokenHandlers", securityTokenHandlers);
            JsonWebSecurityTokenHandler jsonWebSecurityTokenHandler = securityTokenHandlers[typeof(JsonWebSecurityToken)] as JsonWebSecurityTokenHandler;
            if (jsonWebSecurityTokenHandler == null)
            {
                throw new System.ArgumentException("The input security token handlers collection does not contain a handler for JWT tokens.", "securityTokenHandlers");
            }
            string assertion = jsonWebSecurityTokenHandler.WriteTokenAsString(token);
            return new OAuth2AccessTokenRequest
            {
                GrantType = "http://oauth.net/grant_type/jwt/1.0/bearer",
                Assertion = assertion,
                Resource = resource
            };
        }

        private static string GetTokenType(System.IdentityModel.Tokens.GenericXmlSecurityToken token)
        {
            using (System.Xml.XmlReader xmlReader = new System.Xml.XmlNodeReader(token.TokenXml))
            {
                xmlReader.MoveToContent();
                if (xmlReader.IsStartElement("BinarySecurityToken", "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd"))
                {
                    return xmlReader.GetAttribute("ValueType", null);
                }
            }
            return string.Empty;
        }
    }
}
