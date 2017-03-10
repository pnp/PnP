using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Provisioning.Modern.Console.RESTAPI
{
    public class SiteRequest
    {
        public SiteRequest()
        {
            optionalParams = new OptionalParams();
        }
        public string displayName { get; set; }
        public string alias { get; set; }
        public bool isPublic { get; set; }
        public OptionalParams optionalParams { get; set; }
    }

    class SiteResponse
    {
        public object DocumentsUrl { get; set; }
        public object ErrorMessage { get; set; }
        public string GroupId { get; set; }
        public int SiteStatus { get; set; }
        public string SiteUrl { get; set; }
    }

    public class OptionalParams
    {
        public OptionalParams()
        {
            Owners = new List<string>();
        }

        public string Description { get; set; }
        public List<string> Owners { get; set; }
    }

    public class SiteAliasResponse
    {
        public string value { get; set; }
    }
}
