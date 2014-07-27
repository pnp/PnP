using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Contoso.Core.OfficeWebWidgetsWeb
{
    [DataContract]
    public class SharePointUser
    {
        [DataMember]
        public string department;
        [DataMember]
        public string displayName;
        [DataMember]
        public string email;
        [DataMember]
        public bool isResolved;
        [DataMember]
        public string jobTitle;
        [DataMember]
        public string loginName;
        [DataMember]
        public string mobile;
        [DataMember]
        public string principalId;
        [DataMember]
        public string principalType;
        [DataMember]
        public string sipAddress;
        [DataMember]
        public string text;
    }
}