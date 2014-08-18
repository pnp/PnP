using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfficeDevPnP.SPOnline.Core
{
    public static class SPOEvents
    {
        public static void RemoveEventReceiver(List list, Guid id, ClientContext clientContext)
        {
            var eventReceiver = GetEventReceivers(list, id, clientContext).FirstOrDefault();

            if (eventReceiver != null)
            {
                eventReceiver.DeleteObject();
                clientContext.ExecuteQuery();
            }
            else
            {
                throw new Exception("Event receiver not found");
            }
        }

        public static void RemoveEventReceiver(Web web, Guid id, ClientContext clientContext)
        {
            var eventReceiver = GetEventReceivers(web, id, clientContext).FirstOrDefault();
            if(eventReceiver != null)
            {
                eventReceiver.DeleteObject();
                clientContext.ExecuteQuery();
            }
            else
            {
                throw new Exception("Event receiver not found");
            }
        }

        public static List<EventReceiverDefinition> GetEventReceivers(List list, ClientContext clientContext)
        {
            return GetEventReceivers(list, Guid.Empty, clientContext);
        }

        public static List<EventReceiverDefinition> GetEventReceivers(List list, Guid id, ClientContext clientContext)
        {
            IEnumerable<EventReceiverDefinition> receivers = null;
            if (id == Guid.Empty)
            {
                var query = from receiver
                            in list.EventReceivers
                            select receiver;

                receivers = clientContext.LoadQuery(query);
            }
            else
            {
                var query = from receiver
                            in list.EventReceivers
                            where receiver.ReceiverId == id
                            select receiver;

                receivers = clientContext.LoadQuery(query);
            }
            clientContext.ExecuteQuery();

            return receivers.ToList();
        }

        public static List<EventReceiverDefinition> GetEventReceivers(Web web, ClientContext clientContext)
        {
            return GetEventReceivers(web, Guid.Empty, clientContext);
        }

        public static List<EventReceiverDefinition> GetEventReceivers(Web web, Guid id, ClientContext clientContext)
        {
            IEnumerable<EventReceiverDefinition> receivers = null;
            if (id == Guid.Empty)
            {
                var query = from receiver
                            in web.EventReceivers
                            select receiver;

                receivers = clientContext.LoadQuery(query);
            }
            else
            {
                var query = from receiver
                            in web.EventReceivers
                            where receiver.ReceiverId == id
                            select receiver;

                receivers = clientContext.LoadQuery(query);
            }
            clientContext.ExecuteQuery();

            return receivers.ToList();
        }

        public static EventReceiverDefinition RegisterEventReceiver(List list, string name, string url, EventReceiverType eventReceiverType, EventReceiverSynchronization synchronization, bool force, ClientContext clientContext)
        {
            clientContext.Load(list.EventReceivers);

            clientContext.ExecuteQuery();

            bool receiverExists = false;
            foreach (var receiver in list.EventReceivers)
            {
                if (receiver.ReceiverName == name)
                {
                    receiverExists = true;
                    if (force)
                    {
                        receiver.DeleteObject();
                        clientContext.ExecuteQuery();
                        receiverExists = false;
                    }
                }
            }
            EventReceiverDefinition def = null;
            if (!receiverExists)
            {
                EventReceiverDefinitionCreationInformation receiver = new EventReceiverDefinitionCreationInformation();
                receiver.EventType = eventReceiverType;
                receiver.ReceiverUrl = url;
                receiver.ReceiverName = name;
                receiver.Synchronization = synchronization;
                def = list.EventReceivers.Add(receiver);
                clientContext.Load(def);
                clientContext.ExecuteQuery();
            }
            return def;
        }

        public static EventReceiverDefinition RegisterEventReceiver(Web web, string name, string url, EventReceiverType eventReceiverType, EventReceiverSynchronization synchronization, bool force, ClientContext clientContext)
        {
            clientContext.Load(web.EventReceivers);

            clientContext.ExecuteQuery();

            bool receiverExists = false;
            foreach (var receiver in web.EventReceivers)
            {
                if (receiver.ReceiverName == name)
                {
                    receiverExists = true;
                    if (force)
                    {
                        receiver.DeleteObject();
                        clientContext.ExecuteQuery();
                        receiverExists = false;
                    }
                }
            }
            EventReceiverDefinition def = null;
            if (!receiverExists)
            {
                EventReceiverDefinitionCreationInformation receiver = new EventReceiverDefinitionCreationInformation();
                receiver.EventType = eventReceiverType;
                receiver.ReceiverUrl = url;
                receiver.ReceiverName = name;
                receiver.Synchronization = synchronization;
                def = web.EventReceivers.Add(receiver);
                clientContext.Load(def);
                clientContext.ExecuteQuery();
            }
            return def;
        }


    }
}
