using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessApps.HelpDesk.Models.Email
{
    public class EmailMessage
    {
        public string Subject { get; set; }
        public string Body { get; set; }
        public string Sender { get; set; }
        public string SentTimestamp { get; set; }
        public string MessageID { get; set; }
    }
}
