using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfficeAMS.Core.Entities
{
    /// <summary>
    /// Properties of a site policy object
    /// </summary>
    public class SitePolicyEntity
    {
        /// <summary>
        /// The description of the policy
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// The body of the notification email if there is no site mailbox associated with the site. 
        /// </summary>
        public string EmailBody { get; set; }
        /// <summary>
        /// The body of the notification email if there is a site mailbox associated with the site.
        /// </summary>
        public string EmailBodyWithTeamMailbox { get; set; }
        /// <summary>
        /// The subject of the notification email. 
        /// </summary>
        public string EmailSubject { get; set; }
        /// <summary>
        /// The name of the policy
        /// </summary>
        public string Name { get; set; }
    }
}
