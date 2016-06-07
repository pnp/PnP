using Microsoft.AspNet.Authentication;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Http.Authentication;
using Microsoft.AspNet.Http.Features.Authentication;
using System.Linq;

namespace OfficeDevPnP.Core.Framework.Authentication
{
    /// <summary>
    /// Handles the authentication mechanism for SP Provider Hosted Apps
    /// </summary>
    public class SharePointAuthenticationHandler : AuthenticationHandler<SharePointAuthenticationOptions>
    {
        /// <summary>
        /// RedirectionStatus would decide if we contunue to the next middleware in the pipe.
        /// </summary>
        private RedirectionStatus _redirectionStatus;

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            Uri redirectUrl;

            //Set the default error message when no SP Auth is attempted
            AuthenticateResult result = AuthenticateResult.Failed("Could not handle SharePoint authentication.");

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
                    _redirectionStatus = RedirectionStatus.Ok;

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
                    await Options.Events.AuthenticationSucceeded(
                        new Events.AuthenticationSucceededContext(Context, Options)
                    {
                        AuthenticationTicket = ticket, //pass the ticket 
                        SharePointContext = spContext //append the sp context
                    });

                    //Log success
                    LoggingExtensions.TokenValidationSucceeded(this.Logger);

                    break;
                case RedirectionStatus.ShouldRedirect:
                    _redirectionStatus = RedirectionStatus.ShouldRedirect;

                    Response.StatusCode = 301;
                    result = AuthenticateResult.Failed("ShouldRedirect");

                    // Signs out so new signin to be performed on redirect back from SharePoint
                    await Context.Authentication.SignOutAsync(this.Options.AuthenticationScheme);

                    // Redirect to get new context token
                    Context.Response.Redirect(redirectUrl.AbsoluteUri);
                    break;
                case RedirectionStatus.CanNotRedirect:
                    _redirectionStatus = RedirectionStatus.CanNotRedirect;

                    //There's a potential issue here if this is not the only auth middleware
                    //setting the response to 401 might not be safe for the other middlewares
                    Response.StatusCode = 401;
                    result = AuthenticateResult.Failed("CanNotRedirect");
                    
                    //Log that we cannot redirect
                    LoggingExtensions.CannotRedirect(this.Logger);

                    //Throw failed event
                    await Options.Events.AuthenticationFailed(new Events.AuthenticationFailedContext(Context, Options));

                    break;
            }

            return result;
        }
         
        protected override async Task HandleSignInAsync(SignInContext context)
        { 
            //no real need to call base as it doesn't do anything
            await base.HandleSignInAsync(context); 
            SignInAccepted = true; 
        } 
        
        /// <summary>
        /// Fires on each request, allowing the capture of ShouldRedirect
        /// </summary>
        /// <returns></returns>
        public override async Task<bool> HandleRequestAsync()
        {
            if (_redirectionStatus == RedirectionStatus.ShouldRedirect)
            {
                // Stops the execution of next middlewares since redirect to SharePoint is required.
                return await Task.FromResult(true);
            }

            return await base.HandleRequestAsync();
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
                await base.HandleSignOutAsync(context);
            }

            SignOutAccepted = true;
        }
    }
}
