using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Provisioning.Common.Configuration
{
    /// <summary>
    /// Domain Model for Application Settings
    /// </summary>
    public class AppSettings
    {
        /// <summary>
        /// The Tenant Administration Site.
        /// </summary>
        public string TenantAdminUrl { get; internal set; }
        /// <summary>
        /// SharePoint Site that is hosting the Application
        /// </summary>
        public string SPHostUrl { get; internal set; }
        /// <summary>
        /// The Client ID
        /// </summary>
        public string ClientID { get; internal set; }
        /// <summary>
        /// The Client Secret
        /// </summary>
        public string ClientSecret { get; internal set; }      
        /// <summary>
        /// Support Team Email used for notifications
        /// </summary>
        public string SupportEmailNotification { get; internal set; }
        /// <summary>
        /// Configuration option to Auto Approve Site Requests. If you use workflows to approve site creation this
        /// should be set to false
        /// </summary>
        public bool AutoApprove { get; internal set; }

        /// <summary>
        /// TODO
        /// </summary>
        public string TenantAdminAccount { get; internal set; }

        /// <summary>
        /// TODO - SHOULD BE ENCRYPTED
        /// </summary>
        public string TenantAdminAccountPwd { get; internal set; }

        public string MysiteTenantAdminUrl { get; set; }
        /// <summary>
        /// TODO
        /// </summary>
        public string RepositoryManager { get; internal set; }
    }
}
