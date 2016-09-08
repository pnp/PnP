using System;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.S2S.Protocols.OAuth2;
using Microsoft.IdentityModel.Tokens;
using Microsoft.SharePoint.Client;
using Newtonsoft.Json;
using HttpContext = Microsoft.AspNetCore.Http.HttpContext;
using HttpRequest = Microsoft.AspNetCore.Http.HttpRequest;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Options;

namespace OfficeDevPnP.Core.Framework.Authentication
{
    /// <summary>
    /// Encapsulates all the information from SharePoint.
    /// </summary>
    public abstract class SharePointContext
    {
        public const string SPHostUrlKey = "SPHostUrl";
        public const string SPAppWebUrlKey = "SPAppWebUrl";
        public const string SPLanguageKey = "SPLanguage";
        public const string SPClientTagKey = "SPClientTag";
        public const string SPProductNumberKey = "SPProductNumber";

        protected static readonly TimeSpan AccessTokenLifetimeTolerance = TimeSpan.FromMinutes(5.0);

        private readonly Uri _spHostUrl;
        private readonly Uri _spAppWebUrl;
        private readonly string _spLanguage;
        private readonly string _spClientTag;
        private readonly string _spProductNumber;

        // <AccessTokenString, UtcExpiresOn>
        protected Tuple<string, DateTime> userAccessTokenForSPHost;
        protected Tuple<string, DateTime> userAccessTokenForSPAppWeb;
        protected Tuple<string, DateTime> appOnlyAccessTokenForSPHost;
        protected Tuple<string, DateTime> appOnlyAccessTokenForSPAppWeb;

        /// <summary>
        /// Gets the SharePoint host url from QueryString of the specified HTTP request.
        /// </summary>
        /// <param name="httpRequest">The specified HTTP request.</param>
        /// <returns>The SharePoint host url. Returns <c>null</c> if the HTTP request doesn't contain the SharePoint host url.</returns>
        public static Uri GetUriFromQueryStringParameter(HttpRequest httpRequest, string queryStringParameter)
        {
            if (httpRequest == null) { throw new ArgumentNullException(nameof(httpRequest)); }

            string parameterValue = TokenHandler.EnsureTrailingSlash(httpRequest.Query[queryStringParameter]);
            Uri uriValue;

            if (Uri.TryCreate(parameterValue, UriKind.Absolute, out uriValue) &&
               (uriValue.Scheme == Uri.UriSchemeHttp || uriValue.Scheme == Uri.UriSchemeHttps))
            {
                return uriValue;
            }

            return null;
        }

        /// <summary>
        /// The SharePoint host url.
        /// </summary>
        public Uri SPHostUrl => this._spHostUrl;

        /// <summary>
        /// The SharePoint app web url.
        /// </summary>
        public Uri SPAppWebUrl => this._spAppWebUrl;

        /// <summary>
        /// The SharePoint language.
        /// </summary>
        public string SPLanguage => this._spLanguage;

        /// <summary>
        /// The SharePoint client tag.
        /// </summary>
        public string SPClientTag => this._spClientTag;

        /// <summary>
        /// The SharePoint product number.
        /// </summary>
        public string SPProductNumber => this._spProductNumber;

        /// <summary>
        /// The app only access TokenHandler for the SharePoint app web.
        /// </summary>
        public TokenHandler TokenHandler { get; protected set; }

        /// <summary>
        /// The user access token for the SharePoint host.
        /// </summary>
        public abstract string UserAccessTokenForSPHost
        {
            get;
        }

        /// <summary>
        /// The user access token for the SharePoint app web.
        /// </summary>
        public abstract string UserAccessTokenForSPAppWeb
        {
            get;
        }

        /// <summary>
        /// The app only access token for the SharePoint host.
        /// </summary>
        public abstract string AppOnlyAccessTokenForSPHost
        {
            get;
        }

