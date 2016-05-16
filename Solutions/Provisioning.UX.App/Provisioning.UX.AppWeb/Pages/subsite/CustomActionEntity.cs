using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Provisioning.UX.AppWeb.Pages.SubSite
{
    /// <summary>
    /// CustomActionEntity class describes the information for a SPO Custom Action
    /// </summary>
    public class CustomActionEntity
    {
        /// <summary>
        /// Description of the custom action
        /// </summary>
        public string Description
        {
            get;
            set;
        }

        /// <summary>
        /// Custom action title
        /// </summary>
        public string Title
        {
            get;
            set;
        }

        /// <summary>
        /// Custom action location (A string that contains the location; for example, Microsoft.SharePoint.SiteSettings)
        /// </summary>
        public string Location
        {
            get;
            set;
        }

        /// <summary>
        /// The JavaScript to be executed by this custom action
        /// </summary>
        public string ScriptBlock
        {
            get;
            set;
        }

        /// <summary>
        /// The sequence number of this custom action
        /// </summary>
        public int Sequence
        {
            get;
            set;
        }

        /// <summary>
        /// The URL to the image used for this custom action
        /// </summary>
        public string ImageUrl
        {
            get;
            set;
        }

        /// <summary>
        /// The group of this custom action
        /// </summary>
        public string Group
        {
            get;
            set;
        }

        /// <summary>
        /// The URL this custom action should navigate the user to
        /// </summary>
        public string Url
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the value that specifies the permissions needed for the custom action.
        /// </summary>
        public BasePermissions Rights
        {
            get;
            set;
        }

        /// <summary>
        /// Indicates if the custom action will need to be removed
        /// </summary>
        public bool Remove
        {
            get;
            set;
        }
    }
}