using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace SPOGraphConsumer.Models
{
    public class SitesViewModel
    {
        [DisplayName("Site URL or Id")]
        public String SiteUrlOrId { get; set; }
    }
}