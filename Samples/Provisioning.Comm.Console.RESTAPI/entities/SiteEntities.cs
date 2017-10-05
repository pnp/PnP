using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Provisioning.Comm.Console.RESTAPI
{
    public class RootRequest
    {
        public SiteRequest request = new SiteRequest();
    }

    public class RequestMetadata
    {
        public string type = "SP.Publishing.CommunicationSiteCreationRequest";
    }

    public class SiteRequest
    {
        public RequestMetadata __metadata = new RequestMetadata();
        public string Title { get; set; }
        public string Url { get; set; }
        public string Description { get; set; }
        public string Classification { get; set; }
        public bool AllowFileSharingForGuestUsers { get; set; }
        public string SiteDesignId { get; set; }
        public int lcid { get; set; }
    }

    public class SiteResponse
    {
        public int SiteStatus { get; set; }
        public string SiteUrl { get; set; }
    }

    public class SiteAliasResponse
    {
        public string value { get; set; }
    }

}
