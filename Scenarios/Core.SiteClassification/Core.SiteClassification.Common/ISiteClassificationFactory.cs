using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.SiteClassification.Common
{
    public interface ISiteClassificationFactory
    {
        ISiteClassificationManager GetManager(ClientContext ctx);
    }
}
