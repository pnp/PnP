using System;
using System.Collections.Generic;
using System.Linq;
using OfficeDevPnP.Core.Entities;
using OfficeDevPnP.Core.Enums;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using System.Collections;
using Microsoft.SharePoint.Client.Taxonomy;

namespace Microsoft.SharePoint.Client
{

    /// <summary>
    /// This class holds navigation related methods
    /// </summary>
    public static partial class NavigationExtensions
    {

        #region Area Navigation (publishing sites)
        const string PublishingFeatureActivated = "__PublishingFeatureActivated";
        const string WebNavigationSettings = "_webnavigationsettings";
        const string CurrentNavigationIncludeTypes = "__CurrentNavigationIncludeTypes";
        const string CurrentDynamicChildLimit = "__CurrentDynamicChildLimit";
        const string GlobalNavigationIncludeTypes = "__GlobalNavigationIncludeTypes";
        const string GlobalDynamicChildLimit = "__GlobalDynamicChildLimit";
        const string NavigationOrderingMethod = "__NavigationOrderingMethod";
        const string NavigationAutomaticSortingMethod = "__NavigationAutomaticSortingMethod";
        const string NavigationSortAscending = "__NavigationSortAscending";
        const string NavigationShowSiblings = "__NavigationShowSiblings";

        /// <summary>
        /// Returns the navigation settings for the selected web
        /// </summary>
        /// <param name="web"></param>
        /// <returns></returns>
        public static AreaNavigationEntity GetNavigationSettings(this Web web)
        {
            AreaNavigationEntity nav = new AreaNavigationEntity();

            //Read all the properties of the web
            web.Context.Load(web, w => w.AllProperties);
            web.Context.ExecuteQueryRetry();

            if (!ArePublishingFeaturesActivated(web.AllProperties))
            {
                throw new ArgumentException("Structural navigation settings are only supported for publishing sites");
            }

            // Determine if managed navigation is used...if so the other properties are not relevant
            string webNavigationSettings = web.AllProperties.GetPropertyAsString(WebNavigationSettings);
            if (webNavigationSettings == null)
            {
                nav.CurrentNavigation.ManagedNavigation = false;
                nav.GlobalNavigation.ManagedNavigation = false;
            }
            else
            {
                var navigationSettings = XElement.Parse(webNavigationSettings);
                IEnumerable<XElement> navNodes = navigationSettings.XPathSelectElements("./SiteMapProviderSettings/TaxonomySiteMapProviderSettings");
                foreach (var node in navNodes)
                {
                    if (node.Attribute("Name").Value.Equals("CurrentNavigationTaxonomyProvider", StringComparison.InvariantCulture))
                    {
                        bool managedNavigation = true;
                        if (node.Attribute("Disabled") != null)
                        {
                            if (bool.TryParse(node.Attribute("Disabled").Value, out managedNavigation))
                            {
                                managedNavigation = false;
                            }
                        }
                        nav.CurrentNavigation.ManagedNavigation = managedNavigation;
                    }
                    else if (node.Attribute("Name").Value.Equals("GlobalNavigationTaxonomyProvider", StringComparison.InvariantCulture))
                    {
                        bool managedNavigation = true;
                        if (node.Attribute("Disabled") != null)
                        {
                            if (bool.TryParse(node.Attribute("Disabled").Value, out managedNavigation))
                            {
                                managedNavigation = false;
                            }
                        }
                        nav.GlobalNavigation.ManagedNavigation = managedNavigation;
                    }
                }
            }

            // Only read the other values that make sense when not using managed navigation
            if (!nav.CurrentNavigation.ManagedNavigation)
            {
                int currentNavigationIncludeTypes = web.AllProperties.GetPropertyAsInt(CurrentNavigationIncludeTypes);
                if (currentNavigationIncludeTypes > -1)
                {
                    MapFromNavigationIncludeTypes(nav.CurrentNavigation, currentNavigationIncludeTypes);
                }

                int currentDynamicChildLimit = web.AllProperties.GetPropertyAsInt(CurrentDynamicChildLimit);
                if (currentDynamicChildLimit > -1)
                {
                    nav.CurrentNavigation.MaxDynamicItems = currentDynamicChildLimit;
                }

                // For the current navigation there's an option to show the sites siblings in structural navigation
                if (web.IsSubSite())
                {
                    bool showSiblings = false;
                    string navigationShowSiblings = web.AllProperties.GetPropertyAsString(NavigationShowSiblings);
                    if (bool.TryParse(navigationShowSiblings, out showSiblings))
                    {
                        nav.CurrentNavigation.ShowSiblings = showSiblings;
                    }
                }
            }

            if (!nav.GlobalNavigation.ManagedNavigation)
            {
                int globalNavigationIncludeTypes = web.AllProperties.GetPropertyAsInt(GlobalNavigationIncludeTypes);
                if (globalNavigationIncludeTypes > -1)
                {
                    MapFromNavigationIncludeTypes(nav.GlobalNavigation, globalNavigationIncludeTypes);
                }

                int globalDynamicChildLimit = web.AllProperties.GetPropertyAsInt(GlobalDynamicChildLimit);
                if (globalDynamicChildLimit > -1)
                {
                    nav.GlobalNavigation.MaxDynamicItems = globalDynamicChildLimit;
                }
            }

            // Read the sorting value 
            int navigationOrderingMethod = web.AllProperties.GetPropertyAsInt(NavigationOrderingMethod);
            if (navigationOrderingMethod > -1)
            {
                nav.Sorting = (StructuralNavigationSorting)navigationOrderingMethod;
            }

            // Read the sort by value
            int navigationAutomaticSortingMethod = web.AllProperties.GetPropertyAsInt(NavigationAutomaticSortingMethod);
            if (navigationAutomaticSortingMethod > -1)
            {
                nav.SortBy = (StructuralNavigationSortBy)navigationAutomaticSortingMethod;
            }

            // Read the ordering setting
            bool navigationSortAscending = true;
            string navProp = web.AllProperties.GetPropertyAsString(NavigationSortAscending);

            if (bool.TryParse(navProp, out navigationSortAscending))
            {
                nav.SortAscending = navigationSortAscending;
            }

            return nav;
        }

