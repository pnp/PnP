using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Provisioning.Core.Configuration
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
        /// Flag to Indicate if you are running on on-prem bits or MT bits.
        /// </summary>
        public bool SharePointOnPremises { get; internal set; }

        /// <summary>
        /// The Tenant Admin Account if you want to use a Service Account
        /// </summary>
        public string TenantAdminAccount { get; internal set; }

        /// <summary>
        /// The Tenant Admin Account Password
        /// </summary>
        public string TenantAdminAccountPwd { get; internal set; }

        public string MysiteTenantAdminUrl { get; set; }
        
        /// <summary>
        /// Defines which assembly to invoke for your Site Request Repostory
        /// </summary>
        public string RepositoryManager { get; internal set; }
    }
}
