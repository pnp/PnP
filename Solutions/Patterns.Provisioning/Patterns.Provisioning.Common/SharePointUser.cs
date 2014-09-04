using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Patterns.Provisioning.Common
{
    [DataContract]
    public class SharePointUser
    {
        [DataMember]
        public string Login
        {
            get;
            set;
        }
        [DataMember]
        public string Name
        {
            get;
            set;
        }
        [DataMember]
        public string Email
        {
            get;
            set;
        }
    }
}
