using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Provisioning.Common
{
    /// <summary>
    /// Domain Object for SharePoint Users
    /// </summary>
    [DataContract]
    public class SharePointUser
    {
        [DataMember]
        public string LoginName
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
