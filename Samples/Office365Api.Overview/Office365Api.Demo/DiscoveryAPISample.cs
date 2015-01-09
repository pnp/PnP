using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Office365.Discovery;
using Microsoft.Office365.OAuth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Office365Api.Demo
{
    static class DiscoveryAPISample
    {
        static DiscoveryAPISample()
        {
            DiscoveryClient discoveryClient =
                new DiscoveryClient(
                    Office365ServicesUris.DiscoveryServiceEndpointUri,
                                    async () =>
                                    {
                                        var discoveryAuthResult =
                                            await AuthenticationHelper.AuthenticationContext.AcquireTokenSilentAsync(
                                                Office365ServicesUris.DiscoveryServiceResourceId,
                                                AuthenticationHelper.ClientId,
                                                new UserIdentifier(
                                                    AuthenticationHelper.AuthenticationResult.UserInfo.UniqueId, 
                                                    UserIdentifierType.UniqueId));

                                        return discoveryAuthResult.AccessToken;
                                    });

            DiscoveryAPISample.DiscoveryClient = discoveryClient;
        }

        public static DiscoveryClient DiscoveryClient
        {
            get;
            private set;
        }
        
        private static async Task<CapabilityDiscoveryResult> DiscoverCapabilityInternalAsync(String capabilityName)
        {
            if (DiscoveryClient == null)
            {
                throw new ApplicationException("Missing the DiscoveryClient object!");
            }

            var dcr = await DiscoveryClient.DiscoverCapabilityAsync(capabilityName);
            return dcr;
        }

        // Discovery service supports MyFiles, Mail, Contacts and Calendar
        public static async Task<CapabilityDiscoveryResult> DiscoverMail()
        {
            return (await DiscoverCapabilityInternalAsync(Office365Capabilities.Mail.ToString()));
        }

        public static async Task<CapabilityDiscoveryResult> DiscoverContacts()
        {
            return (await DiscoverCapabilityInternalAsync(Office365Capabilities.Contacts.ToString()));
        }

        public static async Task<CapabilityDiscoveryResult> DiscoverCalendar()
        {
            return (await DiscoverCapabilityInternalAsync(Office365Capabilities.Calendar.ToString()));
        }

        public static async Task<CapabilityDiscoveryResult> DiscoverMyFiles()
        {
            return (await DiscoverCapabilityInternalAsync(Office365Capabilities.MyFiles.ToString()));
        }
    }
}
