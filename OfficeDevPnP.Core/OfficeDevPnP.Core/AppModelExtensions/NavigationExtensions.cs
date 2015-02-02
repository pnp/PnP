using Microsoft.SharePoint.Client;
using OfficeDevPnP.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Management;

namespace Microsoft.SharePoint.Client
{
    /// <summary>
    /// This class holds navigation related methods
    /// </summary>
    public static class NavigationExtensions
    {
        #region Navigation elements  - quicklaunch and top navigation
        /// <summary>
        /// Add a node to quickLaunch or top navigation bar
        /// </summary>
        /// <param name="web">Site to be processed - can be root web or sub site</param>
        /// <param name="nodeTitle">the title of node to add</param>
        /// <param name="nodeUri">the url of node to add</param>
        /// <param name="parentNodeTitle">if string.Empty, then will add this node as top level node</param>
        /// <param name="isQucikLaunch">true: add to quickLaunch; otherwise, add to top navigation bar</param>
        public static void AddNavigationNode(this Web web, string nodeTitle, Uri nodeUri, string parentNodeTitle, bool isQuickLaunch)
        {
            web.Context.Load(web, w => w.Navigation.QuickLaunch, w => w.Navigation.TopNavigationBar);
            web.Context.ExecuteQuery();
            NavigationNodeCreationInformation node = new NavigationNodeCreationInformation();
            node.AsLastNode = true;
            node.Title = nodeTitle;
            node.Url = nodeUri != null ? nodeUri.OriginalString : "";

            if (isQuickLaunch)
            {
                var quickLaunch = web.Navigation.QuickLaunch;
                if (string.IsNullOrEmpty(parentNodeTitle))
                {
                    quickLaunch.Add(node);
                }
                else
                {
                    foreach (var nodeInfo in quickLaunch)
                    {
                        if (nodeInfo.Title == parentNodeTitle)
                        {
                            nodeInfo.Children.Add(node);
                            break;
                        }
                    }
                }
            }
            else
            {
                var topLink = web.Navigation.TopNavigationBar;
                topLink.Add(node);
            }
            web.Context.ExecuteQuery();
        }

