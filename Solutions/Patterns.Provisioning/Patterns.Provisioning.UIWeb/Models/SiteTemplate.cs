using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Patterns.Provisioning.UIWeb {
    public class SiteTemplate {
        public string Title { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public string ImageUrl { get; set; }
        public uint LCID { get; set; }

        public string TemplateId { get; set; }
    }
}