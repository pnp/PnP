using System.Collections.Generic;

namespace OutlookNotificationsAPI.WebAPI.Models
{
    public class ResponseStatusModel
    {
        public string Response { get; set; }
        public string Time { get; set; }
    }

    public class BodyModel
    {
        public string ContentType { get; set; }
        public string Content { get; set; }
    }

    public class DateTimeModel
    {
        public string DateTime { get; set; }
        public string TimeZone { get; set; }
    }

    public class LocationModel
    {
        public string DisplayName { get; set; }
    }

    public class EmailAddressModel
    {
        public string Name { get; set; }
        public string Address { get; set; }
    }

    public class OrganizerModel
    {
        public EmailAddressModel EmailAddress { get; set; }
    }

    public class CalendarEventModel
    {
        public string Id { get; set; }
        public string CreatedDateTime { get; set; }
        public string LastModifiedDateTime { get; set; }
        public string ChangeKey { get; set; }
        public List<object> Categories { get; set; }
        public string OriginalStartTimeZone { get; set; }
        public string OriginalEndTimeZone { get; set; }
        public ResponseStatusModel ResponseStatus { get; set; }
        public string iCalUId { get; set; }
        public int ReminderMinutesBeforeStart { get; set; }
        public bool IsReminderOn { get; set; }
        public bool HasAttachments { get; set; }
        public string Subject { get; set; }
        public BodyModel Body { get; set; }
        public string BodyPreview { get; set; }
        public string Importance { get; set; }
        public string Sensitivity { get; set; }
        public DateTimeModel Start { get; set; }
        public DateTimeModel End { get; set; }
        public LocationModel Location { get; set; }
        public bool IsAllDay { get; set; }
        public bool IsCancelled { get; set; }
        public bool IsOrganizer { get; set; }
        public object Recurrence { get; set; }
        public bool ResponseRequested { get; set; }
        public object SeriesMasterId { get; set; }
        public string ShowAs { get; set; }
        public string Type { get; set; }
        public List<object> Attendees { get; set; }
        public OrganizerModel Organizer { get; set; }
        public string WebLink { get; set; }
    }
}