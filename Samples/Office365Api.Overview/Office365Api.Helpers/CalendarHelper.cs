using Microsoft.Office365.OutlookServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Office365Api.Helpers
{
    public class CalendarHelper : BaseOffice365Helper
    {
        public CalendarHelper(AuthenticationHelper authenticationHelper) : 
            base(authenticationHelper)
        {
        }

        public async Task<IOrderedEnumerable<IEvent>> GetCalendarEvents()
        {
            var client = await this.AuthenticationHelper
                .EnsureOutlookServicesClientCreatedAsync(
                Office365Capabilities.Calendar.ToString());

            // Obtain calendar event data
            var eventsResults = await (from i in client.Me.Events
                                      where i.End >= DateTimeOffset.UtcNow
                                      select i).Take(10).ExecuteAsync();

            var events = eventsResults.CurrentPage.OrderBy(e => e.Start);

            return events;
        }
    }
}
