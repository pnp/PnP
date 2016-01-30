using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.EventReceivers;
using System;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace Contoso.Core.EventReceiversWeb.Services
{
    using System.Runtime.Caching;
    using Contoso.Core.EventReceiversWeb.Cache;

    public class AppEventReceiver : IRemoteEventService
    {
        //the cache needs an unique name
        private static MemoryCache _cacheObject = new MemoryCache("A5390789156142B49AA51DBC19FCD9BC3831BDBC0B594D11A67DEC767315640B");

        /// <summary>
        
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


                case SPRemoteEventType.ItemAdding:
                    HandleItemAdding(properties, result);
                    break;
                case SPRemoteEventType.ItemUpdating:
                    HandleItemUpdating(properties, result);
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
        /// Handles the ItemAdding event by check the Description
        /// field of the item.
        /// </summary>
        /// <param name="properties"></param>
        private void HandleItemAdding(SPRemoteEventProperties properties, SPRemoteEventResult result)
        {
            using (ClientContext clientContext = TokenHelper.CreateRemoteEventReceiverClientContext(properties))
            {
                if (clientContext != null && EventFiringEnabled(clientContext, properties, "1"))
                {
                    new RemoteEventReceiverManager().ItemAddingToListEventHandler(clientContext, properties, result);
                }
            }
        }

        /// <summary>
        /// Handles the ItemUpdating event by check the Description
        /// field of the item.
        /// </summary>
        /// <param name="properties"></param>
        private void HandleItemUpdating(SPRemoteEventProperties properties, SPRemoteEventResult result)
        {
            using (ClientContext clientContext = TokenHelper.CreateRemoteEventReceiverClientContext(properties))
            {
                if (clientContext != null && EventFiringEnabled(clientContext, properties, "1"))
                {
                    new RemoteEventReceiverManager().ItemUpdatingToListEventHandler(clientContext, properties, result);
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
                if (clientContext != null && EventFiringEnabled(clientContext, properties, "2"))
                {
                    new RemoteEventReceiverManager().ItemAddedToListEventHandler(clientContext, properties.ItemEventProperties.ListId, properties.ItemEventProperties.ListItemId);
                }
            }
        }

        /// <summary>
        /// Used to chek if the event should processed or not. we use here a local MemoryCache object 
        /// or if the RER is loadbalanced a Redis Server on AZURE or onPrem
        /// </summary>
        /// <param name="ctx">the current context as ClientContext</param>
        /// <param name="properties">the event properies as SPRemoteEventProperties</param>
        /// <param name="eventType">identifier for event receiver type (~ing=1, ~ed=2) as String</param>
        /// <returns></returns>
        private static bool EventFiringEnabled(ClientContext ctx, SPRemoteEventProperties properties, string eventType)
        {
            try
            { 
                // set the correlation id for the next roundtrip to context
                ctx.TraceCorrelationId = properties.CorrelationId.ToString();

                var key = string.Concat(properties.CorrelationId.ToString("N"), eventType);
                var ts = TimeSpan.FromSeconds(20);

                if (SettingsHelper.UseAzureRedisCache == false)
                {
                    return _cacheObject.Add(key, properties.CorrelationId, new CacheItemPolicy
                    {
                        Priority = CacheItemPriority.Default,
                        SlidingExpiration = ts
                    });
                }
                else
                {
                    return CacheConnectionHelper.Connection.GetDatabase().Add(key, properties.CorrelationId, ts);
                }
            }
            catch (Exception oops)
            {
                System.Diagnostics.Trace.WriteLine(oops.Message);
            }

            return false;
        }


    }
}


