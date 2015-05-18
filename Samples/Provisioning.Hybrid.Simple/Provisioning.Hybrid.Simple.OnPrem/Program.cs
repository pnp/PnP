using Microsoft.ServiceBus;
using Provisioning.Hybrid.Simple.Common;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text;
using System.Threading.Tasks;

namespace Provisioning.Hybrid.Simple.OnPrem
{
    class Program
    {
        /// <summary>
        /// Setup the host up and running in the console application. 
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            // Configuration keys
            string issuerSecret = ConfigurationManager.AppSettings[Consts.ServiceBusSecretKey];
            string serviceNamespaceDomain = ConfigurationManager.AppSettings[Consts.ServiceBusNamespaceKey]; 

            // Here's our custom service receiving messages from the cloud
            ServiceHost sh = new ServiceHost(typeof(SiteRequestService));

            // Let's add only service end point to service bus, you could add also local end point, if needed. Keep it simple for this one. 
            sh.AddServiceEndpoint(
               typeof(ISiteRequest), new NetTcpRelayBinding(),
               ServiceBusEnvironment.CreateServiceUri("sb", serviceNamespaceDomain, "solver"))
                .Behaviors.Add(new TransportClientEndpointBehavior
                {
                    TokenProvider = TokenProvider.CreateSharedSecretTokenProvider("owner", issuerSecret)
                });
            sh.Open();

            // Just to keep it hanging in the service... could be hosted for example as windows service for better handling
            Console.WriteLine("Press ENTER to close");
            Console.ReadLine();

            // Enter pressed, let's close up
            sh.Close();
        }
    }
}
