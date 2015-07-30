using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace Contoso.Core.EventReceiversWeb
{
    public class RemoteEventReceiverManager
    {
        private const string RECEIVER_NAME = "ItemAddedEvent";
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

                System.Diagnostics.Trace.WriteLine("Added ItemAdded receiver at " + receiver.ReceiverUrl);
            }
        }

        public void RemoveEventReceiversFromHostWeb(ClientContext clientContext)
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