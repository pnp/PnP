using System;
using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.EventReceivers;

namespace Contoso.Core.AppPartPropertyUIOverrideWeb.Services
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

                    // setup helper classes
                    HostWebManager hostWebManager = new HostWebManager("ContosoAppPartPropertyUIOverride", clientContext);

                    // now see which app event we are in

                    switch (properties.EventType)
                    {
                        case SPRemoteEventType.AppInstalled:

                            // the app for SharePoint was installed
                            // setup helper class
                            AppPartPropertyUIOverrider overrider = new AppPartPropertyUIOverrider(hostWebManager, properties, "jquery-2.1.0.min.js");

                            // do the actual App Part property UI overrides
                            overrider.OverrideAppPartPropertyUI("Custom Category 1", "Contoso.OverrideExample.js");

                            break;
                        case SPRemoteEventType.AppUninstalling:

                            // the app for SharePoint is uninstalling
                            // uninstall all app-specific assets that were deployed (global assets are left for safety)
                            hostWebManager.UninstallAssets();

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

    }
}
