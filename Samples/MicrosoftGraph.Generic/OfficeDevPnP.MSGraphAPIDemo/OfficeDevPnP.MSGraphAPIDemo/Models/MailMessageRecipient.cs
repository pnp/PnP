using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OfficeDevPnP.MSGraphAPIDemo.Models
{
    /// <summary>
    /// Defines a recipient of an email message
    /// </summary>
    public class MailMessageRecipient
    {
        /// <summary>
        /// The email address of the recipient
        /// </summary>
        [JsonProperty("emailAddress")]
        public UserInfo Recipient { get; set; }
    }
}