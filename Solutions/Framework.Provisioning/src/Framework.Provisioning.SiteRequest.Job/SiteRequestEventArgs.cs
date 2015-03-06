using Framework.Provisioning.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Provisioning.SiteRequest.Job
{
    /// <summary>
    /// SiteRequest Event Class
    /// </summary>
    public class SiteRequestEventArgs : EventArgs
    {
        /// <summary>
        /// SiteRequestInformation Object
        /// </summary>
        public SiteRequestInformation SiteRequest { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="siteRequest"></param>
        public SiteRequestEventArgs(SiteRequestInformation siteRequest)
        {
            this.SiteRequest = siteRequest;
        }
    }
}
