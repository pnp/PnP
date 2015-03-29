using System;
using System.Collections.Generic;
using System.Linq;
using OfficeDevPnP.Core.Entities;
using OfficeDevPnP.Core.Enums;

namespace Microsoft.SharePoint.Client
{

    /// <summary>
    /// This class holds navigation related methods
    /// </summary>
    public static partial class NavigationExtensions
    {
        #region Navigation elements - quicklaunch, top navigation, search navigation
        /// <summary>
        /// Add a node to quick launch, top navigation bar or search navigation. The node will be added as the last node in the
        /// collection.
        /// </summary>
        /// <param name="web">Site to be processed - can be root web or sub site</param>
        /// <param name="nodeTitle">the title of node to add</param>
        /// <param name="nodeUri">the url of node to add</param>
        /// <param name="parentNodeTitle">if string.Empty, then will add this node as top level node</param>
        /// <param name="navigationType">the type of navigation, quick launch, top navigation or search navigation</param>
        public static void AddNavigationNode(this Web web, string nodeTitle, Uri nodeUri, string parentNodeTitle, NavigationType navigationType)
        {
            web.Context.Load(web, w => w.Navigation.QuickLaunch, w => w.Navigation.TopNavigationBar);
            web.Context.ExecuteQueryRetry();
            NavigationNodeCreationInformation node = new NavigationNodeCreationInformation
            {
                AsLastNode = true,
                Title = nodeTitle,
                Url = nodeUri != null ? nodeUri.OriginalString : string.Empty
            };

            try
            {
                if (navigationType == NavigationType.QuickLaunch)
                {
                    var quickLaunch = web.Navigation.QuickLaunch;
                    if (string.IsNullOrEmpty(parentNodeTitle))
                    {
                        quickLaunch.Add(node);
                        return;
                    }
                    NavigationNode parentNode = quickLaunch.SingleOrDefault(n => n.Title == parentNodeTitle);
                    if (parentNode != null)
                    {
                        parentNode.Children.Add(node);
                    }
                }
                else if (navigationType == NavigationType.TopNavigationBar)
                {
                    var topLink = web.Navigation.TopNavigationBar;
                    topLink.Add(node);
                }
                else if (navigationType == NavigationType.SearchNav)
                {
                    var searchNavigation = web.LoadSearchNavigation();
                    searchNavigation.Add(node);
                }
            }
            finally
            {
                web.Context.ExecuteQueryRetry();
            }
        }

        /// <summary>
        /// Deletes a navigation node from the quickLaunch or top navigation bar
        /// </summary>
        /// <param name="web">Site to be processed - can be root web or sub site</param>
        /// <param name="nodeTitle">the title of node to delete</param>
        /// <param name="parentNodeTitle">if string.Empty, then will delete this node as top level node</param>
        /// <param name="navigationType">the type of navigation, quick launch, top navigation or search navigation</param>
        public static void DeleteNavigationNode(this Web web, string nodeTitle, string parentNodeTitle, NavigationType navigationType)
        {
            web.Context.Load(web, w => w.Navigation.QuickLaunch, w => w.Navigation.TopNavigationBar);
            web.Context.ExecuteQueryRetry();
            NavigationNode deleteNode = null;
            try
            {
                if (navigationType == NavigationType.QuickLaunch)
                {
                    var quickLaunch = web.Navigation.QuickLaunch;
                    if (string.IsNullOrEmpty(parentNodeTitle))
                    {
                        deleteNode = quickLaunch.SingleOrDefault(n => n.Title == nodeTitle);
                    }
                    else
                    {
                        foreach (var nodeInfo in quickLaunch)
                        {
                            if (nodeInfo.Title != parentNodeTitle)
                            {
                                continue;
                            }

                            web.Context.Load(nodeInfo.Children);
                            web.Context.ExecuteQueryRetry();
                            deleteNode = nodeInfo.Children.SingleOrDefault(n => n.Title == nodeTitle);
                        }
                    }
                }
                else if (navigationType == NavigationType.TopNavigationBar)
                {
                    var topLink = web.Navigation.TopNavigationBar;
                    deleteNode = topLink.SingleOrDefault(n => n.Title == nodeTitle);
                }
                else if (navigationType == NavigationType.SearchNav)
                {
                    NavigationNodeCollection nodeCollection = web.LoadSearchNavigation();
                    deleteNode = nodeCollection.SingleOrDefault(n => n.Title == nodeTitle);
                }
            }
            finally
            {
                if (deleteNode != null)
                {
                    deleteNode.DeleteObject();
                }
                web.Context.ExecuteQueryRetry();
            }
        }

