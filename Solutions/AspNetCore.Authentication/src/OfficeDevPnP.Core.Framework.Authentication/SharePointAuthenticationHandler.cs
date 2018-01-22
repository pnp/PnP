namespace OfficeDevPnP.Core.Framework.Authentication
{
    using Microsoft.AspNetCore.Authentication;
    using System;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using System.Linq;
    using System.Net.Http;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Logging;
    using System.Text.Encodings.Web;
    using Microsoft.Extensions.Options;
    using OfficeDevPnP.Core.Framework.Authentication.Events;

    /// <summary>
    /// Handles the authentication mechanism for SP Provider Hosted Apps
    /// </summary>
    public class SharePointAuthenticationHandler : RemoteAuthenticationHandler<SharePointAuthenticationOptions>, IAuthenticationSignOutHandler 
    {        
        protected HttpClient Backchannel => Options.Backchannel;

        /// <summary>
        /// The handler calls methods on the events which give the application control at certain points where processing is occurring. 
        /// If it is not provided a default instance is supplied which does nothing when the methods are called.
        /// </summary>
        protected new SharePointAuthenticationEvents Events
        {
            get { return (SharePointAuthenticationEvents)base.Events; }
            set { base.Events = value; }
        }

        public SharePointAuthenticationHandler(IOptionsMonitor<SharePointAuthenticationOptions> options, ILoggerFactory logger, UrlEncoder urlEncoder, ISystemClock clock)
            : base(options, logger, urlEncoder, clock)
        { }

        /// <summary>
        /// Creates a new instance of the events instance.
        /// </summary>
        /// <returns>A new instance of the events instance.</returns>
        protected override Task<object> CreateEventsAsync() => Task.FromResult<object>(new SharePointAuthenticationEvents());

        protected override async Task<HandleRequestResult> HandleRemoteAuthenticateAsync()
        {            
            //Set the default error message when no SP Auth is attempted
            HandleRequestResult result = HandleRequestResult.Fail("Could not handle SharePoint authentication.");
            
            // Sets up the SharePoint configuration based on the middleware options.
            var spContextProvider = SharePointContextProvider.GetInstance(
                SharePointConfiguration.GetFromSharePointAuthenticationOptions(Options));

            switch (SharePointContextProvider.CheckRedirectionStatus(Context, out Uri redirectUrl))
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
                        identity.IsAuthenticated && identity.HasClaim(x => x.Issuer == ClaimsIssuer)))
                    {
                        principal = Context.User;
                    }
                    else
                    {
                        //build a claims identity and principal
                        var identity = new ClaimsIdentity(Scheme.Name);

                        // Adds claims with the SharePoint context CacheKey as issuer to the Identity object.
                        var claims = new[]
                        {
                            new Claim(ClaimTypes.Authentication, userCacheKey, "SPCacheKey", ClaimsIssuer)
                        };

                        identity.AddClaims(claims);

                        principal = new ClaimsPrincipal(identity);
                        
                        //Call sign in middleware, defaults to the cookie middleware (if set up) so it issues a cookie, can be overriden
                        await HandleSignInAsync(principal);
                    }

                    // Creates the authentication ticket.
                    var ticket = new AuthenticationTicket(principal, Options.AuthenticationProperties, Options.SignInScheme);
                    result = HandleRequestResult.Success(ticket);

                    //Throw auth ticket success event
                    await Events.AuthenticationSucceeded(
                        new AuthenticationSucceededContext(Context, Scheme, Options, Options.AuthenticationProperties)
                        {
                            Ticket = ticket, //pass the ticket 
                            SharePointContext = spContext //append the sp context
                        });

                    //Log success
                    LoggingExtensions.TokenValidationSucceeded(Logger);

                    break;
                case RedirectionStatus.ShouldRedirect:
                    Response.StatusCode = 301;
                    result = HandleRequestResult.Fail("ShouldRedirect");

                    // Signs out so new signin to be performed on redirect back from SharePoint
                    await Context.SignOutAsync(Scheme.Name);

                    // Redirect to get new context token
                    Context.Response.Redirect(redirectUrl.AbsoluteUri);
                    break;
                case RedirectionStatus.CanNotRedirect:
                    result = HandleRequestResult.Fail("No SPHostUrl to build a SharePoint Context, but Authenticate was called on the SharePoint middleware.");

                    //Log that we cannot redirect
                    LoggingExtensions.CannotRedirect(Logger);

                    //Throw failed event
                    await Events.AuthenticationFailed(new AuthenticationFailedContext(Context, Scheme, Options, Options.AuthenticationProperties));

                    break;
            }

            return result;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            return await HandleRemoteAuthenticateAsync();
        }

        protected virtual async Task HandleSignInAsync(ClaimsPrincipal principal)
        {
            //sign in the cookie middleware so it issues a cookie
            if (!string.IsNullOrWhiteSpace(Options.CookieAuthenticationScheme))
            {
                await Context.SignInAsync(Options.CookieAuthenticationScheme, principal, Options.AuthenticationProperties);
            }
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
                user.Identities.Any(i => i.AuthenticationType == Scheme.Name);
                

            if (!userIsAnonymous && userIsAuthenticatedWithSharePoint) return false; //do not re-authenticate if authenticated
            
            if (!await ShouldHandleRequestAsync())
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
        
        public override Task<bool> ShouldHandleRequestAsync()
            => Task.FromResult(RequestFromSharePoint(Request) || SPCacheKeyCookieExists(Request));

        /// <summary>
        /// Checks if the incoming request is coming from SharePoint for the purpose of Add-in authentication
        /// </summary>
        /// <param name="request">The HttpRequest object for this request</param>
        /// <returns>True if coming from SharePoint</returns>
        private bool RequestFromSharePoint(HttpRequest request) => request.QueryString.HasValue && request.QueryString.Value.ToLowerInvariant().Contains("sphosturl");


        /// <summary>
        /// Checks if the current request contains the SPCacheKey Cookie
        /// </summary>
        /// <param name="request">The HttpRequest object for this request</param>
        /// <returns>True if Cookie with the provided key is found.</returns>
        private bool SPCacheKeyCookieExists(HttpRequest request) => request.Cookies.ContainsKey(Options.SPCacheKeyKey);

        public async Task SignOutAsync(AuthenticationProperties properties)
        {
            if (!string.IsNullOrWhiteSpace(Options.CookieAuthenticationScheme))
            {
                await Context.SignOutAsync(Options.CookieAuthenticationScheme, properties);
            }
        }
    }
}