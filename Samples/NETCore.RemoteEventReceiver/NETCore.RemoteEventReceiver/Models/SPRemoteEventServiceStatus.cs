using System;

namespace NETCore.RemoteEventReceiver.Models
{
	public enum SPRemoteEventServiceStatus
	{
		Continue,
		CancelNoError,
		CancelWithError,
		[Obsolete("Default list forms are committed through asynchronous XmlHttpRequests, so redirect urls specified in this way aren't followed by default.  In order to force a list form to follow a cancelation redirect url, set the list form web part's CSRRenderMode property to CSRRenderMode.ServerRender")]
		CancelWithRedirectUrl
	}
}
