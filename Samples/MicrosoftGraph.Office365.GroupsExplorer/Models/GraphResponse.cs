using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OfficeDevPnP.MSGraphAPIGroups.Models
{

	public class GraphResponse<T>
	{
		public List<T> value { get; set; }
	}

}