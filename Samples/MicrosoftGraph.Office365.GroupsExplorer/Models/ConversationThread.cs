using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OfficeDevPnP.MSGraphAPIGroups.Models
{
	public class ConversationThread
	{
		public Recipient[] ccRecipients { get; set; }
		public bool hasAttachments { get; set; }
		public string id { get; set; }
		public bool isLocked { get; set; }
		public string lastDeliveredDateTime { get; set; }
		public string preview { get; set; }
		public Recipient[] toRecipients { get; set; }
		public string topic { get; set; }
		public string[] uniqueSenders { get; set; }
	}
}