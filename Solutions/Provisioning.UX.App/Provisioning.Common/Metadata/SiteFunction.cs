using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Provisioning.Common.Metadata
{
    public class SiteMetadata
    {
        public int Id { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
        public bool Enabled { get; set; }
        public int DisplayOrder { get; set; }
    }
}
