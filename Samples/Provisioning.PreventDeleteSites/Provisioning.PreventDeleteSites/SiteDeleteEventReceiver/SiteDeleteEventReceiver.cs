using System;
using System.Security.Permissions;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
using Microsoft.SharePoint.Workflow;

namespace Provisioning.PreventDeleteSites {
    /// <summary>
    /// Web Events
    /// </summary>
    public class SiteDeleteEventReceiver : SPWebEventReceiver {
        /// <summary>
        /// A site collection is being deleted.
        /// </summary>
        public override void SiteDeleting(SPWebEventProperties properties) {
            properties.Cancel = true;
            properties.ErrorMessage = "Site collection cannot be deleted";
        }

        /// <summary>
        /// A site is being deleted.
        /// </summary>
        public override void WebDeleting(SPWebEventProperties properties) {
            properties.Cancel = true;
            properties.ErrorMessage = "Site cannot be deleted";
        }


    }
}