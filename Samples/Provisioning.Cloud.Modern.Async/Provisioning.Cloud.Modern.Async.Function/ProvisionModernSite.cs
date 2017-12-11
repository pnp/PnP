using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using OfficeDevPnP.Core;
using Microsoft.SharePoint.Client;
using OfficeDevPnP.Core.Sites;
using OfficeDevPnP.Core.Framework.Provisioning.Connectors;
using OfficeDevPnP.Core.Framework.Provisioning.Providers.Xml;
using Provisioning.Cloud.Modern.Async.Infrastructure;
using System.Configuration;

namespace Provisioning.Cloud.Modern.Async.Function
{
    public static class ProvisionModernSite
    {
        [FunctionName("ProvisionModernSite")]
        public static void Run([QueueTrigger("modernsitesazurefunction", Connection = "AzureWebJobsStorage")]string message, TraceWriter log)
        {
            var currentPath = Environment.GetEnvironmentVariable("HOME");
#if DEBUG
            if (String.IsNullOrEmpty(currentPath))
            {
                currentPath = AppDomain.CurrentDomain.BaseDirectory;
                if (currentPath.ToLower().Contains("\\bin\\debug\\"))
                {
                    currentPath = currentPath.Substring(0, currentPath.Length - 11);
                }
            }
#endif

            ModernSitesHelper.CreateModernSite(message,
                currentPath,
                (s) => { log.Info(s); }, 
                (s) => { log.Error(s); });
        }
    }
}
