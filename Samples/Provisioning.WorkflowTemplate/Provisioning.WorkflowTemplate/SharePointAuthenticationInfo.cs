using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Provisioning.WorkflowTemplate
{
    /// <summary>
    /// Wrapper for Sharepoint specific authetication information
    /// </summary>
    public class SharePointAuthenticationInfo
    {
        /// <summary>
        /// User name for Sharepoint site
        /// </summary>
        public string userName { get; set; }
        /// <summary>
        /// User password for SharePoint site
        /// </summary>
        public string password { get; set; }
        /// <summary>
        /// Cloud or On-Premise SharePoint comptability mode
        /// </summary>
        public SharePointMode mode { get; set; }
    }

    public enum SharePointMode
    {
        Cloud,
        OnPremise
    }
}
