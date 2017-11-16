using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using OfficeDevPnP.Core;
using Microsoft.SharePoint.Client;
using OfficeDevPnP.Core.Sites;
using OfficeDevPnP.Core.Framework.Provisioning.Connectors;
using OfficeDevPnP.Core.Framework.Provisioning.Providers.Xml;

namespace Provisioning.CreateModernSites.Function
{
    public static class ProvisionModernSite
    {
        [FunctionName("ProvisionModernSite")]
        public static void Run([QueueTrigger("modernsitesazurefunction", Connection = "AzureWebJobsStorage")]string message, TraceWriter log)
        {
            var modernSite = JsonConvert.DeserializeObject<ModernSiteCreation>(message);
            if (modernSite != null)
            {
                log.Info($"Processing \"modern\" site creation for site {modernSite.SiteAlias}");

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

                    log.Info($"Created \"modern\" site with URL: {siteUrl}");

                    log.Info($"Applying provisioning template {modernSite.PnPTemplate} to site");

                    using (var siteContext = authManager.GetAzureADAccessTokenAuthenticatedContext(
                        siteUrl, modernSite.UserAccessToken))
                    {
                        var web = siteContext.Web;
                        siteContext.Load(web);
                        siteContext.ExecuteQueryRetry();

                        XMLTemplateProvider provider =
                               new XMLFileSystemTemplateProvider(Environment.GetEnvironmentVariable("WEBJOBS_PATH"), "");
                        var template = provider.GetTemplate(modernSite.PnPTemplate);
                        template.Connector = provider.Connector;

                        web.ApplyProvisioningTemplate(template);
                    }

                    log.Info($"Applyed provisioning template {modernSite.PnPTemplate} to site");
                }
            }
            else
            {
                log.Error($"Error processing \"modern\" site creation for site {modernSite.SiteAlias}! Invalid JSON request!");
            }
        }
    }

    public class ModernSiteCreation
    {
        public String CurrentUserPrincipalName { get; set; }

        public SiteType SiteType { get; set; }

        public String SiteTitle { get; set; }

        public String SiteAlias { get; set; }

        public String SiteDescription { get; set; }

        public String SiteClassification { get; set; }

        public Boolean IsPublic { get; set; }

        public String PnPTemplate { get; set; }

        public String UserAccessToken { get; set; }

        public String SPORootSiteUrl { get; set; }
    }

    /// <summary>
    /// Defines the available "modern" site options
    /// </summary>
    public enum SiteType
    {
        /// <summary>
        /// "modern" team site
        /// </summary>
        TeamSite,
        /// <summary>
        /// "modern" communication site
        /// </summary>
        CommunicationSite,
    }
}
