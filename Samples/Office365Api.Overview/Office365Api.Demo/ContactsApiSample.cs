using Microsoft.Office365.OAuth;
using Microsoft.Office365.OutlookServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Office365Api.Demo
{
    public static class ContactsAPISample
    {
        public static async Task<IEnumerable<IContact>> GetContacts()
        {
            var client = await EnsureClientCreated();

            // Obtain first page of contacts
            var contactsResults = await (from i in client.Me.Contacts
                                         orderby i.DisplayName
                                         select i).ExecuteAsync();
            
            return contactsResults.CurrentPage;
        }

        public static async Task<OutlookServicesClient> EnsureClientCreated()
        {
            var discoveryResult = await DiscoveryAPISample.DiscoveryClient.DiscoverCapabilityAsync(Office365Capabilities.Contacts.ToString());

            var ServiceResourceId = discoveryResult.ServiceResourceId;
            var ServiceEndpointUri = discoveryResult.ServiceEndpointUri;

            // Create the OutlookServicesClient client proxy:
            return new OutlookServicesClient(
                ServiceEndpointUri,
                async () =>
                {
                    return await AuthenticationHelper.GetAccessTokenForServiceAsync(discoveryResult);
                });
        }
    }
}
