using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OfficeDevPnP.MSGraphAPIGroups.Models
{
	public class Event
	{
		public Attendee[] attendees { get; set; }
		public ItemBody body { get; set; }
		public string bodyPreview { get; set; }
		public string[] categories { get; set; }
		public string changeKey { get; set; }
		public string createdDateTime { get; set; }  //DateTimeOffset 
		public DateTimeTimezone end { get; set; }
		public bool hasAttachments { get; set; }
		public string iCalUId { get; set; }
		public string id { get; set; }
		public string importance { get; set; } //	Low = 0, Normal = 1, High = 2.Possible values are: Low, Normal, High.
		public bool isAllDay { get; set; }
		public bool isCancelled { get; set; }
		public bool isOrganizer { get; set; }
		public bool isReminderOn { get; set; }
		public string lastModifiedDateTime { get; set; } //DateTimeOffset 
		public EventLocation location { get; set; }
		public Recipient organizer { get; set; }
		public string originalEndTimeZone { get; set; }
		public string originalStart { get; set; }  // DateTimeOffset 
		public string originalStartTimeZone { get; set; }
		public PatternedRecurrence? recurrence { get; set; }
		public int reminderMinutesBeforeStart { get; set; }
		public bool responseRequested { get; set; }
		public ResponseStatus responseStatus { get; set; }
		public string sensitivity { get; set; }   // Possible values are: Normal, Personal, Private, Confidential.
		public string seriesMasterId { get; set; }
		public string showAs { get; set; } // The status to show: Free = 0, Tentative = 1, Busy = 2, Oof = 3, WorkingElsewhere = 4, Unknown = -1.Possible values are: Free, Tentative, Busy, Oof, WorkingElsewhere, Unknown.
		public DateTimeTimezone start { get; set; }
		public string subject { get; set; }
		public string type { get; set; }  //The event type: SingleInstance = 0, Occurrence = 1, Exception = 2, SeriesMaster = 3.Possible values are: SingleInstance, Occurrence, Exception, SeriesMaster.

		public string webLink { get; set; }
	}
}