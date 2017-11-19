using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using OfficeDevPnP.Core;
using Microsoft.SharePoint.Client;
using OfficeDevPnP.Core.Sites;
using OfficeDevPnP.Core.Framework.Provisioning.Connectors;
using OfficeDevPnP.Core.Framework.Provisioning.Providers.Xml;
using Provisioning.CreateModernSites.Infrastructure;

namespace Provisioning.CreateModernSites.Function
{
    public static class ProvisionModernSite
    {
        [FunctionName("ProvisionModernSite")]
        public static void Run([QueueTrigger("modernsitesazurefunction", Connection = "AzureWebJobsStorage")]string message, TraceWriter log)
        {
            ModernSitesHelper.CreateModernSite(message, 
                (s) => { log.Info(s); }, 
                (s) => { log.Error(s); });
        }
    }
}
