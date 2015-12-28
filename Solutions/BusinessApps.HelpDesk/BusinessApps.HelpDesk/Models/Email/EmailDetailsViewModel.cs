using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessApps.HelpDesk.Helpers;

namespace BusinessApps.HelpDesk.Models.Email
{
    public class EmailDetailsViewModel
    {
        public EmailMessage EmailMessage { get; set; }
        public IEnumerable<HelpdeskOperator> HelpdeskOperators { get; set; }
    }
}