        /// <summary>
        /// The app only access token for the SharePoint app web.
        /// </summary>
        public abstract string AppOnlyAccessTokenForSPAppWeb
        {
            get;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="spHostUrl">The SharePoint host url.</param>
        /// <param name="spAppWebUrl">The SharePoint app web url.</param>
        /// <param name="spLanguage">The SharePoint language.</param>
        /// <param name="spClientTag">The SharePoint client tag.</param>
        /// <param name="spProductNumber">The SharePoint product number.</param>
        protected SharePointContext(Uri spHostUrl, Uri spAppWebUrl, string spLanguage, string spClientTag, string spProductNumber)
        {
            if (spHostUrl == null)
            {
                throw new ArgumentNullException(nameof(spHostUrl));
            }

            if (string.IsNullOrEmpty(spLanguage))
            {
                throw new ArgumentNullException(nameof(spLanguage));
            }

            if (string.IsNullOrEmpty(spClientTag))
            {
                throw new ArgumentNullException(nameof(spClientTag));
            }

            if (string.IsNullOrEmpty(spProductNumber))
            {
                throw new ArgumentNullException(nameof(spProductNumber));
            }

            this._spHostUrl = spHostUrl;
            this._spAppWebUrl = spAppWebUrl;
            this._spLanguage = spLanguage;
            this._spClientTag = spClientTag;
            this._spProductNumber = spProductNumber;
        }

        /// <summary>
        /// Creates a user ClientContext for the SharePoint host.
        /// </summary>
        /// <returns>A ClientContext instance.</returns>
        public ClientContext CreateUserClientContextForSPHost()
        {
            return CreateClientContext(this.SPHostUrl, this.UserAccessTokenForSPHost);
        }

        /// <summary>
        /// Creates a user ClientContext for the SharePoint app web.
        /// </summary>
        /// <returns>A ClientContext instance.</returns>
        public ClientContext CreateUserClientContextForSPAppWeb()
        {
            return CreateClientContext(this.SPAppWebUrl, this.UserAccessTokenForSPAppWeb);
        }

        /// <summary>
        /// Creates app only ClientContext for the SharePoint host.
        /// </summary>
        /// <returns>A ClientContext instance.</returns>
        public ClientContext CreateAppOnlyClientContextForSPHost()
        {
            return CreateClientContext(this.SPHostUrl, this.AppOnlyAccessTokenForSPHost);
        }

        /// <summary>
        /// Creates an app only ClientContext for the SharePoint app web.
        /// </summary>
        /// <returns>A ClientContext instance.</returns>
        public ClientContext CreateAppOnlyClientContextForSPAppWeb()
        {
            return CreateClientContext(this.SPAppWebUrl, this.AppOnlyAccessTokenForSPAppWeb);
        }

        /// <summary>
        /// Determines if the specified access token is valid.
        /// It considers an access token as not valid if it is null, or it has expired.
        /// </summary>
        /// <param name="accessToken">The access token to verify.</param>
        /// <returns>True if the access token is valid.</returns>
        protected static bool IsAccessTokenValid(Tuple<string, DateTime> accessToken)
        {
            return accessToken != null &&
                   !string.IsNullOrEmpty(accessToken.Item1) &&
                   accessToken.Item2 > DateTime.UtcNow;
        }

        /// <summary>
        /// Creates a ClientContext with the specified SharePoint site url and the access token.
        /// </summary>
        /// <param name="spSiteUrl">The site url.</param>
        /// <param name="accessToken">The access token.</param>
        /// <returns>A ClientContext instance.</returns>
        private ClientContext CreateClientContext(Uri spSiteUrl, string accessToken)
        {
            if (spSiteUrl != null && !string.IsNullOrEmpty(accessToken))
            {
                return TokenHandler.GetClientContextWithAccessToken(spSiteUrl.AbsoluteUri, accessToken);
            }

            return null;
        }
    }

    /// <summary>
    /// Redirection status.
    /// </summary>
    public enum RedirectionStatus
    {
        Ok,
        ShouldRedirect,
        CanNotRedirect
    }

    /// <summary>
    /// Provides SharePointContext instances.
    /// </summary>
    public abstract class SharePointContextProvider
    {
        private static SharePointContextProvider _current;
        private static TokenHandler _tokenHandler;
        private static SharePointConfiguration _configuration;

        /// <summary>
        /// The current SharePointContextProvider instance.
        /// </summary>
        public static SharePointContextProvider Current => _current;

        /// <summary>
        /// TokenHandler instance.
        /// </summary>
        protected static TokenHandler TokenHandler
        {
            get { return _tokenHandler; }
            set { _tokenHandler = value; }
        }

        /// <summary>
        /// SharePointConfiguration instance.
        /// </summary>
        protected static SharePointConfiguration Configuration
        {
            get { return _configuration; }
            set { _configuration = value; }
        }

