using Framework.Provisioning.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Provisioning.Azure
{
    /// <summary>
    /// Site Request Message that is used as the Data Contract for the Site Provisioning Engine.
    /// </summary>
    public class ProvisioningRequestMessage
    {
        /// <summary>
        /// Gets or sets the address of the queue to reply to
        /// </summary>
        public string ReplyTo
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the site request payload. The incoming request is sent back to the caller.
        /// This is represents the SiteRequestInformation object as an string in XML string. You must Serialize
        /// the SiteReequestInformation object. 
        /// <see cref="Framework.Provisioning.Core.SiteRequestInformation"/>
        /// <see cref="Framework.Provisioning.Core.Utilities.XmlSerializerHelper"/>
        /// </summary>
        public string SiteRequest
        {
            get;
            set;
        }
      
    }
}
