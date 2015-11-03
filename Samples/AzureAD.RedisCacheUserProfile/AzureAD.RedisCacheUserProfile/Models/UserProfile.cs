using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AzureAD.RedisCacheUserProfile.Models
{
    [Serializable]
    public class UserProfile
    {
        public string odatametadata { get; set; }
        public string odatatype { get; set; }
        public string objectType { get; set; }
        public string objectId { get; set; }
        public object deletionTimestamp { get; set; }
        public bool accountEnabled { get; set; }
        public object[] assignedLicenses { get; set; }
        public object[] assignedPlans { get; set; }
        public object city { get; set; }
        public object companyName { get; set; }
        public object country { get; set; }
        public object creationType { get; set; }
        public object department { get; set; }
        public object dirSyncEnabled { get; set; }
        public string displayName { get; set; }
        public object facsimileTelephoneNumber { get; set; }
        public string givenName { get; set; }
        public object immutableId { get; set; }
        public object jobTitle { get; set; }
        public object lastDirSyncTime { get; set; }
        public object mail { get; set; }
        public string mailNickname { get; set; }
        public object mobile { get; set; }
        public object onPremisesSecurityIdentifier { get; set; }
        public object[] otherMails { get; set; }
        public string passwordPolicies { get; set; }
        public object passwordProfile { get; set; }
        public object physicalDeliveryOfficeName { get; set; }
        public object postalCode { get; set; }
        public object preferredLanguage { get; set; }
        public object[] provisionedPlans { get; set; }
        public object[] provisioningErrors { get; set; }
        public object[] proxyAddresses { get; set; }
        public object sipProxyAddress { get; set; }
        public object state { get; set; }
        public object streetAddress { get; set; }
        public string surname { get; set; }
        public object telephoneNumber { get; set; }
        public object usageLocation { get; set; }
        public string userPrincipalName { get; set; }
        public string userType { get; set; }
        public string extension_33e037a7b1aa42ab96936c22d01ca338_Compania { get; set; }
    }
}