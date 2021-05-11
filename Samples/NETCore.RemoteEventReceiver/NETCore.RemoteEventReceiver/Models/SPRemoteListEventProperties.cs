using System;
using System.Runtime.Serialization;

namespace NETCore.RemoteEventReceiver.Models
{
	[DataContract(Name = "RemoteListEventProperties", Namespace = "http://schemas.microsoft.com/sharepoint/remoteapp/")]
	public class SPRemoteListEventProperties
	{
		[DataMember]
		public string WebUrl
		{
			get;
			set;
		}

		[DataMember]
		public Guid ListId
		{
			get;
			set;
		}

		[DataMember]
		public string ListTitle
		{
			get;
			set;
		}

		[DataMember]
		public string FieldName
		{
			get;
			set;
		}

		[DataMember]
		public string FieldXml
		{
			get;
			set;
		}

		[DataMember]
		public int TemplateId
		{
			get;
			set;
		}

		[DataMember]
		public Guid FeatureId
		{
			get;
			set;
		}
	}
}
