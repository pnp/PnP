using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using BusinessApps.O365ProjectsApp.Infrastructure;
using System.IO;

namespace BusinessApps.O365ProjectsApp.Job
{
    public class JobActivator
    {
        // This function will get triggered/executed when a new message is written 
        // on an Azure Queue called queue.
        public static void ProcessQueueMessage([QueueTrigger(O365ProjectsAppConstants.Blob_Storage_Queue_Name)] String content, TextWriter log)
        {
            log.WriteLine(String.Format("Found Job: {0}", content));


            log.WriteLine("Completed Job execution");
        }
    }
}
