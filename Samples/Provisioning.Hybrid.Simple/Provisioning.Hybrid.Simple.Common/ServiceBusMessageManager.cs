using Microsoft.ServiceBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Provisioning.Hybrid.Simple.Common
{
    public class ServiceBusMessageManager
    {
        /// <summary>
        /// Simple operation call included in the Interface
        /// </summary>
        /// <param name="message">Message to be passed to the other side</param>
        /// <param name="serviceBusNamespace">Service bus namespace</param>
        /// <param name="serviceBusSecret">Service bus secret</param>
        /// <returns></returns>
        public string SendMessage(
                    string message, string serviceBusNamespace, string serviceBusSecret)
        {
            var cf = new ChannelFactory<ISiteRequest>(
                        new NetTcpRelayBinding(),
                        new EndpointAddress(ServiceBusEnvironment.CreateServiceUri("sb", serviceBusNamespace, "solver")));

            cf.Endpoint.Behaviors.Add(
                new TransportClientEndpointBehavior { 
                    TokenProvider = TokenProvider.CreateSharedSecretTokenProvider("owner", serviceBusSecret) });

            // Open channel and call the method
            var ch = cf.CreateChannel();
            string returnValue = ch.SendMessage(message);
            cf.Close();

            // return the updated message
            return returnValue;
        }

        /// <summary>
        /// Operation to create new site collections in the service side
        /// </summary>
        /// <param name="siteTitle"></param>
        /// <param name="siteTemplate"></param>
        /// <param name="ownerIdentifier"></param>
        /// <param name="serviceBusNameSpace"></param>
        /// <param name="serviceBusSecret"></param>
        public string SendSiteRequestMessage(SiteCollectionRequest request, string serviceBusNamespace, string serviceBusSecret)
        {
            var cf = new ChannelFactory<ISiteRequest>(
                        new NetTcpRelayBinding(),
                        new EndpointAddress(ServiceBusEnvironment.CreateServiceUri("sb", serviceBusNamespace, "solver")));

            cf.Endpoint.Behaviors.Add(
                new TransportClientEndpointBehavior { 
                    TokenProvider = TokenProvider.CreateSharedSecretTokenProvider("owner", serviceBusSecret) });

            // Open channel and call the method
            var ch = cf.CreateChannel();
            string createdUrl = ch.ProvisionSiteCollection(request);
            cf.Close();

            return createdUrl;
        }
    }
}