        /// <summary>
        /// Updates navigation settings for the current web
        /// </summary>
        /// <param name="web"></param>
        /// <param name="navigationSettings"></param>
        public static void UpdateNavigationSettings(this Web web, AreaNavigationEntity navigationSettings)
        {
            //Read all the properties of the web
            web.Context.Load(web, w => w.AllProperties);
            web.Context.ExecuteQueryRetry();

            if (!ArePublishingFeaturesActivated(web.AllProperties))
            {
                throw new ArgumentException("Structural navigation settings are only supported for publishing sites");
            }

            // Use publishing CSOM API to switch between managed metadata and structural navigation
            TaxonomySession taxonomySession = TaxonomySession.GetTaxonomySession(web.Context);
            web.Context.Load(taxonomySession);
            web.Context.ExecuteQueryRetry();
            Microsoft.SharePoint.Client.Publishing.Navigation.WebNavigationSettings webNav = new Publishing.Navigation.WebNavigationSettings(web.Context, web);
            if (!navigationSettings.GlobalNavigation.ManagedNavigation)
            {
                webNav.GlobalNavigation.Source = Publishing.Navigation.StandardNavigationSource.PortalProvider;
            }
            else
            {
                webNav.GlobalNavigation.Source = Publishing.Navigation.StandardNavigationSource.TaxonomyProvider;
            }

            if (!navigationSettings.CurrentNavigation.ManagedNavigation)
            {
                webNav.CurrentNavigation.Source = Publishing.Navigation.StandardNavigationSource.PortalProvider;
            }
            else
            {
                webNav.CurrentNavigation.Source = Publishing.Navigation.StandardNavigationSource.TaxonomyProvider;
            }
            webNav.Update(taxonomySession);
            web.Context.ExecuteQueryRetry();

            //Read all the properties of the web again after the above update
            web.Context.Load(web, w => w.AllProperties);
            web.Context.ExecuteQueryRetry();

            if (!navigationSettings.GlobalNavigation.ManagedNavigation)
            {
                int globalNavigationIncludeType = MapToNavigationIncludeTypes(navigationSettings.GlobalNavigation);
                web.AllProperties[GlobalNavigationIncludeTypes] = globalNavigationIncludeType;
                web.AllProperties[GlobalDynamicChildLimit] = navigationSettings.GlobalNavigation.MaxDynamicItems;
            }

            if (!navigationSettings.CurrentNavigation.ManagedNavigation)
            {
                int currentNavigationIncludeType = MapToNavigationIncludeTypes(navigationSettings.CurrentNavigation);
                web.AllProperties[CurrentNavigationIncludeTypes] = currentNavigationIncludeType;
                web.AllProperties[CurrentDynamicChildLimit] = navigationSettings.CurrentNavigation.MaxDynamicItems;

                // Call web.update before the IsSubSite call as this might do an ExecuteQuery. Without the update called the changes will be lost
                web.Update();
                // For the current navigation there's an option to show the sites siblings in structural navigation
                if (web.IsSubSite())
                {
                    web.AllProperties[NavigationShowSiblings] = navigationSettings.CurrentNavigation.ShowSiblings.ToString();
                }
            }

            // if there's either global or current structural navigation then update the sorting settings
            if (!navigationSettings.GlobalNavigation.ManagedNavigation || !navigationSettings.CurrentNavigation.ManagedNavigation)
            {
                // If there's automatic sorting or pages are shown with automatic page sorting then we can set all sort options
                if ((navigationSettings.Sorting == StructuralNavigationSorting.Automatically) ||
                    (navigationSettings.Sorting == StructuralNavigationSorting.ManuallyButPagesAutomatically && (navigationSettings.GlobalNavigation.ShowPages || navigationSettings.CurrentNavigation.ShowPages)))
                {
                    // All sort options can be set
                    web.AllProperties[NavigationOrderingMethod] = (int)navigationSettings.Sorting;
                    web.AllProperties[NavigationAutomaticSortingMethod] = (int)navigationSettings.SortBy;
                    web.AllProperties[NavigationSortAscending] = navigationSettings.SortAscending.ToString();
                }
                else
                {
                    // if pages are not shown we can set sorting to either automatic or manual
                    if (!navigationSettings.GlobalNavigation.ShowPages && !navigationSettings.CurrentNavigation.ShowPages)
                    {
                        if (navigationSettings.Sorting == StructuralNavigationSorting.ManuallyButPagesAutomatically)
                        {
                            throw new ArgumentException("Sorting can only be set to StructuralNavigationSorting.ManuallyButPagesAutomatically when ShowPages has been selected in either the global or current structural navigation settings");
                        }
                    }

                    web.AllProperties[NavigationOrderingMethod] = (int)navigationSettings.Sorting;
                }
            }

            //Persist all property updates at once
            web.Update();
            web.Context.ExecuteQueryRetry();
        }

