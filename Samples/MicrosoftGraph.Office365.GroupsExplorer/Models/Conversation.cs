using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OfficeDevPnP.MSGraphAPIGroups.Models
{
	public class Conversation
	{
		public Boolean hasAttachments { get; set; }
		public string id { get; set; }
		public string lastDeliveredDateTime { get; set; }  // DateTimeOffset
		public string preview { get; set; }
		public string topic { get; set; }
		public string[] uniqueSenders { get; set; }

	}
}