using OfficeDevPnP.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.SharePoint.Client
{
    /// <summary>
    /// This class holds deprecated navigation related methods
    /// </summary>
    public static partial class NavigationExtensions
    {
        #region Will be deprecated in May 2015 release
        /// <summary>
        /// Add a node to quickLaunch or top navigation bar
        /// </summary>
        /// <param name="web">Site to be processed - can be root web or sub site</param>
        /// <param name="nodeTitle">the title of node to add</param>
        /// <param name="nodeUri">the url of node to add</param>
        /// <param name="parentNodeTitle">if string.Empty, then will add this node as top level node</param>
        /// <param name="isQuickLaunch">true: add to quickLaunch; otherwise, add to top navigation bar</param>
        [Obsolete("Use public static void AddNavigationNode(this Web web, string nodeTitle, Uri nodeUri, string parentNodeTitle, NavigationType navigationType). This deprecated method will be removed in the May release.")]
        public static void AddNavigationNode(this Web web, string nodeTitle, Uri nodeUri, string parentNodeTitle, bool isQuickLaunch)
        {
            AddNavigationNode(web, nodeTitle, nodeUri, parentNodeTitle, isQuickLaunch ? NavigationType.QuickLaunch : NavigationType.TopNavigationBar);
        }

        /// <summary>
        /// Deletes a navigation node from the quickLaunch or top navigation bar
        /// </summary>
        /// <param name="web">Site to be processed - can be root web or sub site</param>
        /// <param name="nodeTitle">the title of node to delete</param>
        /// <param name="parentNodeTitle">if string.Empty, then will delete this node as top level node</param>
        /// <param name="isQuickLaunch">true: delete from quickLaunch; otherwise, delete from top navigation bar</param>
        [Obsolete("Use: DeleteNavigationNode(this Web web, string nodeTitle, string parentNodeTitle, NavigationType navigationType). This deprecated method will be removed in the May release.")]
        public static void DeleteNavigationNode(this Web web, string nodeTitle, string parentNodeTitle, bool isQuickLaunch)
        {
            DeleteNavigationNode(web, nodeTitle, parentNodeTitle, isQuickLaunch ? NavigationType.QuickLaunch : NavigationType.TopNavigationBar);
        }

        [Obsolete("Use: CustomActionExists(this Web web, string name). This deprecated method will be removed in the May release.")]
        public static bool CustomActionExists(ClientContext clientContext, string name)
        {
            return clientContext.Web.CustomActionExists(name);
        }
        #endregion
    }
}