        /// <summary>
        /// Initializes the default SharePointContextProvider instance.
        /// </summary>
        public static SharePointContextProvider GetInstance(SharePointConfiguration configuration)
        {
            _tokenHandler = new TokenHandler(configuration);
            _configuration = configuration;
            if (!_tokenHandler.IsHighTrustApp())
            {
                _current = new SharePointAcsContextProvider();
            }
            else
            {
                throw new NotImplementedException("Hight Trust is still not supported by this library.");
                //current = new SharePointHighTrustContextProvider();
            }
            return _current;
        }

        /// <summary>
        /// Initializes the default SharePointContextProvider instance.
        /// </summary>
        public static SharePointContextProvider GetInstance(IConfiguration configuration)
        {
            //setup the SharePoint configuration based on the middleware options
            return GetInstance(SharePointConfiguration.GetFromIConfiguration(configuration));
        }

        /// <summary>
        /// Initializes the default SharePointContextProvider instance.
        /// </summary>
        public static SharePointContextProvider GetInstance(IOptions<SharePointConfiguration> options)
        {
            //setup the SharePoint configuration based on the middleware options
            return GetInstance(SharePointConfiguration.GetFromIOptions(options));
        }

        /// <summary>
        /// Registers the specified SharePointContextProvider instance as current.
        /// It should be called by Application_Start() in Global.asax.
        /// </summary>
        /// <param name="provider">The SharePointContextProvider to be set as current.</param>
        public static void Register(SharePointContextProvider provider)
        {
            if (provider == null)
            {
                throw new ArgumentNullException(nameof(provider));
            }

            _current = provider;
        }

        /// <summary>
        /// Checks if it is necessary to redirect to SharePoint for user to authenticate.
        /// </summary>
        /// <param name="httpContext">The HTTP context.</param>
        /// <param name="redirectUrl">The redirect url to SharePoint if the status is ShouldRedirect. <c>Null</c> if the status is Ok or CanNotRedirect.</param>
        /// <returns>Redirection status.</returns>
        public static RedirectionStatus CheckRedirectionStatus(HttpContext httpContext, out Uri redirectUrl)
        {
            if (httpContext == null)
            {
                throw new ArgumentNullException(nameof(httpContext));
            }

            redirectUrl = null;
            bool contextTokenExpired = false;

            try
            {
                if (Current.GetSharePointContext(httpContext) != null)
                {
                    return RedirectionStatus.Ok;
                }
            }
            catch (SecurityTokenExpiredException)
            {
                contextTokenExpired = true;
            }

            const string SPHasRedirectedToSharePointKey = "SPHasRedirectedToSharePoint";

            if (!string.IsNullOrEmpty(httpContext.Request.Query[SPHasRedirectedToSharePointKey]) && !contextTokenExpired)
            {
                return RedirectionStatus.CanNotRedirect;
            }

            Uri spHostUrl = SharePointContext.GetUriFromQueryStringParameter
                (httpContext.Request, SharePointContext.SPHostUrlKey);

            if (spHostUrl == null)
            {
                return RedirectionStatus.CanNotRedirect;
            }

            if (StringComparer.OrdinalIgnoreCase.Equals(httpContext.Request.Method, "POST"))
            {
                return RedirectionStatus.CanNotRedirect;
            }
            var uri = GetCurrentUrl(httpContext);

            var queryNameValueCollection = Microsoft.AspNetCore.WebUtilities.QueryHelpers.ParseQuery(uri);

            // Removes the values that are included in {StandardTokens}, as {StandardTokens} will be inserted at the beginning of the query string.
            queryNameValueCollection.Remove(SharePointContext.SPHostUrlKey);
            queryNameValueCollection.Remove(SharePointContext.SPAppWebUrlKey);
            queryNameValueCollection.Remove(SharePointContext.SPLanguageKey);
            queryNameValueCollection.Remove(SharePointContext.SPClientTagKey);
            queryNameValueCollection.Remove(SharePointContext.SPProductNumberKey);

            // Adds SPHasRedirectedToSharePoint=1.
            queryNameValueCollection.Add(SPHasRedirectedToSharePointKey, "1");

            UriBuilder returnUrlBuilder = new UriBuilder(uri);
            returnUrlBuilder.Query = queryNameValueCollection.ToString();

            // Inserts StandardTokens.
            const string StandardTokens = "{StandardTokens}";
            string returnUrlString = returnUrlBuilder.Uri.AbsoluteUri;
            returnUrlString = returnUrlString.Insert(returnUrlString.IndexOf("?") + 1, StandardTokens + "&");

            // Constructs redirect url.
            string redirectUrlString = TokenHandler.GetAppContextTokenRequestUrl(spHostUrl.AbsoluteUri, Uri.EscapeDataString(returnUrlString));

            redirectUrl = new Uri(redirectUrlString, UriKind.Absolute);

            return RedirectionStatus.ShouldRedirect;
        }

