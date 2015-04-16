using Microsoft.WindowsAzure;
using Provisioning.Cloud.Async.WebJob.Common;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Provision.Cloud.Async.WebJob.Console.Message
{
    class Program
    {
        /// <summary>
        ///  Can be used to test the storage queue message creation and to see the format what is created to the queue
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            // Update  parameters accordingly based on our environment from app.config
            // Update these accordingly for your environment
            string tenantName = ConfigurationManager.AppSettings["TenantName"];
            string ownwerEmail = ConfigurationManager.AppSettings["SiteColTestOwnerEmail"];

            // Create provisioning message objects for the storage queue
            ProvisioningData data = new ProvisioningData();
            data.TenantName = tenantName;
            // Add request data in
            data.RequestData = new SiteRequestData(){
                 Title = "Sample from queue",
                 Template = "STS#0",
                 Lcid = 1033,
                 Owner = ownwerEmail, 
                 StorageMaximumLevel = 100,
                 TimeZoneId = 16, 
                 Url = Guid.NewGuid().ToString().Replace("-", "")
            };
            // Add branding data structure in
            data.BrandingData = new SiteBrandingData(){
                LogoImagePath = "",
                ThemeBackgrounImagePath = "",
                ThemeColorFilePath = "",
                ThemeFontFilePath = "", 
                ThemeMasterPageName = "",
                ThemeName = ""
            };

            new SiteRequestManager().AddConfigRequestToQueue(data,
                                            CloudConfigurationManager.GetSetting("StorageConnectionString"));
        }
    }
}
