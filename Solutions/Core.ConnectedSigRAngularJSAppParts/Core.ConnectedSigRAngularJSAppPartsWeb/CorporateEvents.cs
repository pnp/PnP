using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

using Core.ConnSigRAngJSApps.Models;
using OfficeDevPnP.Core;
using OfficeDevPnP.Core.Utilities;

using Microsoft.AspNet.SignalR.Hubs;
using Microsoft.AspNet.SignalR.Hosting;
using Microsoft.AspNet.SignalR;
using CorporateEvents;
using Microsoft.SharePoint.Client;
using System.Web;

namespace Core.ConnSigRAngJSApps
{
   
    public class CorporateEvents
    {
        private static IHubConnectionContext<dynamic> context = GlobalHost.ConnectionManager.GetHubContext<CorporateEventsHub>().Clients;

        // Singleton instance
        private readonly static Lazy<CorporateEvents> _instance = new Lazy<CorporateEvents>(
            () => new CorporateEvents(context));

        private readonly ConcurrentDictionary<string, Event> _corporateEvents = new ConcurrentDictionary<string, Event>();
        
        private CorporateEvents(IHubConnectionContext<dynamic> clients)
        {          

            Clients = clients;            
            
        }

        public static CorporateEvents Instance
        {
            get
            {
                return _instance.Value;
            }
        }

        private IHubConnectionContext<dynamic> Clients
        {
            get;
            set;
        }
        
       

        #region [ Public Methods ]
        public void Refresh()
        {          
          BroadcastEventsRefresh();            
        }

        public void SessionCancellation(string data)
        {
            BroadcastSessionCancellation(data);
        }

        public void SessionAddition(string data)
        {
            BroadcastSessionAddition(data);
        }

        public void EventCancellation(string data)
        {
            BroadcastEventCancellation(data);
        }

        public void EventAddition(string data)
        {
            BroadcastEventAddition(data);
        }
    
        public void SelectedEventChanged(string data)
        {
            BroadcastSelectedEventChange(data);
        }

        public void SelectedSessionChanged(string data)
        {
            BroadcastUpdateSpeakers(data);
        }

        public void EventStatus(string data)
        {
            BroadcastEventStatus(data);
        }

        public void SessionStatus(string data)
        {
            BroadcastSessionStatus(data);
        }

        public void UpdateSpeakers(string data)
        {
            BroadcastUpdateSpeakers(data);
        }


        #endregion

        #region [ Private Methods ]

        private void BroadcastEventsRefresh()
        {
            Clients.All.refresh();
        }        

        private void BroadcastEventCancellation(string data)
        {
            Clients.All.eventCancel(data);            
        }

        private void BroadcastEventAddition(string data)
        {
            Clients.All.eventAdded(data);
        }

        private void BroadcastSessionAddition(string data)
        {
            Clients.All.sessionAdded(data);
        }

        private void BroadcastSessionCancellation(string data)
        {
            Clients.All.sessionCancel(data);
        }

        private void BroadcastEventStatus(string data)
        {
            Clients.All.updateEventStatus(data);
        }

        private void BroadcastSessionStatus(string data)
        {
            Clients.All.updateSessionStatus(data);
        }

        private void BroadcastUpdateSpeakers(string data)
        {
            Clients.All.updateSpeakers(data);
        }

        private void BroadcastSelectedEventChange(string data)
        {
            Clients.All.eventChanged(data);
        }

        private void BroadcastSelectedSessionChange(string data)
        {
            Clients.All.sessionChanged(data);            
        }

        #endregion

    }    
}