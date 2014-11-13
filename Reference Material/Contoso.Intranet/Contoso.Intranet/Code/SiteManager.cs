using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contoso.Intranet.Code
{
    public static class SiteManager
    {
        internal static void TeamSiteWebFeatureActivatedHandler(SPWeb web, SPFeaturePropertyCollection properties)
        {
            // Create three additional libraries
            CreateNewList(web, "Meeting Notes", "Meeting notes", SPListTemplateType.DocumentLibrary);
            CreateNewList(web, "Presentations", "Presentations", SPListTemplateType.DocumentLibrary);
            CreateNewList(web, "Issues", "Issues", SPListTemplateType.IssueTracking);

            // Set master page
            web.MasterUrl = SPUrlUtility.CombineUrl(web.ServerRelativeUrl, "/_catalogs/masterpage/contoso.master");
            web.Update();
        }

        internal static void PublishingSiteWebFeatureActivatedHandler(SPWeb web, SPFeaturePropertyCollection properties)
        {
            // Create two additional libraries
            CreateNewList(web, "Presentations", "Presentations", SPListTemplateType.DocumentLibrary);
            CreateNewList(web, "Contacts", "Contacts", SPListTemplateType.Contacts);
        }

        internal static void CreateNewList(SPWeb web, string name, string desc, SPListTemplateType type)
        {
            if (web.Lists.TryGetList(name) == null)
            {
                web.Lists.Add(name, desc, type);
                web.Update();
            }
        }
    }
}
