# SharePoint chat room with SignalR and Azure #

### Summary ###
This solution shows a method for creating a chat room within a SharePoint web part.  The solution uses details from the user's 
profile to identify the sender of the message and uses SignalR to push messages out to each instance of the chat room.


### Full walkthrough ###

A full walkthrough of the development process (including deployment to Azure) can be found at - 
[http://blog.jonathanhuss.com/building-a-sharepoint-online-chat-room-with-signalr-and-azure](http://blog.jonathanhuss.com/building-a-sharepoint-online-chat-room-with-signalr-and-azure)

### Applies to ###
- Office 365 / SharePoint Online

### Prerequisites ###
N/A

### Solution ###
Solution | Author(s)
---------|----------
BusinessApps.ChatRoom | Jonathan Huss (**Microsoft**)

### Version history ###

Version  | Date | Comments
---------| -----| --------
1.0 | April 3rd, 2015 | Initial Release

### Disclaimer ###
**THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.**

----------

### User Interface ###

The user interface is fairly simply, but could be extended heavily.  The user inferface contains a message window, a message 
input box, and send button.  As messages are received from SignalR, they're added to the message window.  Timestamps are 
also added to the messages that are localized to the client's timezone via JavaScript.  Deployed as a web part, the rendered 
user interface looks like this:

![The web page which shows the chat interface](http://blog.jonathanhuss.com/wp-content/uploads/2015/04/image_thumb29.png)

### SignalR Bits ###
The SignalR components in this solution consist of a server and a client.  The server is contained in the ChatHub.cs C# class
and defines the operations that can occur between server and client.  In this case, we've defined three operations:  joinRoom,
pushMessage, and leaveRoom.  When messages are sent from the chat room, they're passed to the server using standard AJAX 
calls.  The server then pushes those messages out to the other members of the chat room using SignalR.

The server side SignalR code looks like this:

```
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
```

The client side code is in JavaScript and contains definitions that match the three operations defined in the ChatHub.  The
relevant client side code looks like this:

```
function StartHub() {
    var chatHub = $.connection.chatHub;

    chatHub.client.pushMessage = function (userName, photoUrl, timeStamp, message) {
        ReceiveMessage(userName, photoUrl, timeStamp, message);
    };

    chatHub.client.joinRoom = function (userName) {
        ReceiveJoinRoom(userName);
    };

    chatHub.client.leaveRoom = function (userName) {
        ReceiveLeaveRoom(userName);
    };

    $.connection.hub.start();
}

function SendMessage() {
    var textBox = $("#message-box textarea");

    if (textBox.val() != "") {
        $.ajax({
            type: "GET",
            url: "/Home/SendMessage" + location.search + "&message=" + textBox.val(),
            cache: false
        });

        textBox.val("");
    }

    textBox.focus();
}

function ReceiveMessage(userName, photoUrl, timeStamp, message) {
    var localTimeStamp = new Date(timeStamp + " UTC");

    AddMessage(
        '<div class=\'message\'>' +
            '<img class=\'message-sender-image\' src=\'' + photoUrl + '\' alt=\'Photo of ' + userName + '\'/>' +
            '<div class=\'message-right\'>' +
                '<div class=\'message-sender\'>' + userName + '</div>' +
                '<div class=\'message-timestamp\'>' + localTimeStamp.toLocaleTimeString() + '</div>' +
                '<div class=\'message-content\'>' + message + '</div>' +
            '</div>' +
        '</div>'
        );
}

function ReceiveJoinRoom(userName) {
    AddMessage(
            '<div class=\'system-message\'>' +
                userName + " has joined the room." +
            '</div>'
            );
}

function ReceiveLeaveRoom(userName) {
    AddMessage(
            '<div class=\'system-message\'>' +
                userName + " has left the room." +
            '</div>'
            );
}
```

### Server Side Operations ###

When a user joins the chat room, the Controller connects to SharePoint to retrieve the user's DisplayName and Email properties.
It then creates a URL to the user's profile image based on the Email property.  The DisplayName and PhotoUrl are then
cached for retrieval later.  When the user sends a message, that information is retrieved from the cache, and, along with 
the message itself, is pushed out to the other users of the chat room.  The relevant Controller code looks like this:

```
[SharePointContextFilter]
public void SendMessage(string message)
{
    if (Session["UserAccountName"] == null || HttpRuntime.Cache[Session["UserAccountName"].ToString()] == null)
        GetUserDetails();

    UserInfo userInfo = (UserInfo)HttpRuntime.Cache[Session["UserAccountName"].ToString()];

    _chatHub.PushMessage(userInfo.DisplayName, userInfo.PhotoUrl, DateTime.UtcNow.ToString(), message);
}

private void GetUserDetails()
{
    var spContext = SharePointContextProvider.Current.GetSharePointContext(HttpContext);

    using (var clientContext = spContext.CreateUserClientContextForSPHost())
    {
        if (clientContext != null)
        {
            PeopleManager peopleManager = new PeopleManager(clientContext);
            PersonProperties properties = peopleManager.GetMyProperties();
            clientContext.Load(properties);
            clientContext.ExecuteQuery();

            UserInfo userInfo = new UserInfo()
            {
                DisplayName = properties.DisplayName,
                PhotoUrl = spContext.SPHostUrl + "/_layouts/userphoto.aspx?accountname=" + properties.Email,
                LastPing = DateTime.Now
            };


            HttpRuntime.Cache.Add(properties.AccountName, userInfo, null, Cache.NoAbsoluteExpiration, new TimeSpan(0, 3, 0), CacheItemPriority.Normal, new CacheItemRemovedCallback(CacheRemovalCallback));

            Session["UserAccountName"] = properties.AccountName;
        }
    }
}
```

### Pinging ###

One of the challenges is detecting when a user leaves the chat room.  If the user simply clicks on a link to another page,
that is easy to catch via JavaScript.  However, if they close the browser entirely, that is undetectable.  The solution 
is to have the client side code ping the server periodically, letting the server know the client is still connected - 
much like the idea of a [heartbeat](http://en.wikipedia.org/wiki/Heartbeat_%28computing%29).  The cache is configured 
for a 3 minute sliding expiration.  When the ping is received, the sliding expiration window in the cache is extended.  
Depending on the exact sliding window, if 8 or 9 consecutive pings are missed, the server expires the cache entry and assumes
that the user has left the chat room, resulting in a leaveRoom SignalR message being sent.


<img src="https://telemetry.sharepointpnp.com/pnp/samples/BusinessApps.ChatRoom" />