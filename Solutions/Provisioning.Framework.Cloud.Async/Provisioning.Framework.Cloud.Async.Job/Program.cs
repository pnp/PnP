using Microsoft.Azure.WebJobs;

namespace Provisioning.Framework.Cloud.Async.Job
{
    // To learn more about Microsoft Azure WebJobs SDK, please see http://go.microsoft.com/fwlink/?LinkID=320976
    class Program
    {
        // Please set the following connection strings in app.config for this WebJob to run:
        // AzureWebJobsDashboard and AzureWebJobsStorage
        static void Main()
        {
            var config = new JobHostConfiguration();
            // Make sure an item can only be picked up once, otherwise you get errors
            config.Queues.MaxDequeueCount = 1;
            var host = new JobHost(config);

            // The following code ensures that the WebJob will be running continuously
            host.RunAndBlock();
        }
    }
}
