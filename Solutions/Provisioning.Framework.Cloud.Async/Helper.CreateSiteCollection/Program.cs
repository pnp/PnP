using Provisioning.Framework.Cloud.Async.Common;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helper.CreateSiteCollection
{
    class Program
    {
        static void Main(string[] args)
        {
            // Update these accordingly for your environment
            string tenantName = ConfigurationManager.AppSettings["TenantName"];
            string ownwerEmail = ConfigurationManager.AppSettings["SiteColTestOwnerEmail"];

            //create site collection using the Tenant object. Notice that you will need to have valid app ID and secret for this one
            var tenantAdminUri = new Uri(String.Format("https://{0}-admin.sharepoint.com", tenantName));
            string realm = TokenHelper.GetRealmFromTargetUrl(tenantAdminUri);
            var token = TokenHelper.GetAppOnlyAccessToken(TokenHelper.SharePointPrincipal, tenantAdminUri.Authority, realm).AccessToken;
            using (var adminContext = TokenHelper.GetClientContextWithAccessToken(tenantAdminUri.ToString(), token))
            {
                // Call the creation.
                SiteCollectionRequest data = new SiteCollectionRequest() {
                    TenantName = tenantName,
                    Url = DateTime.Now.Ticks.ToString(),
                    Owner = ownwerEmail,
                    ManagedPath = "sites",
                    ProvisioningType = SiteProvisioningType.Identity,
                    TemplateId = "CT1",
                    TimeZoneId = 16,
                    StorageMaximumLevel = 110,
                    Title = "Test site collection"
                };
               
                // Process request for new site
                new SiteManager().ProcessSiteCreationRequest(adminContext, data);
            }
        }
    }
}
