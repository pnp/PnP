using Provisioning.Framework.Cloud.Async.Common;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helper.SendQueueMessage
{
    class Program
    {
        static void Main(string[] args)
        {
            // Update  parameters accordingly based on our environment from app.config
            // Update these accordingly for your environment
            string tenantName = ConfigurationManager.AppSettings["TenantName"];
            string ownwerEmail = ConfigurationManager.AppSettings["SiteColTestOwnerEmail"];

            // Create provisioning message objects for the storage queue
            SiteCollectionRequest data = new SiteCollectionRequest()
            {
                TenantName = tenantName,
                Url = DateTime.Now.Ticks.ToString(),
                Owner = ownwerEmail,
                ManagedPath = "sites",
                ProvisioningType = SiteProvisioningType.TemplateSite,
                TemplateId = "https://contoso.sharepoint.com/sites/templatesite",
                TimeZoneId = 16,
                StorageMaximumLevel = 110,
                Title = "Test site collection"
            };
            
            new SiteManager().AddConfigRequestToQueue(data,
                                            ConfigurationManager.AppSettings["StorageConnectionString"]);
        }
    }
}
