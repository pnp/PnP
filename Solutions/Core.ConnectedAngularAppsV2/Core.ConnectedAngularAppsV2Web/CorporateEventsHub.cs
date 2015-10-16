using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Core.ConnectedAngularAppsV2Web.Models;
using System.Threading.Tasks;

namespace Core.ConnectedAngularAppsV2Web
{
    [HubName("corporateEventsHub")]
    public class CorporateEventsHub : Hub
    {
        private readonly CorporateEvents _corporateEvents;

        public CorporateEventsHub() : this(CorporateEvents.Instance) { }

        public CorporateEventsHub(CorporateEvents corpEvents)
        {
            _corporateEvents = corpEvents;
        }

        public Task JoinSession(string connId, string sessionName)
        {
            
            //Context.ConnectionId
            return Groups.Add(connId, sessionName);
        }

        public Task LeaveSession(string connId, string sessionName)
        {
            // Context.ConnectionId
            return Groups.Remove(connId, sessionName);
        }


        public void Send(string name, string message)
        {
            // Call the broadcastMessage method to update clients.
            Clients.All.broadcastMessage(name, message);
        }

        public void SelectedEventChanged(string sessionName, string data)
        {
            _corporateEvents.SelectedEventChanged(sessionName, data);
        }

        public void SelectedSessionChanged(string sessionName, string data)
        {
            _corporateEvents.SelectedSessionChanged(sessionName, data);
        }

        public void EventCancellation(string sessionName, string data)
        {
            _corporateEvents.EventCancellation(sessionName, data);
        }

        public void EventAddition(string sessionName, string data)
        {
            _corporateEvents.EventAddition(sessionName, data);
        }

        public void SessionCancellation(string sessionName, string data)
        {
            _corporateEvents.SessionCancellation(sessionName, data);
        }

        public void SessionAddition(string sessionName, string data)
        {
            _corporateEvents.SessionAddition(sessionName, data);
        }

        public void EventStatus(string sessionName, string data)
        {
            _corporateEvents.EventStatus(sessionName, data);
        }

        public void SessionStatus(string sessionName, string data)
        {
            _corporateEvents.SessionStatus(sessionName, data);
        }

        public void UpdateSpeakers(string sessionName, string data)
        {
            _corporateEvents.UpdateSpeakers(sessionName, data);
        }

        public void Refresh()
        {
            _corporateEvents.Refresh();
        }

    }
}