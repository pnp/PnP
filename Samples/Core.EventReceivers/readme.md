# USING APPINSTALLED EVENTS TO ATTACH EVENTS IN THE HOST WEB #

### Summary ###
This scenario shows how an add-in can use the add-in Installed event to perform additional work in the host web, such as attaching event receivers to lists in the host web.

![](http://i.imgur.com/ZvzkKJD.png)
 
For more information on this scenario, see the blog post: [http://blogs.msdn.com/b/kaevans/archive/2014/02/26/attaching-remote-event-receivers-to-lists-in-the-host-web.aspx](http://blogs.msdn.com/b/kaevans/archive/2014/02/26/attaching-remote-event-receivers-to-lists-in-the-host-web.aspx). 

### Walkthrough Video ###
Visit the video on Channel 9 - [http://channel9.msdn.com/Blogs/Office-365-Dev/Using-appinstalled-events-to-attach-remote-event-receivers-to-SharePoint-Host-Webs-Office-365-Develo](http://channel9.msdn.com/Blogs/Office-365-Dev/Using-appinstalled-events-to-attach-remote-event-receivers-to-SharePoint-Host-Webs-Office-365-Develo)

![](http://i.imgur.com/ASdp83p.png)


### Applies to ###
-  Office 365 Multi Tenant (MT)
-  Office 365 Dedicated (D)
-  SharePoint 2013 on-premises

*Sample has been tested and configured for Office 365 MT, but model works as such with other platforms as well. *

### Prerequisites ###


### Solution ###
Solution | Author(s)
---------| ----------
Core.EventReceivers | Kirk Evans (Microsoft), Vesa Juvonen (Microsoft), Torsten Schuster (SuTor)

### Version history ###
Version  | Date | Comments
---------| -----| --------
3.0  | December 19th 2015 | Added example code to handle remote eventreceivers on refiring itself
2.0  | August 24th 2014 | Updated to be on-demain example with additional notes with AppInstalled event.
1.0  | April 26th 2014 | Initial release

### Disclaimer ###
**THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.**


----------

# Introduction #
The solution is a provider-hosted add-in (remote event receivers are not supported with SharePoint-hosted apps).

This sample shows adding the remote event receiver to the host web by clicking button in the provider hosted add-in, but this could be done also automatically either when the site or site collection is provisioned; or when the add-in is installed by using add-in Installed and Handle add-in Uninstalling properties of the Visual Studio solution.

![](http://i.imgur.com/PbnYf3t.png)

Visual Studio will then add a new WCF service to your web application project named AppEventReceiver.cs.

```C#
private const string RECEIVER_NAME = "ItemAddedEvent";
private const string LIST_TITLE = "Remote Event Receiver Jobs";

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
        case SPRemoteEventType.ItemAdded:
            HandleItemAdded(properties);
            break;
    }


    return result;
}
```

Note:
*  The **AppUninstalling** event only fires when a user completely removes the add-in: the add-in needs to be deleted from the site recycle bins in an end-user scenario. In a development scenario the add-in needs to be removed from the “Apps in testing” library.

Our code runs the centrally located code to see if a list named “Remote Event Receiver Jobs” exists in the host web and, if it does not, it creates it.  The code then checks to see if that list has a remote event receiver attached to it for the ItemAdded event and, if it does not, it attaches one.

```C#
EventReceiverDefinitionCreationInformation receiver =
    new EventReceiverDefinitionCreationInformation();
receiver.EventType = EventReceiverType.ItemAdded;

//Get WCF URL where this message was handled
OperationContext op = OperationContext.Current;
Message msg = op.RequestContext.RequestMessage;

receiver.ReceiverUrl = msg.Headers.To.ToString();

receiver.ReceiverName = RECEIVER_NAME;
receiver.Synchronization = EventReceiverSynchronization.Synchronous;
myList.EventReceivers.Add(receiver);

clientContext.ExecuteQuery();
```

Once this code executes and a call to clientContext.ExecuteQuery() is made, the list will now have a remote event receiver attached to it.

*The address for the remote event receiver currently uses the same address that hosts the add-in Installed remote event receiver by using the host address.  This is done to facilitate easy debugging using Windows Azure Service Bus.  The same code works while debugging locally as well as when the add-in is deployed to production.*


# EXECUTING THE SAMPLE ADD-IN #
To execute the sample, first change the SharePoint URL to a valid SharePoint environment configured for apps.  Next, go to the Contoso.EventReceivers add-in project and choose Properties to reveal the SharePoint tab.  Scroll down to ensure a Windows Azure Service Bus connection string is configured. This is needed for proper debugging of the add-in.

![](http://i.imgur.com/AtcfB3T.png)

For more information on Remote Event Receiver debugging, see the section “Debugging Remote Events” at [http://msdn.microsoft.com/en-us/library/office/jj220047.aspx#DebugRER](http://msdn.microsoft.com/en-us/library/office/jj220047.aspx#DebugRER).  

Once the Windows Azure Service Bus connection string is configured, simply press F5 in Visual Studio.  The web project will run in IIS Express, while the add-in is deployed to SharePoint.  Add a break-point in the ProcessEvent method to debug the add-in. When the add-in is run, you must click Trust It in order to grant the necessary permissions.

![](http://i.imgur.com/1MfAFV9.png)

The add-in begins to install, and the break-point will be hit.

![](http://i.imgur.com/YQHRadM.png)

Continue debugging, and the add-in will finally render the full-page experience.

![](http://i.imgur.com/W8LUyMI.png)

Click the “Back to Site” link in the top left to go back to the SharePoint site.  Notice that the add-in now shows in the Recent navigation heading, as does the newly added list “Remote Event Receiver Jobs”.

![](http://i.imgur.com/S9JOZNe.png)

Open the Remote Event Receiver Jobs list and add a new item.

![](http://i.imgur.com/mm00KKy.png)

Clicking Save will cause the remote event receiver endpoint to be called.  The sample code in this solution simply appends text to the Description field.

![](http://i.imgur.com/2Las9nf.png)

# Handling add-in Uninstalling and Debugging #

If you attempt to uninstall the add-in while debugging, you will notice that you receive a permission denied error.  This occurs only while side-loading the add-in, which is what Visual Studio does when you deploy using F5.  To see the AppUninstalling event work, you will need to install the add-in via an add-in Catalog or the Marketplace.

For more information, see the blog post at [http://blogs.msdn.com/b/kaevans/archive/2014/02/26/attaching-remote-event-receivers-to-lists-in-the-host-web.aspx](http://blogs.msdn.com/b/kaevans/archive/2014/02/26/attaching-remote-event-receivers-to-lists-in-the-host-web.aspx). 

# Required permissions #
Attaching a remote event receiver to an object in the host web only requires Manage permission for that object.  If we were simply attaching an event to an existing list, then the add-in would only require Manage permission for the list.  However, this sample also adds a list to the host web and activates a feature in the host web, both of which require Manage permissions for the Web.