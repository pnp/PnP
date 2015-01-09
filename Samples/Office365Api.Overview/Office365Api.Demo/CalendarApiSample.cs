using Microsoft.Office365.OAuth;
using Microsoft.Office365.OutlookServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Office365Api.Demo
{
    public static class CalendarAPISample
    {
        public static async Task<IOrderedEnumerable<IEvent>> GetCalendarEvents()
        {
            var client = await EnsureClientCreated();

            // Obtain calendar event data
            var eventsResults = await (from i in client.Me.Events
                                      where i.End >= DateTimeOffset.UtcNow
                                      select i).Take(10).ExecuteAsync();

            var events = eventsResults.CurrentPage.OrderBy(e => e.Start);

            return events;
        }

        public static async Task<OutlookServicesClient> EnsureClientCreated()
        {
            var discoveryResult = await DiscoveryAPISample.DiscoveryClient.DiscoverCapabilityAsync(Office365Capabilities.Calendar.ToString());

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
