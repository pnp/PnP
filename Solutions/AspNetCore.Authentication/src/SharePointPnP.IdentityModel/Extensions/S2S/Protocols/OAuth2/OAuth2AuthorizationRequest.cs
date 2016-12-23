using SharePointPnP.IdentityModel.Extensions.S2S.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SharePointPnP.IdentityModel.Extensions.S2S.Protocols.OAuth2
{
    internal class OAuth2AuthorizationRequest : OAuth2Message
    {
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

        public string ResponseType
        {
            get
            {
                return base.Message["response_type"];
            }
            set
            {
                base.Message["response_type"] = value;
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

        public OAuth2AuthorizationRequest(string clientId) : this(clientId, "code")
        {
        }

        public OAuth2AuthorizationRequest(string clientId, string responseType)
        {
            Utility.VerifyNonNullOrEmptyStringArgument("clientId", clientId);
            Utility.VerifyNonNullOrEmptyStringArgument("responseType", responseType);
            this.ResponseType = responseType;
            this.ClientId = clientId;
        }

        public override string ToString()
        {
            return base.EncodeToJson();
        }
    }
}