        /// <summary>
        /// Getting the correct url with the correct url scheme since httpContext.Request.GetDisplayUrl()
        /// provides incorrect 'http' scheme even we are running on 'https'. It needs to be double checked until
        /// better solution has been found.
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        private static string GetCurrentUrl(HttpContext httpContext)
        {
            var url = httpContext.Request.GetDisplayUrl();
            return url;
        }

        /// <summary>
        /// Creates a SharePointContext instance with the specified HTTP request.
        /// </summary>
        /// <param name="httpRequest">The HTTP request.</param>
        /// <returns>The SharePointContext instance. Returns <c>null</c> if errors occur.</returns>
        public SharePointContext CreateSharePointContext(HttpRequest httpRequest)
        {
            if (httpRequest == null) { throw new ArgumentNullException(nameof(httpRequest)); }

            // SPHostUrl
            Uri spHostUrl = SharePointContext.GetUriFromQueryStringParameter(httpRequest, SharePointContext.SPHostUrlKey);
            if (spHostUrl == null) { return null; }

            // SPAppWebUrl
            Uri spAppWebUrl = SharePointContext.GetUriFromQueryStringParameter(httpRequest, SharePointContext.SPAppWebUrlKey);
            if (spAppWebUrl == null) { spAppWebUrl = null; }

            // SPLanguage
            string spLanguage = httpRequest.Query[SharePointContext.SPLanguageKey];
            if (string.IsNullOrEmpty(spLanguage)) { return null; }

            // SPClientTag
            string spClientTag = httpRequest.Query[SharePointContext.SPClientTagKey];
            if (string.IsNullOrEmpty(spClientTag)) { return null; }

            // SPProductNumber
            string spProductNumber = httpRequest.Query[SharePointContext.SPProductNumberKey];
            if (string.IsNullOrEmpty(spProductNumber)) { return null; }

            return CreateSharePointContext(spHostUrl, spAppWebUrl, spLanguage, spClientTag, spProductNumber, httpRequest);
        }

        /// <summary>
        /// Gets a SharePointContext instance associated with the specified HTTP context.
        /// </summary>
        /// <param name="httpContext">The HTTP context.</param>
        /// <returns>The SharePointContext instance. Returns <c>null</c> if not found and a new instance can't be created.</returns>
        public SharePointContext GetSharePointContext(HttpContext httpContext)
        {
            if (httpContext == null)
            {
                throw new ArgumentNullException(nameof(httpContext));
            }

            SharePointContext spContext = LoadSharePointContext(httpContext);

            if (spContext == null || !ValidateSharePointContext(spContext, httpContext))
            {
                spContext = CreateSharePointContext(httpContext.Request);

                if (spContext != null)
                {
                    SaveSharePointContext(spContext, httpContext);
                }
            }

            return spContext;
        }

        /// <summary>
        /// Creates a SharePointContext instance.
        /// </summary>
        /// <param name="spHostUrl">The SharePoint host url.</param>
        /// <param name="spAppWebUrl">The SharePoint app web url.</param>
        /// <param name="spLanguage">The SharePoint language.</param>
        /// <param name="spClientTag">The SharePoint client tag.</param>
        /// <param name="spProductNumber">The SharePoint product number.</param>
        /// <param name="httpRequest">The HTTP request.</param>
        /// <returns>The SharePointContext instance. Returns <c>null</c> if errors occur.</returns>
        protected abstract SharePointContext CreateSharePointContext(Uri spHostUrl, Uri spAppWebUrl, string spLanguage, string spClientTag, string spProductNumber, HttpRequest httpRequest);

        /// <summary>
        /// Validates if the given SharePointContext can be used with the specified HTTP context.
        /// </summary>
        /// <param name="spContext">The SharePointContext.</param>
        /// <param name="httpContext">The HTTP context.</param>
        /// <returns>True if the given SharePointContext can be used with the specified HTTP context.</returns>
        protected abstract bool ValidateSharePointContext(SharePointContext spContext, HttpContext httpContext);

