using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
//The assembly for this is in Program Files\SharePoint Client Components\Assemblies
using Microsoft.Online.SharePoint.TenantAdministration;

namespace Provisioning.Batch
{
    class Program
    {
        static void Main(string[] args)
        {

            string SHAREPOINT_PID = "00000003-0000-0ff1-ce00-000000000000";  //This is hard-coded for SharePoint Online (ie - all tenants)
            //The app must have tenant-level permissions and can be installed on any site in the tenancy. You must use the tenant
            //admin site url to get client context.
            var sharePointUrl = new Uri("https://<Office 365 domain>-admin.sharepoint.com");
            string sharePointRealm = TokenHelper.GetRealmFromTargetUrl(sharePointUrl);
            var token = TokenHelper.GetAppOnlyAccessToken(SHAREPOINT_PID, sharePointUrl.Authority, sharePointRealm).AccessToken;


            //read the Sites.xml file and then release the file so we can save over it later
            string path = @"C:\<path>\Sites.xml";

            XDocument doc;
            using (var fileStream = System.IO.File.OpenRead(path))
            {
                doc = XDocument.Load(fileStream);
            }

            //get all the requested sites from the Sites.xml file and loop through each for processing
            var sites = doc.Root.Elements("site");
            foreach (var site in sites)
            {
                using (var clientContext = TokenHelper.GetClientContextWithAccessToken(sharePointUrl.ToString(), token))
                {

                    clientContext.Load(clientContext.Web.Lists);
                    clientContext.ExecuteQuery();
                    var siteUrl = site.Attribute("url").Value;
                    var tenant = new Tenant(clientContext);
                    var newSite = new SiteCreationProperties()
                    {
                        Url = siteUrl,
                        Owner = "<admin user>@<Office 365 domain>.onmicrosoft.com",
                        Template = "STS#0",
                        Title = "Batch provisioning test site",
                        //StorageMaximumLevel = 100,
                        //StorageWarningLevel = 300,
                        TimeZoneId = 7,
                        UserCodeMaximumLevel = 7,
                        UserCodeWarningLevel = 1,
                    };

                    var spoOperation = tenant.CreateSite(newSite);
                    clientContext.Load(spoOperation);
                    clientContext.ExecuteQuery();

                    while (!spoOperation.IsComplete)
                    {
                        System.Threading.Thread.Sleep(2000);
                        clientContext.Load(spoOperation);
                        clientContext.ExecuteQuery();

                    }

                }
            }

        }
    }
}
