using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Provisioning.Azure
{
    /// <summary>
    /// Response Message to send to Custom Application Incoming Queue that represents the response from the 
    /// site provisioning engine.
    /// </summary>
    public class ProvisioningResponseMessage
    {   
        /// <summary>
        /// Gets or sets if the site request has errored
        /// </summary>
        public bool IsFaulted
        { get; set; }

        /// <summary>
        /// Get or sets the Fault Message
        /// </summary>
        public string FaultMessage
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
