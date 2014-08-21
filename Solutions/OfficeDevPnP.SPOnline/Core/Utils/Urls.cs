using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfficeDevPnP.SPOnline.Core.Utils
{
    public static class Urls
    {
        [Obsolete("Use Combine in OfficeDevPnP.Core.Utilities")]
        public static string CombineUrl(Web web, string url)
        {
            Uri uri = new Uri(new Uri(web.Url), url);
            return uri.ToString();
        }
    }
}
