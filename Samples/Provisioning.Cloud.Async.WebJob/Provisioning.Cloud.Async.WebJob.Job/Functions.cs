using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Provisioning.Cloud.Async.WebJob.Common;

namespace Provisioning.Cloud.Async.WebJob.Job
{
    public class Functions
    {
        // This function will get triggered/executed when a new message is written 
        // on an Azure Queue called queue.
        public static void ProcessQueueMessage([QueueTrigger(SiteRequestManager.StorageQueueName)] ProvisioningData provisioningData, TextWriter log)
        {
            log.WriteLine(string.Format("Received new site request with URL of {0}.", provisioningData.RequestData.Url));

            try
            {
                //create site collection using the Tenant object. Notice that you will need to have valid app ID and secret for this one
                var tenantAdminUri = new Uri(String.Format("https://{0}-admin.sharepoint.com", provisioningData.TenantName));
                string realm = TokenHelper.GetRealmFromTargetUrl(tenantAdminUri);
                var token = TokenHelper.GetAppOnlyAccessToken(TokenHelper.SharePointPrincipal, tenantAdminUri.Authority, realm).AccessToken;
                using (var adminContext = TokenHelper.GetClientContextWithAccessToken(tenantAdminUri.ToString(), token))
                {
                    // Resolve full URL 
                    var webFullUrl = String.Format("https://{0}.sharepoint.com/{1}/{2}", provisioningData.TenantName, "sites", provisioningData.RequestData.Url);

                    // Ensure that we do not overlap with existing URL
                    if (!new SiteRequestManager().SiteURLAlreadyInUse(adminContext, webFullUrl))
                    {
                        // Add branding data structure in
                        provisioningData.BrandingData = new SiteBrandingData()
                        {
                            LogoImagePath = Path.Combine(Environment.GetEnvironmentVariable("WEBROOT_PATH"), "Resources\\garagelogo.png"),
                            ThemeBackgrounImagePath = Path.Combine(Environment.GetEnvironmentVariable("WEBROOT_PATH"), "Resources\\garagebg.jpg"),
                            ThemeColorFilePath = Path.Combine(Environment.GetEnvironmentVariable("WEBROOT_PATH"), "Resources\\garagewhite.spcolor"),
                            ThemeFontFilePath = "",
                            ThemeName = "Garage",
                            ThemeMasterPageName = "seattle.master"
                        };

                        // Process request for new site
                        new SiteRequestManager().ProcessSiteCreationRequest(adminContext, provisioningData);
                    }

                    log.WriteLine(string.Format("Successfully created site collection at {0}.", webFullUrl));
                }
            }
            catch (Exception ex)
            {
                log.WriteLine(string.Format("Site collection creation to URL {0} failed with following details.", provisioningData.RequestData.Url));
                log.WriteLine(ex.ToString());
                throw;
            }   
        }
    }
}
