using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SharePointPnP.IdentityModel.Extensions.S2S.Protocols.OAuth2
{
    public class OAuth2ErrorResponse : OAuth2Message
    {
        public string Error
        {
            get
            {
                return base.Message["error"];
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new System.ArgumentException("Error property cannot be null or empty.", "value");
                }
                base.Message["error"] = value;
            }
        }

        public string ErrorDescription
        {
            get
            {
                return base.Message["error_description"];
            }
            set
            {
                base.Message["error_description"] = value;
            }
        }

        public string ErrorUri
        {
            get
            {
                return base.Message["error_uri"];
            }
            set
            {
                base.Message["error_uri"] = value;
            }
        }

        public static OAuth2ErrorResponse CreateFromEncodedResponse(string responseString)
        {
            OAuth2ErrorResponse oAuth2ErrorResponse = new OAuth2ErrorResponse();
            oAuth2ErrorResponse.DecodeFromJson(responseString);
            if (string.IsNullOrEmpty(oAuth2ErrorResponse.Error))
            {
                throw new System.ArgumentException("Error property is null or empty. This message is not a valid OAuth2 error response.", "responseString");
            }
            return oAuth2ErrorResponse;
        }

        private OAuth2ErrorResponse()
        {
        }

        public OAuth2ErrorResponse(string error)
        {
            this.Error = error;
        }

        public override string ToString()
        {
            return base.EncodeToJson();
        }
    }
}
