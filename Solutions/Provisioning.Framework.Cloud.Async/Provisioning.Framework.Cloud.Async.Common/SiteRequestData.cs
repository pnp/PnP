using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Provisioning.Framework.Cloud.Async.Common
{
    /// <summary>
    ///  Actual request data object for the site request
    /// </summary>
    public class SiteRequestData
    {
        public string Url { get; set; }
        public string Owner { get; set; }
        public string Title { get; set; }
        public SiteProvisioningType ProvisioningType { get; set; }
        public string TemplateId { get; set; }
        public string TemplateSiteUrl { get; set; }
        public int TimeZoneId { get; set; }
        public uint Lcid { get; set; }
        public long StorageMaximumLevel { get; set; }
    }
}
