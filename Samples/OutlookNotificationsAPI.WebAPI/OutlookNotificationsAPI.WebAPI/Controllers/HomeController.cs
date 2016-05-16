using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Newtonsoft.Json;
using OutlookNotificationsAPI.Models;
using OutlookNotificationsAPI.WebAPI.Models;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace OutlookNotificationsAPI.WebAPI.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
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
                var token = await TokenHelper.GetTokenForApplicationAsync(TokenHelper.OutlookResourceID);
                var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                // Create and send a subscription message for newly created calendar events.
                var response = await httpClient.PostAsJsonAsync(TokenHelper.OutlookResourceID + "api/v2.0/me/subscriptions",
                    new PushSubscriptionModel
                    {
                        NotificationURL = notificationUrl,
                        Resource = TokenHelper.OutlookResourceID + "api/v2.0/me/events",
                        ChangeType = "Created",
                        ClientState = Guid.NewGuid()
                    });

                if (!response.IsSuccessStatusCode)
                {
                    // Return to error page.
                    return View("Error");
                }

                // Deserialize the response.
                var responseString = await response.Content.ReadAsStringAsync();
                var subscription = JsonConvert.DeserializeObject<PushSubscriptionModel>(responseString);

                // Save the subscription ID to map with the current user.
                var entities = new ApplicationDbContext();
                entities.SubscriptionList.Add(new Subscription
                {
                    SubscriptionId = subscription.Id,
                    SubscriptionExpirationDateTime = subscription.SubscriptionExpirationDateTime,
                    SignedInUserID = ClaimsPrincipal.Current.FindFirst(ClaimTypes.NameIdentifier).Value,
                    TenantID = ClaimsPrincipal.Current.FindFirst("http://schemas.microsoft.com/identity/claims/tenantid").Value,
                    UserObjectID = ClaimsPrincipal.Current.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier").Value
                });
                await entities.SaveChangesAsync();

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
    }
}