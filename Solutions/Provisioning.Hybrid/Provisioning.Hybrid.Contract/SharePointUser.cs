using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace Contoso.Provisioning.Hybrid.Contract
{
    [DataContract]
    public class SharePointUser
    {
        [DataMember]
        public string Login;
        [DataMember]
        public string Name;
        [DataMember]
        public string Email;
    }
}