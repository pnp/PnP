using OfficeAMS.Core.Utilities;
using Contoso.Provisioning.Hybrid.Contract;
using Microsoft.ServiceBus;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using OfficeAMS.Core;
using Microsoft.SharePoint.Client;

namespace Contoso.Provisioning.Hybrid.Test
{
    class Program
    {
        static void Main(string[] args)
        {

            // Determine the system connectivity mode based on the command line
            // arguments: -http, -tcp or -auto  (defaults to auto)
            ServiceBusEnvironment.SystemConnectivity.Mode = GetConnectivityMode(args);

            string serviceNamespace = ConfigurationManager.AppSettings["General.SBServiceNameSpace"];
            string issuerName = ConfigurationManager.AppSettings["General.SBIssuerName"];
            string issuerSecret = EncryptionUtility.Decrypt(ConfigurationManager.AppSettings["SBIssuerSecret"], ConfigurationManager.AppSettings["General.EncryptionThumbPrint"]);

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

            SharePointProvisioningData sharePointProvisioningData = new SharePointProvisioningData();
            sharePointProvisioningData.Title = "Test site on-premises";
            sharePointProvisioningData.Url = String.Format("{0}{1}", "https://bertonline.sharepoint.com/sites/", Guid.NewGuid().ToString());
            sharePointProvisioningData.Template = "ContosoCollaboration";
            sharePointProvisioningData.Name = "";
            sharePointProvisioningData.DataClassification = "HBI";

            SharePointUser[] owners = new SharePointUser[1];
            SharePointUser owner = new SharePointUser();
            owner.Login = "kevinc@set1.bertonline.info";
            owner.Name = "Kevin Cook";
            owner.Email = "kevincook@set1.bertonline.info";
            owners[0] = owner;
            sharePointProvisioningData.Owners = owners;

            channel.ProvisionSiteCollection(sharePointProvisioningData);

            //Console.WriteLine("Enter text to echo (or [Enter] to exit):");
            //string input = Console.ReadLine();
            //while (input != String.Empty)
            //{
            //    try
            //    {
            //        Console.WriteLine("Server echoed: {0}", channel.Echo(input));
            //    }
            //    catch (Exception e)
            //    {
            //        Console.WriteLine("Error: " + e.Message);
            //    }
            //    input = Console.ReadLine();
            //}
            Console.ReadLine();
            channel.Close();
            channelFactory.Close();

        }

        static ConnectivityMode GetConnectivityMode(string[] args)
        {
            foreach (string arg in args)
            {
                if (arg.Equals("/auto", StringComparison.InvariantCultureIgnoreCase) ||
                     arg.Equals("-auto", StringComparison.InvariantCultureIgnoreCase))
                {
                    return ConnectivityMode.AutoDetect;
                }
                else if (arg.Equals("/tcp", StringComparison.InvariantCultureIgnoreCase) ||
                     arg.Equals("-tcp", StringComparison.InvariantCultureIgnoreCase))
                {
                    return ConnectivityMode.Tcp;
                }
                else if (arg.Equals("/http", StringComparison.InvariantCultureIgnoreCase) ||
                     arg.Equals("-http", StringComparison.InvariantCultureIgnoreCase))
                {
                    return ConnectivityMode.Http;
                }
            }
            return ConnectivityMode.AutoDetect;
        }    
    }
}
