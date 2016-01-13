using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OfficeDevPnP.MSGraphAPIDemo.Models
{
    /// <summary>
    /// Defines a user's calendar's event
    /// </summary>
    public class Event : BaseModel
    {
        /// <summary>
        /// The list of email address for the event's attendees
        /// </summary>
        public List<UserInfoContainer> Attendees { get; set; }

        /// <summary>
        /// The body of the email message for the event
        /// </summary>
        public ItemBody Body { get; set; }

        /// <summary>
        /// The subject of the event
        /// </summary>
        public String Subject { get; set; }

        /// <summary>
        /// The Type of the event
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public EventType Type { get; set; }

        /// <summary>
        /// Date and time of creation
        /// </summary>
        public Nullable<DateTime> CreatedDateTime { get; set; }

        /// <summary>
        /// Defines whether the event is an all day event
        /// </summary>
        public Boolean IsAllDay { get; set; }

        /// <summary>
        /// Defines whether the current user is the organizer of the event
        /// </summary>
        public Boolean IsOrganizer { get; set; }

        /// <summary>
        /// The importance of the email message for the event
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public ItemImportance Importance { get; set; }

        /// <summary>
        /// The location of the event
        /// </summary>
        public EventLocation Location { get; set; }

        /// <summary>
        /// The event organizer
        /// </summary>
        public UserInfo Organizer { get; set; }

        /// <summary>
        /// The Original Zone of the end time
        /// </summary>
        public String OriginalStartTimeZone { get; set; }

        /// <summary>
        /// The Original Zone of the end time
        /// </summary>
        public String OriginalEndTimeZone { get; set; }

        /// <summary>
        /// The start date and time of the event
        /// </summary>
        public TimeInfo Start { get; set; }

        /// <summary>
        /// The end date and time of the event
        /// </summary>
        public TimeInfo End { get; set; }

        /// <summary>
        /// The status (show as) of the event
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public EventStatus ShowAs { get; set; }

        /// <summary>
        /// The ID of the Master Event of the Series of events
        /// </summary>
        public String SeriesMasterId { get; set; }

        /// <summary>
        /// The Recurrence pattern for the Series of events
        /// </summary>
        public EventRecurrence Recurrence { get; set; }

        /// <summary>
        /// The Response Status for a Meeting Request
        /// </summary>
        public EventResponseStatus ResponseStatus { get; set; }
    }

    /// <summary>
    /// Defines the type of event
    /// </summary>
    public enum EventType
    {
        /// <summary>
        /// Single instance event
        /// </summary>
        SingleInstance,
        /// <summary>
        /// Master of a Series of events
        /// </summary>
        SeriesMaster,
        /// <summary>
        /// Recurring event
        /// </summary>
        Occurrence,
        /// <summary>
        /// Exception of a Recurring event
        /// </summary>
        Exception,
    }

    /// <summary>
    /// Defines the status (show as) of an event
    /// </summary>
    public enum EventStatus
    {
        /// <summary>
        /// Free
        /// </summary>
        Free,
        /// <summary>
        /// Tentative
        /// </summary>
        Tentative,
        /// <summary>
        /// Busy
        /// </summary>
        Busy,
        /// <summary>
        /// Out of Office
        /// </summary>
        Oof,
        /// <summary>
        /// Working elsewhere
        /// </summary>
        WorkingElsewhere,
        /// <summary>
        /// Unknown
        /// </summary>
        Unknown,
    }
}