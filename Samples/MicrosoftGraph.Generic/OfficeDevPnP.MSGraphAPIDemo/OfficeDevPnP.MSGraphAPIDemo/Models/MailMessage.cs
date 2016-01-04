using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OfficeDevPnP.MSGraphAPIDemo.Models
{
    /// <summary>
    /// Defines an email message
    /// </summary>
    public class MailMessage : BaseModel
    {
        public MailMessage()
        {
            this.Attachments = new List<MailAttachment>();
        }

        /// <summary>
        /// The importance of the email message
        /// </summary>
        [JsonProperty("displayName")]
        public MailImportance Importance { get; set; }

        /// <summary>
        /// The sender email address
        /// </summary>
        [JsonProperty("from")]
        // [JsonConverter(typeof(MailMessageFromConverter))]
        public MailMessageRecipient From { get; set; }

        /// <summary>
        /// The list of email address TO recipients
        /// </summary>
        [JsonProperty("toRecipients")]
        public List<MailMessageRecipient> To { get; set; }

        /// <summary>
        /// The list of email address CC recipients
        /// </summary>
        [JsonProperty("ccRecipients")]
        public List<MailMessageRecipient> CC { get; set; }

        /// <summary>
        /// The list of email address BCC recipients
        /// </summary>
        [JsonProperty("bccRecipients")]
        public List<MailMessageRecipient> BCC { get; set; }

        /// <summary>
        /// The subject of the email message
        /// </summary>
        public String Subject { get; set; }

        /// <summary>
        /// The body of the email message
        /// </summary>
        public MailMessageBody Body { get; set; }

        /// <summary>
        /// The UTC sent date and time of the email message
        /// </summary>
        public DateTime SentDateTime { get; set; }

        /// <summary>
        /// The UTC received date and time of the email message
        /// </summary>
        public DateTime ReceivedDateTime { get; set; }

        /// <summary>
        /// Defines whether the email message is read on unread
        /// </summary>
        public Boolean IsRead { get; set; }

        /// <summary>
        /// Defines whether the email message is a draft
        /// </summary>
        public Boolean IsDraft { get; set; }

        /// <summary>
        /// Defines whether the email has attachments
        /// </summary>
        public Boolean HasAttachments { get; set; }

        /// <summary>
        /// The list of email message attachments, if any
        /// </summary>
        public List<MailAttachment> Attachments { get; private set; }
    }

    /// <summary>
    /// Defines the importance of an email message
    /// </summary>
    public enum MailImportance
    {
        /// <summary>
        /// Normal importance, default value
        /// </summary>
        Normal,
        /// <summary>
        /// High importance
        /// </summary>
        High,
        /// <summary>
        /// Low importance
        /// </summary>
        Low,
    }
}