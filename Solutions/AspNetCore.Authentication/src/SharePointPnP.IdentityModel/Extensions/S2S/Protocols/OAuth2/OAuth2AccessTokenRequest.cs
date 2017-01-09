using Microsoft.IdentityModel.SecurityTokenService;
using SharePointPnP.IdentityModel.Extensions.S2S.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace SharePointPnP.IdentityModel.Extensions.S2S.Protocols.OAuth2
{
    public class OAuth2AccessTokenRequest : OAuth2Message
    {
        public static System.Collections.Specialized.StringCollection TokenResponseParameters = 
            OAuth2AccessTokenRequest.GetTokenResponseParameters();

        public string Password
        {
            get
            {
                return base.Message["password"];
            }
            set
            {
                base.Message["password"] = value;
            }
        }

        public string RefreshToken
        {
            get
            {
                return base.Message["refresh_token"];
            }
            set
            {
                base.Message["refresh_token"] = value;
            }
        }

        public string Resource
        {
            get
            {
                return base.Message["resource"];
            }
            set
            {
                base.Message["resource"] = value;
            }
        }

        public string Scope
        {
            get
            {
                return base.Message["scope"];
            }
            set
            {
                base.Message["scope"] = value;
            }
        }

        public string AppContext
        {
            get
            {
                return base["AppContext"];
            }
            set
            {
                base["AppContext"] = value;
            }
        }

        public string Assertion
        {
            get
            {
                return base["assertion"];
            }
            set
            {
                base["assertion"] = value;
            }
        }

        public string GrantType
        {
            get
            {
                return base["grant_type"];
            }
            set
            {
                base["grant_type"] = value;
            }
        }

        public string ClientId
        {
            get
            {
                return base["client_id"];
            }
            set
            {
                base["client_id"] = value;
            }
        }

        public string ClientSecret
        {
            get
            {
                return base["client_secret"];
            }
            set
            {
                base["client_secret"] = value;
            }
        }

        public string Code
        {
            get
            {
                return base["code"];
            }
            set
            {
                base["code"] = value;
            }
        }

        public string RedirectUri
        {
            get
            {
                return base["redirect_uri"];
            }
            set
            {
                base["redirect_uri"] = value;
            }
        }

        public static OAuth2AccessTokenRequest Read(System.IO.StreamReader reader)
        {
            string requestString = null;
            try
            {
                requestString = reader.ReadToEnd();
            }
            catch (System.Text.DecoderFallbackException innerException)
            {
                throw new System.IO.InvalidDataException("Request encoding is not ASCII", innerException);
            }
            return OAuth2AccessTokenRequest.Read(requestString);
        }

        public static OAuth2AccessTokenRequest Read(string requestString)
        {
            OAuth2AccessTokenRequest oAuth2AccessTokenRequest = new OAuth2AccessTokenRequest();
            try
            {
                oAuth2AccessTokenRequest.Decode(requestString);
            }
            catch (InvalidRequestException)
            {
                System.Collections.Specialized.NameValueCollection nameValueCollection = HttpUtility.ParseQueryString(requestString);
                if (string.IsNullOrEmpty(nameValueCollection["client_id"]) && string.IsNullOrEmpty(nameValueCollection["assertion"]))
                {
                    throw new System.IO.InvalidDataException("The request body must contain a client_id or assertion parameter.");
                }
                throw;
            }
            foreach (string current in oAuth2AccessTokenRequest.Keys)
            {
                if (OAuth2AccessTokenRequest.TokenResponseParameters.Contains(current))
                {
                    throw new System.IO.InvalidDataException();
                }
            }
            return oAuth2AccessTokenRequest;
        }

        private static System.Collections.Specialized.StringCollection GetTokenResponseParameters()
        {
            return new System.Collections.Specialized.StringCollection
            {
                "access_token",
                "expires_in"
            };
        }

        public void SetCustomProperty(string propertyName, string propertyValue)
        {
            Utility.VerifyNonNullOrEmptyStringArgument("propertyName", propertyName);
            Utility.VerifyNonNullOrEmptyStringArgument("propertyValue", propertyValue);
            base[propertyName] = propertyValue;
        }

        public virtual void Write(System.IO.StreamWriter writer)
        {
            if (writer == null)
            {
                throw new System.ArgumentNullException("writer");
            }
            writer.Write(base.Encode());
        }
    }
}
