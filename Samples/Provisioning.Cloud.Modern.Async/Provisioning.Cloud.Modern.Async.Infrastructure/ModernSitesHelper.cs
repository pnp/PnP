using Microsoft.SharePoint.Client;
using Newtonsoft.Json;
using OfficeDevPnP.Core;
using OfficeDevPnP.Core.Framework.Provisioning.ObjectHandlers;
using OfficeDevPnP.Core.Framework.Provisioning.Providers.Xml;
using OfficeDevPnP.Core.Sites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Provisioning.Cloud.Modern.Async.Infrastructure
{
    public static class ModernSitesHelper
    {
        public static void CreateModernSite(String json, String templatesPath, Action<String> logInfo = null, Action<String> logError = null)
        {
            var modernSite = JsonConvert.DeserializeObject<ModernSiteCreation>(json);
            if (modernSite != null)
            {
                logInfo?.Invoke($"Processing \"modern\" site creation for site {modernSite.SiteAlias}");

                AuthenticationManager authManager = new AuthenticationManager();
                using (var context = authManager.GetAzureADAccessTokenAuthenticatedContext(
                    modernSite.SPORootSiteUrl, modernSite.UserAccessToken))
                {
                    String siteUrl = String.Empty;

                    switch (modernSite.SiteType)
                    {
                        case SiteType.CommunicationSite:

                            context.Web.EnsureProperty(w => w.Language);

                            siteUrl = context.CreateSiteAsync(new CommunicationSiteCollectionCreationInformation
                            {
                                Title = modernSite.SiteTitle,
                                Owner = modernSite.CurrentUserPrincipalName,
                                Lcid = context.Web.Language,
                                Description = modernSite.SiteDescription,
                                Url = $"{modernSite.SPORootSiteUrl}sites/{modernSite.SiteAlias}",
                            }).GetAwaiter().GetResult().Url;
                            break;
                        case SiteType.TeamSite:
                        default:
                            siteUrl = context.CreateSiteAsync(new TeamSiteCollectionCreationInformation
                            {
                                DisplayName = modernSite.SiteTitle,
                                Description = modernSite.SiteDescription,
                                Alias = modernSite.SiteAlias,
                                IsPublic = modernSite.IsPublic,
                            }).GetAwaiter().GetResult().Url;
                            break;
                    }

                    logInfo?.Invoke($"Created \"modern\" site with URL: {siteUrl}");

                    logInfo?.Invoke($"Applying provisioning template {modernSite.PnPTemplate} to site");

                    using (var siteContext = authManager.GetAzureADAccessTokenAuthenticatedContext(
                        siteUrl, modernSite.UserAccessToken))
                    {
                        var web = siteContext.Web;
                        siteContext.Load(web);
                        siteContext.ExecuteQueryRetry();

                        logInfo?.Invoke($"Getting templates from path: {templatesPath}");

                        XMLTemplateProvider provider =
                               new XMLFileSystemTemplateProvider(templatesPath, "Templates");
                        var template = provider.GetTemplate(modernSite.PnPTemplate);
                        template.Connector = provider.Connector;

                        var ptai = new ProvisioningTemplateApplyingInformation();
                        ptai.MessagesDelegate += delegate (string message, ProvisioningMessageType messageType) {
                            logInfo?.Invoke($"{messageType} - {message}");
                        };
                        ptai.ProgressDelegate += delegate (string message, int step, int total) {
                            logInfo?.Invoke($"{step:00}/{total:00} - {message}");
                        };

                        logInfo?.Invoke($"Provisioning Started: {DateTime.Now:hh.mm.ss}");
                        web.ApplyProvisioningTemplate(template, ptai);
                        logInfo?.Invoke($"Provisioning Completed: {DateTime.Now:hh.mm.ss}");
                    }
                }
            }
            else
            {
                logError?.Invoke($"Error processing \"modern\" site creation for site {modernSite.SiteAlias}! Invalid JSON request!");
            }
        }
    }
}
