using Contoso.Provisioning.Hybrid.Contract;
using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contoso.Provisioning.Hybrid.Core.SiteTemplates
{
    public interface ISiteProvisioningOnPremises
    {
        void CreateSiteCollectionOnPremises(SharePointProvisioningData SharePointProvisioningData);
        string GetNextSiteCollectionUrl(ClientContext cc, Web web, string siteDirectoryUrl, string siteDirectoryListName, string baseSiteUrl);
        //void UpdateSiteDirectoryStatus(ClientContext cc, Web web, string siteDirectoryHost, string listName, string siteUrl, string newSiteUrl, string status, Exception ex);
        ClientContext SpOnPremiseAuthentication(string siteUrl);
    }
}
