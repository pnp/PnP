using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace NETCore.RemoteEventReceiver.Models
{
	[DataContract(Name = "RemoteItemEventProperties", Namespace = "http://schemas.microsoft.com/sharepoint/remoteapp/")]
	public class SPRemoteItemEventProperties
	{
		[DataMember(Name = "BeforeProperties")]
		private Dictionary<string, object> m_beforeProperties;

		[DataMember(Name = "AfterProperties")]
		private Dictionary<string, object> m_afterProperties;

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
		public int ListItemId
		{
			get;
			set;
		}

		[DataMember]
		public bool Versionless
		{
			get;
			set;
		}

		[DataMember]
		public string UserDisplayName
		{
			get;
			set;
		}

		[DataMember]
		public string UserLoginName
		{
			get;
			set;
		}

		[DataMember]
		public bool IsBackgroundSave
		{
			get;
			set;
		}

		[DataMember]
		public int CurrentUserId
		{
			get;
			set;
		}

		[DataMember]
		public string BeforeUrl
		{
			get;
			set;
		}

		[DataMember]
		public string AfterUrl
		{
			get;
			set;
		}

		[DataMember]
		public byte[] ExternalNotificationMessage
		{
			get;
			set;
		}

		public Dictionary<string, object> BeforeProperties
		{
			get
			{
				if (m_beforeProperties == null)
				{
					m_beforeProperties = new Dictionary<string, object>();
				}
				return m_beforeProperties;
			}
		}

		public Dictionary<string, object> AfterProperties
		{
			get
			{
				if (m_afterProperties == null)
				{
					m_afterProperties = new Dictionary<string, object>();
				}
				return m_afterProperties;
			}
		}
	}
}
