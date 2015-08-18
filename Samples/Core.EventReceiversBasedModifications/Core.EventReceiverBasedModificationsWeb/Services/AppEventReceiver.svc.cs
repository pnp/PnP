using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.EventReceivers;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace Contoso.Core.EventReceiverBasedModificationsWeb.Services
{
    public class AppEventReceiver : IRemoteEventService
    {

        private const string RECEIVER_NAME = "ListAddedEvent";

        /// <summary>
        /// Handles app events that occur after the app is installed or upgraded, or when app is being uninstalled.
        /// </summary>
        /// <param name="properties">Holds information about the app event.</param>
        /// <returns>Holds information returned from the app event.</returns>
        public SPRemoteEventResult ProcessEvent(SPRemoteEventProperties properties)
        {
            SPRemoteEventResult result = new SPRemoteEventResult();

            switch (properties.EventType)
            {
                case SPRemoteEventType.AppInstalled:
                    HandleAppInstalled(properties);
                    break;
                case SPRemoteEventType.AppUninstalling:
                    HandleAppUninstalling(properties);
                    break;
                case SPRemoteEventType.ListAdded:
                    HandleListAdded(properties);
                    break;
            }

            return result;
        }

        /// <summary>
        /// This method is a required placeholder, but is not used by app events.
        /// </summary>
        /// <param name="properties">Unused.</param>
        public void ProcessOneWayEvent(SPRemoteEventProperties properties)
        {
            //throw new NotImplementedException();
        }

        private void HandleAppInstalled(SPRemoteEventProperties properties)
        {
            using (ClientContext cc = TokenHelper.CreateAppEventClientContext(properties, false))
            {
                if (cc != null)
                {
                    bool rerExists = false;
                    cc.Load(cc.Web.EventReceivers);
                    cc.ExecuteQuery();

                    foreach (var rer in cc.Web.EventReceivers)
                    {
                        if (rer.ReceiverName == RECEIVER_NAME)
                        {
                            rerExists = true;
                            System.Diagnostics.Trace.WriteLine("Found existing ListAdded receiver at " + rer.ReceiverUrl);
                        }
                    }

                    if (!rerExists)
                    {
                        EventReceiverDefinitionCreationInformation receiver = new EventReceiverDefinitionCreationInformation();
                        receiver.EventType = EventReceiverType.ListAdded;

                        //Get WCF URL where this message was handled
                        OperationContext op = OperationContext.Current;
                        Message msg = op.RequestContext.RequestMessage;
                        receiver.ReceiverUrl = msg.Headers.To.ToString();
                        receiver.ReceiverName = RECEIVER_NAME;
                        receiver.Synchronization = EventReceiverSynchronization.Synchronous;

                        cc.Web.EventReceivers.Add(receiver);
                        cc.ExecuteQuery();
                        System.Diagnostics.Trace.WriteLine("Added ListAdded receiver at " + msg.Headers.To.ToString());
                    }

                }
            }
        }

        private void HandleAppUninstalling(SPRemoteEventProperties properties)
        {
            using (ClientContext cc = TokenHelper.CreateAppEventClientContext(properties, false))
            {
                if (cc != null)
                {
                    cc.Load(cc.Web.EventReceivers);
                    cc.ExecuteQuery();
                    var rer = cc.Web.EventReceivers.Where(e => e.ReceiverName == RECEIVER_NAME).FirstOrDefault();

                    try
                    {
                        System.Diagnostics.Trace.WriteLine("Removing ListAdded receiver at " + rer.ReceiverUrl);

                        rer.DeleteObject();
                        cc.ExecuteQuery();
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Trace.WriteLine(ex.Message);
                    }
                }
            }
        }


        private void HandleListAdded(SPRemoteEventProperties properties)
        {
            using (ClientContext cc = TokenHelper.CreateRemoteEventReceiverClientContext(properties))
            {
                if (cc != null)
                {
                    try
                    {
                        if (properties.ListEventProperties.TemplateId == (int)ListTemplateType.DocumentLibrary)
                        {
                            //set versioning 
                            cc.Web.GetListByTitle(properties.ListEventProperties.ListTitle).UpdateListVersioning(true, true);
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Trace.WriteLine(ex.Message);
                    }
                }

            }

        }

    }
}
