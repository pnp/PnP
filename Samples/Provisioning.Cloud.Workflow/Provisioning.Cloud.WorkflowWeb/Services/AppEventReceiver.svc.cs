using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.EventReceivers;

namespace Provisioning.Cloud.WorkflowWeb.Services
{
    public class AppEventReceiver : IRemoteEventService
    {
        public SPRemoteEventResult ProcessEvent(SPRemoteEventProperties properties)
        {
            SPRemoteEventResult result = new SPRemoteEventResult();

            using (ClientContext clientContext = TokenHelper.CreateAppEventClientContext(properties, false))
            {
                if (clientContext != null)
                {
                    clientContext.Load(clientContext.Web);
                    clientContext.ExecuteQuery();

                    var requestProperty = (System.ServiceModel.Channels.HttpRequestMessageProperty)System.ServiceModel.OperationContext.Current.IncomingMessageProperties[System.ServiceModel.Channels.HttpRequestMessageProperty.Name];
                    string opContext = System.ServiceModel.OperationContext.Current.Channel.LocalAddress.Uri.AbsoluteUri.Substring(0, System.ServiceModel.OperationContext.Current.Channel.LocalAddress.Uri.AbsoluteUri.LastIndexOf("/"));
                    string remoteUrl = string.Format("{0}/CreateSite.svc", opContext);
                    //string remoteUrl = string.Format("{0}/CreateSite2.svc", System.ServiceModel.OperationContext.Current.Channel.LocalAddress.Uri.DnsSafeHost + "/services");
                    var appWebUrl = "https://" + requestProperty.Headers[System.Net.HttpRequestHeader.Host];

                    List createSiteRequests = clientContext.Web.Lists.GetByTitle("SiteCreationRequests");
                    if (properties.EventType == SPRemoteEventType.AppInstalled)
                    {
                        EventReceiverDefinitionCreationInformation newEventReceiver = new EventReceiverDefinitionCreationInformation()
                        {
                            EventType = EventReceiverType.ItemUpdated,
                            ReceiverName = "CreateSite",
                            ReceiverUrl = remoteUrl,
                            SequenceNumber = 1000 //Should be higher number if lower priority, particularly for async events
                        };
                        createSiteRequests.EventReceivers.Add(newEventReceiver);
                        clientContext.ExecuteQuery();
                    }

                }

            }

            return result;
        }

        public void ProcessOneWayEvent(SPRemoteEventProperties properties)
        {
            // This method is not used by app events
        }
    }
}
