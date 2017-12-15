using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Provisioning.Cloud.Modern.Async.Infrastructure
{
    public class ModernSiteCreation
    {
        public String CurrentUserPrincipalName { get; set; }

        public SiteType SiteType { get; set; }

        public String SiteTitle { get; set; }

        public String SiteAlias { get; set; }

        public String SiteDescription { get; set; }

        public Boolean IsPublic { get; set; }

        public String PnPTemplate { get; set; }

        public String UserAccessToken { get; set; }

        public String SPORootSiteUrl { get; set; }
    }

    /// <summary>
    /// Defines the available "modern" site options
    /// </summary>
    public enum SiteType
    {
        /// <summary>
        /// "modern" team site
        /// </summary>
        TeamSite,
        /// <summary>
        /// "modern" communication site
        /// </summary>
        CommunicationSite,
    }
}
