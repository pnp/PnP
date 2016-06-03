using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessApps.O365ProjectsApp.Infrastructure
{
    public class GroupCreationInformation
    {
        public String AccessToken { get; set; }

        public Guid JobId { get; set; }

        public String Name { get; set; }

        public String[] Members { get; set; }

        public Byte[] Photo { get; set; }
    }
}