        /// <summary>
        /// Loads the SharePointContext instance associated with the specified HTTP context.
        /// </summary>
        /// <param name="httpContext">The HTTP context.</param>
        /// <returns>The SharePointContext instance. Returns <c>null</c> if not found.</returns>
        protected abstract SharePointContext LoadSharePointContext(HttpContext httpContext);

        /// <summary>
        /// Saves the specified SharePointContext instance associated with the specified HTTP context.
        /// <c>null</c> is accepted for clearing the SharePointContext instance associated with the HTTP context.
        /// </summary>
        /// <param name="spContext">The SharePointContext instance to be saved, or <c>null</c>.</param>
        /// <param name="httpContext">The HTTP context.</param>
        protected abstract void SaveSharePointContext(SharePointContext spContext, HttpContext httpContext);
    }

    #region ACS

    /// <summary>
    /// Encapsulates all the information from SharePoint in ACS mode.
    /// </summary>
    public class SharePointAcsContext : SharePointContext
    {
        private readonly string contextToken;
        private readonly SharePointContextToken contextTokenObj;

        /// <summary>
        /// The context token.
        /// </summary>
        public string ContextToken
        {
            get { return this.contextTokenObj.ValidTo > DateTime.UtcNow ? this.contextToken : null; }
        }

        /// <summary>
        /// The context token's "CacheKey" claim.
        /// </summary>
        public string CacheKey
        {
            get { return this.contextTokenObj.ValidTo > DateTime.UtcNow ? this.contextTokenObj.CacheKey : null; }
        }

        /// <summary>
        /// The context token's "refreshtoken" claim.
        /// </summary>
        public string RefreshToken
        {
            get { return this.contextTokenObj.ValidTo > DateTime.UtcNow ? this.contextTokenObj.RefreshToken : null; }
        }



        public override string UserAccessTokenForSPHost
        {
            get
            {
                return GetAccessTokenString(ref this.userAccessTokenForSPHost,
                                            () => TokenHandler.GetAccessToken(this.contextTokenObj, this.SPHostUrl.Authority));
            }
        }

        public override string UserAccessTokenForSPAppWeb
        {
            get
            {
                if (this.SPAppWebUrl == null)
                {
                    return null;
                }

                return GetAccessTokenString(ref this.userAccessTokenForSPAppWeb,
                                            () => TokenHandler.GetAccessToken(this.contextTokenObj, this.SPAppWebUrl.Authority));
            }
        }

        public override string AppOnlyAccessTokenForSPHost
        {
            get
            {
                return GetAccessTokenString(ref this.appOnlyAccessTokenForSPHost,
                                            () => TokenHandler.GetAppOnlyAccessToken(TokenHandler.SharePointPrincipal, this.SPHostUrl.Authority, TokenHandler.GetRealmFromTargetUrl(this.SPHostUrl)));
            }
        }

        public override string AppOnlyAccessTokenForSPAppWeb
        {
            get
            {
                if (this.SPAppWebUrl == null)
                {
                    return null;
                }

                return GetAccessTokenString(ref this.appOnlyAccessTokenForSPAppWeb,
                                            () => TokenHandler.GetAppOnlyAccessToken(TokenHandler.SharePointPrincipal, this.SPAppWebUrl.Authority, TokenHandler.GetRealmFromTargetUrl(this.SPAppWebUrl)));
            }
        }

        public SharePointAcsContext(Uri spHostUrl, Uri spAppWebUrl, string spLanguage, string spClientTag, string spProductNumber, string contextToken, SharePointContextToken contextTokenObj, SharePointConfiguration configuration)
            : base(spHostUrl, spAppWebUrl, spLanguage, spClientTag, spProductNumber)
        {
            if (string.IsNullOrEmpty(contextToken))
            {
                throw new ArgumentNullException(nameof(contextToken));
            }

            if (contextTokenObj == null)
            {
                throw new ArgumentNullException(nameof(contextTokenObj));
            }

            this.contextToken = contextToken;
            this.contextTokenObj = contextTokenObj;
            this.TokenHandler = new TokenHandler(configuration);
        }

