using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Contoso.Provisioning.Services.SiteManager.Services
{
    [DataContract]
    public class SiteData
    {
        [DataMember]
        public string Title { get; set; }

        [DataMember]
        public string Url { get; set;  }

        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public string WebTemplate { get; set; }

        [DataMember]
        public string LcId { get; set; }

        [DataMember]
        public string OwnerLogin { get; set; }

        [DataMember]
        public string SecondaryContactLogin { get; set; }
    }
}
