using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Provisioning.Cloud.Async.WebJob.Common
{
    /// <summary>
    ///  Actual request data object
    /// </summary>
    public class SiteRequestData
    {
        public string Url { get; set; }
        public string Owner { get; set; }
        public string Title { get; set; }
        public string Template { get; set; }
        public int TimeZoneId { get; set; }
        public uint Lcid { get; set; }
        public long StorageMaximumLevel { get; set; }
    }
}