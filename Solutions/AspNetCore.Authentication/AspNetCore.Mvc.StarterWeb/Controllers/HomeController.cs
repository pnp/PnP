using Microsoft.AspNet.Mvc;
using Microsoft.SharePoint.Client;
using OfficeDevPnP.Core.Framework.Authentication;

namespace AspNet5.Mvc6.StarterWeb.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            //get a SharePoint context
            var spContext = SharePointContextProvider.Current.GetSharePointContext(HttpContext);

            if (spContext != null)
            {
                //build a client context to work with data
                using (var clientContext = spContext.CreateUserClientContextForSPHost())
                {
                    if (clientContext != null)
                    {
                        User spUser = clientContext.Web.CurrentUser;

                        clientContext.Load(spUser, user => user.Title);

                        clientContext.ExecuteQuery();

                        ViewBag.UserName = spUser.Title;
                    }
                }
            }
            else //no SP Context, which means there is no SP auth tokens and handshaking
            {
                ViewData["Message"] = "You must access this app from SharePoint.";
                ViewBag.UserName = "Unauthorised."; 
            }

            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";
            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";
            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
