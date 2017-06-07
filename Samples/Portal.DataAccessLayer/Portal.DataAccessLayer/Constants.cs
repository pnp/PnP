using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portal.DataAccessLayer
{
    /// <summary>
    /// This class holds all constants used by the program.  No code
    /// </summary>
    public class Constants
    {
        public static readonly string MASTERPAGE_CONTENT_TYPE = "0x01010500B45822D4B60B7B40A2BFCC0995839404";

        public static readonly string PropertyBagInheritsMaster = "__InheritsMasterUrl";
        public static readonly string PropertyBagInheritsCustomMaster = "__InheritsCustomMasterUrl";

        public static readonly string CdnWebRelativeUrl = "/style%20library/PnP";
        public static readonly string CdnConfigurationFileName = "configuration.js";

        public static readonly string PnPSiteColumns_GroupName = "PnP Site Columns";

        // *****************************************************************************************************************
        // NOTE: If you add/edit a Site Column here, be sure that its InternalName stays in sync with the auto-generated
        //       managed property names defined in the configuration.js CDN resource file.
        //       example: "PnPPortalConfigKey" <===> "PnPPortalConfigKeyOWSTEXT"
        // *****************************************************************************************************************

        public static readonly string PnPConfigKey_InternalName = "PnPPortalConfigKey";
        public static readonly string PnPConfigKey_DisplayName = "Config Key";
        public static readonly string PnPConfigKey_GUID = "{6BCF3C60-3DEE-4958-B5AD-2FEE50D74306}";

        public static readonly string PnPConfigValue_InternalName = "PnPPortalConfigValue";
        public static readonly string PnPConfigValue_DisplayName = "Config Value";
        public static readonly string PnPConfigValue_GUID = "{1DBD9D30-115B-4220-8731-A9B950EA19B0}";

        public static readonly string PnPLinkText_InternalName = "PnPPortalLinkText";
        public static readonly string PnPLinkText_DisplayName = "Link Text";
        public static readonly string PnPLinkText_GUID = "{819119DA-D689-4A24-8304-BF9E7E7F03CE}";

        public static readonly string PnPLinkUrl_InternalName = "PnPPortalLinkUrl";
        public static readonly string PnPLinkUrl_DisplayName = "Link Url";
        public static readonly string PnPLinkUrl_GUID = "{32068F58-9EA3-4D21-8F3B-04B232416181}";

        public static readonly string PnPDisplayOrder_InternalName = "PnPPortalDisplayOrder";
        public static readonly string PnPDisplayOrder_DisplayName = "Display Order";
        public static readonly string PnPDisplayOrder_GUID = "{36C21078-85CB-4A46-B640-31EC08208BE7}";


        public static readonly string PortalMasterPageFileName = "Portal.DataAccessLayer.master";
        public static readonly string PortalMasterPageTitle = "PnP Portal DataAccessLayer Master Page";
        public static readonly string PortalMasterPageDescription = "Demonstrates the PnP Portal DataAccessLayer";

        public static readonly string DalDemoWebTitle = "DAL Demo";
        public static readonly string DalDemoWebDescription = "PnP Portal Data Access Layer Demo";
        public static readonly string DalDemoWebLeafUrl = "dal";

        // Reminder: Web-Relative Urls and Leaf Urls should not contain a leading slash '/'
        public static readonly string ConfigurationListTitle = "Portal Config";
        public static readonly string ConfigurationListLeafUrl = "PortalConfig";
        public static readonly string ConfigurationListWebRelativeUrl = "Lists/" + ConfigurationListLeafUrl;
        public static readonly string ConfigurationListFooterKey = "FooterHtml";

        public static readonly string GlobalNavListTitle = "Global Nav Config";
        public static readonly string GlobalNavListLeafUrl = "GlobalNavConfig";
        public static readonly string GlobalNavListWebRelativeUrl = "Lists/" + GlobalNavListLeafUrl;

        public static readonly string CompanyLinksListTitle = "Company Links Config";
        public static readonly string CompanyLinksListLeafUrl = "CompanyLinksConfig";
        public static readonly string CompanyLinksListWebRelativeUrl = "Lists/" + CompanyLinksListLeafUrl;

        public static readonly string LocalNavListTitle = "Local Nav Config";
        public static readonly string LocalNavListLeafUrl = "LocalNavConfig";
        public static readonly string LocalNavListWebRelativeUrl = "Lists/" + LocalNavListLeafUrl;

        public static readonly string ListViewQueryFormatString = "<OrderBy><FieldRef Name=\"{0}\" /></OrderBy>";

        public static readonly string[] CompanyLinkTitles = new string[] {
            "Portal Guidance Overview",
            "Portal Performance" ,
            "Portal Info Arch",
            "Portal Navigation",
            "Portal Data Aggregation",
            "Portal Branding",
            "Portal Go-Live"
        };
        public static readonly string[] CompanyLinkUrls = new string[] {
            "https://msdn.microsoft.com/en-us/pnp_articles/portal-overview",
            "https://msdn.microsoft.com/en-us/pnp_articles/portal-performance" ,
            "https://msdn.microsoft.com/en-us/pnp_articles/portal-information-architecture",
            "https://msdn.microsoft.com/en-us/pnp_articles/portal-navigation",
            "https://msdn.microsoft.com/en-us/pnp_articles/portal-data-aggregation",
            "https://msdn.microsoft.com/en-us/pnp_articles/portal-branding",
            "https://msdn.microsoft.com/en-us/pnp_articles/portal-rollout"
        };

        public static readonly string GlobalNavLinkUrl = "https://msdn.microsoft.com/en-us/pnp_articles/portal-overview";
        public static readonly string LocalNavLinkUrl = "https://github.com/SharePoint/PnP/tree/master/Samples/Portal.DataAccessLayer";
        public static readonly string FooterNavLinkUrl = "https://msdn.microsoft.com/en-us/pnp_articles/portal-performance";

        // the name of the local project file containing the Welcome Page Content
        public static readonly string WelcomePageContentFileName = "WelcomePageContent.html";
        // the name of the welcome page in the Pages library
        public static readonly string WelcomePageName = "welcome.aspx";
    }
}
