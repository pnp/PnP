using Microsoft.SharePoint.Client;
using OfficeDevPnP.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OfficeDevPnP.Core.AppModelExtensions
{
    /// <summary>
    /// Class that holds deprecated methods for variations
    /// </summary>
    public static partial class VariationExtensions
    {
        #region Will be deprecated in August 2015 release
        /// <summary>
        /// Add a node to quickLaunch or top navigation bar
        /// </summary>
        /// <param name="web">Site to be processed - can be root web or sub site</param>
        /// <param name="nodeTitle">the title of node to add</param>
        /// <param name="nodeUri">the url of node to add</param>
        /// <param name="parentNodeTitle">if string.Empty, then will add this node as top level node</param>
        /// <param name="isQuickLaunch">true: add to quickLaunch; otherwise, add to top navigation bar</param>
        [Obsolete("Use public static void ProvisionTargetVariationLabels(this ClientContext context, List<VariationLabelEntity> variationLabels). This deprecated method will be removed in the August 2015 release.")]
        public static void ProviosionTargetVariationLabels(this ClientContext context, List<VariationLabelEntity> variationLabels) 
        {
            context.ProvisionTargetVariationLabels(variationLabels);
        }
        #endregion
    }
}