        /// <summary>
        /// Deletes a navigation node from the quickLaunch or top navigation bar
        /// </summary>
        /// <param name="web">Site to be processed - can be root web or sub site</param>
        /// <param name="nodeTitle">the title of node to delete</param>
        /// <param name="parentNodeTitle">if string.Empty, then will delete this node as top level node</param>
        /// <param name="isQuickLaunch">true: delete from quickLaunch; otherwise, delete from top navigation bar</param>
        public static void DeleteNavigationNode(this Web web, string nodeTitle, string parentNodeTitle, bool isQuickLaunch)
        {
            web.Context.Load(web, w => w.Navigation.QuickLaunch, w => w.Navigation.TopNavigationBar);
            web.Context.ExecuteQuery();

            if (isQuickLaunch)
            {
                var quickLaunch = web.Navigation.QuickLaunch;
                if (string.IsNullOrEmpty(parentNodeTitle))
                {
                    foreach (var nodeInfo in quickLaunch)
                    {
                        if (nodeInfo.Title == nodeTitle)
                        {
                            nodeInfo.DeleteObject();
                            web.Context.ExecuteQuery();
                            break;
                        }
                    }
                }
                else
                {
                    bool done = false;
                    foreach (var nodeInfo in quickLaunch)
                    {
                        if (nodeInfo.Title == parentNodeTitle)
                        {
                            web.Context.Load(nodeInfo.Children);
                            web.Context.ExecuteQuery();
                            foreach (var nodeInfo2 in nodeInfo.Children)
                            {
                                if (nodeInfo2.Title == nodeTitle)
                                {
                                    nodeInfo2.DeleteObject();
                                    web.Context.ExecuteQuery();
                                    done = true;
                                    break;
                                }
                            }
                            if (done) break;
                        }
                    }
                }
            }
            else
            {
                var topLink = web.Navigation.TopNavigationBar;
                foreach (var nodeInfo in topLink)
                {
                    if (nodeInfo.Title == nodeTitle)
                    {
                        nodeInfo.DeleteObject();
                        web.Context.ExecuteQuery();
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Deletes all Quick Launch nodes
        /// </summary>
        /// <param name="web">Site to be processed - can be root web or sub site</param>
        public static void DeleteAllQuickLaunchNodes(this Web web)
        {

            web.Context.Load(web, w => w.Navigation.QuickLaunch);
            web.Context.ExecuteQuery();

            var quickLaunch = web.Navigation.QuickLaunch;
            for (int i = quickLaunch.Count - 1; i >= 0; i--)
            {
                quickLaunch[i].DeleteObject();
            }
            web.Context.ExecuteQuery();
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
            web.Context.ExecuteQuery();
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
        ///     Title = "Edit Site Classification",
        ///     Description = "Manage business impact information for site collection or sub sites.",
        ///     Sequence = 1000,
        ///     Group = "SiteActions",
        ///     Location = "Microsoft.SharePoint.StandardMenu",
        ///     Url = EditFormUrl,
        ///     ImageUrl = EditFormImageUrl,
        ///     Rights = new BasePermissions(),
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
            UserCustomAction targetAction = null;
            UserCustomActionCollection existingActions = null;
            if (clientObject is Web)
            {
                var web = (Web) clientObject;

                existingActions = web.UserCustomActions;
                web.Context.Load(existingActions);
                web.Context.ExecuteQuery();

                targetAction = web.UserCustomActions.FirstOrDefault(uca => uca.Name == customAction.Name);
            }
            else
            {
                var site = (Site) clientObject;

                existingActions = site.UserCustomActions;
                site.Context.Load(existingActions);
                site.Context.ExecuteQuery();

                targetAction = site.UserCustomActions.FirstOrDefault(uca => uca.Name == customAction.Name);
            }

            if (targetAction == null)
            {
                // If we're removing the custom action then we need to leave when not found...else we're creating the custom action
                if (customAction.Remove)
                {
                    return true;
                }
                else
                {
                    targetAction = existingActions.Add();
                }
            }
            else if (customAction.Remove)
            {
                targetAction.DeleteObject();
                clientObject.Context.ExecuteQuery();
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
                web.Context.ExecuteQuery();
            }
            else
            {
                var site = (Site) clientObject;
                site.Context.Load(site, s => s.UserCustomActions);
                site.Context.ExecuteQuery();
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
            var clientContext = web.Context as ClientContext;

            List<UserCustomAction> actions = new List<UserCustomAction>();

            clientContext.Load(web.UserCustomActions);
            clientContext.ExecuteQuery();

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
            var clientContext = site.Context as ClientContext;

            List<UserCustomAction> actions = new List<UserCustomAction>();

            clientContext.Load(site.UserCustomActions);
            clientContext.ExecuteQuery();

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
        /// <param name="id">The id of the action to remove. <seealso cref="GetCustomActions"/></param>
        public static void DeleteCustomAction(this Web web, Guid id)
        {
            var clientContext = web.Context as ClientContext;

            clientContext.Load(web.UserCustomActions);
            clientContext.ExecuteQuery();

            foreach (UserCustomAction action in web.UserCustomActions)
            {
                if (action.Id == id)
                {
                    action.DeleteObject();
                    clientContext.ExecuteQuery();
                    break;
                }
            }

        }

        /// <summary>
        /// Removes a custom action
        /// </summary>
        /// <param name="site">The site to process</param>
        /// <param name="id">The id of the action to remove. <seealso cref="GetCustomActions"/></param>
        public static void DeleteCustomAction(this Site site, Guid id)
        {
            var clientContext = site.Context as ClientContext;

            clientContext.Load(site.UserCustomActions);
            clientContext.ExecuteQuery();

            foreach (UserCustomAction action in site.UserCustomActions)
            {
                if (action.Id == id)
                {
                    action.DeleteObject();
                    clientContext.ExecuteQuery();
                    break;
                }
            }
        }

        /// <summary>
        /// Utility method to check particular custom action already exists on the web
        /// </summary>
        /// <param name="clientContext"></param>
        /// <param name="name">Name of the custom action</param>
        /// <returns></returns>
        public static bool CustomActionExists(ClientContext clientContext, string name)
        {
            if (clientContext == null)
                throw new ArgumentNullException("clientContext");

            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            clientContext.Load(clientContext.Web.UserCustomActions);
            clientContext.ExecuteQuery();

            var customActions = clientContext.Web.UserCustomActions.Cast<UserCustomAction>();
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