        /// <summary>
        /// Deletes all Quick Launch nodes
        /// </summary>
        /// <param name="web">Site to be processed - can be root web or sub site</param>
        public static void DeleteAllQuickLaunchNodes(this Web web)
        {
            web.Context.Load(web, w => w.Navigation.QuickLaunch);
            web.Context.ExecuteQueryRetry();

            var quickLaunch = web.Navigation.QuickLaunch;
            for (int i = quickLaunch.Count - 1; i >= 0; i--)
            {
                quickLaunch[i].DeleteObject();
            }
            web.Context.ExecuteQueryRetry();
        }

        /// <summary>
        /// Updates the navigation inheritance setting
        /// </summary>
        /// <param name="web">Site to be processed - can be root web or sub site</param>
        /// <param name="inheritNavigation">boolean indicating if navigation inheritance is needed or not</param>
        public static void UpdateNavigationInheritance(this Web web, bool inheritNavigation)
        {
            web.Navigation.UseShared = inheritNavigation;
            web.Update();
            web.Context.ExecuteQueryRetry();
        }

        /// <summary>
        /// Loads the search navigation nodes
        /// </summary>
        /// <param name="web">Site to be processed - can be root web or sub site</param>
        /// <returns>Collection of NavigationNode instances</returns>
        public static NavigationNodeCollection LoadSearchNavigation(this Web web)
        {
            var searchNav = web.Navigation.GetNodeById(1040); // 1040 is the id of the search navigation            
            var nodeCollection = searchNav.Children;
            web.Context.Load(searchNav);
            web.Context.Load(nodeCollection);
            web.Context.ExecuteQueryRetry();
            return nodeCollection;
        }
        #endregion

        #region Custom actions
        /// <summary>
        /// Adds custom action to a web. If the CustomAction exists the item will be updated.
        /// Setting CustomActionEntity.Remove == true will delete the CustomAction.
        /// </summary>
        /// <param name="web">Site to be processed - can be root web or sub site</param>
        /// <param name="customAction">Information about the custom action be added or deleted</param>
        /// <example>
        /// var editAction = new CustomActionEntity()
        /// {
        ///    Title = "Edit Site Classification",
        ///    Description = "Manage business impact information for site collection or sub sites.",
        ///    Sequence = 1000,
        ///    Group = "SiteActions",
        ///    Location = "Microsoft.SharePoint.StandardMenu",
        ///    Url = EditFormUrl,
        ///    ImageUrl = EditFormImageUrl,
        ///    Rights = new BasePermissions(),
        /// };
        /// editAction.Rights.Set(PermissionKind.ManageWeb);
        /// AddCustomAction(editAction, new Uri(site.Properties.Url));
        /// </example>
        /// <returns>True if action was successfull</returns>
        public static bool AddCustomAction(this Web web, CustomActionEntity customAction)
        {
            return AddCustomActionImplementation(web, customAction);
        }

        public static bool AddCustomAction(this Site site, CustomActionEntity customAction)
        {
            return AddCustomActionImplementation(site, customAction);
        }

        private static bool AddCustomActionImplementation(ClientObject clientObject, CustomActionEntity customAction)
        {
            UserCustomAction targetAction;
            UserCustomActionCollection existingActions;
            if (clientObject is Web)
            {
                var web = (Web)clientObject;

                existingActions = web.UserCustomActions;
                web.Context.Load(existingActions);
                web.Context.ExecuteQueryRetry();

                targetAction = web.UserCustomActions.FirstOrDefault(uca => uca.Name == customAction.Name);
            }
            else
            {
                var site = (Site)clientObject;

                existingActions = site.UserCustomActions;
                site.Context.Load(existingActions);
                site.Context.ExecuteQueryRetry();

                targetAction = site.UserCustomActions.FirstOrDefault(uca => uca.Name == customAction.Name);
            }

            if (targetAction == null)
            {
                // If we're removing the custom action then we need to leave when not found...else we're creating the custom action
                if (customAction.Remove)
                {
                    return true;
                }
                targetAction = existingActions.Add();
            }
            else if (customAction.Remove)
            {
                targetAction.DeleteObject();
                clientObject.Context.ExecuteQueryRetry();
                return true;
            }

            targetAction.Name = customAction.Name;
            targetAction.Description = customAction.Description;
            targetAction.Location = customAction.Location;

            if (customAction.Location == JavaScriptExtensions.SCRIPT_LOCATION)
            {
                targetAction.ScriptBlock = customAction.ScriptBlock;
                targetAction.ScriptSrc = customAction.ScriptSrc;
            }
            else
            {
                targetAction.Sequence = customAction.Sequence;
                targetAction.Url = customAction.Url;
                targetAction.Group = customAction.Group;
                targetAction.Title = customAction.Title;
                targetAction.ImageUrl = customAction.ImageUrl;

                if (customAction.RegistrationId != null)
                {
                    targetAction.RegistrationId = customAction.RegistrationId;
                }

                if (customAction.CommandUIExtension != null)
                {
                    targetAction.CommandUIExtension = customAction.CommandUIExtension;
                }

                if (customAction.Rights != null)
                {
                    targetAction.Rights = customAction.Rights;
                }

                if (customAction.RegistrationType.HasValue)
                {
                    targetAction.RegistrationType = customAction.RegistrationType.Value;
                }
            }

            targetAction.Update();
            if (clientObject is Web)
            {
                var web = (Web)clientObject;
                web.Context.Load(web, w => w.UserCustomActions);
                web.Context.ExecuteQueryRetry();
            }
            else
            {
                var site = (Site)clientObject;
                site.Context.Load(site, s => s.UserCustomActions);
                site.Context.ExecuteQueryRetry();
            }

            return true;
        }


