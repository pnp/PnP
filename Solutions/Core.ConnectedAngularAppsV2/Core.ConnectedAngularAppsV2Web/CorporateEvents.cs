using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

using Core.ConnectedAngularAppsV2Web.Models;
using OfficeDevPnP.Core;
using OfficeDevPnP.Core.Utilities;

using Microsoft.AspNet.SignalR.Hubs;
using Microsoft.AspNet.SignalR.Hosting;
using Microsoft.AspNet.SignalR;
using CorporateEvents;
using Microsoft.SharePoint.Client;
using System.Web;

namespace Core.ConnectedAngularAppsV2Web
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

        public void SessionCancellation(string sessionName, string data)
        {
            BroadcastSessionCancellation(sessionName, data);
        }

        public void SessionAddition(string sessionName, string data)
        {
            BroadcastSessionAddition(sessionName, data);
        }

        public void EventCancellation(string sessionName, string data)
        {
            BroadcastEventCancellation(sessionName, data);
        }

        public void EventAddition(string sessionName, string data)
        {
            BroadcastEventAddition(sessionName, data);
        }

        public void SelectedEventChanged(string sessionName, string data)
        {
            BroadcastSelectedEventChange(sessionName, data);
        }

        public void SelectedSessionChanged(string sessionName, string data)
        {
            BroadcastUpdateSpeakers(sessionName, data);
        }

        public void EventStatus(string sessionName, string data)
        {
            BroadcastEventStatus(sessionName, data);
        }

        public void SessionStatus(string sessionName, string data)
        {
            BroadcastSessionStatus(sessionName, data);
        }

        public void UpdateSpeakers(string sessionName, string data)
        {
            BroadcastUpdateSpeakers(sessionName, data);
        }


        #endregion

        #region [ Private Methods ]

        private void BroadcastEventsRefresh()
        {
            Clients.All.refresh();
        }

        private void BroadcastEventCancellation(string sessionName, string data)
        {
            Clients.Group(sessionName).eventCancel(data);            
        }

        private void BroadcastEventAddition(string sessionName, string data)
        {
            Clients.Group(sessionName).eventAdded(data);
        }

        private void BroadcastSessionAddition(string sessionName, string data)
        {
            Clients.Group(sessionName).sessionAdded(data);
        }

        private void BroadcastSessionCancellation(string sessionName, string data)
        {
            Clients.Group(sessionName).sessionCancel(data);
        }

        private void BroadcastEventStatus(string sessionName, string data)
        {
            Clients.Group(sessionName).updateEventStatus(data);
        }

        private void BroadcastSessionStatus(string sessionName, string data)
        {
            Clients.Group(sessionName).updateSessionStatus(data);
        }

        private void BroadcastUpdateSpeakers(string sessionName, string data)
        {
            Clients.Group(sessionName).updateSpeakers(data);
        }

        private void BroadcastSelectedEventChange(string sessionName, string data)
        {            
            Clients.Group(sessionName).eventChanged(data);
        }

        private void BroadcastSelectedSessionChange(string sessionName, string data)
        {
            Clients.Group(sessionName).sessionChanged(data);            
        }
        
        #endregion

    }    
}