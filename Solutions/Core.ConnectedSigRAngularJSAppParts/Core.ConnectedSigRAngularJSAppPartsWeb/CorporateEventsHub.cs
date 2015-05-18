using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Core.ConnSigRAngJSApps.Models;

namespace Core.ConnSigRAngJSApps
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

        
        public void Send(string name, string message)
        {
            // Call the broadcastMessage method to update clients.
            Clients.All.broadcastMessage(name, message);
        }
                    
        public void SelectedEventChanged(string data)
        {
            _corporateEvents.SelectedEventChanged(data);
        }

        public void SelectedSessionChanged(string data)
        {
            _corporateEvents.SelectedSessionChanged(data);
        }

        public void EventCancellation(string data)
        {
            _corporateEvents.EventCancellation(data);
        }

        public void EventAddition(string data)
        {
            _corporateEvents.EventAddition(data);
        }

        public void SessionCancellation(string data)
        {
            _corporateEvents.SessionCancellation(data);
        }

        public void SessionAddition(string data)
        {
            _corporateEvents.SessionAddition(data);
        }        

        public void EventStatus(string data)
        {
            _corporateEvents.EventStatus(data);
        }

        public void SessionStatus(string data)
        {
            _corporateEvents.SessionStatus(data);
        }

        public void UpdateSpeakers(string data)
        {
            _corporateEvents.UpdateSpeakers(data);
        }

        public void Refresh()
        {
            _corporateEvents.Refresh();
        }

    }
}