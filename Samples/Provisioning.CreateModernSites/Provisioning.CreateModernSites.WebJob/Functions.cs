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
using Provisioning.CreateModernSites.Infrastructure;

namespace Provisioning.CreateModernSites.WebJob
{
    public class Functions
    {
        // This function will get triggered/executed when a new message is written 
        // on an Azure Queue called queue.
        public static void ProcessQueueMessage([QueueTrigger("modernsitesazurewebjob")] string message, TextWriter log)
        {
            ModernSitesHelper.CreateModernSite(message,
                (s) => { log.WriteLine(s); },
                (s) => { log.WriteLine(s); });
        }
    }
}
