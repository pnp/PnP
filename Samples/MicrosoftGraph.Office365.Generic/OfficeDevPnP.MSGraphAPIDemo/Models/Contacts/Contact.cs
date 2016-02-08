using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OfficeDevPnP.MSGraphAPIDemo.Models
{
    /// <summary>
    /// Defines a user's contact
    /// </summary>
    public class Contact : BaseModel
    {
        /// <summary>
        /// The business address of the contact
        /// </summary>
        public PhysicalAddress BusinessAddress { get; set; }

        /// <summary>
        /// The business phones of the contact
        /// </summary>
        public List<String> BusinessPhones { get; set; }

        /// <summary>
        /// The business home page of the contact
        /// </summary>
        public String BusinessHomePage { get; set; }

        /// <summary>
        /// The company name of the contact
        /// </summary>
        public String CompanyName { get; set; }

        /// <summary>
        /// The department name of the contact
        /// </summary>
        public String Department { get; set; }

        /// <summary>
        /// The display name of the contact
        /// </summary>
        public String DisplayName { get; set; }

        /// <summary>
        /// The list of email addresses of the contact
        /// </summary>
        public List<UserInfo> EmailAddresses { get; set; }

        /// <summary>
        /// The "File As" of the contact
        /// </summary>
        public String FileAs { get; set; }

        /// <summary>
        /// The home address of the contact
        /// </summary>
        public PhysicalAddress HomeAddress { get; set; }

        /// <summary>
        /// The home phones of the contact
        /// </summary>
        public List<String> HomePhones { get; set; }

        /// <summary>
        /// The office location of the contact
        /// </summary>
        public String OfficeLocation { get; set; }

        /// <summary>
        /// The other address of the contact
        /// </summary>
        public PhysicalAddress OtherAddress { get; set; }

        /// <summary>
        /// Personal notes about the contact
        /// </summary>
        public String PersonalNotes { get; set; }

        /// <summary>
        /// The first name of the contact
        /// </summary>
        public String GivenName { get; set; }

        /// <summary>
        /// The family name of the contact
        /// </summary>
        public String Surname { get; set; }

        /// <summary>
        /// The title of the contact
        /// </summary>
        public String Title { get; set; }
    }
}