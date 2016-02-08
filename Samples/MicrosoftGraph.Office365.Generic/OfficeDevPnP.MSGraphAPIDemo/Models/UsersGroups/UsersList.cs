using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OfficeDevPnP.MSGraphAPIDemo.Models
{
    /// <summary>
    /// Defines a list of users
    /// </summary>
    public class UsersList
    {
        /// <summary>
        /// The list of users
        /// </summary>
        [JsonProperty("value")]
        public List<User> Users { get; set; }
    }
}