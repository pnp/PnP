using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OfficeDevPnP.MSGraphAPIDemo.Models
{
    /// <summary>
    /// Defines a list of DriveItem objects
    /// </summary>
    public class DriveItemList
    {
        /// <summary>
        /// The list of contacts
        /// </summary>
        [JsonProperty("value")]
        public List<DriveItem> DriveItems { get; set; }
    }
}