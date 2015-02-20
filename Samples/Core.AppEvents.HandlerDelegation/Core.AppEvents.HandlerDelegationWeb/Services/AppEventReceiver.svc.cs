using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.EventReceivers;

namespace Core.AppEvents.HandlerDelegationWeb.Services
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
            String listTitle = "MyList";

            switch (properties.EventType)
            {
                case SPRemoteEventType.AppInstalled:

                    try
                    {
                        string error = TryCreateList(listTitle, properties);
                        if (error != String.Empty)
                        {
                            throw new Exception(error);
                        }
                    }
                    catch (Exception e)
                    {
                        // Tell SharePoint to cancel the event.
                        result.ErrorMessage = e.Message;
                        result.Status = SPRemoteEventServiceStatus.CancelWithError;
                    }
                    break;
                case SPRemoteEventType.AppUpgraded:
                    break;
                case SPRemoteEventType.AppUninstalling:

                    try
                    {
                        string error = TryRecycleList(listTitle, properties);
                        if (error != String.Empty)
                        {
                            throw new Exception(error);
                        }
                    }
                    catch (Exception e)
                    {
                        // Tell SharePoint to cancel the event.
                        result.ErrorMessage = e.Message;
                        result.Status = SPRemoteEventServiceStatus.CancelWithError;
                    }
                    break;
            }
            return result;
        }

        private string TryCreateList(String listTitle, SPRemoteEventProperties properties)
        {
            string errorMessage = String.Empty;

            using (ClientContext clientContext = TokenHelper.CreateAppEventClientContext(properties, useAppWeb: false))
            {
                if (clientContext != null)
                {
                    // Get references to the objects needed later.
                    ListCollection allLists = clientContext.Web.Lists;
                    IEnumerable<List> matchingLists = clientContext.LoadQuery(allLists.Where(list => list.Title == listTitle));

                    clientContext.ExecuteQuery();

                    var foundList = matchingLists.FirstOrDefault();
                    List createdList = null;

                    // Delegate the rollback logic to the SharePoint server.
                    ExceptionHandlingScope scope = new ExceptionHandlingScope(clientContext);
                    using (scope.StartScope())
                    {

                        using (scope.StartTry())
                        {
                            // SharePoint might be retrying the event after a time-out, so
                            // check to see if there's already a list with that name. 
                            // If there isn't, create it.                             
                            ConditionalScope condScope = new ConditionalScope(clientContext, () => foundList.ServerObjectIsNull.Value == true, true);
                            using (condScope.StartScope())
                            {
                                ListCreationInformation listInfo = new ListCreationInformation();
                                listInfo.Title = listTitle;
                                listInfo.TemplateType = (int)ListTemplateType.GenericList;
                                listInfo.Url = listTitle;
                                createdList = clientContext.Web.Lists.Add(listInfo);
                            }
                            // To test that your StartCatch block runs, uncomment the following two lines
                            // and put them somewhere in the StartTry block.
                            //List fakeList = clientContext.Web.Lists.GetByTitle("NoSuchList");
                            //clientContext.Load(fakeList);
                        }
                        using (scope.StartCatch())
                        {
                            // Check to see if the try code got far enough to create the list before it errored.
                            // If it did, roll this change back by deleting the list.
                            ConditionalScope condScope = new ConditionalScope(clientContext, () => createdList.ServerObjectIsNull.Value != true, true);
                            using (condScope.StartScope())
                            {
                                createdList.DeleteObject();
                            }
                        }
                        using (scope.StartFinally())
                        {
                        }
                    }
                    clientContext.ExecuteQuery();

                    if (scope.HasException)
                    {
                        errorMessage = String.Format("{0}: {1}; {2}; {3}; {4}; {5}", scope.ServerErrorTypeName, scope.ErrorMessage, scope.ServerErrorDetails, scope.ServerErrorValue, scope.ServerStackTrace, scope.ServerErrorCode);
                    }
                }
            }
            return errorMessage;
        }

        private string TryRecycleList(String listTitle, SPRemoteEventProperties properties)
        {
            string errorMessage = String.Empty;

            using (ClientContext clientContext = TokenHelper.CreateAppEventClientContext(properties, useAppWeb: false))
            {
                if (clientContext != null)
                {
                    // Get references to all the objects you are going to need.
                    ListCollection allLists = clientContext.Web.Lists;
                    IEnumerable<List> matchingLists = clientContext.LoadQuery(allLists.Where(list => list.Title == listTitle));
                    RecycleBinItemCollection bin = clientContext.Web.RecycleBin;
                    IEnumerable<RecycleBinItem> matchingRecycleBinItems = clientContext.LoadQuery(bin.Where(item => item.Title == listTitle));

                    clientContext.ExecuteQuery();

                    List foundList = matchingLists.FirstOrDefault();
                    RecycleBinItem recycledList = matchingRecycleBinItems.FirstOrDefault();

                    // Delegate the rollback logic to the SharePoint server.
                    ExceptionHandlingScope scope = new ExceptionHandlingScope(clientContext);
                    using (scope.StartScope())
                    {
                        using (scope.StartTry())
                        {
                            // Check to see that a user hasn't already recycled the list in the SharePoint UI.
                            // If it is still there, recycle it.
                            ConditionalScope condScope = new ConditionalScope(clientContext, () => foundList.ServerObjectIsNull.Value == false, true);
                            using (condScope.StartScope())
                            {
                                // Looks crazy to test for nullity inside a test for nullity,
                                // but without this inner test, foundList.Recycle() throws a null reference
                                // exception when the client side runtime is creating the XML to
                                // send to the server.
                                if (foundList != null)
                                {
                                    foundList.Recycle();
                                }
                            }
                            // To test that your StartCatch block runs, uncomment the following two lines
                            // and put them somewhere in the StartTry block.
                            //List fakeList = clientContext.Web.Lists.GetByTitle("NoSuchList");
                            //clientContext.Load(fakeList);
                        }
                        using (scope.StartCatch())
                        {
                            // Check to see that the list is in the Recycle Bin. 
                            // A user might have manually deleted the list from the Recycle Bin,
                            // or StartTry block may have errored before it recycled the list.
                            // If it is in the Recycle Bin, restore it.
                            ConditionalScope condScope = new ConditionalScope(clientContext, () => recycledList.ServerObjectIsNull.Value == false, true);
                            using (condScope.StartScope())
                            {
                                // Another test within a test to avoid a null reference.
                                if (recycledList != null)
                                {
                                    recycledList.Restore();
                                }
                            }
                        }
                        using (scope.StartFinally())
                        {
                        }
                    }
                    clientContext.ExecuteQuery();

                    if (scope.HasException)
                    {
                        errorMessage = String.Format("{0}: {1}; {2}; {3}; {4}; {5}", scope.ServerErrorTypeName, scope.ErrorMessage, scope.ServerErrorDetails, scope.ServerErrorValue, scope.ServerStackTrace, scope.ServerErrorCode);
                    }
                }
            }
            return errorMessage;
        }

        /// <summary>
        /// This method is a required placeholder, but is not used by app events.
        /// </summary>
        /// <param name="properties">Unused.</param>
        public void ProcessOneWayEvent(SPRemoteEventProperties properties)
        {
            throw new NotImplementedException();
        }
    }   
}
