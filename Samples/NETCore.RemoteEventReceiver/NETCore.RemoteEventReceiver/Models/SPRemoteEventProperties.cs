using System;
using System.Runtime.Serialization;
using System.ServiceModel;

namespace NETCore.RemoteEventReceiver.Models
{
	[DataContract(Name = "RemoteEventProperties", Namespace = "http://schemas.microsoft.com/sharepoint/remoteapp/")]
	public class SPRemoteEventProperties
	{
		[DataMember]
		public SPRemoteEventType EventType
		{
			get;
			set;
		}

		[DataMember]
		public Guid CorrelationId
		{
			get;
			set;
		}

		[DataMember]
		public string ContextToken
		{
			get;
			set;
		}

		[DataMember]
		public int CultureLCID
		{
			get;
			set;
		}

		[DataMember]
		public int UICultureLCID
		{
			get;
			set;
		}

		[DataMember]
		public string ErrorCode
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

		[DataMember]
		public SPRemoteItemEventProperties ItemEventProperties
		{
			get;
			set;
		}

		[DataMember]
		public SPRemoteListEventProperties ListEventProperties
		{
			get;
			set;
		}

		[DataMember]
		public SPRemoteWebEventProperties WebEventProperties
		{
			get;
			set;
		}

		[DataMember]
		public SPRemoteSecurityEventProperties SecurityEventProperties
		{
			get;
			set;
		}

		[DataMember]
		public SPRemoteAppEventProperties AppEventProperties
		{
			get;
			set;
		}

		[DataMember]
		public SPRemoteEntityInstanceEventProperties EntityInstanceEventProperties
		{
			get;
			set;
		}
	}
}
