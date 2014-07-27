using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfficeAMS.Core.Entities
{
    /// <summary>
    /// SiteEntity class describes the information for a SharePoint site (collection)
    /// </summary>
    public class SiteEntity
    {
        /// <summary>
        /// The SPO url
        /// </summary>
        public string Url
        {
            get;
            set;
        }

        /// <summary>
        /// The site title
        /// </summary>
        public string Title
        {
            get;
            set;
        }

        /// <summary>
        /// The site description
        /// </summary>
        public string Description
        {
            get;
            set;
        }

        public String SiteOwnerLogin
        {
            get;
            set;
        }

        /// <summary>
        /// The site locale. See http://technet.microsoft.com/en-us/library/ff463597.aspx for a complete list of Lcid's
        /// </summary>
        public uint Lcid
        {
            get;
            set;
        }

        /// <summary>
        /// Site quota in MB
        /// </summary>
        public long StorageMaximumLevel
        {
            get;
            set;
        }

        /// <summary>
        /// Site quota warning level in MB
        /// </summary>
        public long StorageWarningLevel
        {
            get;
            set;
        }

        /// <summary>
        /// Site template being used
        /// </summary>
        public string Template
        {
            get;
            set;
        }

        /// <summary>
        /// TimeZoneID for the site. "(UTC+01:00) Brussels, Copenhagen, Madrid, Paris" = 3 
        /// See http://blog.jussipalo.com/2013/10/list-of-sharepoint-timezoneid-values.html for a complete list
        /// </summary>
        public int TimeZoneId
        {
            get;
            set;
        }

        /// <summary>
        /// The user code quota in points
        /// </summary>
        public double UserCodeMaximumLevel
        {
            get;
            set;
        }

        /// <summary>
        /// The user code quota warning level in points
        /// </summary>
        public double UserCodeWarningLevel
        {
            get;
            set;
        }
    }
}
