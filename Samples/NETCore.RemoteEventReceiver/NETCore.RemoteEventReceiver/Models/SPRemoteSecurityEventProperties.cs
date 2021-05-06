using System;
using System.Runtime.Serialization;

namespace NETCore.RemoteEventReceiver.Models
{
	[DataContract(Name = "RemoteSecurityEventProperties", Namespace = "http://schemas.microsoft.com/sharepoint/remoteapp/")]
	public class SPRemoteSecurityEventProperties
	{
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
		public Guid WebId
		{
			get;
			set;
		}

		[DataMember]
		public string WebFullUrl
		{
			get;
			set;
		}

		[DataMember]
		public string GroupName
		{
			get;
			set;
		}

		[DataMember]
		public int GroupId
		{
			get;
			set;
		}

		[DataMember]
		public int GroupOwnerId
		{
			get;
			set;
		}

		[DataMember]
		public int GroupNewOwnerId
		{
			get;
			set;
		}

		[DataMember]
		public int GroupUserId
		{
			get;
			set;
		}

		[DataMember]
		public string GroupUserLoginName
		{
			get;
			set;
		}

		[DataMember]
		public string RoleDefinitionName
		{
			get;
			set;
		}

		[DataMember]
		public ulong RoleDefinitionPermissions
		{
			get;
			set;
		}

		[DataMember]
		public int RoleDefinitionId
		{
			get;
			set;
		}

		[DataMember]
		public int ObjectType
		{
			get;
			set;
		}

		[DataMember]
		public string ScopeUrl
		{
			get;
			set;
		}

		[DataMember]
		public int PrincipalId
		{
			get;
			set;
		}
	}
}
