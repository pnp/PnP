'use strict'
// include utility.js

var ns = CreateNamespace('PortalDataAccessLayer');

/*
    Contains configuration values used across all DAL JS files

    This file should be uploaded into the JS folder of the CDN
    You may need to set some vars to work within the context of the given web app
*/
ns.Configuration = function () {

}

//Web App ABSOLUTE Url (do not include a trailing slash '/')
ns.Configuration.PortalWebAppAbsoluteUrl = 'https://contoso.sharepoint.com';

// Returns the Absolute Url (e.g., https://contoso.sharepoint.com) of the current web application
// This Absolute URL does not include a trailing slash '/'
ns.Configuration.GetWebAppAbsoluteUrl = function () {
    var urlArray = "";

    try {
        var currentSiteUrl = window.location.href.toLowerCase();
        urlArray = currentSiteUrl.split("://");
        var domain = urlArray[1].substr(0, urlArray[1].indexOf('/'));
        return urlArray[0] + "://" + domain;
    }
    catch (ex) {
        ns.LogError(ex.message);
    }

    return ns.Configuration.PortalWebAppAbsoluteUrl;
}

//PnP Portal Admin Site Collection ABSOLUTE Url (do not include a trailing slash '/')
ns.Configuration.PortalAdminSiteAbsoluteUrl = 'https://contoso.sharepoint.com/sites/admin';

// Url of PnP Portal CDN root folder (do not include a trailing slash '/')
// - NOTE: Use an ABSOLUTE Url if the CDN is external to SharePoint; 
//         Otherwise, use a SERVER-RELATIVE Url (e.g., '/sites/<site>/<cdnFolder>' or '/<cdnFolder>')
// - Upload all JS files into a subfolder named "js"
// - Upload all Image files into a subfolder named "images"
ns.Configuration.PortalCdnUrl = "/style%20library/pnp";

//=========================================================================
// In order to support the Stock Ticker control:
// Specify a _single_ stock symbol to be used by the stock ticker control
//=========================================================================
ns.Configuration.StockTickerSymbol = "MSFT";

//=========================================================================
// In order to support the various configuration lists:
//
// Create the following Site Columns in the Admin and Demo Site Collections
//  - PnPPortalConfigKey (text)
//  - PnPPortalConfigValue (multi-line text; Plain-Text)
//  - PnPPortalLinkText (text)
//  - PnPPortalLinkUrl (text)
//  - PnPPortalDisplayOrder (number, min:1, max:999, zero decimal places)
//
// Create the configuration lists
// - add a sample row to each list
// - force a re-index on each list
// - wait 30 mins for incremental crawl to execute
// - Go to SharePoint Admin Center : Search and verify the following Managed Properties exist in the Search Schema:
//   - PnPPortalConfigKeyOWSTXT
//   - PnPPortalConfigValueOWSMTXT
//   - PnPPortalLinkTextOWSTXT
//   - PnPPortalLinkUrlOWSTXT
// - choose ONE of the RefinableInt[00-99] Managed Properties and configure as follows:
//   - Alias: PnPPortalDisplayOrder
//   - Select: Include Content from all crawled properties
//     - ADD: ows_PnPPortalDisplayOrder
//     - ADD: ows_q_NMBR_PnPPortalDisplayOrder
// - force a re-index on each list
// - wait 30 mins for incremental crawl to execute
//=========================================================================

ns.Configuration.ManagedProp_PnPConfigKey = "PnPPortalConfigKeyOWSTEXT";        // auto-generated MP
ns.Configuration.ManagedProp_PnPConfigValue = "PnPPortalConfigValueOWSMTXT";    // auto-generated MP
ns.Configuration.ManagedProp_PnPLinkText = "PnPPortalLinkTextOWSTEXT";          // auto-generated MP
ns.Configuration.ManagedProp_PnPLinkUrl = "PnPPortalLinkUrlOWSTEXT";            // auto-generated MP 
ns.Configuration.ManagedProp_PnPDisplayOrder = "PnPPortalDisplayOrder";         // alias

//Portal Config List WEB-RELATIVE Url (do not include a leading OR trailing slash '/')
// - list should reside at the root web of the Portal Admin Site Collection (SPSite) instance 
// - list should contain the following Site Columns: 
//   - PnPPortalConfigKey
//   - PnPPortalConfigValue
ns.Configuration.ConfigurationListWebRelativeUrl = "Lists/PortalConfig";
// The list should contain an entry with the following PnPConfigKey
ns.Configuration.ConfigurationListFooterKey = "FooterHtml";

//Global Nav Config List WEB-RELATIVE Url (do not include a leading OR trailing slash '/')
// - list should reside at the root web of the Portal Admin Site Collection (SPSite) instance 
// - list should contain the following Site Columns: 
//   - PnPPortalLinkText
//   - PnPPortalLinkUrl
//   - PnPPortalDisplayOrder
ns.Configuration.GlobalNavListWebRelativeUrl = "Lists/GlobalNavConfig";

//Company Links Config List WEB-RELATIVE Url (do not include a leading OR trailing slash '/')
// - list should reside at the root web of each Site Collection (SPSite) instance that leverages Company Links 
// - list should contain the following Site Columns: 
//   - PnPPortalLinkText
//   - PnPPortalLinkUrl
//   - PnPPortalDisplayOrder
ns.Configuration.CompanyLinksListWebRelativeUrl = "Lists/CompanyLinksConfig";

//Local Nav Config List WEB-RELATIVE Url (do not include a leading OR trailing slash '/')
// - list should reside at the root of each website (SPWeb) instance that leverages Local Nav 
// - list should contain the following Site Columns: 
//   - PnPPortalLinkText
//   - PnPPortalLinkUrl
//   - PnPPortalDisplayOrder
ns.Configuration.LocalNavListWebRelativeUrl = "Lists/LocalNavConfig";
