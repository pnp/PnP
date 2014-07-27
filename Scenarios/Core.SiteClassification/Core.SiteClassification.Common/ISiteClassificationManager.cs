using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.SiteClassification.Common
{
    /// <summary>
    /// Interface for working with SiteClassification Manager
    /// </summary>
    public interface ISiteClassificationManager
    {
        SiteProfile GetSiteProfile(ClientContext ctx);
        void SaveSiteProperties(ClientContext ctx, SiteProfile profile);
    }
}
