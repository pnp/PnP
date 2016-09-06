# Connected Add-In Parts with SignalR #

### Summary ###
This sample shows how to inject custom CSS to the host web using SP Add-In. For more information on this sample, please see Vesa Juvonen's thorough blog post on the concept: [http://blogs.msdn.com/b/vesku/archive/2014/05/14/connected-sharepoint-app-parts-with-signalr.aspx](http://blogs.msdn.com/b/vesku/archive/2014/05/14/connected-sharepoint-app-parts-with-signalr.aspx "Connected SharePoint add-in parts with SignalR")

### Video Walkthrough ##
A comprehensive video of the solution can be found at [http://www.youtube.com/watch?v=_Duwtgn9rhc](http://www.youtube.com/watch?v=_Duwtgn9rhc "http://www.youtube.com/watch?v=_Duwtgn9rhc")

### Applies to ###
-  Office 365 Multi Tenant (MT)
-  Office 365 Dedicated (D)
-  SharePoint 2013 on-premises

### Solution ###
Solution | Author(s)
---------|----------
Core.ConnectedAppParts | Vesa Juvonen (Microsoft)

### Version history ###
Version  | Date | Comments
---------| -----| --------
1.0  | May 14th 2014 | Initial release

### Disclaimer ###
**THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.**


----------

# Connected Add-In Parts using Server Side Connections #
One of the classic questions related on the SharePoint add-in parts is their capability to connect between each other. This has been classic scenario with web parts since the dawn of the SharePoint, so question is pretty understandable. Since add-in parts are essentially IFrames in steroids, they don't  natively support similar connectivity models as the classic web parts, but we can solve the requirement using alternative approach.

We have basically two different options for the connectivity with the add-in parts:
- Client side approach
- Server-side approach

This sample leverages a server-side approach using ASP.NET SignalR. SignalR allows add-in parts to communicate to each through active socket connections with the server. Ultimately the web server serves the purpose of a communication "proxy" between add-in parts as depicted in the diagram below

![Connected Add-In Parts with SignalR](http://i.imgur.com/ueQjqPS.png) 

<img src="https://telemetry.sharepointpnp.com/pnp/samples/Core.ConnectedAppParts" />