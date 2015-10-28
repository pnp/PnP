using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace BusinessApps.RemoteCalendarAccess.Models.CalendarModel
{
    public class Event
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public DateTime Created { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public string Category { get; set; }
        public string UID { get; set; }
        public DateTime EventDate { get; set; }
        public DateTime EndDate { get; set; }
        public int Duration { get; set; }
        public bool Recurrence { get; set; }
        public string RecurrenceData { get; set; }
        public DateTime RecurrenceID { get; set; }
        public int MasterSeriesItemID { get; set; }
        public EventType EventType { get; set; }
        public bool AllDayEvent { get; set; }
        public DateTime LastModified { get; set; }

        public static Event Parse(ListItem sharepointListItem)
        {
            Event e = new Event()
            {
                ID = (int)sharepointListItem["ID"],
                Title = (string)sharepointListItem["Title"],
                Created = DateTime.Parse(sharepointListItem["Created"].ToString()),
                Description = (string)sharepointListItem["Description"],
                Location = (string)sharepointListItem["Location"],
                Category = (string)sharepointListItem["Category"],
                UID = sharepointListItem["UID"] == null ? Guid.NewGuid().ToString() : sharepointListItem["UID"].ToString(),
                EventDate = DateTime.Parse(sharepointListItem["EventDate"].ToString()),
                EndDate = DateTime.Parse(sharepointListItem["EndDate"].ToString()),
                Duration = (int)sharepointListItem["Duration"],
                Recurrence = (bool)sharepointListItem["fRecurrence"],
                RecurrenceData = (string)sharepointListItem["RecurrenceData"],
                RecurrenceID = sharepointListItem["RecurrenceID"] != null ? DateTime.Parse(sharepointListItem["RecurrenceID"].ToString()) : DateTime.MinValue,
                MasterSeriesItemID = sharepointListItem["MasterSeriesItemID"] == null ? -1 : (int)sharepointListItem["MasterSeriesItemID"],
                EventType = (EventType)Enum.Parse(typeof(EventType), sharepointListItem["EventType"].ToString()),
                AllDayEvent = (bool)sharepointListItem["fAllDayEvent"],
                LastModified = DateTime.Parse(sharepointListItem["Last_x0020_Modified"].ToString())
            };

            return e;
        }
     
        public string ToString(List<Event> Events)
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendLine("BEGIN:VEVENT");
            builder.AppendLine("SUMMARY:" + CleanText(Title));
            builder.AppendLine("DTSTAMP:" + Created.ToString("yyyyMMddTHHmmssZ"));
            builder.AppendLine("DESCRIPTION:" + CleanText(Description));
            builder.AppendLine("LOCATION:" + CleanText(Location));
            builder.AppendLine("CATEGORIES:" + CleanText(Category));
            builder.AppendLine("UID:" + UID);
            builder.AppendLine("STATUS:CONFIRMED");
            builder.AppendLine("LAST-MODIFIED:" + LastModified.ToString("yyyyMMddTHHmmssZ"));

            if(AllDayEvent)
            {
                builder.AppendLine("DTSTART;VALUE=DATE:" + EventDate.ToString("yyyyMMdd"));

                double days = Math.Round(((Double)Duration / (double)(60 * 60 * 24)));
                builder.AppendLine("DTEND;VALUE=DATE:" + EventDate.AddDays(days).ToString("yyyyMMdd"));
            }
            else
            {
                builder.AppendLine("DTSTART:" + EventDate.ToString("yyyyMMddTHHmmssZ"));
                builder.AppendLine("DTEND:" + EventDate.AddSeconds(Duration).ToString("yyyyMMddTHHmmssZ"));
            }

            IEnumerable<Event> deletedEvents = Events.Where(e => e.MasterSeriesItemID == ID && e.EventType == EventType.Deleted);
            foreach(Event deletedEvent in deletedEvents)
            {
                if(AllDayEvent)
                    builder.AppendLine("EXDATE;VALUE=DATE:" + deletedEvent.RecurrenceID.ToString("yyyyMMdd"));
                else
                    builder.AppendLine("EXDATE:" + deletedEvent.RecurrenceID.ToString("yyyyMMddTHHmmssZ"));
            }

            if(RecurrenceID != DateTime.MinValue && EventType == EventType.Exception) //  Event is exception to recurring item
            {
                if(AllDayEvent)
                    builder.AppendLine("RECURRENCE-ID;VALUE=DATE:" + RecurrenceID.ToString("yyyyMMdd"));
                else
                    builder.AppendLine("RECURRENCE-ID:" + RecurrenceID.ToString("yyyyMMddTHHmmssZ"));
            }
            else if (Recurrence && !RecurrenceData.Contains("V3RecurrencePattern"))
            {
                RecurrenceHelper recurrenceHelper = new RecurrenceHelper();
                builder.AppendLine(recurrenceHelper.BuildRecurrence(RecurrenceData, EndDate));
            }

            if(EventType == CalendarModel.EventType.Exception)
            {
                List<Event> exceptions = Events.Where(e => e.MasterSeriesItemID == MasterSeriesItemID).OrderBy(e => e.Created).ToList<Event>();
                builder.AppendLine("SEQUENCE:" + (exceptions.IndexOf(this) + 1));
            }
            else
                builder.AppendLine("SEQUENCE:0");

            builder.AppendLine("BEGIN:VALARM");
            builder.AppendLine("ACTION:DISPLAY");
            builder.AppendLine("TRIGGER:-PT10M");
            builder.AppendLine("DESCRIPTION:Reminder");
            builder.AppendLine("END:VALARM");

            builder.AppendLine("END:VEVENT");

            return builder.ToString();
        }

        private string CleanText(string text)
        {
            if (text != null)
                text = text.Replace("\"", "DQUOTE")
                            .Replace("\\", "\\\\")
                            .Replace(",", "\\,")
                            .Replace(":", "\":\"")
                            .Replace(";", "\\;")
                            .Replace("\r\n", "\\n")
                            .Replace("\n", "\\n");

            return text;
        }
    }
}