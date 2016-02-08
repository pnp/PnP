using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
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
        [JsonConverter(typeof(StringEnumConverter))]
        public ItemImportance Importance { get; set; }

        /// <summary>
        /// The sender email address
        /// </summary>
        [JsonProperty("from")]
        public UserInfoContainer From { get; set; }

        /// <summary>
        /// The list of email address TO recipients
        /// </summary>
        [JsonProperty("toRecipients")]
        public List<UserInfoContainer> To { get; set; }

        /// <summary>
        /// The list of email address CC recipients
        /// </summary>
        [JsonProperty("ccRecipients")]
        public List<UserInfoContainer> CC { get; set; }

        /// <summary>
        /// The list of email address BCC recipients
        /// </summary>
        [JsonProperty("bccRecipients")]
        public List<UserInfoContainer> BCC { get; set; }

        /// <summary>
        /// The subject of the email message
        /// </summary>
        public String Subject { get; set; }

        /// <summary>
        /// The body of the email message
        /// </summary>
        public ItemBody Body { get; set; }

        /// <summary>
        /// The UTC sent date and time of the email message
        /// </summary>
        public Nullable<DateTime> SentDateTime { get; set; }

        /// <summary>
        /// The UTC received date and time of the email message
        /// </summary>
        public Nullable<DateTime> ReceivedDateTime { get; set; }

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
}