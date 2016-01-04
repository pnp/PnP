using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OfficeDevPnP.MSGraphAPIDemo.Models
{
    public class MailMessageRecipient
    {
        [JsonProperty("emailAddress")]
        public EmailAddress Recipient { get; set; }
    }
}