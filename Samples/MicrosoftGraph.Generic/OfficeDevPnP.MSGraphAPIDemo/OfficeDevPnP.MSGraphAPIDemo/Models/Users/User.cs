using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OfficeDevPnP.MSGraphAPIDemo.Models
{
    /// <summary>
    /// Defines a single tenant user
    /// </summary>
    public class User
    {
        public Boolean accountEnabled;
        public List<AssignedLicense> assignedLicenses;
        public List<AssignedPlan> assignedPlans;
        public List<String> businessPhones;
        public String city;
        public String companyName;
        public String country;
        public String department;
        public String displayName;
        public String givenName;
        public String jobTitle;
        public String mail;
        public String mailNickname;
        public String mobilePhone;
        public String onPremisesImmutableId;
        public Nullable<DateTimeOffset> onPremisesLastSyncDateTime;
        public String onPremisesSecurityIdentifier;
        public Nullable<Boolean> onPremisesSyncEnabled;
        public String passwordPolicies;
        public PasswordProfile passwordProfile;
        public String officeLocation;
        public String postalCode;
        public String preferredLanguage;
        public List<ProvisionedPlan> provisionedPlans;
        public List<String> proxyAddresses;
        public String state;
        public String streetAddress;
        public String surname;
        public String usageLocation;
        public String userPrincipalName;
        public String userType;
        public String aboutMe;
        public DateTimeOffset birthday;
        public DateTimeOffset hireDate;
        public List<String> interests;
        public String mySite;
        public List<String> pastProjects;
        public String preferredName;
        public List<String> responsibilities;
        public List<String> schools;
        public List<String> skills;
    }
}