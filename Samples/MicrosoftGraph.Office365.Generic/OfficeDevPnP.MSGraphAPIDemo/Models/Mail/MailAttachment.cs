using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OfficeDevPnP.MSGraphAPIDemo.Models
{
    /// <summary>
    /// Defines an email attachment
    /// </summary>
    public class MailAttachment : BaseModel
    {
        /// <summary>
        /// The content type of the email attachment
        /// </summary>
        public String ContentType { get; set; }

        /// <summary>
        /// The file name of the email attachment
        /// </summary>
        [JsonProperty("name")]
        public String FileName { get; set; }

        /// <summary>
        /// The content bytes of the email attachment
        /// </summary>
        [JsonProperty("contentBytes")]
        public Byte[] Content { get; set; }

        /// <summary>
        /// The size of the email attachment
        /// </summary>
        public Int32 Size { get; set; }

        /// <summary>
        /// The ID of the parent email message
        /// </summary>
        [JsonIgnore]
        public String ParentMessageId { get; set; }
    }
}