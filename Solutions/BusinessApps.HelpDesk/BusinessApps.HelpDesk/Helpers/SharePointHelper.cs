using BusinessApps.HelpDesk.Models;
using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;

namespace BusinessApps.HelpDesk.Helpers
{
    public class SharePointHelper
    {
        public async Task<IEnumerable<Announcement>> GetAnnouncements()
        {
            ClientContext context = await GetClientContext(ConfigurationManager.AppSettings["OperationsWebsite"]);

            context.Load(context.Web);
            context.Load(context.Web.Lists);
            context.ExecuteQuery();

            List list = context.Web.Lists.Where(l => l.Title == "Announcements").FirstOrDefault();
            ListItemCollection listItems = list.GetItems(CamlQuery.CreateAllItemsQuery());

            context.Load(listItems);
            context.ExecuteQuery();

            return listItems.Select(l => new Announcement() { Title = l["Title"].ToString(), Timestamp = DateTime.Parse(l["Created"].ToString()) });
        }

        public async Task<ClientContext> GetClientContext(string url)
        {
            AuthenticationHelper authHelper = new AuthenticationHelper();

            ClientContext clientContext = TokenHelper.GetClientContextWithAccessToken(url, (await authHelper.GetToken(SettingsHelper.SharePointResource)).AccessToken);

            return clientContext;
        }
    }
}