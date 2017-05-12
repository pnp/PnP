using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SharePointPnP.IdentityModel.Extensions.S2S.Protocols.OAuth2
{
    public class OAuth2AccessTokenResponse : OAuth2Message
    {
        public string AccessToken
        {
            get
            {
                return base.Message["access_token"];
            }
            set
            {
                base.Message["access_token"] = value;
            }
        }

        public virtual string ExpiresIn
        {
            get
            {
                return base.Message["expires_in"];
            }
            set
            {
                base.Message["expires_in"] = value;
            }
        }

        public System.DateTime ExpiresOn
        {
            get
            {
                return this.GetDateTimeParameter("expires_on");
            }
            set
            {
                this.SetDateTimeParameter("expires_on", value);
            }
        }

        public System.DateTime NotBefore
        {
            get
            {
                return this.GetDateTimeParameter("not_before");
            }
            set
            {
                this.SetDateTimeParameter("not_before", value);
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

        public string TokenType
        {
            get
            {
                return base.Message["token_type"];
            }
            set
            {
                base.Message["token_type"] = value;
            }
        }

        public static OAuth2AccessTokenResponse Read(string responseString)
        {
            OAuth2AccessTokenResponse oAuth2AccessTokenResponse = new OAuth2AccessTokenResponse();
            oAuth2AccessTokenResponse.DecodeFromJson(responseString);
            return oAuth2AccessTokenResponse;
        }

        public override string ToString()
        {
            return base.EncodeToJson();
        }

        private System.DateTime GetDateTimeParameter(string parameterName)
        {
            return new EpochTime(base.Message[parameterName]).DateTime;
        }

        private void SetDateTimeParameter(string parameterName, System.DateTime value)
        {
            base.Message[parameterName] = new EpochTime(value).SecondsSinceUnixEpoch.ToString();
        }
    }
}
