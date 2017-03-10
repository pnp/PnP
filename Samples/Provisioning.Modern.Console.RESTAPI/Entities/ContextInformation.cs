using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Provisioning.Modern.Console.RESTAPI
{
    public class Metadata
    {
        public string type { get; set; }
    }

    public class Metadata2 
    {
        public string type { get; set; }
    }

    public class SupportedSchemaVersions
    {
        public Metadata2 __metadata { get; set; }
        public List<string> results { get; set; }
    }

    public class GetContextWebInformation
    {
        public Metadata __metadata { get; set; }
        public int FormDigestTimeoutSeconds { get; set; }
        public string FormDigestValue { get; set; }
        public string LibraryVersion { get; set; }
        public string SiteFullUrl { get; set; }
        public SupportedSchemaVersions SupportedSchemaVersions { get; set; }
        public string WebFullUrl { get; set; }
    }

    public class D
    {
        public GetContextWebInformation GetContextWebInformation { get; set; }
    }

    public class RootObject
    {
        public D d { get; set; }
    }
}
