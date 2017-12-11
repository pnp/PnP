using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Newtonsoft.Json;
using OfficeDevPnP.Core;
using Microsoft.SharePoint.Client;
using OfficeDevPnP.Core.Sites;
using OfficeDevPnP.Core.Framework.Provisioning.Providers.Xml;
using Provisioning.Cloud.Modern.Async.Infrastructure;

namespace Provisioning.Cloud.Modern.Async.WebJob
{
    public class Functions
    {
        // This function will get triggered/executed when a new message is written 
        // on an Azure Queue called queue.
        public static void ProcessQueueMessage([QueueTrigger("modernsitesazurewebjob")] string message, TextWriter log)
        {
            var currentPath = Environment.GetEnvironmentVariable("WEBROOT_PATH");
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
                (s) => { log.WriteLine(s); },
                (s) => { log.WriteLine(s); });
        }
    }
}
