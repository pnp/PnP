using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.EventReceivers;
using System.Web.Configuration;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Activation;

namespace Provisioning.SiteModifierWeb.Services
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
                    Web web = clientContext.Web;
                   
                    clientContext.Load(web, w => w.UserCustomActions);
                    clientContext.ExecuteQuery();
                    if (properties.EventType == SPRemoteEventType.AppInstalled)
                    {
                        clientContext.Load(web, w => w.UserCustomActions, w => w.Url, w => w.AppInstanceId);
                        clientContext.ExecuteQuery();

                        UserCustomAction userCustomAction = web.UserCustomActions.Add();
                        userCustomAction.Location = "Microsoft.SharePoint.StandardMenu";
                        userCustomAction.Group = "SiteActions";
                        BasePermissions perms = new BasePermissions();
                        perms.Set(PermissionKind.ManageWeb);
                        userCustomAction.Rights = perms;
                        userCustomAction.Sequence = 100;
                        userCustomAction.Title = "Modify Site";

                        string realm = TokenHelper.GetRealmFromTargetUrl(new Uri(clientContext.Url));
                        string issuerId = WebConfigurationManager.AppSettings.Get("ClientId");

                        var modifyPageUrl = string.Format("https://{0}/Pages/Modify.aspx?{{StandardTokens}}", GetHostUrl());
                        string url = "javascript:LaunchApp('{0}', 'i:0i.t|ms.sp.ext|{1}@{2}','{3}',{{width:300,height:200,title:'Modify Site'}});";
                        url = string.Format(url, Guid.NewGuid().ToString(), issuerId, realm, modifyPageUrl);

                        userCustomAction.Url = url;

                        userCustomAction.Update();

                        clientContext.ExecuteQuery();

                        // Remove the entry from the 'Recents' node
                        NavigationNodeCollection nodes = web.Navigation.QuickLaunch;
                        clientContext.Load(nodes, n => n.IncludeWithDefaultProperties(c => c.Children));
                        clientContext.ExecuteQuery();
                        var recent = nodes.Where(x => x.Title == "Recent").FirstOrDefault();
                        if (recent != null)
                        {
                            var appLink = recent.Children.Where(x => x.Title == "Site Modifier").FirstOrDefault();
                            if (appLink != null) appLink.DeleteObject();
                            clientContext.ExecuteQuery();
                        }


                    }
                    else if (properties.EventType == SPRemoteEventType.AppUninstalling)
                    {
                        foreach (var action in web.UserCustomActions)
                        {
                            if (action.Title == "Modify Site")
                            {
                                action.DeleteObject();
                                clientContext.ExecuteQuery();
                                break;
                            }
                        }
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

        private string GetHostUrl()
        {
            string url = string.Empty;
            OperationContext op = OperationContext.Current;

            foreach(Uri baseAddress in op.Host.BaseAddresses)
            {
                if(baseAddress.Scheme.Equals("https",StringComparison.OrdinalIgnoreCase))
                {
                    url = baseAddress.Authority;
                    break;
                }
            }

            return url;
        }
    }
}
