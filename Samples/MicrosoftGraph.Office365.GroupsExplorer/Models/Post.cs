using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OfficeDevPnP.MSGraphAPIGroups.Models
{
	public class Post
	{
		public ItemBody body { get; set; }
		public string[] categories { get; set; }
		public string changeKey { get; set; }
		public string conversationId { get; set; }
		public string conversationThreadId { get; set; }
		public string createdDateTime { get; set; } // DateTimeOffset 
		public Recipient from { get; set; }
		public bool hasAttachments { get; set; }
		public string id { get; set; }
		public string lastModifiedDateTime { get; set; } // DateTimeOffset  
		public Recipient[] newParticipants { get; set; }
		public string receivedDateTime { get; set; } // DateTimeOffset 
		public Recipient sender { get; set; }
	}
}