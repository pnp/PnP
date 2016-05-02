using Microsoft.SharePoint.Client;
using OfficeDevPnP.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BusinessApps.O365ProjectsApp.WebApp.Components
{
    public static class ProvisionSPOArtifacts
    {
        public static void Provision()
        {
            // Create a PnP AuthenticationManager object
            AuthenticationManager am = new AuthenticationManager();

            // Authenticate against SPO with an App-Only access token
            using (ClientContext context = am.GetAzureADAppOnlyAuthenticatedContext(
                O365ProjectsAppContext.CurrentSiteUrl, O365ProjectsAppSettings.ClientId,
                O365ProjectsAppSettings.TenantId, O365ProjectsAppSettings.AppOnlyCertificate))
            {
                Web web = context.Web;
                List targetLibrary = null;

                // If the target library does not exist (PnP extension method)
                if (!web.ListExists(O365ProjectsAppSettings.LibraryTitle))
                {
                    // Create it using another PnP extension method
                    targetLibrary = web.CreateList(ListTemplateType.DocumentLibrary,
                        O365ProjectsAppSettings.LibraryTitle, true, true);

                    // Add the user custom action to the just created list
                    var customAction = targetLibrary.UserCustomActions.Add();
                    customAction.Location = "EditControlBlock";
                    customAction.Sequence = 100;
                    customAction.Title = "Manage Business Project";
                    customAction.Url = $"{O365ProjectsAppContext.CurrentAppSiteUrl}Project/?SiteUrl={{SiteUrl}}&ListId={{ListId}}&ItemId={{ItemId}}&ItemUrl={{ItemUrl}}";
                    customAction.Update();

                    context.ExecuteQueryRetry();
                }
                else
                {
                    targetLibrary = web.GetListByTitle(O365ProjectsAppSettings.LibraryTitle);
                }
            }
        }
    }
}