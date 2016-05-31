using BusinessApps.O365ProjectsApp.Infrastructure.MicrosoftGraph;
using Microsoft.SharePoint.Client;
using OfficeDevPnP.Core;
using OfficeDevPnP.Core.Framework.Provisioning.Model;
using OfficeDevPnP.Core.Framework.Provisioning.ObjectHandlers;
using OfficeDevPnP.Core.Framework.Provisioning.Providers.Xml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BusinessApps.O365ProjectsApp.Infrastructure
{
    public static class SPORemoteActions
    {
        public static void ProvisionArtifactsByCode()
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
                }
                else
                {
                    targetLibrary = web.GetListByTitle(O365ProjectsAppSettings.LibraryTitle);
                }

                // If the target library exists
                if (targetLibrary != null)
                {
                    // Try to get the user's custom action
                    UserCustomAction customAction = targetLibrary.GetCustomAction(O365ProjectsAppConstants.ECB_Menu_Name);

                    // If it doesn't exist
                    if (customAction == null)
                    {
                        // Add the user custom action to the list
                        customAction = targetLibrary.UserCustomActions.Add();
                        customAction.Name = O365ProjectsAppConstants.ECB_Menu_Name;
                        customAction.Location = "EditControlBlock";
                        customAction.Sequence = 100;
                        customAction.Title = "Manage Business Project";
                        customAction.Url = $"{O365ProjectsAppContext.CurrentAppSiteUrl}Project/?SiteUrl={{SiteUrl}}&ListId={{ListId}}&ItemId={{ItemId}}&ItemUrl={{ItemUrl}}";
                    }
                    else
                    {
                        // Update the already existing Custom Action
                        customAction.Name = O365ProjectsAppConstants.ECB_Menu_Name;
                        customAction.Location = "EditControlBlock";
                        customAction.Sequence = 100;
                        customAction.Title = "Manage Business Project";
                        customAction.Url = $"{O365ProjectsAppContext.CurrentAppSiteUrl}Project/?SiteUrl={{SiteUrl}}&ListId={{ListId}}&ItemId={{ItemId}}&ItemUrl={{ItemUrl}}";
                    }

                    customAction.Update();
                    context.ExecuteQueryRetry();

                }
            }
        }

        public static void ProvisionArtifactsByTemplate()
        {
            // Create a PnP AuthenticationManager object
            AuthenticationManager am = new AuthenticationManager();

            // Authenticate against SPO with an App-Only access token
            using (ClientContext context = am.GetAzureADAppOnlyAuthenticatedContext(
                O365ProjectsAppContext.CurrentSiteUrl, O365ProjectsAppSettings.ClientId,
                O365ProjectsAppSettings.TenantId, O365ProjectsAppSettings.AppOnlyCertificate))
            {
                Web web = context.Web;

                // Load the template from the file system
                XMLTemplateProvider provider =
                new XMLFileSystemTemplateProvider(
                    String.Format(HttpContext.Current.Server.MapPath(".")),
                    "ProvisioningTemplates");

                ProvisioningTemplate template = provider.GetTemplate("O365ProjectsAppSite.xml");

                // Configure the AppSiteUrl parameter
                template.Parameters["AppSiteUrl"] = O365ProjectsAppContext.CurrentAppSiteUrl;

                // Apply the template to the target site
                template.Connector = provider.Connector;
                web.ApplyProvisioningTemplate(template);
            }
        }

        public static void BrowseFilesLibrary()
        {
            // Create a PnP AuthenticationManager object
            AuthenticationManager am = new AuthenticationManager();

            // Authenticate against SPO with a delegated access token
            using (ClientContext context = am.GetAzureADWebApplicationAuthenticatedContext(
                O365ProjectsAppContext.CurrentSiteUrl, (url) => {
                    return (MicrosoftGraphHelper.GetAccessTokenForCurrentUser(url));
                }))
            {
                Web web = context.Web;
                var targetLibrary = web.GetListByTitle(O365ProjectsAppSettings.LibraryTitle);

                context.Load(targetLibrary.RootFolder, 
                    fld => fld.ServerRelativeUrl,
                    fld => fld.Files.Include(f => f.Title, f => f.ServerRelativeUrl));
                context.ExecuteQueryRetry();

                foreach (var file in targetLibrary.RootFolder.Files)
                {
                    // Handle each file object ... this is just a sample ...
                }
            }
        }
    }
}