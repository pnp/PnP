using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace BusinessApps.RemoteCalendarAccess.Models.CalendarModel
{
    public class Calendar
    {
        public Timezone Timezone { get; set; }
        public List<Event> Events { get; set; }
        public string Title { get; set; }
        public Calendar()
        {
            Events = new List<Event>();
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendLine("BEGIN:VCALENDAR");
            builder.AppendLine("VERSION:2.0");
            builder.AppendLine("METHOD:PUBLISH");
            builder.AppendLine("PRODID:Remote Calendar Access");
            builder.AppendLine("X-WR-CALNAME:" + Title);
            builder.AppendLine("X-PUBLISHED-TTL:PT5M");

            builder.Append(Timezone.ToString());

            foreach(Event e in Events)
            {
                if(e.EventType != EventType.Deleted)
                    builder.Append(e.ToString(Events));
            }

            builder.AppendLine("END:VCALENDAR");

            return builder.ToString();
        }
    }
}