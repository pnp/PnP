using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OfficeDevPnP.MSGraphAPIDemo.Models
{
    /// <summary>
    /// Defines an email message to send
    /// </summary>
    public class MailMessageToSend
    {
        /// <summary>
        /// The email message
        /// </summary>
        public MailMessageToSendContent Message { get; set; }

        public Boolean SaveToSentItems { get; set; }
    }

    /// <summary>
    /// Defines an email message
    /// </summary>
    public class MailMessageToSendContent
    {
        /// <summary>
        /// The list of email address TO recipients
        /// </summary>
        [JsonProperty("toRecipients")]
        public List<MailMessageRecipient> To { get; set; }

        /// <summary>
        /// The subject of the email message
        /// </summary>
        public String Subject { get; set; }

        /// <summary>
        /// The body of the email message
        /// </summary>
        public MailMessageBody Body { get; set; }
    }
}