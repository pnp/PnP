using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.EventReceivers;

namespace Core.AppEventsWeb.Services
{
    public class AppEventReceiver : IRemoteEventService
    {
        /// <summary>
        /// Handles app events that occur after the app is installed or upgraded, or when app is being uninstalled.
        /// </summary>
        /// <param name="properties">Holds information about the app event.</param>
        /// <returns>Holds information returned from the app event.</returns>
        public SPRemoteEventResult ProcessEvent(SPRemoteEventProperties properties)
        {
            SPRemoteEventResult result = new SPRemoteEventResult();
            String listTitle = "TestList";
            Guid listID = Guid.Empty;

            switch (properties.EventType)
            {
                case SPRemoteEventType.AppInstalled:

                    try
                    {
                        listID = CreateList(listTitle, properties);
                        // Uncomment the next line to test exception handling.
                        // throw new Exception("My test exception");
                        // Also try putting the preceding line above the CreateList call.
                    }
                    catch (Exception e)
                    {
                        // Tell SharePoint to cancel the event.
                        result.ErrorMessage = e.Message;
                        result.Status = SPRemoteEventServiceStatus.CancelWithError;

                        // Delete the list if it was created.
                        DeleteList(listID, properties);
                    }
                    break;
                case SPRemoteEventType.AppUpgraded:
                    break;
                case SPRemoteEventType.AppUninstalling:

                    try
                    {
                        RecycleList(listTitle, properties);
                        // Uncomment the next line to test exception handling.
                        // throw new Exception("My test exception");
                        // Also try putting the preceding line above the RecycleList call.
                    }
                    catch (Exception e)
                    {
                        // Tell SharePoint to cancel the event.
                        result.ErrorMessage = e.Message;
                        result.Status = SPRemoteEventServiceStatus.CancelWithError;

                        // Recover the list if it is in Recycle Bin.
                        RestoreList(listTitle, properties);
                    }
                    break;
            }
            return result;
        }


        /// <summary>
        /// This method is a required placeholder, but is not used by app events.
        /// </summary>
        /// <param name="properties">Unused.</param>
        public void ProcessOneWayEvent(SPRemoteEventProperties properties)
        {
            throw new NotImplementedException();
        }


        private Guid CreateList(String listTitle, SPRemoteEventProperties properties)
        {
            Guid listID = Guid.Empty;

            using (ClientContext clientContext = TokenHelper.CreateAppEventClientContext(properties, useAppWeb: false))
            {
                if (clientContext != null)
                {
                    // SharePoint might be retrying the event after a time-out. It
                    // may have created the list on the last try of this handler, so
                    // check to see if there's already a list with that name.                                
                    List targetList = GetListByTitle(listTitle, clientContext);

                    // If there isn't one, create it.
                    if (targetList == null)
                    {
                        ListCreationInformation listInfo = new ListCreationInformation();
                        listInfo.Title = listTitle;
                        listInfo.TemplateType = (int)ListTemplateType.GenericList;
                        listInfo.Url = listTitle;
                        targetList = clientContext.Web.Lists.Add(listInfo);
                        clientContext.Load(targetList, l => l.Id);
                        clientContext.ExecuteQuery();
                        listID = targetList.Id;
                    }
                }
            }
            return listID;
        }

        private void DeleteList(Guid listID, SPRemoteEventProperties properties)
        {
            using (ClientContext clientContext = TokenHelper.CreateAppEventClientContext(properties, useAppWeb: false))
            {
                if (clientContext != null)
                {
                    // Check to see if the "try" block code got far enough to create the list before it errored. 
                    List targetList = GetListByID(listID, clientContext);

                    // If it did, delete the list.
                    if (targetList != null)
                    {
                        targetList.DeleteObject();
                        clientContext.ExecuteQuery();
                    }
                }
            }
        }


        private List GetListByTitle(String listTitle, ClientContext clientContext)
        {
            ListCollection allLists = clientContext.Web.Lists;
            IEnumerable<List> matchingLists = clientContext.LoadQuery(allLists.Where(list => list.Title == listTitle));
            clientContext.ExecuteQuery();
            return matchingLists.FirstOrDefault();
        }
        private List GetListByID(Guid listID, ClientContext clientContext)
        {
            ListCollection allLists = clientContext.Web.Lists;
            IEnumerable<List> matchingLists = clientContext.LoadQuery(allLists.Where(list => list.Id == listID));
            clientContext.ExecuteQuery();
            return matchingLists.FirstOrDefault();
        }

        private void RecycleList(String title, SPRemoteEventProperties properties)
        {
            using (ClientContext clientContext = TokenHelper.CreateAppEventClientContext(properties, useAppWeb: false))
            {
                if (clientContext != null)
                {
                    // Check to see that a user hasn't already recycled the list in the SharePoint UI.
                    List targetList = GetListByTitle(title, clientContext);

                    // If its still there, recycle it.
                    if (targetList != null)
                    {
                        targetList.Recycle();
                        clientContext.ExecuteQuery();
                    }
                }
            }
        }

        private void RestoreList(String title, SPRemoteEventProperties properties)
        {
            using (ClientContext clientContext = TokenHelper.CreateAppEventClientContext(properties, useAppWeb: false))
            {
                if (clientContext != null)
                {
                    // Check to see that a user hasn't manually deleted the list from the Recycle Bin
                    RecycleBinItemCollection bin = clientContext.Web.RecycleBin;
                    IEnumerable<RecycleBinItem> matchingItems = clientContext.LoadQuery(bin.Where(item => item.Title == title));
                    clientContext.ExecuteQuery();
                    RecycleBinItem recycledList = matchingItems.FirstOrDefault();

                    // If it is there, restore it. 
                    if (recycledList != null)
                    {
                        recycledList.Restore();
                        clientContext.ExecuteQuery();
                    }
                }
            }
        }
    }
}
