using Microsoft.ServiceBus;
using OfficeDevPnP.Core.Utilities;
using System;
using System.Configuration;
using System.ServiceModel;
using System.ServiceModel.Description;

namespace Contoso.Provisioning.Hybrid
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
            string issuerSecret = EncryptionUtility.Decrypt(ConfigurationManager.AppSettings["General.SBIssuerSecret"], ConfigurationManager.AppSettings["General.EncryptionThumbPrint"]);

            // create the service URI based on the service namespace
            Uri address = ServiceBusEnvironment.CreateServiceUri("sb", serviceNamespace, "SharePointProvisioning");

            // create the credentials object for the endpoint
            TransportClientEndpointBehavior sharedSecretServiceBusCredential = new TransportClientEndpointBehavior();
            sharedSecretServiceBusCredential.TokenProvider = TokenProvider.CreateSharedSecretTokenProvider(issuerName, issuerSecret);

            // create the service host reading the configuration
            ServiceHost host = new ServiceHost(typeof(SharePointProvisioning), address);

            // create the ServiceRegistrySettings behavior for the endpoint
            IEndpointBehavior serviceRegistrySettings = new ServiceRegistrySettings(DiscoveryType.Public);

            // add the Service Bus credentials to all endpoints specified in configuration
            foreach (ServiceEndpoint endpoint in host.Description.Endpoints)
            {
                endpoint.Behaviors.Add(serviceRegistrySettings);
                endpoint.Behaviors.Add(sharedSecretServiceBusCredential);
            }

            // open the service
            host.OpenTimeout = TimeSpan.FromMinutes(15);
            host.CloseTimeout = TimeSpan.FromMinutes(15);
            host.Open();

            Console.WriteLine("Service address: " + address);
            Console.WriteLine("Press [Enter] to exit");
            Console.ReadLine();

            // close the service
            host.Close();

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