        private static int MapToNavigationIncludeTypes(StructuralNavigationEntity sne)
        {
            int navigationIncludeType = -1;

            if (!sne.ShowPages && !sne.ShowSubsites)
            {
                navigationIncludeType = 0;
            }
            else if (!sne.ShowPages && sne.ShowSubsites)
            {
                navigationIncludeType = 1;
            }
            else if (sne.ShowPages && !sne.ShowSubsites)
            {
                navigationIncludeType = 2;
            }
            else if (sne.ShowPages && sne.ShowSubsites)
            {
                navigationIncludeType = 3;
            }

            return navigationIncludeType;
        }


        private static void MapFromNavigationIncludeTypes(StructuralNavigationEntity sne, int navigationIncludeTypes)
        {
            if (navigationIncludeTypes == 0)
            {
                sne.ShowPages = false;
                sne.ShowSubsites = false;
            }
            else if (navigationIncludeTypes == 1)
            {
                sne.ShowPages = false;
                sne.ShowSubsites = true;
            }
            else if (navigationIncludeTypes == 2)
            {
                sne.ShowPages = true;
                sne.ShowSubsites = false;
            }
            else if (navigationIncludeTypes == 3)
            {
                sne.ShowPages = true;
                sne.ShowSubsites = true;
            }
        }

