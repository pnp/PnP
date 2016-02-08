using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OfficeDevPnP.MSGraphAPIDemo.Models
{
    /// <summary>
    /// Defines a single tenant user
    /// </summary>
    public class User : BaseModel
    {
        /// <summary>
        /// Defines whether the user's account is enabled or not
        /// </summary>
        public Boolean AccountEnabled;

        /// <summary>
        /// List of licenses assigned to the user
        /// </summary>
        public List<AssignedLicense> AssignedLicenses;

        /// <summary>
        /// List of Office 365 plans assigned to the user
        /// </summary>
        public List<AssignedPlan> AssignedPlans;

        /// <summary>
        /// List of user's business phones
        /// </summary>
        public List<String> BusinessPhones;

        /// <summary>
        /// City of the user
        /// </summary>
        public String City;

        /// <summary>
        /// Company of the user
        /// </summary>
        public String CompanyName;

        /// <summary>
        /// Country of the user
        /// </summary>
        public String Country;

        /// <summary>
        /// Department of the user
        /// </summary>
        public String Department;

        /// <summary>
        /// Display Name of the user
        /// </summary>
        public String DisplayName;

        /// <summary>
        /// Given name of the user
        /// </summary>
        public String GivenName;

        /// <summary>
        /// Job title of the user
        /// </summary>
        public String JobTitle;

        /// <summary>
        /// Mail of the user
        /// </summary>
        public String Mail;

        /// <summary>
        /// Nickname of the user
        /// </summary>
        public String MailNickname;

        /// <summary>
        /// Mobile phone of the user
        /// </summary>
        public String MobilePhone;

        /// <summary>
        /// Unique ID of the user, from the on-premises perspective (if any)
        /// </summary>
        public String OnPremisesImmutableId;

        /// <summary>
        /// Last date and time of sync with on-premises (if any)
        /// </summary>
        public Nullable<DateTimeOffset> OnPremisesLastSyncDateTime;

        /// <summary>
        /// Security Identifier (SID) of the on-premises user (if any)
        /// </summary>
        public String OnPremisesSecurityIdentifier;

        /// <summary>
        /// Defines whether the synchronization with on-premises is enabled or not
        /// </summary>
        public Nullable<Boolean> OnPremisesSyncEnabled;

        /// <summary>
        /// Password policies
        /// </summary>
        public String PasswordPolicies;

        /// <summary>
        /// Password profile
        /// </summary>
        public PasswordProfile PasswordProfile;

        /// <summary>
        /// Office location for the user
        /// </summary>
        public String OfficeLocation;

        /// <summary>
        /// Postal code of the user
        /// </summary>
        public String PostalCode;

        /// <summary>
        /// Preferred language for the user
        /// </summary>
        public String PreferredLanguage;

        /// <summary>
        /// List of Office 365 plans provisioned for the user
        /// </summary>
        public List<ProvisionedPlan> ProvisionedPlans;

        /// <summary>
        /// List of proxy addresses for the user
        /// </summary>
        public List<String> ProxyAddresses;

        /// <summary>
        /// State of the user
        /// </summary>
        public String State;

        /// <summary>
        /// Street address of the user
        /// </summary>
        public String StreetAddress;

        /// <summary>
        /// Lastname of the user
        /// </summary>
        public String Surname;

        /// <summary>
        /// Usage location of the user
        /// </summary>
        public String UsageLocation;

        /// <summary>
        /// UPN for the user
        /// </summary>
        public String UserPrincipalName;

        /// <summary>
        /// Type of user
        /// </summary>
        public String UserType;

        /// <summary>
        /// About me sentence from the user
        /// </summary>
        public String AboutMe;

        /// <summary>
        /// Birthdate of the user
        /// </summary>
        public Nullable<DateTimeOffset> Birthday;

        /// <summary>
        /// Hire date for the user
        /// </summary>
        public Nullable<DateTimeOffset> HireDate;

        /// <summary>
        /// List of interests for the user
        /// </summary>
        public List<String> Interests;

        /// <summary>
        /// URL of the user's MySite
        /// </summary>
        public String MySite;

        /// <summary>
        /// List of past projects for the user
        /// </summary>
        public List<String> PastProjects;

        /// <summary>
        /// The user's preferred name
        /// </summary>
        public String PreferredName;

        /// <summary>
        /// List of user's responsibilities
        /// </summary>
        public List<String> Responsibilities;

        /// <summary>
        /// List of user's schools
        /// </summary>
        public List<String> Schools;

        /// <summary>
        /// List of user's skills
        /// </summary>
        public List<String> Skills;
    }
}