using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Storage;
using System.IO;
using Microsoft.WindowsAzure.Storage.Queue;
using System.Xml.Serialization;
using Microsoft.ServiceBus;
using System.ServiceModel;
using Contoso.Provisioning.Hybrid.Core.SiteTemplates;
using OfficeDevPnP.Core.Utilities;
using Contoso.Provisioning.Hybrid.Contract;
using Contoso.Provisioning.Hybrid;

namespace Contoso.Provisioning.Hybrid.Worker
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
    //   <AppPermissionRequest Scope="http://sharepoint/content/tenant" Right="FullControl" />
    // </AppPermissionRequests>
    //
    // As you're granting tenant wide full control to an app the appsecret is as important as the password from your SharePoint administration account!
    //


    public class WorkerRole : RoleEntryPoint
    {
        private const string queueName = "sharepointprovisioning";
        private const string azureConnectionSettingKey = "Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString";
        private CloudQueue queue;
        private static object gate = new Object();

        public override void Run()
        {
            // This is a sample worker implementation. Replace with your logic.
            Trace.TraceInformation("Contoso.Azure.CloudServices.Provisioning.Worker entry point called", "Information");

            while (true)
            {
                // retrieve a new message from the queue
                CloudQueueMessage msg = queue.GetMessage();

                // There's a message in the queue, let's process it
                if (msg != null)
                {
                    //Trace.TraceInformation(string.Format("received message {0}", msg.AsString));
                    if (ProcessMessage(msg.AsString))
                    {
                        // Remove the message from the queue after successfull processing
                        queue.DeleteMessage(msg);
                    }
                }
                else
                {
                    // Pause for 1 second before we check the queue again
                    System.Threading.Thread.Sleep(1000);
                }
            }
        }

        private bool ProcessMessage(string message)
        {
            bool processed = true;

            SharePointProvisioningData sharePointProvisioningData = DeserializeData(message);

            if (sharePointProvisioningData.DataClassification.Equals("HBI", StringComparison.InvariantCultureIgnoreCase))
            {
                try
                {
                    // Determine the system connectivity mode based on the command line
                    // arguments: -http, -tcp or -auto  (defaults to auto)
                    ServiceBusEnvironment.SystemConnectivity.Mode = ConnectivityMode.AutoDetect;

                    string serviceNamespace = RoleEnvironment.GetConfigurationSettingValue("General.SBServiceNameSpace");
                    string issuerName = RoleEnvironment.GetConfigurationSettingValue("General.SBIssuerName");
                    string issuerSecret = EncryptionUtility.Decrypt(RoleEnvironment.GetConfigurationSettingValue("General.SBIssuerSecret"), RoleEnvironment.GetConfigurationSettingValue("General.EncryptionThumbPrint"));

                    // create the service URI based on the service namespace
                    Uri serviceUri = ServiceBusEnvironment.CreateServiceUri("sb", serviceNamespace, "SharePointProvisioning");

                    // create the credentials object for the endpoint
                    TransportClientEndpointBehavior sharedSecretServiceBusCredential = new TransportClientEndpointBehavior();
                    sharedSecretServiceBusCredential.TokenProvider = TokenProvider.CreateSharedSecretTokenProvider(issuerName, issuerSecret);

                    // create the channel factory loading the configuration
                    ChannelFactory<ISharePointProvisioningChannel> channelFactory = new ChannelFactory<ISharePointProvisioningChannel>("RelayEndpoint", new EndpointAddress(serviceUri));

                    // apply the Service Bus credentials
                    channelFactory.Endpoint.Behaviors.Add(sharedSecretServiceBusCredential);

                    // create and open the client channel
                    ISharePointProvisioningChannel channel = channelFactory.CreateChannel();
                    channel.Open();
                    channel.ProvisionSiteCollection(sharePointProvisioningData);
                    channel.Close();
                    channelFactory.Close();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    //log error
                }
            }
            else
            {
                try
                {
                    SiteProvisioningBase siteToProvision = null;
                    switch (sharePointProvisioningData.Template)
                    {
                        case SiteProvisioningTypes.ContosoCollaboration:
                            siteToProvision = new ContosoCollaboration();
                            break;
                        case SiteProvisioningTypes.ContosoProject:
                            siteToProvision = new ContosoProject();
                            break;
                    }

                    siteToProvision.SharePointProvisioningData = sharePointProvisioningData;
                    HookupAuthentication(siteToProvision);

                    // Provision the site collection
                    processed = siteToProvision.Execute();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    //log error
                }
            }
            // always return true to get the item of the queue...no retry mechanism foreseen
            return true;
        }

        private void HookupAuthentication(SiteProvisioningBase siteProvisioningInstance)
        {
            siteProvisioningInstance.Realm = RoleEnvironment.GetConfigurationSettingValue("Realm");
            siteProvisioningInstance.AppId = RoleEnvironment.GetConfigurationSettingValue("AppId");
            siteProvisioningInstance.AppSecret = EncryptionUtility.Decrypt(RoleEnvironment.GetConfigurationSettingValue("AppSecret"), RoleEnvironment.GetConfigurationSettingValue("General.EncryptionThumbPrint"));

            siteProvisioningInstance.InstantiateAppOnlyClientContext(RoleEnvironment.GetConfigurationSettingValue("TenantAdminUrl"));
            siteProvisioningInstance.InstantiateSiteDirectorySiteClientContext(RoleEnvironment.GetConfigurationSettingValue("General.SiteDirectoryUrl"));
        }

        public override bool OnStart()
        {
            // Set the maximum number of concurrent connections 
            ServicePointManager.DefaultConnectionLimit = 12;

            // For information on handling configuration changes
            // see the MSDN topic at http://go.microsoft.com/fwlink/?LinkId=166357.
            CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(RoleEnvironment.GetConfigurationSettingValue(azureConnectionSettingKey));

            // initialize queue storage 
            CloudQueueClient queueStorage = cloudStorageAccount.CreateCloudQueueClient();
            queue = queueStorage.GetQueueReference(queueName);

            bool storageInitialized = false;
            while (!storageInitialized)
            {
                // create the message queue(s)
                queue.CreateIfNotExists();
                storageInitialized = true;
            }

            return base.OnStart();
        }

        /// <summary>
        /// Deserializes the retrieved XML message 
        /// </summary>
        /// <param name="sharePointProvisioningData">XML representation as string</param>
        /// <returns>SharePointProvisioningData object</returns>
        private static SharePointProvisioningData DeserializeData(string sharePointProvisioningData)
        {
            SharePointProvisioningData deserializedSharePointProvisioningaData = null;
            using (Stream stream = new MemoryStream())
            {
                StreamWriter writer = new StreamWriter(stream);
                writer.Write(sharePointProvisioningData);
                writer.Flush();

                stream.Position = 0;
                object result = new XmlSerializer(typeof(SharePointProvisioningData)).Deserialize(stream);
                deserializedSharePointProvisioningaData = (SharePointProvisioningData)result;
            }
            return deserializedSharePointProvisioningaData;
        }

    }
}
