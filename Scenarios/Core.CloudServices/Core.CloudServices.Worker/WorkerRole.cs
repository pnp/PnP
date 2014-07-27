using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using OfficeDevPnP.Core;
using OfficeDevPnP.Core.Utilities;
using Microsoft.SharePoint.Client;

namespace Contoso.Core.CloudServices.Worker
{
    // Use (Get-MsolCompanyInformation).ObjectID to obtain Target/Tenant realm: <guid>
    // Manually register an app via the appregnew.aspx page and generate an App ID and App Secret. The App title and App domain can be a simple string like "MyApp"
    // Update the AppID in your worker role settings
    // Add the AppSecret in your worker role settings. Note that this sample project requires to store the encrypted value of the AppSecret. Use the Contoso.Azure.CloudServices.Encryptor project 
    // to encrypt the AppId
    //
    // Manually set the permission XML for you app via the appinv.aspx page:
    // 1/ Lookup your app via it's AppID
    // 2/ Paste the permission XML and click on create
    //
    // Sample permission XML:
    // <AppPermissionRequests AllowAppOnlyPolicy="true">
    //   <AppPermissionRequest Scope="http://sharepoint/content/sitecollection" Right="FullControl" />
    // </AppPermissionRequests>
    //
    // As you're granting tenant wide full control to an app the appsecret is as important as the password from your SharePoint administration account!
    //

    /// <summary>
    /// Entry point for the Azure worker role
    /// </summary>
    public class WorkerRole : RoleEntryPoint
    {
        private const string queueName = "workerusernamepassword";
        private const string queueNameOAuth = "oauth";
        private const string azureConnectionSettingKey = "Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString";
        private CloudQueue queue;
        private CloudQueue queueOAuth;
        private static object gate = new Object();

        public override void Run()
        {
            Trace.TraceInformation("Contoso.Core.CloudServices.Worker entry point called", "Information");

            // set visibility timeout to 30 minutes to give the provisioning enough time to complete the task
            TimeSpan visibilityTimeout = new TimeSpan(0, 30, 0);

            while (true)
            {
                CloudQueueMessage msg = queue.GetMessage(visibilityTimeout);

                // There's a message in the queue, let's process it
                if (msg != null)
                {
                    Trace.TraceInformation(string.Format("Creds received message {0}", msg.AsString));
                    ProcessMessage(msg.AsString);
                    // Remove the message from the queue 
                    queue.DeleteMessage(msg);
                }
                else
                {
                    msg = queueOAuth.GetMessage(visibilityTimeout);
                    if (msg != null)
                    {
                        Trace.TraceInformation(string.Format("OAuth received message {0}", msg.AsString));
                        ProcessMessageOAuth(msg.AsString);
                        // Remove the message from the queue 
                        queueOAuth.DeleteMessage(msg);
                    }
                    else
                    {
                        // Pause for 1 second before we check the queue again
                        System.Threading.Thread.Sleep(1000);
                    }
                }

                //Trace.TraceInformation("Working", "Information");
            }
        }

        /// <summary>
        /// Role initialization
        /// </summary>
        /// <returns>true is ok, false otherwise</returns>
        public override bool OnStart()
        {
            // Set the maximum number of concurrent connections 
            System.Net.ServicePointManager.DefaultConnectionLimit = 12;

            // For information on handling configuration changes
            // see the MSDN topic at http://go.microsoft.com/fwlink/?LinkId=166357.

            CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(RoleEnvironment.GetConfigurationSettingValue(azureConnectionSettingKey));

            // initialize queue storage 
            CloudQueueClient queueStorage = cloudStorageAccount.CreateCloudQueueClient();
            queue = queueStorage.GetQueueReference(queueName);
            queueOAuth = queueStorage.GetQueueReference(queueNameOAuth);

            bool storageInitialized = false;
            while (!storageInitialized)
            {
                // create the message queue(s)
                queue.CreateIfNotExists();
                queueOAuth.CreateIfNotExists();
                storageInitialized = true;
            }

            return base.OnStart();
        }

        /// <summary>
        /// Processes a message from the workerusernamepassword queue
        /// </summary>
        /// <param name="message">Message retrieved from the queue</param>
        /// <returns>true if ok</returns>
        private bool ProcessMessage(string message)
        {
            bool processed = true;

            // first part contains the title, second the site to apply the title on
            string[] messageParts = message.Split(new string[] { "|" }, StringSplitOptions.None);
            if (messageParts[0].Length > 0)
            {
                ClientContext cc = new AuthenticationManager().GetSharePointOnlineAuthenticatedContextTenant(messageParts[1],
                                                                  RoleEnvironment.GetConfigurationSettingValue("TenantAdminUser"),
                                                                  EncryptionUtility.Decrypt(RoleEnvironment.GetConfigurationSettingValue("TenantAdminPassword"), RoleEnvironment.GetConfigurationSettingValue("ThumbPrint")));
                //Update the site title
                cc.Web.Title = messageParts[0];
                cc.Web.Update();
                cc.ExecuteQuery();
            }

            return processed;
        }

        /// <summary>
        /// Processes a message from the oauth queue
        /// </summary>
        /// <param name="message">Message retrieved from the queue</param>
        /// <returns>true if ok</returns>
        private bool ProcessMessageOAuth(string message)
        {
            bool processed = true;

            // first part contains the title, second the site to apply the title on
            string[] messageParts = message.Split(new string[] { "|" }, StringSplitOptions.None);
            if (messageParts[0].Length > 0)
            {
                ClientContext cc = new AuthenticationManager().GetAppOnlyAuthenticatedContext(messageParts[1],
                                                                                              RoleEnvironment.GetConfigurationSettingValue("Realm"),
                                                                                              RoleEnvironment.GetConfigurationSettingValue("AppId"),
                                                                                              EncryptionUtility.Decrypt(RoleEnvironment.GetConfigurationSettingValue("AppSecret"),RoleEnvironment.GetConfigurationSettingValue("ThumbPrint")));
                //Update the site title
                cc.Web.Title = messageParts[0];
                cc.Web.Update();
                cc.ExecuteQuery();
            }
            return processed;
        }

    }
}
