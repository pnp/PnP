using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OfficeDevPnP.MSGraphAPIDemo.Models
{
    /// <summary>
    /// Defines an email Folder
    /// </summary>
    public class MailFolder : BaseModel
    {
        /// <summary>
        /// The display name of the email folder
        /// </summary>
        [JsonProperty("displayName")]
        public String Name { get; set; }

        /// <summary>
        /// Total number of items
        /// </summary>
        public Int32 TotalItemCount { get; set; }

        /// <summary>
        /// Number of unread items
        /// </summary>
        public Int32 UnreadItemCount { get; set; }
    }
}