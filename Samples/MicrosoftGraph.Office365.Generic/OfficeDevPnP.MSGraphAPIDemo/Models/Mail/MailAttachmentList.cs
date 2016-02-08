using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OfficeDevPnP.MSGraphAPIDemo.Models
{
    /// <summary>
    /// Defines a list of email message attachments
    /// </summary>
    public class MailAttachmentList
    {
        /// <summary>
        /// The list of email message attachments
        /// </summary>
        [JsonProperty("value")]
        public List<MailAttachment> Attachments { get; set; }
    }
}