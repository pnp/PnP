using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Provisioning.UX.AppWeb.Models
{
    [DataContract]
    public class SiteTemplates
    {
        private List<SiteTemplate> _siteTemplates = new List<SiteTemplate>();

        [DataMember(Name="templates")]
        public List<SiteTemplate> Templates
        {
            get { return _siteTemplates; }
            internal set
            {
                _siteTemplates = value;
            }
        }
        
    }
}