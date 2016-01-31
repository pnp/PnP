using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Office365Api.Graph.Simple.MailAndFiles.Models
{
    public class EmailMessage
    {
        public string Subject { get; set; }
        public string Body { get; set; }
        public string Sender { get; set; }
        public DateTime SentTimestamp { get; set; }
        public string SentTimestampString { get; set; }
        public string MessageID { get; set; }
    }
}
