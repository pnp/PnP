using Provisioning.Hybrid.Simple.Common;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Provisioning.Hybrid.Simple.Console.SendMessage
{
    /// <summary>
    /// Tester console which can be used to verify the service bus configuration and sending messages cross both sides of the service bus
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            string returnMessage = "";

            try
            {
                string message = string.Format("Test message sent at {0}", DateTime.Now.ToLongTimeString());
                System.Console.WriteLine(String.Format("Sending this message: '{0}'", message));

                // Send message using centralized business component for testing purposes
                returnMessage = new ServiceBusMessageManager().SendMessage(message,
                                                                ConfigurationManager.AppSettings[Consts.ServiceBusNamespaceKey],
                                                                ConfigurationManager.AppSettings[Consts.ServiceBusSecretKey]);

                System.Console.WriteLine(String.Format("Got back this message: '{0}' at {1}", returnMessage, DateTime.Now.ToLongTimeString()));

                // Alternative to test site collection creation using console
                SiteCollectionRequest request = new SiteCollectionRequest()
                 {
                     Template = "STS#0",
                     Title = "New site",
                     OwnerIdentifier = "",
                     TargetEnvironment = Consts.DeploymentTypeOnPremises
                 };
                System.Console.WriteLine(String.Format("Send request to create new site collection at {0}", DateTime.Now.ToLongTimeString()));
                returnMessage = new ServiceBusMessageManager().SendSiteRequestMessage(request,
                                                                ConfigurationManager.AppSettings[Consts.ServiceBusNamespaceKey],
                                                                ConfigurationManager.AppSettings[Consts.ServiceBusSecretKey]);


                System.Console.ForegroundColor = ConsoleColor.Green;
                System.Console.WriteLine(String.Format("Got followign message back: '{0}' at {1}", returnMessage, DateTime.Now.ToLongTimeString()));
            }
            catch (Exception ex)
            {
                System.Console.ForegroundColor = ConsoleColor.Red;
                System.Console.WriteLine(String.Format("Exception with the execution. Error description: '{0}'", ex.ToString()));
            }


            // Just to keep it hanging in the service... could be hosted for example as windows service for better handling
            System.Console.ForegroundColor = ConsoleColor.Gray;
            System.Console.WriteLine("Press ENTER to close");
            System.Console.ReadLine();

        }
    }
}
