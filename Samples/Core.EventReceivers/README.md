# USING APPINSTALLED EVENTS TO ATTACH EVENTS IN THE HOST WEB #

### Summary ###
This scenario shows how an add-in can use the add-in Installed event to perform additional work in the host web, such as attaching event receivers to lists in the host web.

![A diagram of an add-in installed event with the messages, App Installed, Attach ItemAdded, and ItemAdded.](http://i.imgur.com/ZvzkKJD.png)
 
For more information on this scenario, see the blog post: [http://blogs.msdn.com/b/kaevans/archive/2014/02/26/attaching-remote-event-receivers-to-lists-in-the-host-web.aspx](http://blogs.msdn.com/b/kaevans/archive/2014/02/26/attaching-remote-event-receivers-to-lists-in-the-host-web.aspx). 

### Walkthrough Video ###
Visit the video on Channel 9 - [http://channel9.msdn.com/Blogs/Office-365-Dev/Using-appinstalled-events-to-attach-remote-event-receivers-to-SharePoint-Host-Webs-Office-365-Develo](http://channel9.msdn.com/Blogs/Office-365-Dev/Using-appinstalled-events-to-attach-remote-event-receivers-to-SharePoint-Host-Webs-Office-365-Develo)

![An image of the video.](http://i.imgur.com/ASdp83p.png)


### Applies to ###
-  Office 365 Multi Tenant (MT)
-  Office 365 Dedicated (D)
-  SharePoint 2013 on-premises

*Sample has been tested and configured for Office 365 MT, but model works as such with other platforms as well. *
### Prerequisites ###

### Solution ###
Solution | Author(s)
---------| ----------
Core.EventReceivers | Kirk Evans (Microsoft), Vesa Juvonen (Microsoft)

### Version history ###
Version  | Date | Comments
---------| -----| --------
3.0  | January 30th 2016 | Added documentation showing a sample solution against recursively calling remote event receivers by caching the CorreleationID (Torsten Schuster)
2.0  | August 24th 2014 | Updated to be on-demain example with additional notes with AppInstalled event.
1.0  | April 26th 2014 | Initial release

### Disclaimer ###
**THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.**


----------

# Introduction #
The solution is a provider-hosted add-in (remote event receivers are not supported with SharePoint-hosted apps).

This sample shows adding the remote event receiver to the host web by clicking button in the provider hosted add-in, but this could be done also automatically either when the site or site collection is provisioned; or when the add-in is installed by using add-in Installed and Handle add-in Uninstalling properties of the Visual Studio solution.

![The project properties, showing Active Deployment C as Deploy app for Share, Handle App Installed as true, and Handle app uninstalling as true.](http://i.imgur.com/PbnYf3t.png)

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

![A screenshot which shows a checked check box next to Enable debugging via Windows Azure Service Bus, a Windows Azure Service Bus connection string, and a checked check box next to Notify me if Windows Azure Service Bus debugging is not configured.](http://i.imgur.com/AtcfB3T.png)

For more information on Remote Event Receiver debugging, see the section “Debugging Remote Events” at [http://msdn.microsoft.com/en-us/library/office/jj220047.aspx#DebugRER](http://msdn.microsoft.com/en-us/library/office/jj220047.aspx#DebugRER).  

Once the Windows Azure Service Bus connection string is configured, simply press F5 in Visual Studio.  The web project will run in IIS Express, while the add-in is deployed to SharePoint.  Add a break-point in the ProcessEvent method to debug the add-in. When the add-in is run, you must click Trust It in order to grant the necessary permissions.

![A dialog box with the title Do you trust Contoso.EventReceivers? and a highlighted button labeled Trust it.](http://i.imgur.com/1MfAFV9.png)

The add-in begins to install, and the break-point will be hit.

![A screenshot of the breakpoint being hit.](http://i.imgur.com/YQHRadM.png)

Continue debugging, and the add-in will finally render the full-page experience.

![A screenshot of the full page entitled, Remote Event Receiver. A link, Back to Site, is in the upper left corner.](http://i.imgur.com/W8LUyMI.png)

Click the “Back to Site” link in the top left to go back to the SharePoint site.  Notice that the add-in now shows in the Recent navigation heading, as does the newly added list “Remote Event Receiver Jobs”.

![A screenshot of the navigation heading, Recent, with a list of two items, Contoso.EventReceivers, and Remote Event Receiver Jobs.](http://i.imgur.com/S9JOZNe.png)

Open the Remote Event Receiver Jobs list and add a new item.

![A dialog which contains the field Title with the text This list item is in the host web, a field Description with the text Saving this new item will cause an event to fire, and a field AssignedTo with the text Kirk Evans.](http://i.imgur.com/mm00KKy.png)

Clicking Save will cause the remote event receiver endpoint to be called.  The sample code in this solution simply appends text to the Description field.

![A dialog which contains the field Title with the text This list item is in the host web, a field Description with the text Saving this new item will cause an event to fire Updated by RER 4:22:39 PM, and a field AssignedTo with the text Kirk Evans. Below this is text which reads Created at 2/22/2014 2:22 PM by MOD Administrator. Last modified at 2/22/2014 2:22 PM by Contoso.EventRecievers on behalf of MOD Administrator, and a button labeled Close.](http://i.imgur.com/2Las9nf.png)

# Handling add-in Uninstalling and Debugging #

If you attempt to uninstall the add-in while debugging, you will notice that you receive a permission denied error.  This occurs only while side-loading the add-in, which is what Visual Studio does when you deploy using F5.  To see the AppUninstalling event work, you will need to install the add-in via an add-in Catalog or the Marketplace.

For more information, see the blog post at [http://blogs.msdn.com/b/kaevans/archive/2014/02/26/attaching-remote-event-receivers-to-lists-in-the-host-web.aspx](http://blogs.msdn.com/b/kaevans/archive/2014/02/26/attaching-remote-event-receivers-to-lists-in-the-host-web.aspx). 

# Required permissions #
Attaching a remote event receiver to an object in the host web only requires Manage permission for that object.  If we were simply attaching an event to an existing list, then the add-in would only require Manage permission for the list.  However, this sample also adds a list to the host web and activates a feature in the host web, both of which require Manage permissions for the Web.

# Dealing with recursively called ItemUpdated/ItemUpdating event receivers
When you update an item in an ItemUpdated/ItemUpdating remote event receiver then this will trigger a new execution of that same ItemUpdated/ItemUpdating event receiver...and again...and again...

Using SSOM you would disable the event receiver from firing using `this.EventFiringEnabled = false;` but that's not an option for remote event receivers. A good alternative is to add a condition on the actual update logic: only perform an update if there's a change. [Stina Qvarnström](http://tech.bool.se/how-to-stop-the-itemupdated-event-from-refiring-itself-in-an-remote-event-receiver/) did write an nice blog describing this approach.

## Sample implementation to prevent recursive event receiver triggering (by Torsten Schuster)
A possible implementation for providing recursive event receiver calls starts with storing the SharePoint CorrelationID before a RER is triggered into a persistent store. Below sample is using a static memory cache object. In a production scenario, the RER is usually implemented as load-balanced Web App, so we save the CorrelationID using a mechanism that can work across multiple servers like the Microsoft Azure Redis cache service. 

The code to prevent recursively calling the remote event receiver uses a EventFiringEnabled  method. You can do this by expanding this sample like this: to the existing ItemAdded RER an ItemAdding and ItemUpdating RER has been added. ItemAdding checks for example that the Description field is not set by the user and prevents on the ItemUpdating changes by the user after ItemAdded has set this Description. 

The EventFiringEnabled uses the CorrelationID as a string typed key and a 1 for ~ing and 2 for ~ed RER to disable the recursive code flow. If we need within a ~ing RER to block the following connected ~ed, we will be able to call the EventFiringEnabled with a 2 for example.

New version of the **AppEventReceiver.svc.cs** class:
```c#
using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.EventReceivers;
using System;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace Contoso.Core.EventReceiversWeb.Services
{
    using System.Runtime.Caching;
    using Contoso.Core.EventReceiversWeb.Cache;

    public class AppEventReceiver : IRemoteEventService
    {
        //the cache needs an unique name
        private static MemoryCache _cacheObject = new MemoryCache("A5390789156142B49AA51DBC19FCD9BC3831BDBC0B594D11A67DEC767315640B");

        /// <summary>
        
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


                case SPRemoteEventType.ItemAdding:
                    HandleItemAdding(properties, result);
                    break;
                case SPRemoteEventType.ItemUpdating:
                    HandleItemUpdating(properties, result);
                    break;

                case SPRemoteEventType.ItemAdded:
                    HandleItemAdded(properties);
                    break;
            }


            return result;
        }

        public void ProcessOneWayEvent(SPRemoteEventProperties properties)
        {
        }


        /// <summary>
        /// Handles when an app is installed.  Activates a feature in the
        /// host web.  The feature is not required.  
        /// Next, if the Jobs list is
        /// not present, creates it.  Finally it attaches a remote event
        /// receiver to the list.  
        /// </summary>
        /// <param name="properties"></param>
        private void HandleAppInstalled(SPRemoteEventProperties properties)
        {
            using (ClientContext clientContext =
                TokenHelper.CreateAppEventClientContext(properties, false))
            {
                if (clientContext != null)
                {
                    new RemoteEventReceiverManager().AssociateRemoteEventsToHostWeb(clientContext);
                }
            }
        }

        /// <summary>
        /// Removes the remote event receiver from the list and 
        /// adds a new item to the list.
        /// </summary>
        /// <param name="properties"></param>
        private void HandleAppUninstalling(SPRemoteEventProperties properties)
        {
            using (ClientContext clientContext =
                TokenHelper.CreateAppEventClientContext(properties, false))
            {
                if (clientContext != null)
                {
                    new RemoteEventReceiverManager().RemoveEventReceiversFromHostWeb(clientContext);
                }
            }
        }

        /// <summary>
        /// Handles the ItemAdding event by check the Description
        /// field of the item.
        /// </summary>
        /// <param name="properties"></param>
        private void HandleItemAdding(SPRemoteEventProperties properties, SPRemoteEventResult result)
        {
            using (ClientContext clientContext = TokenHelper.CreateRemoteEventReceiverClientContext(properties))
            {
                if (clientContext != null && EventFiringEnabled(clientContext, properties, "1"))
                {
                    new RemoteEventReceiverManager().ItemAddingToListEventHandler(clientContext, properties, result);
                }
            }
        }

        /// <summary>
        /// Handles the ItemUpdating event by check the Description
        /// field of the item.
        /// </summary>
        /// <param name="properties"></param>
        private void HandleItemUpdating(SPRemoteEventProperties properties, SPRemoteEventResult result)
        {
            using (ClientContext clientContext = TokenHelper.CreateRemoteEventReceiverClientContext(properties))
            {
                if (clientContext != null && EventFiringEnabled(clientContext, properties, "1"))
                {
                    new RemoteEventReceiverManager().ItemUpdatingToListEventHandler(clientContext, properties, result);
                }
            }
        }

        /// <summary>
        /// Handles the ItemAdded event by modifying the Description
        /// field of the item.
        /// </summary>
        /// <param name="properties"></param>
        private void HandleItemAdded(SPRemoteEventProperties properties)
        {
            using (ClientContext clientContext =
                TokenHelper.CreateRemoteEventReceiverClientContext(properties))
            {
                if (clientContext != null && EventFiringEnabled(clientContext, properties, "2"))
                {
                    new RemoteEventReceiverManager().ItemAddedToListEventHandler(clientContext, properties.ItemEventProperties.ListId, properties.ItemEventProperties.ListItemId);
                }
            }
        }

        /// <summary>
        /// Used to chek if the event should processed or not. we use here a local MemoryCache object 
        /// or if the RER is loadbalanced a Redis Server on AZURE or onPrem
        /// </summary>
        /// <param name="ctx">the current context as ClientContext</param>
        /// <param name="properties">the event properies as SPRemoteEventProperties</param>
        /// <param name="eventType">identifier for event receiver type (~ing=1, ~ed=2) as String</param>
        /// <returns></returns>
        private static bool EventFiringEnabled(ClientContext ctx, SPRemoteEventProperties properties, string eventType)
        {
            try
            { 
                // set the correlation id for the next roundtrip to context
                ctx.TraceCorrelationId = properties.CorrelationId.ToString();

                var key = string.Concat(properties.CorrelationId.ToString("N"), eventType);
                var ts = TimeSpan.FromSeconds(20);

                if (SettingsHelper.UseAzureRedisCache == false)
                {
                    return _cacheObject.Add(key, properties.CorrelationId, new CacheItemPolicy
                    {
                        Priority = CacheItemPriority.Default,
                        SlidingExpiration = ts
                    });
                }
                else
                {
                    return CacheConnectionHelper.Connection.GetDatabase().Add(key, properties.CorrelationId, ts);
                }
            }
            catch (Exception oops)
            {
                System.Diagnostics.Trace.WriteLine(oops.Message);
            }

            return false;
        }


    }
}
```

New version of the **RemoteEventReceiverManager** class:

```c#
using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;
using System.ServiceModel.Channels;
using Microsoft.SharePoint.Client.EventReceivers;

namespace Contoso.Core.EventReceiversWeb
{
    public class RemoteEventReceiverManager
    {
        private const string RECEIVER_NAME_ADDED = "ItemAddedEvent";
        private const string LIST_TITLE = "Remote Event Receiver Jobs";

        public void AssociateRemoteEventsToHostWeb(ClientContext clientContext)
        {
            //Add Push Notification Feature to HostWeb
            //Not required here, just a demonstration that you
            //can activate features.
            clientContext.Web.Features.Add(
                     new Guid("41e1d4bf-b1a2-47f7-ab80-d5d6cbba3092"),
                     true, FeatureDefinitionScope.None);


            //Get the Title and EventReceivers lists
            clientContext.Load(clientContext.Web.Lists,
                lists => lists.Include(
                    list => list.Title,
                    list => list.EventReceivers).Where
                        (list => list.Title == LIST_TITLE));

            clientContext.ExecuteQuery();

            List jobsList = clientContext.Web.Lists.FirstOrDefault();

#if (DEBUG)
            // In debug mode we will delete the existing list, so we prevent our system from orphaned event receicers.
            // RemoveEventReceiversFromHostWeb is sometimes not called in debug mode and/or the app id has changed. 
            // On RER registration SharePoint adds the app id to the event registration information, and you are only able 
            // to remove the event with the same app where it was registered. Also note that you would need to completely 
            // uninstall an app before SharePoint will trigger the appuninstalled event. From the documentation: 
            // The **AppUninstalling** event only fires when a user completely removes the add-in: the add-in needs to be deleted 
            // from the site recycle bins in an end-user scenario. In a development scenario the add-in needs to be removed from 
            // the “Apps in testing” library.

            if (null != jobsList)
            {
                jobsList.DeleteObject();
                clientContext.ExecuteQuery();
                jobsList = null;
            }
#endif

            bool rerExists = false;
            if (null == jobsList)
            {
                //List does not exist, create it
                jobsList = CreateJobsList(clientContext);

            }
            else
            {
                foreach (var rer in jobsList.EventReceivers)
                {
                    if (rer.ReceiverName == RECEIVER_NAME_ADDED)
                    {
                        rerExists = true;
                        System.Diagnostics.Trace.WriteLine("Found existing ItemAdded receiver at "
                            + rer.ReceiverUrl);
                    }
                }
            }

            if (!rerExists)
            {
                EventReceiverDefinitionCreationInformation receiver =
                    new EventReceiverDefinitionCreationInformation();
                receiver.EventType = EventReceiverType.ItemAdded;
                
                //Get WCF URL where this message was handled
                OperationContext op = OperationContext.Current;
                Message msg = op.RequestContext.RequestMessage;
                receiver.ReceiverUrl = msg.Headers.To.ToString();

                receiver.ReceiverName = EventReceiverType.ItemAdded.ToString();
                receiver.Synchronization = EventReceiverSynchronization.Synchronous;

                //Add the new event receiver to a list in the host web
                jobsList.EventReceivers.Add(receiver);
                clientContext.ExecuteQuery();

                System.Diagnostics.Trace.WriteLine("Added ItemAdded receiver at " + receiver.ReceiverUrl);

                receiver =
                    new EventReceiverDefinitionCreationInformation();
                receiver.EventType = EventReceiverType.ItemAdding;

                receiver.ReceiverUrl = msg.Headers.To.ToString();
                receiver.ReceiverName = EventReceiverType.ItemAdding.ToString();
                receiver.Synchronization = EventReceiverSynchronization.Synchronous;

                //Add the new event receiver to a list in the host web
                jobsList.EventReceivers.Add(receiver);
                clientContext.ExecuteQuery();

                System.Diagnostics.Trace.WriteLine("Added ItemAdding receiver at " + receiver.ReceiverUrl);

                receiver =
                    new EventReceiverDefinitionCreationInformation();
                receiver.EventType = EventReceiverType.ItemUpdating;

                receiver.ReceiverUrl = msg.Headers.To.ToString();
                receiver.ReceiverName = EventReceiverType.ItemUpdating.ToString();
                receiver.Synchronization = EventReceiverSynchronization.Synchronous;

                //Add the new event receiver to a list in the host web
                jobsList.EventReceivers.Add(receiver);
                clientContext.ExecuteQuery();

                System.Diagnostics.Trace.WriteLine("Added ItemUpdating receiver at " + receiver.ReceiverUrl);
            }
        }

        public void RemoveEventReceiversFromHostWeb(ClientContext clientContext)
        {
            List myList = clientContext.Web.Lists.GetByTitle(LIST_TITLE);
            clientContext.Load(myList, p => p.EventReceivers);
            clientContext.ExecuteQuery();

            var rer = myList.EventReceivers.Where(
                e => e.ReceiverName == RECEIVER_NAME_ADDED).FirstOrDefault();

            try
            {
                System.Diagnostics.Trace.WriteLine("Removing receiver at "
                        + rer.ReceiverUrl);

                var rerList = myList.EventReceivers.Where(
                e => e.ReceiverUrl == rer.ReceiverUrl).ToList<EventReceiverDefinition>();

                foreach (var rerFromUrl in rerList)
                {
                    //This will fail when deploying via F5, but works
                    //when deployed to production
                    rerFromUrl.DeleteObject();
                }
                clientContext.ExecuteQuery();
            }
            catch (Exception oops)
            {
                System.Diagnostics.Trace.WriteLine(oops.Message);
            }

            //Now the RER is removed, add a new item to the list
            ListItemCreationInformation itemCreateInfo = new ListItemCreationInformation();
            ListItem newItem = myList.AddItem(itemCreateInfo);
            newItem["Title"] = "App deleted";
            newItem["Description"] = "Deleted on " + System.DateTime.Now.ToLongTimeString();
            newItem.Update();

            clientContext.ExecuteQuery();

        }

        public void ItemAddingToListEventHandler(ClientContext clientContext, 
            SPRemoteEventProperties properties, SPRemoteEventResult result)
        {
            try
            {
                // only for demo we check here the Description
                if (properties.ItemEventProperties.AfterProperties["Description"] != null &&
                    !string.IsNullOrEmpty(properties.ItemEventProperties.AfterProperties["Description"].ToString()))
                {
                    throw new Exception("Description should be empty!");
                }
                else
                {
                    result.Status = SPRemoteEventServiceStatus.Continue;
                }
            }
            catch (Exception oops)
            {
                result.Status = SPRemoteEventServiceStatus.CancelWithError;
                result.ErrorMessage = oops.Message;

                System.Diagnostics.Trace.WriteLine(oops.Message);
            }
        }

        public void ItemUpdatingToListEventHandler(ClientContext clientContext,
            SPRemoteEventProperties properties, SPRemoteEventResult result)
        {
            try
            {
                // only for demo we check here the Description
                if (properties.ItemEventProperties.BeforeProperties["Description"] !=
                    properties.ItemEventProperties.AfterProperties["Description"])
                {
                    throw new Exception("Description change is not allowed!");
                }
                else
                {
                    result.Status = SPRemoteEventServiceStatus.Continue;
                }
            }
            catch (Exception oops)
            {
                result.Status = SPRemoteEventServiceStatus.CancelWithError;
                result.ErrorMessage = oops.Message;

                System.Diagnostics.Trace.WriteLine(oops.Message);
            }
        }

        public void ItemAddedToListEventHandler(ClientContext clientContext, Guid listId, int listItemId)
        {
            try
            {
                List photos = clientContext.Web.Lists.GetById(listId);
                ListItem item = photos.GetItemById(listItemId);
                clientContext.Load(item);
                clientContext.ExecuteQuery();

                item["Description"] += "\nUpdated by RER " +
                    System.DateTime.Now.ToLongTimeString();
                item.Update();
                clientContext.ExecuteQuery();
            }
            catch (Exception oops)
            {
                System.Diagnostics.Trace.WriteLine(oops.Message);
            }

        }

        /// <summary>
        /// Creates a list with Description and AssignedTo fields
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        internal List CreateJobsList(ClientContext context)
        {

            ListCreationInformation creationInfo = new ListCreationInformation();
            creationInfo.Title = LIST_TITLE;

            creationInfo.TemplateType = (int)ListTemplateType.GenericList;
            List list = context.Web.Lists.Add(creationInfo);
            list.Description = "List of jobs and assignments";
            list.Fields.AddFieldAsXml("<Field DisplayName='Description' Type='Text' />",
                true,
                AddFieldOptions.DefaultValue);
            list.Fields.AddFieldAsXml("<Field DisplayName='AssignedTo' Type='Text' />",
                true,
                AddFieldOptions.DefaultValue);

            list.Update();

            //Do not execute the call.  We simply create the list in the context, 
            //it's up to the caller to call ExecuteQuery.
            return list;
        }
    }
}
```

Additional classses used in this implementation are:

**CacheConnectionHelper.cs**:
```c#
using System;

namespace Contoso.Core.EventReceiversWeb.Cache
{
    using StackExchange.Redis;

    public class CacheConnectionHelper
    {
        private static Lazy<ConnectionMultiplexer> lazyConnection = new Lazy<ConnectionMultiplexer>(() =>
        {
            return ConnectionMultiplexer.Connect(SettingsHelper.AzureRedisCache);
        });

        public static ConnectionMultiplexer Connection
        {
            get
            {
                return lazyConnection.Value;
            }
        }
    }
}
```


**SettingsHelper.cs**:
```c#
using System;
using System.Configuration;

namespace Contoso.Core.EventReceiversWeb.Cache
{
    public class SettingsHelper
    {
        public static bool UseAzureRedisCache
        {
            get { return Convert.ToBoolean(ConfigurationManager.AppSettings["UseAzureRedisForCache"]); }
        }

        public static string AzureRedisCache
        {
            get { return ConfigurationManager.AppSettings["AzureRedisCache"]; }
        }
    }
}
```

**StackExchangeRedisExtensions.cs**:
```c#
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Contoso.Core.EventReceiversWeb.Cache
{
    using StackExchange.Redis;

    public static class StackExchangeRedisExtensions
    {
        public static bool Add(this IDatabase cache, string key, object value, TimeSpan expiration)
        {
            if (null == Deserialize<object>(cache.StringGet(key)))
            {
                cache.StringSet(key, Serialize(value), expiration);
                return true;
            }
            return false;
        }

        private static byte[] Serialize(object o)
        {
            if (o == null)
                return null;

            BinaryFormatter binaryFormatter = new BinaryFormatter();
            using (MemoryStream memoryStream = new MemoryStream())
            {
                binaryFormatter.Serialize(memoryStream, o);
                byte[] objectDataAsStream = memoryStream.ToArray();
                return objectDataAsStream;
            }
        }

        private static T Deserialize<T>(byte[] stream)
        {
            if (stream == null)
                return default(T);

            using (MemoryStream memoryStream = new MemoryStream(stream))
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                T result = (T)binaryFormatter.Deserialize(memoryStream);
                return result;
            }
        }
    }
}
```

<img src="https://telemetry.sharepointpnp.com/pnp/samples/Core.EventReceivers" />