namespace OfficeDevPnP.Core.Framework.Authentication
{
    using Microsoft.AspNetCore.Authentication;
    using System;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http.Authentication;
    using Microsoft.AspNetCore.Http.Features.Authentication;
    using System.Linq;
    using System.Net.Http;
    using Microsoft.AspNetCore.Http;

    /// <summary>
    /// Handles the authentication mechanism for SP Provider Hosted Apps
    /// </summary>
    public class SharePointAuthenticationHandler : RemoteAuthenticationHandler<SharePointAuthenticationOptions>
    {
        private const string SPCacheKeyKey = "SPCacheKey";

        public SharePointAuthenticationHandler(HttpClient backchannel)
        {
            Backchannel = backchannel;
        }

        protected HttpClient Backchannel { get; private set; }

        protected override async Task<AuthenticateResult> HandleRemoteAuthenticateAsync()
        {
            Uri redirectUrl;

            if (string.IsNullOrEmpty(Options.ClientId)) return AuthenticateResult.Fail("ClientId is not configured in the appsettings.json file.");

            //Set the default error message when no SP Auth is attempted
            AuthenticateResult result = AuthenticateResult.Fail("Could not handle SharePoint authentication.");

            var authenticationProperties = new AuthenticationProperties()
            {
                ExpiresUtc = DateTimeOffset.UtcNow.AddDays(10),
                IsPersistent = false,
                AllowRefresh = false
            };

            // Sets up the SharePoint configuration based on the middleware options.
            var spContextProvider = SharePointContextProvider.GetInstance(
                SharePointConfiguration.GetFromSharePointAuthenticationOptions(Options));

            switch (SharePointContextProvider.CheckRedirectionStatus(Context, out redirectUrl))
            {
                case RedirectionStatus.Ok:
                    // Gets the current SharePoint context
                    var spContext = SharePointContextProvider.Current.GetSharePointContext(Context);

                    // Gets the SharePoint context CacheKey. The CacheKey would be assigned as issuer for new claim.
                    // It is also used to validate identity that is authenticated.
                    //Currently, we don't support High Trust
                    var userCacheKey = ((SharePointAcsContext)spContext).CacheKey;

                    // Checks if we already have an authenticated principal
                    ClaimsPrincipal principal;
                    if (Context.User.Identities.Any(identity =>
                        identity.IsAuthenticated && identity.HasClaim(x => x.Issuer == GetType().Assembly.GetName().Name)))
                    {
                        principal = Context.User;
                    }
                    else
                    {
                        //build a claims identity and principal
                        var identity = new ClaimsIdentity(this.Options.AuthenticationScheme);

                        // Adds claims with the SharePoint context CacheKey as issuer to the Identity object.
                        var claims = new[]
                        {
                            new Claim(ClaimTypes.Authentication, userCacheKey, "SPCacheKey",  GetType().Assembly.GetName().Name),
                        };

                        identity.AddClaims(claims);

                        principal = new ClaimsPrincipal(identity);

                        // Handles the sign in method of the SP auth middleware
                        await Context.Authentication.SignInAsync
                            (this.Options.AuthenticationScheme, principal, authenticationProperties);

                        //sign in the cookie middleware so it issues a cookie
                        if (!string.IsNullOrWhiteSpace(this.Options.CookieAuthenticationScheme))
                        {
                            SignInAccepted = true;
                            await Context.Authentication.SignInAsync
                                  (this.Options.CookieAuthenticationScheme, principal, authenticationProperties);
                        }
                    }

                    // Creates the authentication ticket.
                    var ticket = new AuthenticationTicket(principal, authenticationProperties, this.Options.AuthenticationScheme);
                    result = AuthenticateResult.Success(ticket);

                    //Throw auth ticket success event
                    await Options.SharePointAuthenticationEvents.AuthenticationSucceeded(
                        new Events.AuthenticationSucceededContext(Context, Options)
                        {
                            Ticket = ticket, //pass the ticket 
                            SharePointContext = spContext //append the sp context
                        });

                    //Log success
                    LoggingExtensions.TokenValidationSucceeded(this.Logger);

                    break;
                case RedirectionStatus.ShouldRedirect:
                    Response.StatusCode = 301;
                    result = AuthenticateResult.Fail("ShouldRedirect");

                    // Signs out so new signin to be performed on redirect back from SharePoint
                    await Context.Authentication.SignOutAsync(this.Options.AuthenticationScheme);

                    // Redirect to get new context token
                    Context.Response.Redirect(redirectUrl.AbsoluteUri);
                    break;
                case RedirectionStatus.CanNotRedirect:
                    result = AuthenticateResult.Fail("No SPHostUrl to build a SharePoint Context, but Authenticate was called on the SharePoint middleware.");

                    //Log that we cannot redirect
                    LoggingExtensions.CannotRedirect(this.Logger);

                    //Throw failed event
                    await Options.SharePointAuthenticationEvents.AuthenticationFailed(new Events.AuthenticationFailedContext(Context, Options));

                    break;
            }

            return result;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            //var baseResult = await base.HandleAuthenticateAsync();
            var baseRemoteResult = await HandleRemoteAuthenticateAsync();
            return baseRemoteResult;
        }

