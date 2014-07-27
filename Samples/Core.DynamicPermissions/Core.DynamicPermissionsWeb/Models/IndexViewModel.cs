using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Contoso.Core.DynamicPermissionsWeb.Models
{
    public class IndexViewModel
    {
        public bool IsConnectedToO365 { get; set; }
        public string SiteTitle { get; set; }
    }
}