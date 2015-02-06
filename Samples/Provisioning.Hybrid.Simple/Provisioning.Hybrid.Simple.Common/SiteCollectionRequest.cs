using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Provisioning.Hybrid.Simple.Common
{
    /// <summary>
    /// Data object for the site collection creation details.
    /// </summary>
     [DataContract]
    public class SiteCollectionRequest
    {
        /// <summary>
        ///  Title for the new site being requested.
        /// </summary> 
         [DataMember]
        public string Title { get; set; }

        /// <summary>
        ///  Which template we should use.
        /// </summary>
        [DataMember]
        public string Template { get; set; }

        /// <summary>
        /// Which environment site collection should be created.
        /// </summary>
        [DataMember]
        public string TargetEnvironment { get; set; }

        /// <summary>
        /// Identifier for the site collection owner.
        /// </summary>
        [DataMember]
        public string OwnerIdentifier { get; set; }
    }
}
