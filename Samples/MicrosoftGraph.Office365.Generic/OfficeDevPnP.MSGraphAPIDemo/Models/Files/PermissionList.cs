using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OfficeDevPnP.MSGraphAPIDemo.Models
{
    /// <summary>
    /// Defines a list of permissions for DriveItem objects
    /// </summary>
    public class PermissionList
    {
        /// <summary>
        /// The list of permissions for DriveItem objects
        /// </summary>
        [JsonProperty("value")]
        public List<Permission> Permissions { get; set; }
    }
}