namespace SharePointPnP.IdentityModel.Extensions.S2S.Protocols.OAuth2
{
    public class OAuth2ErrorCode
    {
        private readonly string _value;

        public static OAuth2ErrorCode InvalidClient = new OAuth2ErrorCode("invalid_client");

        public static OAuth2ErrorCode InvalidGrant = new OAuth2ErrorCode("invalid_grant");

        public static OAuth2ErrorCode InvalidRequest = new OAuth2ErrorCode("invalid_request");

        public static OAuth2ErrorCode InvalidScope = new OAuth2ErrorCode("invalid_scope");

        public static OAuth2ErrorCode UnauthorizedClient = new OAuth2ErrorCode("unauthorized_client");

        public static OAuth2ErrorCode UnsupportedGrantType = new OAuth2ErrorCode("unsupported_grant_type");

        public static OAuth2ErrorCode TemporarilyUnavailable = new OAuth2ErrorCode("temporarily_unavailable");

        public OAuth2ErrorCode(string errorCode)
        {
            switch (errorCode)
            {
                case "invalid_client":
                case "invalid_grant":
                case "invalid_request":
                case "invalid_scope":
                case "temporarily_unavailable":
                case "unauthorized_client":
                case "unsupported_grant_type":
                    this._value = errorCode;
                    return;
            }
            throw new System.InvalidOperationException(string.Format(System.Globalization.CultureInfo.InvariantCulture, "'{0}' is not a supported OAuth2 error code value.", new object[]
            {
                errorCode
            }));
        }

        public override string ToString()
        {
            return this._value;
        }
    }
}
