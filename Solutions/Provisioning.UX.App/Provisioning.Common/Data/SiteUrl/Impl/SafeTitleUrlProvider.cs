using Provisioning.Common.Data.Templates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Provisioning.Common.Utilities;

namespace Provisioning.Common.Data.SiteUrl.Impl
{
    public class SafeTitleUrlProvider : ISiteUrlProvider
    {
        public string GenerateSiteUrl(
            SiteInformation siteRequest, 
            Template template, 
            bool avoidDuplicateUrls = false)
        {
            var url = template.HostPath;
            if(!url.EndsWith("/"))
            {
                url += "/";
            }
            url += siteRequest.Title.UrlNameFromString();

            return url;
        }
    }
}
