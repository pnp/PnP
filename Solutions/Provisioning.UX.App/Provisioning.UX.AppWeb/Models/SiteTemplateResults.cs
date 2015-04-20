using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Provisioning.UX.AppWeb.Models
{
    [DataContract]
    public class SiteTemplateResults
    {
        [DataMember(Name = "title")]
        public string Title { get; set; }

        [DataMember(Name = "description")]
        public string Description { get; set; }

        [DataMember(Name = "imageUrl")]
        public string ImageUrl { get; set; }

        [DataMember(Name = "hostPath")]
        public string HostPath { get; set; }
    }
}