        private static bool ArePublishingFeaturesActivated(PropertyValues props)
        {
            bool activated = false;

            if (bool.TryParse(props.GetPropertyAsString(PublishingFeatureActivated), out activated))
            {
            }

            return activated;
        }

        private static string GetPropertyAsString(this PropertyValues props, string key)
        {
            if (props.FieldValues.ContainsKey(key))
            {
                return props.FieldValues[key].ToString();
            }
            else
            {
                return null;
            }
        }
        private static int GetPropertyAsInt(this PropertyValues props, string key)
        {
            if (props.FieldValues.ContainsKey(key))
            {
                int res;
                if (int.TryParse(props.FieldValues[key].ToString(), out res))
                {
                    return res;
                }
                else
                {
                    return -1;
                }
            }
            else
            {
                return -1;
            }
        }
        #endregion

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
        /// <param name="isExternal">true if the link is an external link</param>

        public static void AddNavigationNode(this Web web, string nodeTitle, Uri nodeUri, string parentNodeTitle, NavigationType navigationType, bool isExternal = false)
        {
            web.Context.Load(web, w => w.Navigation.QuickLaunch, w => w.Navigation.TopNavigationBar);
            web.Context.ExecuteQueryRetry();
            NavigationNodeCreationInformation node = new NavigationNodeCreationInformation
            {
                AsLastNode = true,
                Title = nodeTitle,
                Url = nodeUri != null ? nodeUri.OriginalString : string.Empty,
                IsExternal = isExternal
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
                    if (!string.IsNullOrEmpty(parentNodeTitle))
                    {
                        var parentNode = topLink.FirstOrDefault(n => n.Title == parentNodeTitle);
                        if (parentNode != null)
                        {
                            parentNode.Children.Add(node);
                        }
                    }
                    else
                    {
                        topLink.Add(node);
                    }
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
                    if(string.IsNullOrEmpty(parentNodeTitle))
                    {
                        deleteNode = topLink.SingleOrDefault(n => n.Title == nodeTitle);
                    } else
                    {
                        foreach(var nodeInfo in topLink)
                        {
                            if(nodeInfo.Title != parentNodeTitle)
                            {
                                continue;
                            }
                            web.Context.Load(nodeInfo.Children);
                            web.Context.ExecuteQueryRetry();
                            deleteNode = nodeInfo.Children.SingleOrDefault(n => n.Title == nodeTitle);
                        }
                    }
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
        /// web.AddCustomAction(editAction);
        /// </example>
        /// <returns>True if action was successfull</returns>
        public static bool AddCustomAction(this Web web, CustomActionEntity customAction)
        {
            return AddCustomActionImplementation(web, customAction);
        }

        /// <summary>
        /// Adds custom action to a site collection. If the CustomAction exists the item will be updated.
        /// Setting CustomActionEntity.Remove == true will delete the CustomAction.
        /// </summary>
        /// <param name="site">Site collection to be processed</param>
        /// <param name="customAction">Information about the custom action be added or deleted</param>
        /// <returns>True if action was successfull</returns>
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

            var customActions = web.UserCustomActions.AsEnumerable<UserCustomAction>();
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
        /// <param name="site"></param>
        /// <param name="name">Name of the custom action</param>
        /// <returns></returns>        
        public static bool CustomActionExists(this Site site, string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            site.Context.Load(site.UserCustomActions);
            site.Context.ExecuteQueryRetry();

            var customActions = site.UserCustomActions.AsEnumerable<UserCustomAction>();
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