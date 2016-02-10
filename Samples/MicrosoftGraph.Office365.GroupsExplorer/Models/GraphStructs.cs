using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OfficeDevPnP.MSGraphAPIGroups.Models
{
	public class Attendee
	{
		public ResponseStatus status { get; set; }
		public string type { get; set; }  //The attendee type:  Required ,  Optional ,  Resource .
		public EmailAddress emailAddress { get; set; }

		public override string ToString()
		{
			return (emailAddress == null) ? type : String.Format("{0}: {1}", type, emailAddress.name);
		}

	}

	public struct Audio
	{
		public string album { get; set; }
		public string albumArtist { get; set; }
		public string artist { get; set; }
		public string bitrate { get; set; } //kbps
		public string composers { get; set; }
		public string copyright { get; set; }
		public int disc { get; set; }
		public int discCount { get; set; }
		public int duration { get; set; }  //milliseconds
		public string genre { get; set; }
		public bool hasDrm { get; set; }
		public bool isVariableBitrate { get; set; }
		public string title { get; set; }
		public int track { get; set; }
		public int trackCount { get; set; }
		public int year { get; set; }
	}
	public struct DateTimeTimezone
	{
		public DateTime dateTime { get; set; }
		public string Timezone { get; set; }
	}

	public struct Deleted
	{
		public string state { get; set; }
	}

	public class EmailAddress
	{
		public string address { get; set; }
		public string name { get; set; }
	}

	public struct File
	{
		public object hashes { get; set; }
		public string mimeType { get; set; }
	}

	public struct FileSystemInfo
	{
		public string createdDateTime { get; set; } // DateTimeOffset
		public string lastModifiedDateTime { get; set; } // DateTimeOffset
	}

	public struct FolderMetadata
	{
		public Int64 childCount { get; set; }
	}

	public struct ImageMetadata
	{
		public Int32 height { get; set; }
		public Int32 width { get; set; }
	}

	public struct IdentitySet
	{
		public Identity application { get; set; }
		public Identity device { get; set; }
		public Identity user { get; set; }
	}

	public struct Identity
	{
		public string displayName { get; set; }
		public string id { get; set; }
	}

	public struct ItemBody
	{
		public string content { get; set; }
		public string contentType { get; set; }
	}

	public struct ItemReference
	{
		public string driveId { get; set; }
		public string id { get; set; }
		public string path { get; set; }
	}

	public struct EventLocation
	{
		public string displayName { get; set; }
		public LocationAddresss address { get; set; }
		public LocationCoordinates coordinates { get; set; }
	}

	public struct LocationAddresss
	{
		public string street { get; set; }
		public string city { get; set; }
		public string state { get; set; }
		public string countryOrRegion { get; set; }
		public string postalCode { get; set; }

	}
	public struct LocationCoordinates
	{
		public double altitude { get; set; }
		public double latitude { get; set; }
		public double longitude { get; set; }
	}

	public struct PatternedRecurrence
	{
		public RecurrencePattern pattern { get; set; }
		public RecurrenceRange range { get; set; }
	}

	public struct Photo
	{
		public string takenDateTime { get; set; } // DateTimeOffset
		public string cameraMake { get; set; }
		public string cameraModel { get; set; }
		public string fNumber { get; set; }
		public Int32 exposureDenominator { get; set; }
		public Int32 exposureNumerator { get; set; }
		public double focalLength { get; set; }
		public Int32 iso { get; set; }
	}

	public struct Quota
	{
		public Int64 deleted { get; set; }
		public Int64 remaining { get; set; }
		public string state { get; set; }  // Can be normal, nearing, critical, or exceeded. 
		public Int64 total { get; set; }
		public Int64 used { get; set; }

	}

	public struct RecurrencePattern
	{
		public int dayOfMonth { get; set; }
		public string[] daysOfWeek { get; set; }     //	Possible values are: Sunday, Monday, Tuesday, Wednesday, Thursday, Friday, Saturday.
		public string firstDayOfWeek { get; set; }   // Possible values are: Sunday, Monday, Tuesday, Wednesday, Thursday, Friday, Saturday.
		public string index { get; set; } // First, Second, Third, Fourth, Last.
		public int interval { get; set; }
		public int month { get; set; }
		public string type { get; set; }   //The recurrence pattern type: Daily, Weekly, AbsoluteMonthly, RelativeMonthly, AbsoluteYearly, RelativeYearly.

	}

	public struct RecurrenceRange
	{
		public string endDate { get; set; }  //Date 
		public int numberOfOccurrences { get; set; }
		public string recurrenceTimeZone { get; set; }
		public string startDate { get; set; }  //Date 
		public string type { get; set; }
	}

	public struct Recipient
	{
		public EmailAddress emailAddress { get; set; }
	}

	public struct ResponseStatus
	{
		public string response { get; set; } //  None = 0, Organizer = 1, TentativelyAccepted = 2, Accepted = 3, Declined = 4, NotResponded = 5. Possible values are: None, Organizer, TentativelyAccepted, Accepted, Declined, NotResponded.
		public string time { get; set; } // DateTimeOffset
	}

	public struct SearchResult
	{
		public string onClickTelemetryUrl { get; set; }
	}

	public struct SpecialFolder
	{
		public string name { get; set; }
	}

	public struct Video
	{
		public Int32 bitrate { get; set; }
		public Int64 duration { get; set; } // milliseconds
		public Int32 height { get; set; }
		public Int32 width { get; set; }
	}
}