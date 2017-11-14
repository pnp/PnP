using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Newtonsoft.Json;
using OfficeDevPnP.Core;
using Microsoft.SharePoint.Client;
using OfficeDevPnP.Core.Sites;

namespace Provisioning.CreateModernSites.WebJob
{
    public class Functions
    {
        // This function will get triggered/executed when a new message is written 
        // on an Azure Queue called queue.
        public static void ProcessQueueMessage([QueueTrigger("modernsitesazurewebjob")] string message, TextWriter log)
        {
            var modernSite = JsonConvert.DeserializeObject<ModernSiteCreation>(message);
            if (modernSite != null)
            {
                log.WriteLine($"Processing \"modern\" site creation for site {modernSite.SiteAlias}");

                AuthenticationManager authManager = new AuthenticationManager();
                using (var context = authManager.GetAzureADAccessTokenAuthenticatedContext(
                    modernSite.SPORootSiteUrl, modernSite.UserAccessToken))
                {
                    String siteUrl = String.Empty;

                    switch (modernSite.SiteType)
                    {
                        case SiteType.CommunicationSite:

                            context.Web.EnsureProperty(w => w.Language);

                            siteUrl = context.CreateSiteAsync(new CommunicationSiteCollectionCreationInformation {
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

                    log.WriteLine($"Created \"modern\" site with URL: {siteUrl}");
                }
            }
            else
            {
                log.WriteLine($"Error processing \"modern\" site creation for site {modernSite.SiteAlias}! Invalid JSON request!");
            }
        }
    }
}
