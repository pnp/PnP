using Provisioning.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Provisioning.UX.AppWeb.Models
{
    [DataContract]
    public class SiteRequestsResponse
    {
        [DataMember(Name = "requests")]
        public ICollection<SiteInformation> SiteRequests
        {
            get;
            set;
        }

        #region Public Members
        [DataMember(Name = "success")]
        public bool Success { get; set; }

        [DataMember(Name = "errorMessage")]
        public string ErrorMessage { get; set; }
        #endregion

    }
}