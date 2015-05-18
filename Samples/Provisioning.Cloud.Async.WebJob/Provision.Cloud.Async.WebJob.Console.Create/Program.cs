using Provisioning.Cloud.Async.WebJob.Common;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Provision.Cloud.Async.WebJob.Console.Create
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
                // Let's randomize URL for your testing
                string webUrl = Guid.NewGuid().ToString().Replace("-", "");
                var webFullUrl = String.Format("https://{0}.sharepoint.com/{1}/{2}", tenantName, "sites", webUrl);

                if (!new SiteRequestManager().SiteURLAlreadyInUse(adminContext, webFullUrl))
                {
                    // Call the creation.
                    ProvisioningData data = new ProvisioningData();
                    data.TenantName = tenantName;
                    // Add request data in
                    data.RequestData = new SiteRequestData()
                    {
                        Title = "Test Provisioning",
                        Template = "STS#0",
                        Lcid = 1033,
                        Owner = "vesaj@veskuonline.com",
                        StorageMaximumLevel = 100,
                        TimeZoneId = 10,  // US east coast
                        Url = webUrl
                    };
                    // Add branding data structure in
                    data.BrandingData = new SiteBrandingData()
                    {
                        LogoImagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources\\garagelogo.png"),
                        ThemeBackgrounImagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources\\garagebg.jpg"),
                        ThemeColorFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources\\garagewhite.spcolor"),
                        ThemeFontFilePath = "",
                        ThemeName = "Garage",
                        ThemeMasterPageName = "seattle.master"
                    };

                    // Process request for new site
                    new SiteRequestManager().ProcessSiteCreationRequest(adminContext, data);
                }
            }
        }
    }
}
