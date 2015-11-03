using BusinessApps.RemoteCalendarAccess.Models;
using BusinessApps.RemoteCalendarAccessWeb.BusinessLayer.Logic;
using BLM = BusinessApps.RemoteCalendarAccessWeb.BusinessLayer.Model;
using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BusinessApps.RemoteCalendarAccessWeb.Utils;
using Microsoft.Azure.ActiveDirectory.GraphClient;
using BusinessApps.RemoteCalendarAccess.Models.CalendarModel;

namespace BusinessApps.RemoteCalendarAccessWeb.Controllers
{
    public class HomeController : Controller
    {
        [SharePointContextFilter]
        public ActionResult URL()
        {
            var spContext = SharePointContextProvider.Current.GetSharePointContext(HttpContext);
            using (var clientContext = spContext.CreateUserClientContextForSPHost())
            {
                clientContext.Load(clientContext.Web, w => w.CurrentUser.Email);
                clientContext.Load(clientContext.Web, w => w.Url);
                clientContext.ExecuteQuery();

                RemoteCalendarAccessManager manager = new RemoteCalendarAccessManager();
                BLM.RemoteCalendarAccess rca = manager.AddRemoteCalendarAccess(Guid.Parse(HttpContext.Request.QueryString["SPListId"]),
                                                                                clientContext.Web.Url,
                                                                                clientContext.Web.CurrentUser.Email);

                ICSURLViewModel model = new ICSURLViewModel();
                model.URL = Request.Url.Scheme + "://" + Request.Url.Authority + Url.Content("~") + "?Id=" + rca.ID;
                return View(model);
            }                  
        }

        public FileResult Index(Guid? Id)
        {
            if (Id == null)
                return AccessDenied();

            RemoteCalendarAccessManager manager = new RemoteCalendarAccessManager();
            BLM.RemoteCalendarAccess remoteCalendarAccess = manager.GetRemoteCalendarAccess(Id.Value);

            if (remoteCalendarAccess == null)
                return AccessDenied();

            AzureActiveDirectory azureAD = new AzureActiveDirectory();

            IUser user = null;
            try
            {
                user = azureAD.GetUser(remoteCalendarAccess.UserId).Result;
            }
            catch (AggregateException e)
            {
                if (!e.InnerExceptions.Any(i => i.Message == "User " + remoteCalendarAccess.UserId + " not found."))
                    throw;
            }

            if (user == null || user.AccountEnabled == false)
                return AccessDenied();

            manager.UpdateLastAccessTime(remoteCalendarAccess);

            Uri uri = new Uri(remoteCalendarAccess.SiteAddress);
            string realm = TokenHelper.GetRealmFromTargetUrl(uri);
            var token = TokenHelper.GetAppOnlyAccessToken("00000003-0000-0ff1-ce00-000000000000", uri.Authority, realm);
            ClientContext clientContext = TokenHelper.GetClientContextWithAccessToken(uri.ToString(), token.AccessToken);

            clientContext.Load(clientContext.Web.Lists);
            clientContext.ExecuteQuery();

            List list = clientContext.Web.Lists.Where(l => l.Id == remoteCalendarAccess.CalendarId).First();

            if (list == null)
                return AccessDenied();

            ListItemCollection items = list.GetItems(CamlQuery.CreateAllItemsQuery());
            clientContext.Load(items);

            clientContext.Load(clientContext.Web);
            clientContext.Load(clientContext.Web.RegionalSettings);
            clientContext.Load(clientContext.Web.RegionalSettings.TimeZone);
            clientContext.Load(clientContext.Web, w => w.Title);

            clientContext.ExecuteQuery();

            Calendar calendar = new Calendar();
            calendar.Title = clientContext.Web.Title + " - " + list.Title;
            calendar.Timezone = Timezone.Parse(clientContext.Web.RegionalSettings.TimeZone.Description);
            calendar.Events = items.Select(i => Event.Parse(i)).ToList<Event>();

            FileContentResult result = File(System.Text.Encoding.Default.GetBytes(calendar.ToString()), "text/calendar", "calendar.ics");

            return result;
        }

        private FileResult AccessDenied()
        {
            Response.StatusCode = 403;
            return null;
        }
    }
}
