using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace NETCore.RemoteEventReceiver.Models
{
	[DataContract(Name = "RemoteEventResult", Namespace = "http://schemas.microsoft.com/sharepoint/remoteapp/")]
	public class SPRemoteEventResult
	{
		private Dictionary<string, object> changedItemProperties;

		[DataMember]
		public SPRemoteEventServiceStatus Status
		{
			get;
			set;
		}

		[DataMember]
		public string ErrorMessage
		{
			get;
			set;
		}

		[Obsolete("Default list forms are committed through asynchronous XmlHttpRequests, so redirect urls specified in this way aren't followed by default.  In order to force a list form to follow a cancelation redirect url, set the list form web part's CSRRenderMode property to CSRRenderMode.ServerRender")]
		[DataMember]
		public string RedirectUrl
		{
			get;
			set;
		}

		[DataMember]
		public Dictionary<string, object> ChangedItemProperties
		{
			get
			{
				if (changedItemProperties == null)
				{
					changedItemProperties = new Dictionary<string, object>();
				}
				return changedItemProperties;
			}
		}
	}
}
