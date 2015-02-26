using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.EventReceivers;
using System.Configuration;

namespace Core.SiteInformationWeb.Services
{
    public class AppEventReceiver : IRemoteEventService
    {
        /// <summary>
        /// Handles app events that occur after the app is installed or upgraded, or when app is being uninstalled.
        /// </summary>
        /// <param name="properties">Holds information about the app event.</param>
        /// <returns>Holds information returned from the app event.</returns>
        public SPRemoteEventResult ProcessEvent(SPRemoteEventProperties properties)
        {
            SPRemoteEventResult result = new SPRemoteEventResult();

            using (ClientContext clientContext = TokenHelper.CreateAppEventClientContext(properties, useAppWeb: false))
            {
                if (clientContext != null)
                {
                    clientContext.Load(clientContext.Web);
                    clientContext.ExecuteQuery();

                    switch (properties.EventType)
                    {
                        case SPRemoteEventType.AppInstalled:
                            RemoveQuickLaunchNode(clientContext);
                            AddSiteInformationJsLink(clientContext);
                            result.Status = SPRemoteEventServiceStatus.Continue;
                            break;
                        case SPRemoteEventType.AppUninstalling:
                            RemoveQuickLaunchNode(clientContext);
                            break;
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// This method is a required placeholder, but is not used by app events.
        /// </summary>
        /// <param name="properties">Unused.</param>
        public void ProcessOneWayEvent(SPRemoteEventProperties properties)
        {
            throw new NotImplementedException();
        }

        private void RemoveQuickLaunchNode(ClientContext clientContext)
        {
            //load web again... could be improved
            Web web = clientContext.Web;
            clientContext.Load(web);
            clientContext.ExecuteQuery();

            // Remove the entry from the 'Recents' node
            NavigationNodeCollection nodes = web.Navigation.QuickLaunch;
            clientContext.Load(nodes, n => n.IncludeWithDefaultProperties(c => c.Children));
            clientContext.ExecuteQuery();
            var recent = nodes.Where(x => x.Title == "Recent").FirstOrDefault();
            if (recent != null)
            {
                var appLink = recent.Children.Where(x => x.Title == "Core.SiteInformation").FirstOrDefault();
                if (appLink != null) appLink.DeleteObject();
                clientContext.ExecuteQuery();
            }
        }

        private void AddSiteInformationJsLink(Microsoft.SharePoint.Client.ClientContext clientContext)
        {
            Web web = clientContext.Web;
            clientContext.Load(web, w => w.UserCustomActions, w => w.Url, w => w.AppInstanceId);
            clientContext.ExecuteQuery();

            string issuerId = ConfigurationManager.AppSettings.Get("ClientId");

            DeleteExistingActions(clientContext, web);

            UserCustomAction userCustomAction = web.UserCustomActions.Add();
            userCustomAction.Location = "Microsoft.SharePoint.StandardMenu";
            userCustomAction.Group = "SiteActions";
            BasePermissions perms = new BasePermissions();
            perms.Set(PermissionKind.ManageWeb);
            userCustomAction.Rights = perms;
            userCustomAction.Sequence = 100;
            userCustomAction.Title = "Site Information";

            string realm = TokenHelper.GetRealmFromTargetUrl(new Uri(clientContext.Url));

            var appPageUrl = string.Format("https://{0}/Pages/Default.aspx?{{StandardTokens}}", System.Web.HttpContext.Current.Request.Url.Authority);
            string url = "javascript:LaunchApp('{0}', 'i:0i.t|ms.sp.ext|{1}@{2}','{3}', {{width:600,height:400,title:'Site Information'}});";
            url = string.Format(url, Guid.NewGuid().ToString(), issuerId, realm, appPageUrl);

            userCustomAction.Url = url;
            userCustomAction.Update();
            clientContext.ExecuteQuery();
        }

        private void DeleteExistingActions(Microsoft.SharePoint.Client.ClientContext clientContext, Web web)
        {
            string issuerId = ConfigurationManager.AppSettings.Get("ClientId");

            for (int i = 0; i < web.UserCustomActions.Count - 1; i++)
            {
                if (web.UserCustomActions[i].Url != null && web.UserCustomActions[i].Url.ToLowerInvariant().Contains(issuerId.ToLowerInvariant()))
                {
                    web.UserCustomActions[i].DeleteObject();
                }
            }
            clientContext.ExecuteQuery();
        }
    }
}