        /// <summary>
        /// Ensures the access token is valid and returns it.
        /// </summary>
        /// <param name="accessToken">The access token to verify.</param>
        /// <param name="tokenRenewalHandler">The token renewal handler.</param>
        /// <returns>The access token string.</returns>
        private static string GetAccessTokenString(ref Tuple<string, DateTime> accessToken, Func<OAuth2AccessTokenResponse> tokenRenewalHandler)
        {
            RenewAccessTokenIfNeeded(ref accessToken, tokenRenewalHandler);

            return IsAccessTokenValid(accessToken) ? accessToken.Item1 : null;
        }

        /// <summary>
        /// Renews the access token if it is not valid.
        /// </summary>
        /// <param name="accessToken">The access token to renew.</param>
        /// <param name="tokenRenewalHandler">The token renewal handler.</param>
        private static void RenewAccessTokenIfNeeded(ref Tuple<string, DateTime> accessToken, Func<OAuth2AccessTokenResponse> tokenRenewalHandler)
        {
            if (IsAccessTokenValid(accessToken))
            {
                return;
            }

            //try
            //{
                OAuth2AccessTokenResponse oAuth2AccessTokenResponse = tokenRenewalHandler();

                DateTime expiresOn = oAuth2AccessTokenResponse.ExpiresOn;

                if ((expiresOn - oAuth2AccessTokenResponse.NotBefore) > AccessTokenLifetimeTolerance)
                {
                    // Make the access token get renewed a bit earlier than the time when it expires
                    // so that the calls to SharePoint with it will have enough time to complete successfully.
                    expiresOn -= AccessTokenLifetimeTolerance;
                }

                accessToken = Tuple.Create(oAuth2AccessTokenResponse.AccessToken, expiresOn);
            //}
            //catch (WebException)
            //{
            //}
            //TODO: validate how to capture the web exception
        }
    }

    /// <summary>
    /// Default provider for SharePointAcsContext.
    /// </summary>
    public class SharePointAcsContextProvider : SharePointContextProvider
    {
        private const string SPContextKey = "SPContext";
        private const string SPCacheKeyKey = "SPCacheKey";

        protected override SharePointContext CreateSharePointContext(Uri spHostUrl, Uri spAppWebUrl, string spLanguage, string spClientTag, string spProductNumber, HttpRequest httpRequest)
        {
            string contextTokenString = TokenHandler.GetContextTokenFromRequest(httpRequest);
            if (string.IsNullOrEmpty(contextTokenString))
            {
                return null;
            }

            SharePointContextToken contextToken = null;
            try
            {
                contextToken = TokenHandler.ReadAndValidateContextToken(contextTokenString, httpRequest.Host.Value);
            }
            //catch (WebException)
            //{
            //    return null;
            //}
            catch (AudienceUriValidationFailedException)
            {
                return null;
            }

            return new SharePointAcsContext(spHostUrl, spAppWebUrl, spLanguage, spClientTag, spProductNumber, contextTokenString, contextToken, Configuration);
        }

        protected override bool ValidateSharePointContext(SharePointContext spContext, HttpContext httpContext)
        {
            SharePointAcsContext spAcsContext = spContext as SharePointAcsContext;

            //Checks for the SPCacheKey cookie and gets the value
            if (spAcsContext != null)
            {
                //Uri spHostUrl = SharePointContext.GetUriFromQueryStringParameter
                //    (httpContext.Request, SharePointContext.SPHostUrlKey);

                string contextToken = TokenHandler.GetContextTokenFromRequest(httpContext.Request);
                //read the cookie value
                var cookieCollection = httpContext.Request.Cookies;

                if (!cookieCollection.ContainsKey(SPCacheKeyKey)) return false;

                var spCacheKeyCookieValue = httpContext.Request.Cookies[SPCacheKeyKey];
                string spCacheKey = spCacheKeyCookieValue != null ? spCacheKeyCookieValue : null;

                //return spHostUrl == spAcsContext.SPHostUrl && (taken out)
                return 
                       !string.IsNullOrEmpty(spAcsContext.CacheKey) &&
                       spCacheKey == spAcsContext.CacheKey &&
                       !string.IsNullOrEmpty(spAcsContext.ContextToken) &&
                       (string.IsNullOrEmpty(contextToken) || contextToken == spAcsContext.ContextToken);
            }

            return false;
        }

