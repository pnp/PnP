using Microsoft.Azure.WebJobs;
using Microsoft.Online.SharePoint.TenantAdministration;
using Microsoft.SharePoint.Client;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;
using Provisioning.Framework.Cloud.Async.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Provisioning.Framework.Cloud.Async.Job
{
    public class Functions
    {
        // This function will get triggered/executed when a new message is written 
        // on an Azure Queue called queue.
        public static void ProcessQueueMessage([QueueTrigger(SiteManager.StorageQueueName)] SiteCollectionRequest siteRequest, TextWriter log)
        {
            log.WriteLine(string.Format("Received new site request with URL of {0}.", siteRequest.Url));

            try
            {
                string webFullUrl = string.Empty;

                //create site collection using the Tenant object. Notice that you will need to have valid app ID and secret for this one
                var tenantAdminUri = new Uri(String.Format("https://{0}-admin.sharepoint.com", siteRequest.TenantName));
                string realm = TokenHelper.GetRealmFromTargetUrl(tenantAdminUri);
                var token = TokenHelper.GetAppOnlyAccessToken(TokenHelper.SharePointPrincipal, tenantAdminUri.Authority, realm).AccessToken;
                using (var ctx = TokenHelper.GetClientContextWithAccessToken(tenantAdminUri.ToString(), token))
                {
                    // Perform processing somewhere else
                    webFullUrl = new SiteManager().ProcessSiteCreationRequest(ctx, siteRequest);
                    // Log successful creation
                    log.WriteLine(string.Format("Successfully created site collection at {0}.", webFullUrl));
                }

                var newWebUri = new Uri(webFullUrl);
                token = TokenHelper.GetAppOnlyAccessToken(TokenHelper.SharePointPrincipal, newWebUri.Authority, TokenHelper.GetRealmFromTargetUrl(newWebUri)).AccessToken;
                using (var ctx = TokenHelper.GetClientContextWithAccessToken(webFullUrl, token))
                {
                    new SiteManager().ApplyCustomTemplateToSite(ctx, siteRequest, Path.Combine(Environment.GetEnvironmentVariable("WEBROOT_PATH"), "Resources"));
                    log.WriteLine(string.Format("Successfully applied template to site collection at {0}.", webFullUrl));
                }
            }
            catch (Exception ex)
            {
                log.WriteLine(string.Format("Site collection creation to URL {0} failed with following details.", siteRequest.Url));
                log.WriteLine(ex.ToString());
                throw;
            }
        }


    }
}
