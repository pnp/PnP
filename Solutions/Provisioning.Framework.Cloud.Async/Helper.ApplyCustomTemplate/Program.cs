using Microsoft.SharePoint.Client;
using OfficeDevPnP.Core.Framework.Provisioning.Connectors;
using OfficeDevPnP.Core.Framework.Provisioning.Model;
using OfficeDevPnP.Core.Framework.Provisioning.ObjectHandlers;
using OfficeDevPnP.Core.Framework.Provisioning.Providers.Xml;
using Provisioning.Framework.Cloud.Async.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace Helper.ApplyCustomTemplate
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("*************");
            Console.WriteLine("** PnP provisioning Engine");
            Console.WriteLine("************");

            string templateSite = GetUserInput("Template site URL:");
            string targetSite = GetUserInput("Target site URL:"); 

            // Log the start time
            Console.WriteLine("Start: {0:hh.mm.ss}", DateTime.Now);

            //Get the realm for the target URL
            Uri siteUri = new Uri(targetSite);
            string realm = TokenHelper.GetRealmFromTargetUrl(siteUri);
            //Get the access token for the URL.  Requires this app to be registered with the tenant
            string accessToken = TokenHelper.GetAppOnlyAccessToken(TokenHelper.SharePointPrincipal,
                                                                    siteUri.Authority, realm).AccessToken;

            //Get client context with access token
            using (var ctx = TokenHelper.GetClientContextWithAccessToken(siteUri.ToString(), accessToken))
            {
                SiteCollectionRequest data = new SiteCollectionRequest()
                {
                    TenantName = "contoso",
                    Url = DateTime.Now.Ticks.ToString(),
                    Owner = "admin@contoso.onsharepoint.com",
                    ManagedPath = "sites",
                    ProvisioningType = SiteProvisioningType.TemplateSite,
                    TemplateId = templateSite,
                    TimeZoneId = 16,
                    StorageMaximumLevel = 110,
                    Title = "Test site collection"
                };

                // Execute the transformation
                new SiteManager().ApplyCustomTemplateToSite(ctx, data, @".\Resources");
            }

            // Log the end time
            Console.WriteLine("End: {0:hh.mm.ss}", DateTime.Now);
        }

        static string GetUserInput(string userInput)
        {
            string strUserName = string.Empty;
            try
            {
                Console.Write(userInput);
                strUserName = Console.ReadLine();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                strUserName = string.Empty;
            }
            return strUserName;
        }

    }
}