        /// <summary>
        /// Returns all custom actions in a web
        /// </summary>
        /// <param name="web">The web to process</param>
        /// <returns></returns>
        public static IEnumerable<UserCustomAction> GetCustomActions(this Web web)
        {
            var clientContext = (ClientContext)web.Context;

            List<UserCustomAction> actions = new List<UserCustomAction>();

            clientContext.Load(web.UserCustomActions);
            clientContext.ExecuteQueryRetry();

            foreach (UserCustomAction uca in web.UserCustomActions)
            {
                actions.Add(uca);
            }
            return actions;
        }

        /// <summary>
        /// Returns all custom actions in a web
        /// </summary>
        /// <param name="site">The site to process</param>
        /// <returns></returns>
        public static IEnumerable<UserCustomAction> GetCustomActions(this Site site)
        {
            var clientContext = (ClientContext)site.Context;

            List<UserCustomAction> actions = new List<UserCustomAction>();

            clientContext.Load(site.UserCustomActions);
            clientContext.ExecuteQueryRetry();

            foreach (UserCustomAction uca in site.UserCustomActions)
            {
                actions.Add(uca);
            }
            return actions;
        }

        /// <summary>
        /// Removes a custom action
        /// </summary>
        /// <param name="web">The web to process</param>
        /// <param name="id">The id of the action to remove. <seealso>
        ///         <cref>GetCustomActions</cref>
        ///     </seealso>
        /// </param>
        public static void DeleteCustomAction(this Web web, Guid id)
        {
            var clientContext = (ClientContext)web.Context;

            clientContext.Load(web.UserCustomActions);
            clientContext.ExecuteQueryRetry();

            foreach (UserCustomAction action in web.UserCustomActions)
            {
                if (action.Id == id)
                {
                    action.DeleteObject();
                    clientContext.ExecuteQueryRetry();
                    break;
                }
            }
        }

        /// <summary>
        /// Removes a custom action
        /// </summary>
        /// <param name="site">The site to process</param>
        /// <param name="id">The id of the action to remove. <seealso>
        ///         <cref>GetCustomActions</cref>
        ///     </seealso>
        /// </param>
        public static void DeleteCustomAction(this Site site, Guid id)
        {
            var clientContext = (ClientContext)site.Context;

            clientContext.Load(site.UserCustomActions);
            clientContext.ExecuteQueryRetry();

            foreach (UserCustomAction action in site.UserCustomActions)
            {
                if (action.Id == id)
                {
                    action.DeleteObject();
                    clientContext.ExecuteQueryRetry();
                    break;
                }
            }
        }

        /// <summary>
        /// Utility method to check particular custom action already exists on the web
        /// </summary>
        /// <param name="web"></param>
        /// <param name="name">Name of the custom action</param>
        /// <returns></returns>        
        public static bool CustomActionExists(this Web web, string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            web.Context.Load(web.UserCustomActions);
            web.Context.ExecuteQueryRetry();

            var customActions = web.UserCustomActions.Cast<UserCustomAction>();
            foreach (var customAction in customActions)
            {
                var customActionName = customAction.Name;
                if (!string.IsNullOrEmpty(customActionName) &&
                    customActionName.Equals(name, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Utility method to check particular custom action already exists on the web
        /// </summary>
        /// <param name="web"></param>
        /// <param name="name">Name of the custom action</param>
        /// <returns></returns>        
        public static bool CustomActionExists(this Site site, string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            site.Context.Load(site.UserCustomActions);
            site.Context.ExecuteQueryRetry();

            var customActions = site.UserCustomActions.Cast<UserCustomAction>();
            foreach (var customAction in customActions)
            {
                var customActionName = customAction.Name;
                if (!string.IsNullOrEmpty(customActionName) &&
                    customActionName.Equals(name, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }

        #endregion
    }
}