        protected override SharePointContext LoadSharePointContext(HttpContext httpContext)
        {
            byte[] value;
            httpContext.Session.TryGetValue(SPContextKey, out value);
            if (value == null)
            {
                return null;
            }

            char[] chars = new char[value.Length / sizeof(char)];
            System.Buffer.BlockCopy(value, 0, chars, 0, value.Length);
            string acsSessionContext = new string(chars);
            var dto = JsonConvert.DeserializeObject<SharePointSessionData>(acsSessionContext);
            var contextTokenObj = TokenHandler.ReadAndValidateContextToken(dto.ContextToken, httpContext.Request.Host.Value);
            return new SharePointAcsContext(dto.SpHostUrl, dto.SpAppWebUrl, dto.SpLanguage, dto.SpClientTag, dto.SpProductNumber, dto.ContextToken, contextTokenObj, Configuration);
        }

        protected override void SaveSharePointContext(SharePointContext spContext, HttpContext httpContext)
        {
            SharePointAcsContext spAcsContext = spContext as SharePointAcsContext;

            //creates a cookie to store the SPCacheKey
            if (spAcsContext != null)
            {
                //The following code generates a cookie in the response with the SPCacheKey as a value
                var options = new CookieOptions() { HttpOnly = true, Secure = true };
                httpContext.Response.Cookies.Append(SPCacheKeyKey, spAcsContext.CacheKey, options);
            }
            string output = JsonConvert.SerializeObject(spAcsContext);
            byte[] bytes = new byte[output.Length * sizeof(char)];
            System.Buffer.BlockCopy(output.ToCharArray(), 0, bytes, 0, bytes.Length);
            httpContext.Session.Set(SPContextKey, bytes);
        }
    }

    /// <summary>
    /// SharePointSessionData is DTO that would map serialized / deserialized data form / in the session
    /// </summary>
    public class SharePointSessionData
    {
        public Uri SpHostUrl { get; set; }
        public Uri SpAppWebUrl { get; set; }
        public string SpLanguage { get; set; }
        public string SpClientTag { get; set; }
        public string SpProductNumber { get; set; }
        public string ContextToken { get; set; }
    }

    #endregion ACS

    #region HighTrust
    //TODO: still to be implemented...

    /// <summary>
    /// Encapsulates all the information from SharePoint in HighTrust mode.
    /// </summary>
    //public class SharePointHighTrustContext : SharePointContext
    //{
    //    private readonly WindowsIdentity logonUserIdentity;

    //    /// <summary>
    //    /// The Windows identity for the current user.
    //    /// </summary>
    //    public WindowsIdentity LogonUserIdentity
    //    {
    //        get { return this.logonUserIdentity; }
    //    }

    //    public override string UserAccessTokenForSPHost
    //    {
    //        get
    //        {
    //            return GetAccessTokenString(ref this.userAccessTokenForSPHost,
    //                                        () => TokenHandler.GetS2SAccessTokenWithWindowsIdentity(this.SPHostUrl, this.LogonUserIdentity));
    //        }
    //    }

    //    public override string UserAccessTokenForSPAppWeb
    //    {
    //        get
    //        {
    //            if (this.SPAppWebUrl == null)
    //            {
    //                return null;
    //            }

    //            return GetAccessTokenString(ref this.userAccessTokenForSPAppWeb,
    //                                        () => TokenHandler.GetS2SAccessTokenWithWindowsIdentity(this.SPAppWebUrl, this.LogonUserIdentity));
    //        }
    //    }

    //    public override string AppOnlyAccessTokenForSPHost
    //    {
    //        get
    //        {
    //            return GetAccessTokenString(ref this.appOnlyAccessTokenForSPHost,
    //                                        () => TokenHandler.GetS2SAccessTokenWithWindowsIdentity(this.SPHostUrl, null));
    //        }
    //    }

    //    public override string AppOnlyAccessTokenForSPAppWeb
    //    {
    //        get
    //        {
    //            if (this.SPAppWebUrl == null)
    //            {
    //                return null;
    //            }

    //            return GetAccessTokenString(ref this.appOnlyAccessTokenForSPAppWeb,
    //                                        () => TokenHandler.GetS2SAccessTokenWithWindowsIdentity(this.SPAppWebUrl, null));
    //        }
    //    }

