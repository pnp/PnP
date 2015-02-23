using Microsoft.WindowsAzure;
using Provisioning.Cloud.Async.WebJob.Common;
using System;
using System.Collections.Generic;
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
            ProvisioningData data = new ProvisioningData();
            data.TenantName = "vesaj";
            // Add request data in
            data.RequestData = new SiteRequestData(){
                 Title = "",
                 Template = "",
                 Lcid = 1033,
                 Owner = "", 
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
