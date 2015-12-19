using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.EventReceivers;
using System;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace Contoso.Core.EventReceiversWeb.Services
{

    public class AppEventReceiver : IRemoteEventService
    {
        // in the example code on the bottom the new lines handle or prevent an RER to refiring itself
        // the best scenario would be an ItemUpdating and ItemUpdated RER
        // this used example was always a demonstration for an Added RER (because he will not refire;)

        // you need a reference to System.Runtime.Caching.dll
        // the cache needs an unique name
        private static System.Runtime.Caching.MemoryCache memoryCache =
            new System.Runtime.Caching.MemoryCache("A5390789156142B49AA51DBC19FCD9BC3831BDBC0B594D11A67DEC767315640B");

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
                case SPRemoteEventType.ItemAdded:
                    HandleItemAdded(properties);
                    break;
            }


            return result;
        }


        public void ProcessOneWayEvent(SPRemoteEventProperties properties)
        {

        }


        /// <summary>
        /// Handles when an app is installed.  Activates a feature in the
        /// host web.  The feature is not required.  
        /// Next, if the Jobs list is
        /// not present, creates it.  Finally it attaches a remote event
        /// receiver to the list.  
        /// </summary>
        /// <param name="properties"></param>
        private void HandleAppInstalled(SPRemoteEventProperties properties)
        {
            using (ClientContext clientContext =
                TokenHelper.CreateAppEventClientContext(properties, false))
            {
                if (clientContext != null)
                {
                    new RemoteEventReceiverManager().AssociateRemoteEventsToHostWeb(clientContext);
                }
            }
        }

        /// <summary>
        /// Removes the remote event receiver from the list and 
        /// adds a new item to the list.
        /// </summary>
        /// <param name="properties"></param>
        private void HandleAppUninstalling(SPRemoteEventProperties properties)
        {
            using (ClientContext clientContext =
                TokenHelper.CreateAppEventClientContext(properties, false))
            {
                if (clientContext != null)
                {
                    new RemoteEventReceiverManager().RemoveEventReceiversFromHostWeb(clientContext);
                }
            }
        }

        /// <summary>
        /// Handles the ItemAdded event by modifying the Description
        /// field of the item.
        /// </summary>
        /// <param name="properties"></param>
        private void HandleItemAdded(SPRemoteEventProperties properties)
        {
            using (ClientContext clientContext =
                TokenHelper.CreateRemoteEventReceiverClientContext(properties))
            {
                // we use "1" and "2" for the different EventReceiverSynchronization types ~ing / ~ed
                if (clientContext != null && EventFiringEnabled(clientContext, properties, "2"))
                {
                    new RemoteEventReceiverManager().ItemAddedToListEventHandler(clientContext, properties.ItemEventProperties.ListId, properties.ItemEventProperties.ListItemId);
                }

            }

        }

        
        /// <summary>
        /// prevents an RER to refire itself (if we make update in ItemUpdated, the ItemUpdating is fired ..)
        /// EventReceiverSynchronization.Asynchronous = 1
        /// EventReceiverSynchronization.Synchronous = 2
        /// if eventReceiverSynchronizationType is always empty we allow only one event type in the flow.. 
        /// </summary>
        /// <param name="ctx">context to set the TraceCorrelationId as ClientContext</param>
        /// <param name="properties">event receiver properties to get the CorrelationId as SPRemoteEventProperties</param>
        /// <param name="eventReceiverSynchronizationType">a string indicating the EventReceiverSynchronization type, or empty String</param>
        /// <returns></returns>
        private bool EventFiringEnabled(ClientContext ctx, SPRemoteEventProperties properties, string eventReceiverSynchronizationType)
        {
            // set the correlation id for the next roundtrip to context
            ctx.TraceCorrelationId = properties.CorrelationId.ToString();

            // if the key is not in our cache, the key will be added to the cache
            // and returns true, otherwise false
            return memoryCache.Add(
                new System.Runtime.Caching.CacheItem(string.Concat(properties.CorrelationId.ToString("N"), eventReceiverSynchronizationType),
                              properties.CorrelationId),
                              new System.Runtime.Caching.CacheItemPolicy
                              {
                                  Priority = System.Runtime.Caching.CacheItemPriority.Default,
                                  SlidingExpiration = System.Diagnostics.Debugger.IsAttached
                                    ? TimeSpan.FromMinutes(3)
                                    : TimeSpan.FromSeconds(3)
                              });
        }
    }
}


