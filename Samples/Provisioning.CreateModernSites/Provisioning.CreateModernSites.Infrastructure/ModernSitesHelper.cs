using Microsoft.SharePoint.Client;
using Newtonsoft.Json;
using OfficeDevPnP.Core;
using OfficeDevPnP.Core.Framework.Provisioning.Providers.Xml;
using OfficeDevPnP.Core.Sites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Provisioning.CreateModernSites.Infrastructure
{
    public static class ModernSitesHelper
    {
        public static void CreateModernSite(String json, Action<String> logInfo = null, Action<String> logError = null)
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
                                Classification = modernSite.SiteClassification,
                                Url = $"{modernSite.SPORootSiteUrl}sites/{modernSite.SiteAlias}",
                            }).GetAwaiter().GetResult().Url;
                            break;
                        case SiteType.TeamSite:
                        default:
                            siteUrl = context.CreateSiteAsync(new TeamSiteCollectionCreationInformation
                            {
                                DisplayName = modernSite.SiteTitle,
                                Description = modernSite.SiteDescription,
                                Classification = modernSite.SiteClassification,
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

                        var currentPath = Environment.GetEnvironmentVariable("WEBJOBS_PATH");
                        logInfo?.Invoke($"Getting templates from path: {currentPath}");
#if DEBUG
                        if (String.IsNullOrEmpty(currentPath))
                        {
                            currentPath = AppDomain.CurrentDomain.BaseDirectory;
                            if (currentPath.ToLower().Contains("\\bin\\debug\\"))
                            {
                                currentPath = currentPath.Substring(0, currentPath.Length - 11);
                            }
                        }
#endif

                        XMLTemplateProvider provider =
                               new XMLFileSystemTemplateProvider(currentPath, "Templates");
                        var template = provider.GetTemplate(modernSite.PnPTemplate);
                        template.Connector = provider.Connector;

                        web.ApplyProvisioningTemplate(template);
                    }

                    logInfo?.Invoke($"Applyed provisioning template {modernSite.PnPTemplate} to site");
                }
            }
            else
            {
                logError?.Invoke($"Error processing \"modern\" site creation for site {modernSite.SiteAlias}! Invalid JSON request!");
            }
        }
    }
}
