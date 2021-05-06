using System.ServiceModel;

namespace NETCore.RemoteEventReceiver.Models
{
	[ServiceContract(Namespace = "http://schemas.microsoft.com/sharepoint/remoteapp/")]
	public interface IRemoteEventService
	{
		[OperationContract]
		SPRemoteEventResult ProcessEvent(SPRemoteEventProperties properties);

		[OperationContract(IsOneWay = true)]
		void ProcessOneWayEvent(SPRemoteEventProperties properties);
	}
}
