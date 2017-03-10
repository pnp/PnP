using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SharePointPnP.IdentityModel.Extensions.S2S.Protocols.OAuth2
{
    internal class OAuth2AuthorizationResponse : OAuth2Message
    {
        public string Code
        {
            get
            {
                return base.Message["code"];
            }
            set
            {
                base.Message["code"] = value;
            }
        }

        private OAuth2AuthorizationResponse()
        {
        }

        public static OAuth2AuthorizationResponse Read(string authorizationResponseString)
        {
            OAuth2AuthorizationResponse oAuth2AuthorizationResponse = new OAuth2AuthorizationResponse();
            oAuth2AuthorizationResponse.DecodeFromJson(authorizationResponseString);
            return oAuth2AuthorizationResponse;
        }
    }
}
