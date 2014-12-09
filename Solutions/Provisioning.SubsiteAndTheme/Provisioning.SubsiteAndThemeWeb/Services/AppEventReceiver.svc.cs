using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.EventReceivers;
using OfficeDevPnP.Core.Entities;
using System.Web;
using System.ServiceModel;

namespace Provisioning.SubsiteAndThemeWeb.Services {
    public class AppEventReceiver : IRemoteEventService {
        const string CUSTOM_ACTION_NAME = "CREATE_NEW_SITE_SCRIPT_OVERRIDE";

        /// <summary>
        /// Handles app events that occur after the app is installed or upgraded, or when app is being uninstalled.
        /// </summary>
        /// <param name="properties">Holds information about the app event.</param>
        /// <returns>Holds information returned from the app event.</returns>
        public SPRemoteEventResult ProcessEvent(SPRemoteEventProperties properties) {
            SPRemoteEventResult result = new SPRemoteEventResult();

            using (ClientContext clientContext = TokenHelper.CreateAppEventClientContext(properties, useAppWeb: false)) {
                if (clientContext != null) {
                    var web = clientContext.Web;
                    clientContext.Load(web);
                    clientContext.ExecuteQuery();

                    // get the custom actions and look for the 
                    var customActions = web.GetCustomActions();
                    var target = customActions.FirstOrDefault(ca => ca.Name.Equals(CUSTOM_ACTION_NAME));
                    var host = OperationContext.Current.Host.BaseAddresses.First(
                                h => h.Scheme.Equals("https", StringComparison.OrdinalIgnoreCase));

                    // APP INSTALLED
                    if (properties.EventType == SPRemoteEventType.AppInstalled && target == null) {
                        var providerHostUrl = host.GetLeftPart(UriPartial.Scheme | UriPartial.Authority);
                        var siteUrlEscaped = Uri.EscapeUriString(properties.AppEventProperties.HostWebFullUrl.ToString());

                        // create the script block
                        var script = new StringBuilder();
                        script.Append("_spBodyOnLoadFunctions.push(function() { ");
                        script.AppendFormat("var newSiteLink = document.getElementById(\"createnewsite\"); if (newSiteLink != null) newSiteLink.href=\"{0}/Pages/NewSite.aspx?SPHostUrl={1}\";", providerHostUrl, siteUrlEscaped);
                        script.Append("});");

                        // add the custom action
                        web.AddCustomAction(new CustomActionEntity() {
                            ScriptBlock = script.ToString(),
                            Location = "ScriptLink",
                            Name = CUSTOM_ACTION_NAME
                        });
                    }
                    // APP UNINSTALLING
                    else if (properties.EventType == SPRemoteEventType.AppUninstalling && target != null) {
                        web.DeleteCustomAction(target.Id);
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// This method is a required placeholder, but is not used by app events.
        /// </summary>
        /// <param name="properties">Unused.</param>
        public void ProcessOneWayEvent(SPRemoteEventProperties properties) {
            
        }
    }
}
