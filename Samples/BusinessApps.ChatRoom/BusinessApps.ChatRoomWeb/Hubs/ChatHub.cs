using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;

namespace BusinessApps.ChatRoomWeb.Hubs
{
    public class ChatHub : Hub
    {
        public void PushMessage(string userName, string photoUrl, string timestamp, string message)
        {
            IHubContext context = GlobalHost.ConnectionManager.GetHubContext<ChatHub>();
            context.Clients.All.pushMessage(userName, photoUrl, timestamp, message);
        }

        public void JoinRoom(string userName)
        {
            IHubContext context = GlobalHost.ConnectionManager.GetHubContext<ChatHub>();
            context.Clients.All.joinRoom(userName);
        }

        public void LeaveRoom(string userName)
        {
            IHubContext context = GlobalHost.ConnectionManager.GetHubContext<ChatHub>();
            context.Clients.All.leaveRoom(userName);
        }
    }
}