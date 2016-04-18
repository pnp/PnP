using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Provisioning.Common
{
    /// <summary>
    /// Domain Object for Site Users
    /// </summary>
    [DataContract]
    public class SiteUser
    {   
        /// <summary>
        /// Gets or sets the name. Can be an email address or group name 
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "name")]
        public string Name
        {
            get;
            set;
        }


        /// <summary>
        /// Gets or sets the name. Can be an email address or group name 
        /// </summary>
        [DataMember]
        [JsonProperty(PropertyName = "email")]
        public string Email
        {
            get;
            set;
        }
    }
}
