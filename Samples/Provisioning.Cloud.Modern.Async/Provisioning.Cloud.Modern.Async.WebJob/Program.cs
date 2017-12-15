using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using System.Configuration;

namespace Provisioning.Cloud.Modern.Async.WebJob
{
    class Program
    {
        // Please set the following connection strings in app.config for this WebJob to run:
        // AzureWebJobsDashboard and AzureWebJobsStorage
        static void Main()
        {
            var host = new JobHost();
            // The following code ensures that the WebJob will be running continuously
            host.RunAndBlock();
        }
    }
}
