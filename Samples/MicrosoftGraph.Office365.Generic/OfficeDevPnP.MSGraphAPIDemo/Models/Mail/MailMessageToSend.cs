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
        public MailMessage Message { get; set; }

        public Boolean SaveToSentItems { get; set; }
    }
}