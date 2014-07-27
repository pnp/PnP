using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;

namespace ECM.AutoTaggingWeb
{

    public class ReceiverHelper
    {
        public static EventReceiverDefinitionCreationInformation CreateEventReciever(string receiverName, EventReceiverType type)
        {
            EventReceiverDefinitionCreationInformation _rer = new EventReceiverDefinitionCreationInformation();
            _rer.EventType = type;
            _rer.ReceiverAssembly = Assembly.GetExecutingAssembly().FullName;
            _rer.ReceiverClass = "Core.ReRWeb.Services.InformationManagementReceiver";
            _rer.ReceiverName = receiverName;
            _rer.ReceiverUrl = "https://informationmanagementrer.azurewebsites.net/Services/InformationManagement.svc";
            _rer.SequenceNumber = 10000;
            return _rer;
        }

        public static bool DoesEventReceiverExist(string eventReceiverName, ClientContext ctx, List list)
        {
            bool _doesExist = false;
            ctx.Load(list, lib => lib.EventReceivers);
            ctx.ExecuteQuery();

            foreach (EventReceiverDefinition _def in list.EventReceivers)
            {
                if (eventReceiverName.Equals(_def.ReceiverName, StringComparison.InvariantCultureIgnoreCase))
                {
                    _doesExist = true;
                    break;
                }
            }
            return _doesExist;
        }

        public static void AddEventReceiver(ClientContext ctx, List list, EventReceiverDefinitionCreationInformation eventReceiverInfo)
        {
            if (!DoesEventReceiverExist(eventReceiverInfo.ReceiverName, ctx, list))
            {
                list.EventReceivers.Add(eventReceiverInfo);
                ctx.ExecuteQuery();
            }
        }

        public static void RemoveEventReceiver(ClientContext ctx, List list, string receiverName)
        {
            if (DoesEventReceiverExist(receiverName, ctx, list))
            {
                ctx.Load(list, lib => lib.EventReceivers);
                ctx.ExecuteQuery();

                var rer = list.EventReceivers.Where(e => e.ReceiverName == receiverName).FirstOrDefault();
                rer.DeleteObject();
                ctx.ExecuteQuery();
            }
        }
    }
}