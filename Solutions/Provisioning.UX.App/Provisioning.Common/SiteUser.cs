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
        //[DataMember]
        //[JsonProperty(PropertyName = "loginName")]
        //public string LoginName
        //{
        //    get;
        //    set;
        //}
        [DataMember]
        [JsonProperty(PropertyName = "name")]
        public string Name
        {
            get;
            set;
        }
        //[DataMember]
        //[JsonProperty(PropertyName = "email")]
        //public string Email
        //{
        //    get;
        //    set;
        //}
    }
}
