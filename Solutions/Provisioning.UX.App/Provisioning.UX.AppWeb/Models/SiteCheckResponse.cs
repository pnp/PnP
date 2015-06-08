using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Provisioning.UX.AppWeb.Models
{
    [DataContract]
    public class SiteCheckResponse
    {
        [DataMember(Name = "success")]
        public bool Success { get; set; }

        [DataMember(Name = "errorMessage")]
        public string ErrorMessage { get; set; }

        [DataMember(Name = "doesExist")]
        public bool DoesExist { get; set; }
    }
}