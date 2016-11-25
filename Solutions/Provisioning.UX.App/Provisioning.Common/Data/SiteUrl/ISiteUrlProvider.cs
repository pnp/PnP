using Provisioning.Common.Data.Templates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Provisioning.Common.Data.SiteUrl
{
    public interface ISiteUrlProvider
    {
        string GenerateSiteUrl(SiteInformation siteRequest, Template template, bool avoidDuplicateUrls = false);
    }
}
