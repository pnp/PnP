using System;
using System.Runtime.Serialization;

namespace NETCore.RemoteEventReceiver.Models
{
	[DataContract(Name = "RemoteAppEventProperties", Namespace = "http://schemas.microsoft.com/sharepoint/remoteapp/")]
	public class SPRemoteAppEventProperties
	{
		[DataMember]
		public Uri HostWebFullUrl
		{
			get;
			set;
		}

		[DataMember]
		public Uri AppWebFullUrl
		{
			get;
			set;
		}

		[DataMember]
		public Version Version
		{
			get;
			set;
		}

		[DataMember]
		public Version PreviousVersion
		{
			get;
			set;
		}

		[DataMember]
		public Guid ProductId
		{
			get;
			set;
		}

		[DataMember]
		public string AssetId
		{
			get;
			set;
		}

		[DataMember]
		public string ContentMarket
		{
			get;
			set;
		}
	}
}
