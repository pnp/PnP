using Microsoft.Graph;
using MicrosoftGraph.Office365.DotNetSDK.WebApp.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace MicrosoftGraph.Office365.DotNetSDK.WebApp.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public async Task<ActionResult> About()
        {
            var graphClient = new GraphServiceClient(
                new DelegateAuthenticationProvider(
                    (requestMessage) =>
                    {
                        // Get back the access token.
                        var accessToken = ADALHelper.GetAccessTokenForCurrentUser();

                        if (!String.IsNullOrEmpty(accessToken))
                        {
                            // Configure the HTTP bearer Authorization Header
                            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("bearer", accessToken);
                        }
                        else
                        {
                            throw new Exception("Invalid authorization context");
                        }

                        return (Task.FromResult(0));
                    }
                    ));

            try
            {
                var me = await graphClient.Me.Request().Select("DisplayName").GetAsync();
                ViewBag.Message = me.DisplayName;
            }
            catch (Exception)
            {
                // Skip any exception, so far
            }

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}