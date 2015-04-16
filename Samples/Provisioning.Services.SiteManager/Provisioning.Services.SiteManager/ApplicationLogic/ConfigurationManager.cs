using Microsoft.SharePoint.Administration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contoso.Provisioning.Services.SiteManager.ApplicationLogic
{
    public static class ConfigurationManager
    {
        public static void ConfigureRemoteManagerTimeout(string service)
        {
            SPWebService contentService = SPWebService.ContentService;

            if (contentService.WcfServiceSettings.ContainsKey(service))
            {
                SPWcfServiceSettings wcfServiceSettings = new SPWcfServiceSettings();
                wcfServiceSettings.ReaderQuotasMaxStringContentLength = 10485760;
                wcfServiceSettings.ReaderQuotasMaxArrayLength = int.MaxValue;
                wcfServiceSettings.ReaderQuotasMaxBytesPerRead = 10485760;
                wcfServiceSettings.MaxReceivedMessageSize = 10485760;
                wcfServiceSettings.ReceiveTimeout = TimeSpan.FromMinutes(15);
                wcfServiceSettings.OpenTimeout = TimeSpan.FromMinutes(15);
                wcfServiceSettings.CloseTimeout = TimeSpan.FromMinutes(15);

                contentService.WcfServiceSettings[service.ToLower()] = wcfServiceSettings;
                contentService.Update();
            }
            else
            {
               // TODO - Exception handling
            }
        }
    }
}