    //    public SharePointHighTrustContext(Uri spHostUrl, Uri spAppWebUrl, string spLanguage, string spClientTag, string spProductNumber, WindowsIdentity logonUserIdentity)
    //        : base(spHostUrl, spAppWebUrl, spLanguage, spClientTag, spProductNumber)
    //    {
    //        if (logonUserIdentity == null)
    //        {
    //            throw new ArgumentNullException("logonUserIdentity");
    //        }

    //        this.logonUserIdentity = logonUserIdentity;
    //    }

    //    /// <summary>
    //    /// Ensures the access token is valid and returns it.
    //    /// </summary>
    //    /// <param name="accessToken">The access token to verify.</param>
    //    /// <param name="tokenRenewalHandler">The token renewal handler.</param>
    //    /// <returns>The access token string.</returns>
    //    private static string GetAccessTokenString(ref Tuple<string, DateTime> accessToken, Func<string> tokenRenewalHandler)
    //    {
    //        RenewAccessTokenIfNeeded(ref accessToken, tokenRenewalHandler);

    //        return IsAccessTokenValid(accessToken) ? accessToken.Item1 : null;
    //    }

    //    /// <summary>
    //    /// Renews the access token if it is not valid.
    //    /// </summary>
    //    /// <param name="accessToken">The access token to renew.</param>
    //    /// <param name="tokenRenewalHandler">The token renewal handler.</param>
    //    private static void RenewAccessTokenIfNeeded(ref Tuple<string, DateTime> accessToken, Func<string> tokenRenewalHandler)
    //    {
    //        if (IsAccessTokenValid(accessToken))
    //        {
    //            return;
    //        }

    //        DateTime expiresOn = DateTime.UtcNow.Add(TokenHandler.HighTrustAccessTokenLifetime);

    //        if (TokenHandler.HighTrustAccessTokenLifetime > AccessTokenLifetimeTolerance)
    //        {
    //            // Make the access token get renewed a bit earlier than the time when it expires
    //            // so that the calls to SharePoint with it will have enough time to complete successfully.
    //            expiresOn -= AccessTokenLifetimeTolerance;
    //        }

    //        accessToken = Tuple.Create(tokenRenewalHandler(), expiresOn);
    //    }
    //}

    /// <summary>
    /// Default provider for SharePointHighTrustContext.
    /// </summary>
    //public class SharePointHighTrustContextProvider : SharePointContextProvider
    //{
    //    private const string SPContextKey = "SPContext";

    //    protected override SharePointContext CreateSharePointContext(Uri spHostUrl, Uri spAppWebUrl, string spLanguage, string spClientTag, string spProductNumber, HttpRequest httpRequest)
    //    {
    //        WindowsIdentity logonUserIdentity = httpRequest.LogonUserIdentity;
    //        if (logonUserIdentity == null || !logonUserIdentity.IsAuthenticated || logonUserIdentity.IsGuest || logonUserIdentity.User == null)
    //        {
    //            return null;
    //        }

    //        return new SharePointHighTrustContext(spHostUrl, spAppWebUrl, spLanguage, spClientTag, spProductNumber, logonUserIdentity);
    //    }

    //    protected override bool ValidateSharePointContext(SharePointContext spContext, HttpContext httpContext)
    //    {
    //        SharePointHighTrustContext spHighTrustContext = spContext as SharePointHighTrustContext;

    //        if (spHighTrustContext != null)
    //        {
    //            Uri spHostUrl = SharePointContext.GetSPHostUrl(httpContext.Request);
    //            WindowsIdentity logonUserIdentity = httpContext.Request.LogonUserIdentity;

    //            return spHostUrl == spHighTrustContext.SPHostUrl &&
    //                   logonUserIdentity != null &&
    //                   logonUserIdentity.IsAuthenticated &&
    //                   !logonUserIdentity.IsGuest &&
    //                   logonUserIdentity.User == spHighTrustContext.LogonUserIdentity.User;
    //        }

    //        return false;
    //    }

    //    //protected override SharePointContext LoadSharePointContext(HttpContext httpContext)
    //    //{
    //    //    var session = httpContext.Session as Microsoft.AspNet.Http.Features.ISession;
    //    //    return httpContext.Session[SPContextKey] as SharePointHighTrustContext;
    //    //}

    //    //protected override void SaveSharePointContext(SharePointContext spContext, HttpContext httpContext)
    //    //{
    //    //    httpContext.Session[SPContextKey] = spContext as SharePointHighTrustContext;
    //    //}
    //}

    #endregion HighTrust
}