        protected override Task<bool> HandleRemoteCallbackAsync()
        {
            return base.HandleRemoteCallbackAsync();
        }

        protected override Task HandleSignInAsync(SignInContext context)
        {
            SignInAccepted = true;
            return Task.FromResult<object>(null);
        }

        /// <summary>
        /// Fires on each request, allowing the capture of ShouldRedirect
        /// </summary>
        /// <returns></returns>
        public override async Task<bool> HandleRequestAsync()
        {
            var user = Context.User;
            var userIsAnonymous =
                user?.Identity == null ||
                !user.Identities.Any(i => i.IsAuthenticated);

            var userIsAuthenticatedWithSharePoint =
                user.Identities.Any(i => i.AuthenticationType == GetType().Assembly.GetName().Name);

            if (!userIsAnonymous && userIsAuthenticatedWithSharePoint) return false; //do not re-authenticate if authenticated

            bool requestFromSharePoint = RequestFromSharePoint(Context);
            bool spCacheCookieExists = CookieExists(Context, SPCacheKeyKey);

            if (!spCacheCookieExists && !requestFromSharePoint)
            {
                //return Handled, but not authenticated...
                return false; // continue the middleware pipeline
            }

            if (userIsAnonymous)
            {
                await HandleRemoteAuthenticateAsync();
            }

            return false;
        }

        /// <summary>
        /// Overrides Sign Out logic
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        protected override async Task HandleSignOutAsync(SignOutContext context)
        {
            if (!string.IsNullOrWhiteSpace(this.Options.CookieAuthenticationScheme))
            {
                await Context.Authentication.SignOutAsync(this.Options.CookieAuthenticationScheme);
            }

            SignOutAccepted = true;
        }

        public new bool ShouldHandleScheme(string authenticationScheme, bool handleAutomatic)
        {
            return string.Equals(Options.AuthenticationScheme, authenticationScheme, StringComparison.Ordinal) ||
                (handleAutomatic && string.Equals(authenticationScheme, AuthenticationManager.AutomaticScheme, StringComparison.Ordinal));
        }

        /// <summary>
        /// Checks if the incoming request is coming from SharePoint for the purpose of Add-in authentication
        /// </summary>
        /// <param name="context">The HttpContext object with information about the request</param>
        /// <returns>True if coming from SharePoint</returns>
        private bool RequestFromSharePoint(HttpContext context)
        {
            var hasSPHostUrl = (context.Request.QueryString.HasValue && context.Request.QueryString.Value.ToLowerInvariant().Contains("sphosturl"));
            return hasSPHostUrl;
        }

        /// <summary>
        /// Checks if the current request contains an Cookie by key
        /// </summary>
        /// <param name="context">The HttpContext object with information about the request</param>
        /// <returns>True if Cookie with the provided key is found.</returns>
        private bool CookieExists(HttpContext context, string key)
        {
            var requestCookies = context.Request.Cookies;
            var cookieExists = requestCookies.ContainsKey(key);
            if (cookieExists)
            {
                return true;
            }
            return false;
        }
       
    }
}