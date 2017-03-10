namespace SharePointPnP.IdentityModel.Extensions.S2S.Protocols.OAuth2
{
    public static class OAuth2Constants
    {
        public static class GrantTypeConstants
        {
            public const string AuthorizationCode = "authorization_code";

            public const string ClientCredentials = "client_credentials";

            public const string RefreshToken = "refresh_token";
        }

        public static class ContentTypes
        {
            public const string Json = "application/json";

            public const string UrlEncoded = "application/x-www-form-urlencoded";
        }

        public static class ErrorConstants
        {
            public const string Error = "error";

            public const string ErrorDescription = "error_description";

            public const string ErrorUri = "error_uri";
        }

        public static class ErrorCodes
        {
            public const string InvalidClient = "invalid_client";

            public const string InvalidGrant = "invalid_grant";

            public const string InvalidRequest = "invalid_request";

            public const string InvalidScope = "invalid_scope";

            public const string UnauthorizedClient = "unauthorized_client";

            public const string UnsupportedGrantType = "unsupported_grant_type";

            public const string TemporarilyUnavailable = "temporarily_unavailable";
        }

        public const string AccessToken = "access_token";

        public const string Assertion = "assertion";

        public const string ClientId = "client_id";

        public const string ClientSecret = "client_secret";

        public const string Code = "code";

        public const string ExpiresIn = "expires_in";

        public const string GrantType = "grant_type";

        public const string BearerAuthenticationType = "Bearer";

        public const string RedirectUri = "redirect_uri";

        public const string RefreshToken = "refresh_token";

        public const string ResponseType = "response_type";

        public const string Scope = "scope";

        public const string State = "state";

        public const string TokenType = "token_type";

        public const string Password = "password";

        public const string AppContext = "AppContext";

        public const string ExpiresOn = "expires_on";

        public const string NotBefore = "not_before";

        public const string Resource = "resource";
    }
}
