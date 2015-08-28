using System;
using System.IO;
using System.Threading;
using Microsoft.Azure.WebJobs;
using Provisioning.Framework.Cloud.Async.Common;

namespace Provisioning.Framework.Cloud.Async.Job
{
    public class Functions
    {
        // This function will get triggered/executed when a new message is written 
        // on an Azure Queue called queue.
        public static void ProcessQueueMessage([QueueTrigger(SiteManager.StorageQueueName)] SiteCollectionRequest siteRequest, TextWriter log)
        {
            log.WriteLine("Received new site request with URL of {0}.", siteRequest.Url);

            try
            {
                string webFullUrl;

                //create site collection using the Tenant object. Notice that you will need to have valid app ID and secret for this one
                var tenantAdminUri = new Uri(string.Format("https://{0}-admin.sharepoint.com", siteRequest.TenantName));
                string realm = TokenHelper.GetRealmFromTargetUrl(tenantAdminUri);
                var token = TokenHelper.GetAppOnlyAccessToken(TokenHelper.SharePointPrincipal, tenantAdminUri.Authority, realm).AccessToken;
                using (var ctx = TokenHelper.GetClientContextWithAccessToken(tenantAdminUri.ToString(), token))
                {
                    // Perform processing somewhere else
                    webFullUrl = new SiteManager().ProcessSiteCreationRequest(ctx, siteRequest);
                    // Log successful creation
                    log.WriteLine("Successfully created site collection at {0}.", webFullUrl);
                }

                var newWebUri = new Uri(webFullUrl);
                token = TokenHelper.GetAppOnlyAccessToken(TokenHelper.SharePointPrincipal, newWebUri.Authority, TokenHelper.GetRealmFromTargetUrl(newWebUri)).AccessToken;
                using (var ctx = TokenHelper.GetClientContextWithAccessToken(webFullUrl, token))
                {
                    var rootPath = !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("WEBROOT_PATH"))
                        ? Environment.GetEnvironmentVariable("WEBROOT_PATH")
                        : Environment.CurrentDirectory;

                    new SiteManager().ApplyCustomTemplateToSite(ctx, siteRequest, Path.Combine(rootPath, "Resources"));
                    log.WriteLine("Successfully applied template to site collection at {0}.", webFullUrl);
                }
            }
            catch (Exception ex)
            {
                log.WriteLine("Site collection creation to URL {0} failed with following details.", siteRequest.Url);
                log.WriteLine(ex.ToString());
                throw;
            }
        }


    }
}
