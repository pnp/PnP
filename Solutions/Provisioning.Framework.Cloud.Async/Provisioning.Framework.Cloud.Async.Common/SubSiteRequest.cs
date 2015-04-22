using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Provisioning.Framework.Cloud.Async.Common
{
    class SubSiteRequest
    {
        public string Url { get; set; }
        public string Template { get; set; }
        public string Description { get; set; }
        public string Title { get; set; }
        public uint Lcid { get; set; }
    }
}
