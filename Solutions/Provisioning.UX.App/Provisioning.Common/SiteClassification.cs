using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Provisioning.Common
{
    /// <summary>
    /// Domain Model for working with Site Classification
    /// </summary>
    public class SiteClassification
    {
        /// <summary>
        /// Gets or Sets the Key of the Policy
        /// </summary>
        [JsonProperty(PropertyName = "id")]
        public int Id { get; internal set; }

        /// <summary>
        /// Gets or Sets the Key of the Policy
        /// </summary>
        [JsonProperty(PropertyName = "key")]
        public string Key { get; set; }
        
        /// <summary>
        /// Gets or sets the Value of the SiteClassification
        /// </summary>
        [JsonProperty(PropertyName = "value")]
        public string Value { get; set; }

        /// <summary>
        /// Gets or sets the DisplayOrder for the User interface
        /// </summary>
        [JsonProperty(PropertyName = "displayOrder")]
        public int DisplayOrder { get; set; }

        /// <summary>
        /// Gets or sets the Expiration in Months
        /// </summary>
        [JsonProperty(PropertyName = "expirateMonths")]
        public int ExpirationMonths { get; set; }

        /// <summary>
        /// Gets or sets if AllAuthenticated users should be added to the site during provisioning
        /// </summary>
        [JsonProperty(PropertyName = "addAllAuthenticatedUsers")]
        public bool AddAllAuthenticatedUsers { get; set; }

        /// <summary>
        /// Gets or sets if the classification entry is enabled or disabled
        /// </summary>
        [JsonProperty(PropertyName = "enabled")]
        public bool Enabled { get; set; }
    }
}
