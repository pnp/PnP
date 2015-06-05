using Provisioning.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Provisioning.UX.AppWeb.Models
{
    public class SiteRequestReponse
    {
        #region Public Members
        [DataMember(Name = "success")]
        public bool Success { get; set; }

        [DataMember(Name = "errorMessage")]
        public string ErrorMessage { get; set; }


        [DataMember(Name = "siteRequest")]
        public SiteRequestInformation SiteRequest { get; set; }
        #endregion
    }
}