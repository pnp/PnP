using Microsoft.IdentityModel.Clients.ActiveDirectory;
using OutlookNotificationsAPI.Models;
using OutlookNotificationsAPI.WebAPI.Models;
using System;
using System.Configuration;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace OutlookNotificationsAPI.WebAPI.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private string clientId = ConfigurationManager.AppSettings["ida:ClientId"];
        private string appKey = ConfigurationManager.AppSettings["ida:ClientSecret"];
        private string aadInstance = ConfigurationManager.AppSettings["ida:AADInstance"];
        private string outlookResourceID = "https://outlook.office.com/";

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Register(string notificationUrl)
        {
            try
            {
                // Get an access token to use when calling the Outlook REST APIs.
                var token = await GetTokenForApplication();
                var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                // Create and send a subscription message for newly created calendar events.
                var response = await httpClient.PostAsJsonAsync(outlookResourceID + "api/v2.0/me/subscriptions", new PushSubscriptionModel
                {
                    NotificationURL = notificationUrl,
                    Resource = outlookResourceID + "api/v2.0/me/events",
                    ChangeType = "Created",
                    ClientState = Guid.NewGuid()
                });

                if (!response.IsSuccessStatusCode)
                {
                    // Return to error page.
                    return View("Error");
                }
                return View("Success");
            }
            catch (AdalException)
            {
                // Return to error page.
                return View("Error");
            }
            // If the above failed, the user needs to explicitly re-authenticate for 
            // the app to obtain the required token.
            catch (Exception)
            {
                return View("Relogin");
            }
        }

        public async Task<string> GetTokenForApplication()
        {
            string signedInUserID = ClaimsPrincipal.Current.FindFirst(ClaimTypes.NameIdentifier).Value;
            string tenantID = ClaimsPrincipal.Current.FindFirst("http://schemas.microsoft.com/identity/claims/tenantid").Value;
            string userObjectID = ClaimsPrincipal.Current.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier").Value;

            // Get a token for the Graph without triggering any user 
            // interaction (from the cache, via multi-resource refresh token, etc.).
            ClientCredential clientcred = new ClientCredential(clientId, appKey);

            // Initialize AuthenticationContext with the token cache of 
            // the currently signed in user, as kept in the app's database.
            AuthenticationContext authenticationContext = new AuthenticationContext(aadInstance + tenantID,
                new ADALTokenCache(signedInUserID));
            AuthenticationResult authenticationResult = await authenticationContext.AcquireTokenSilentAsync(outlookResourceID, 
                clientcred, new UserIdentifier(userObjectID, UserIdentifierType.UniqueId));
            return authenticationResult.AccessToken;
        }
    }
}