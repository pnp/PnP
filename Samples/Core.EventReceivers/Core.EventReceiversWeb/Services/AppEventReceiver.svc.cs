using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.EventReceivers;
using System;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace Contoso.Core.EventReceiversWeb.Services
{

    public class AppEventReceiver : IRemoteEventService
    {

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


        public void ProcessOneWayEvent(SPRemoteEventProperties properties)
        {
            // This method is not used by app events
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
                            if (rer.ReceiverName == RECEIVER_NAME)
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

                        receiver.ReceiverName = RECEIVER_NAME;
                        receiver.Synchronization = EventReceiverSynchronization.Synchronous;

                        //Add the new event receiver to a list in the host web
                        jobsList.EventReceivers.Add(receiver);

                        clientContext.ExecuteQuery();

                        System.Diagnostics.Trace.WriteLine("Added ItemAdded receiver at "
                                + msg.Headers.To.ToString());
                    }
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
                    List myList = clientContext.Web.Lists.GetByTitle(LIST_TITLE);
                    clientContext.Load(myList, p => p.EventReceivers);
                    clientContext.ExecuteQuery();

                    var rer = myList.EventReceivers.Where(
                        e => e.ReceiverName == RECEIVER_NAME).FirstOrDefault();

                    try
                    {
                        System.Diagnostics.Trace.WriteLine("Removing ItemAdded receiver at "
                                + rer.ReceiverUrl);

                        //This will fail when deploying via F5, but works
                        //when deployed to production
                        rer.DeleteObject();
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
                if (clientContext != null)
                {
                    try
                    {
                        List photos = clientContext.Web.Lists.GetById(
                            properties.ItemEventProperties.ListId);
                        ListItem item = photos.GetItemById(
                            properties.ItemEventProperties.ListItemId);
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


