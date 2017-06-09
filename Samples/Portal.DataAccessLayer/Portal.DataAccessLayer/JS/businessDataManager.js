'use strict'
// include utility.js

var ns = CreateNamespace('PortalDataAccessLayer');

//=====================================================================================================
// The job of the Business Data Manager is to return Business Data Objects to the JS control
//
// We have defined a series of Business Data Objects (BDO) for use by the various sample JS Controls
// - A BDO must support the Get (i.e., Read) operation
// - A BDO may (optionally) support the Set (i.e., write) operation
//
// Get (i.e., Read) Operations:
//  The BDO is created by processing the result of a query to a back-end datasource.
//  The resulting BDO is then cached in the Storage Manager.
//
//  When the calling logic (i.e., the JS Control) GETs a BDO:
//   - we return the cached BDO item if it is in the cache and has not yet expired
//   - if the item is not in the cache, or if it is present but expired, we query the back-end data source
//   - we construct a BDO from the result of the call and cache the BDO
//
// Set (i.e., Write) Operations:
//  The BDO is created and/or updated by the calling logic
//  The resulting BDO is then sent the back-end data source.
//
//  When the calling logic (i.e., the JS Control) SETS a BDO:
//   - we compare the updated BDO to the cached BDO
//   - if the item is in the cache and has not been changed, we do nothing
//   - otherwise, we update the back-end data source and cache the updated BDO
//=====================================================================================================
ns.BusinessDataManager = function () { };

//----------------------------------------------------------------------------------------------------------
// These are lightweight JSON Business Data Objects that contain the data to be rendered by the  controls
// We cache the JSON representation of these BDOs (e.g., JSON.stringify()) in the StorageManager. As such 
// we design these BDOs to be lightweight in order to avoid wasting Storage Space: 
// - we include the minimal content and structure
// - we use concise property names 
//----------------------------------------------------------------------------------------------------------

// A NavLink represents a link to a target Url
// - A NavLink must have a title
// - A NavLink may have a target Url
// - A NavLink may have a description
//
// - Title: (string) Display Text
// - Url: (string) Target Url
// - Desc: (string) Description (tooltip)
ns.BusinessDataManager.NavLinkType = "NavLink";
ns.BusinessDataManager.NavLink = function (title, url, desc)
{
    this.Type = ns.BusinessDataManager.NavLinkType;
    this.Title = title;
    this.Url = url;
    this.Desc = desc;
};
// A NavHeader represents a container for a group of Nav Nodes (links and/or headers)
// - A NavHeader must have a title
// - A NavHeader may have a target Url
// - A NavHeader may have a description
// - A NavHeader may contain NavNodes
//
// - Title: (string) Display Text
// - Url: (string) Target Url
// - Desc: (string) Description (tooltip)
// - Nodes: (array) child Nav Nodes
ns.BusinessDataManager.NavHeaderType = "NavHeader";
ns.BusinessDataManager.NavHeader = function (title, url, desc)
{
    this.Type = ns.BusinessDataManager.NavHeaderType;
    this.Title = title;
    this.Url = url;
    this.Desc = desc;
    this.Nodes = [];
};
// A NavHeaderPlus represents a container for a group of Nav Nodes (links and/or headers). 
// However, it extends a NavHeader by adding the connotation "importance".  It provides
// properties to express additional information and support enhanced display treatments.
// - A NavHeaderPlus must have a title
// - A NavHeaderPlus may have a target Url
// - A NavHeaderPlus may have a description
// - A NavHeaderPlus may contain NavNodes
// - A NavHeaderPlus may have an image Url
//
// - Title: (string) Display Text
// - Url: (string) Target Url
// - Desc: (string) Description (tooltip)
// - Img: (string) Image Url
ns.BusinessDataManager.NavHeaderPlusType = "NavHeaderPlus";
ns.BusinessDataManager.NavHeaderPlus = function (title, url, desc, img)
{
    this.Type = ns.BusinessDataManager.NavHeaderPlusType;
    this.Title = title;
    this.Url = url;
    this.Desc = desc;
    this.Nodes = [];
    this.Img = img;
};

// A MegaMenuData BDO provides the Navigation element heirarchy for the Mega Menu display control.
ns.BusinessDataManager.MegaMenuDataType = "MegaMenuData";
ns.BusinessDataManager.MegaMenuData = function ()
{
    this.Type = ns.BusinessDataManager.MegaMenuDataType;
    this.Nodes = [];
};
// A GlobalNavData BDO provides the Navigation element heirarchy for the Global Nav display control.
ns.BusinessDataManager.GlobalNavDataType = "GlobalNavData";
ns.BusinessDataManager.GlobalNavData = function ()
{
    this.Type = ns.BusinessDataManager.GlobalNavDataType;
    this.Nodes = [];
};
// A CompanyLinksData BDO provides the Navigation element heirarchy for the Company Links display control.
ns.BusinessDataManager.CompanyLinksDataType = "CompanyLinksData";
ns.BusinessDataManager.CompanyLinksData = function ()
{
    this.Type = ns.BusinessDataManager.CompanyLinksDataType;
    this.Nodes = [];
};
// A LocalNavData BDO provides the Navigation element heirarchy for the Local Nav display control.
ns.BusinessDataManager.LocalNavDataType = "LocalNavData";
ns.BusinessDataManager.LocalNavData = function ()
{
    this.Type = ns.BusinessDataManager.LocalNavDataType;
    this.Nodes = [];
};
// A FooterData BDO provides the HTML content for the Footer display control.
ns.BusinessDataManager.FooterDataType = "FooterData";
ns.BusinessDataManager.FooterData = function ()
{
    this.Type = ns.BusinessDataManager.FooterDataType;
    this.Html = "";
};

// A StockQuote represents the stock quote data for a specific stock symbol.
// - A StockQuote must have a symbol
// - A StockQuote must have a price
// - A StockQuote may have a price change
// - A StockQuote may have a percentage change
//
// - Symbol: (string) Stock Symbol
// - Price: (string) Stock Price (format TBD)
// - Change: (string) Stock Price change (format TBD)
// - Percent: (string) Stock Percent change (format TBD)
ns.BusinessDataManager.StockQuoteType = "StockQuote";
ns.BusinessDataManager.StockQuote = function (symbol, price, change, percent)
{
    this.Type = ns.BusinessDataManager.StockQuoteType;
    this.Symbol = symbol;
    this.Price = price;
    this.Change = change;
    this.Percent = percent;
};
// A StockTickerData BDO provides the stock quote data for the Stock Ticker display control.
ns.BusinessDataManager.StockTickerDataType = "StockTickerData";
ns.BusinessDataManager.StockTickerData = function ()
{
    this.Type = ns.BusinessDataManager.StockTickerDataType;
    this.Quotes = [];
};

// A UserInfoData BDO provides profile information of the current user for the UserInfo display control.
ns.BusinessDataManager.UserInfoDataType = "UserInfoData";
ns.BusinessDataManager.UserInfoData = function () {
    this.Type = ns.BusinessDataManager.UserInfoDataType;
    this.Account = "";
    this.Dept = "";
    this.Email = "";
    this.First = "";
    this.Last = "";
    this.Name = "";
    this.OneDriveUrl = "";
    this.ProfileUrl = "";
    this.Title = "";
    this.Phone = "";
};

// An ErrorData BDO provides error details to the display control when a BDO operation fails.
ns.BusinessDataManager.ErrorDataType = "ErrorData";
ns.BusinessDataManager.ErrorData = function ()
{
    this.Type = ns.BusinessDataManager.ErrorDataType;
    this.Code = 0;
    this.Message = "";
};

//----------------------------------------------------------------------------------------------------------
// These are the storage keys used for caching the lightweight JSON Business Data Objects.
// Note: the length of the key names counts against the Storage quota
// Note: be sure to consider the ClearStorage() method if you add/remove a storage key
//----------------------------------------------------------------------------------------------------------
// all BDO storage keys should start with this root
ns.BusinessDataManager.StorageKeyRoot = "pnp";
// single global instances; these keys are used to access the global instance
ns.BusinessDataManager.MegaMenuStorageKey = ns.BusinessDataManager.StorageKeyRoot + "MegaMenu";
ns.BusinessDataManager.GlobalNavStorageKey = ns.BusinessDataManager.StorageKeyRoot + "GlobalNav";
ns.BusinessDataManager.FooterStorageKey = ns.BusinessDataManager.StorageKeyRoot + "Footer";
ns.BusinessDataManager.StockTickerStorageKey = ns.BusinessDataManager.StorageKeyRoot + "Stocks";
// multiple context-specific instances; these keys serve as roots for the unique instance-specific keys we generate
ns.BusinessDataManager.CompanyLinksStorageKeyRoot = ns.BusinessDataManager.StorageKeyRoot + "CompanyLinks";
ns.BusinessDataManager.LocalNavigationStorageKeyRoot = ns.BusinessDataManager.StorageKeyRoot + "LocalNav";
ns.BusinessDataManager.UserInfoStorageKeyRoot = ns.BusinessDataManager.StorageKeyRoot + "UserInfo";

//----------------------------------------------------------------------------------------------------------
// Utility methods
//----------------------------------------------------------------------------------------------------------

// Retrieves the specified property from the prop collection of a search result row
ns.BusinessDataManager.GetPropertyValueFromResult = function (key, props)
{
    var value = "";
    if (props !== null && props.length > 0 && key !== null)
    {
        for (var i = 0; i < props.length; i++)
        {
            var prop = props[i];
            if (prop.Key === key)
            {
                value = prop.Value;
                break;
            }
        }
    }
    return value;
};

/// Clears only the BDO storage; we do not want to nuke the storage of other applications.
ns.BusinessDataManager.ClearStorage = function (storageMode)
{
    ns.LogMessage('ns.BusinessDataManager.ClearStorage(): clearing BDO ' + storageMode + ' storage...');

    if (storageMode.toLowerCase() == ns.StorageManager.DurableStorageMode.toLowerCase() || storageMode.toLowerCase() == ns.StorageManager.SessionStorageMode.toLowerCase())
    {
        // Clear all BDO storage items (i.e., those with a key that starts with the storage key root)
        ns.StorageManager.ClearItems(storageMode, ns.BusinessDataManager.StorageKeyRoot);
    }
    ns.LogMessage('ns.BusinessDataManager.ClearStorage(): BDO ' + storageMode + ' storage cleared.');
};

/// returns true if a valid storage option has been specified; otherwise, false.
ns.BusinessDataManager.UseStorage = function (storageOptions)
{
    if (storageOptions && storageOptions.storageMode)
    {
        switch (storageOptions.storageMode.toLowerCase())
        {
            case ns.StorageManager.DurableStorageMode.toLowerCase(): return true;
            case ns.StorageManager.SessionStorageMode.toLowerCase(): return true;
            default:
                break;
        }
    }
    return false;
};

//----------------------------------------------------------------------------------------------------------
// BDO Methods
//----------------------------------------------------------------------------------------------------------

/// Generates and returns the lightweight JSON Business Data Object for the Mega Menu control
ns.BusinessDataManager.GetMegaMenuData = function (storageOptions)
{
    var deferred = $.Deferred();

    // Mega Menu data is global; we can use the global storage key.
    var storageKey = ns.BusinessDataManager.MegaMenuStorageKey;

    // If null, the BDO is not in storage; otherwise, storageItem.data holds the BDO and storageItem.hasExpired indicates freshness 
    // By design, we store/return a stale BDO if an exception occurs while building the fresh BDO; doing so allows the control to 
    // continue showing reasonable content; it also prevents a cascade of data source call attempts/failures.
    var storageItem = null;

    try
    {
        // If storage is in play, request the BDO from storage
        if (ns.BusinessDataManager.UseStorage(storageOptions))
        {
            // Return the BDO to the caller if it is still fresh; if the BDO is stale, keep it around in case we encounter an issue building a fresh BDO
            storageItem = ns.StorageManager.Get(storageOptions.storageMode, storageKey);
            if (storageItem && storageItem.hasExpired == false)
            {
                deferred.resolve(storageItem.data);
                return deferred.promise();
            }
        }

        ns.LogMessage('ns.BusinessDataManager.GetMegaMenuData(): regenerating data');

        // construct the BDO for the control; for now, we simply demonstrate the use of sample/mock data...
        // TODO: map this into a custom list in SharePoint 
        var megaMenu = new ns.BusinessDataManager.MegaMenuData();

        for (var i = 0; i < 5; i++)
        {
            megaMenu.Nodes[i] = new ns.BusinessDataManager.NavHeader("Mega Tab " + (i + 1), null, "This is a heading node that has no target Url; it has children");
            for (var j = 0; j < 9; j++)
            {
                // Mark the first group of every even-numbered tab as "featured"
                if (((i % 2) == 0) && j == 0)
                {
                    megaMenu.Nodes[i].Nodes[j] = new ns.BusinessDataManager.NavHeaderPlus(
                        "Menu Group " + (j + 1) + " [Featured]", "http://msdn.microsoft.com",
                        "This is a FEATURED heading node that has a target Url; it has no children",
                        "/images/hilite.png"
                        );
                    megaMenu.Nodes[i].Nodes[j].Nodes[0] = new ns.BusinessDataManager.NavLink("Phone Link", "tel:+1-212-555-0101", "This is a link node that has a Phone target Url");
                    megaMenu.Nodes[i].Nodes[j].Nodes[1] = new ns.BusinessDataManager.NavLink("Email Link", "mailto:user@domain.com", "This is a link node that has an email target Url");
                    continue;
                }
                var hasLink = ((j % 3) > 0);        // every 3rd group has no link
                var hasChildren = ((j % 4) > 0);    // every 4th group has no children

                var link = (hasLink) ? "http://msdn.microsoft.com" : null;
                var desc = "This is a heading node that has " + ((hasLink) ? "a " : "no ") + "target Url; it has " + ((hasChildren) ? "" : "no ") + "children";

                megaMenu.Nodes[i].Nodes[j] = new ns.BusinessDataManager.NavHeader("Menu Group " + (j + 1), link, desc);

                if (hasChildren)
                {
                    for (var k = 0; k < 3; k++)
                    {
                        megaMenu.Nodes[i].Nodes[j].Nodes[k] = new ns.BusinessDataManager.NavLink("Menu Link " + (k + 1), "http://msdn.microsoft.com", "This is a link node that has a target Url");
                    }
                }
            }
        }

        // If storage is in play, store the resulting BDO and return it to the caller
        if (ns.BusinessDataManager.UseStorage(storageOptions))
        {
            ns.StorageManager.Set(storageOptions.storageMode, storageKey, megaMenu, storageOptions.useSlidingExpiration, storageOptions.timeout);
        }
        deferred.resolve(megaMenu);
    }
    catch (ex)
    {
        ns.LogError('ns.BusinessDataManager.GetMegaMenuData(): unexpected exception occurred; error=' + ex.message);
        if (storageItem)
        {
            // Store the stale BDO and return it to the caller; doing so provides reasonable display content and prevents a cascade of data source call attempts/failures.
            ns.LogWarning('ns.BusinessDataManager.GetMegaMenuData(): storing/returning stale data as a fallback');
            ns.StorageManager.Set(storageOptions.storageMode, storageKey, storageItem.data, storageOptions.useSlidingExpiration, storageOptions.timeout);
        }
        // return the stale BDO if present; otherwise, return a null BDO and let the caller decide what to render
        // TODO: instead of returning null, consider returning an ErrorData BDO if you wish to pass verbose error data to the caller.
        deferred.resolve(storageItem ? storageItem.data : null);
    }
    return deferred.promise();
};
/// Consumes and persists an updated lightweight JSON Business Data Object for the Mega Menu control
//  - megaMenu: an instance of the MegaMenuData object, containing the updated data
ns.BusinessDataManager.SetMegaMenuData = function (storageOptions, megaMenu)
{
    var jsonContract = JSON.stringify(megaMenu);
    console.log("ns.BusinessDataManager.SetMegaMenuData()  JSON: " + jsonContract);

    //TODO: implement the peristance model only if you leverage a custom admin UX that has a SAVE button; otherwise, ignore and use the OOB admin UX
};

/// Generates and returns the lightweight JSON Business Data Object for the Global Nav control
ns.BusinessDataManager.GetGlobalNavData = function (storageOptions)
{
    var deferred = $.Deferred();

    // Global Nav data is global; we can use the global storage key.
    var storageKey = ns.BusinessDataManager.GlobalNavStorageKey;

    // If null, the BDO is not in storage; otherwise, storageItem.data holds the BDO and storageItem.hasExpired indicates freshness 
    // By design, we store/return a stale BDO if an exception occurs while building the fresh BDO; doing so allows the control to 
    // continue showing reasonable content; it also prevents a cascade of data source call attempts/failures.
    var storageItem = null;

    try
    {
        // If storage is in play, request the BDO from storage
        if (ns.BusinessDataManager.UseStorage(storageOptions))
        {
            // Return the BDO to the caller if it is still fresh; if the BDO is stale, keep it around in case we encounter an issue building a fresh BDO
            storageItem = ns.StorageManager.Get(storageOptions.storageMode, storageKey);
            if (storageItem && storageItem.hasExpired == false)
            {
                deferred.resolve(storageItem.data);
                return deferred.promise();
            }
        }

        ns.LogMessage('ns.BusinessDataManager.GetGlobalNavData(): calling data source');

        // Query the Global Nav configuration list
        var queryText = '(Path:"' + ns.Configuration.PortalAdminSiteAbsoluteUrl + '/' + ns.Configuration.GlobalNavListWebRelativeUrl + '" AND contentclass=STS_ListItem_GenericList)';
        var queryUrl = ns.Configuration.GetWebAppAbsoluteUrl() + "/_api/search/query?querytext='" + encodeURIComponent(queryText) + "'" +
            "&selectproperties='" + encodeURIComponent(ns.Configuration.ManagedProp_PnPLinkText + "," + ns.Configuration.ManagedProp_PnPLinkUrl) + "'" +
            "&sortlist='" + ns.Configuration.ManagedProp_PnPDisplayOrder + ":ascending'&enablesorting=true&trimduplicates=false&rowlimit=20";

        $.ajax({
            url: queryUrl,
            method: "GET",
            headers: { "ACCEPT": "application/json;odata=verbose" },
            cache: false
        })
        .done(function (data)
        {
            ns.LogMessage('ns.BusinessDataManager.GetGlobalNavData(): processing data response');

            //  construct the BDO for the control
            var globalNav = new ns.BusinessDataManager.GlobalNavData();

            var pqr = data.d.query.PrimaryQueryResult;
            if (pqr && pqr.RelevantResults && pqr.RelevantResults.RowCount > 0)
            {
                var results = pqr.RelevantResults.Table.Rows.results;
                var numResults = results.length;
                ns.LogMessage('ns.BusinessDataManager.GetGlobalNavData(): ' + numResults + ' results returned');
                for (var i = 0; i < numResults; i++)
                {
                    var result = results[i];
                    var resultProps = result.Cells.results;
                    var linkText = ns.BusinessDataManager.GetPropertyValueFromResult(ns.Configuration.ManagedProp_PnPLinkText, resultProps);
                    var linkUrl = ns.BusinessDataManager.GetPropertyValueFromResult(ns.Configuration.ManagedProp_PnPLinkUrl, resultProps);

                    // For now, the nav data is simply a collection of Nav Links with no Nav Headers
                    globalNav.Nodes[i] = new ns.BusinessDataManager.NavLink(linkText, linkUrl, null);
                }
            }
            else
            {
                ns.LogMessage('ns.BusinessDataManager.GetGlobalNavData(): no results returned');
                // No results is a valid result; cache a default/empty BDO
                globalNav = new ns.BusinessDataManager.GlobalNavData();
            }

            // If storage is in play, store the resulting BDO and return it to the caller
            if (ns.BusinessDataManager.UseStorage(storageOptions))
            {
                ns.StorageManager.Set(storageOptions.storageMode, storageKey, globalNav, storageOptions.useSlidingExpiration, storageOptions.timeout);
            }
            deferred.resolve(globalNav);
        })
        .fail(function (xhr, status, error)
        {
            ns.LogError('ns.BusinessDataManager.GetGlobalNavData(): failed to get data - Status=' + status + '; error=' + error);
            if (storageItem)
            {
                // Store the stale BDO and return it to the caller; doing so provides reasonable display content and prevents a cascade of data source call attempts/failures.
                ns.LogWarning('ns.BusinessDataManager.GetGlobalNavData(): storing/returning stale data as a fallback');
                ns.StorageManager.Set(storageOptions.storageMode, storageKey, storageItem.data, storageOptions.useSlidingExpiration, storageOptions.timeout);
            }
            // return the stale BDO if present; otherwise, return a null BDO and let the caller decide what to render
            // TODO: instead of returning null, consider returning an ErrorData BDO if you wish to pass verbose error data to the caller.
            deferred.resolve(storageItem ? storageItem.data : null);
        });
    }
    catch (ex)
    {
        ns.LogError('ns.BusinessDataManager.GetGlobalNavData(): unexpected exception occurred; error=' + ex.message);
        if (storageItem)
        {
            // Store the stale BDO and return it to the caller; doing so provides reasonable display content and prevents a cascade of data source call attempts/failures.
            ns.LogWarning('ns.BusinessDataManager.GetGlobalNavData(): storing/returning stale data as a fallback');
            ns.StorageManager.Set(storageOptions.storageMode, storageKey, storageItem.data, storageOptions.useSlidingExpiration, storageOptions.timeout);
        }
        // return the stale BDO if present; otherwise, return a null BDO and let the caller decide what to render
        // TODO: instead of returning null, consider returning an ErrorData BDO if you wish to pass verbose error data to the caller.
        deferred.resolve(storageItem ? storageItem.data : null);
    }
    return deferred.promise();
};
/// Consumes and persists an updated lightweight JSON Business Data Object for the GlobalNav control
//  - globalNav: an instance of the GlobalNavData object, containing the updated data
ns.BusinessDataManager.SetGlobalNavData = function (storageOptions, globalNav)
{
    var jsonContract = JSON.stringify(globalNav);
    console.log("ns.BusinessDataManager.SetGlobalNavData()  JSON: " + jsonContract);

    //TODO: implement the peristance model only if you leverage a custom admin UX that has a SAVE button; otherwise, ignore and use the OOB admin UX
};

/// Generates and returns the lightweight JSON Business Data Object for the Footer control
ns.BusinessDataManager.GetFooterData = function (storageOptions)
{
    var deferred = $.Deferred();

    // Footer data is global; we can use the global storage key.
    var storageKey = ns.BusinessDataManager.FooterStorageKey;

    // If null, the BDO is not in storage; otherwise, storageItem.data holds the BDO and storageItem.hasExpired indicates freshness 
    // By design, we store/return a stale BDO if an exception occurs while building the fresh BDO; doing so allows the control to 
    // continue showing reasonable content; it also prevents a cascade of data source call attempts/failures.
    var storageItem = null;

    try
    {
        // If storage is in play, request the BDO from storage
        if (ns.BusinessDataManager.UseStorage(storageOptions))
        {
            // Return the BDO to the caller if it is still fresh; if the BDO is stale, keep it around in case we encounter an issue building a fresh BDO
            storageItem = ns.StorageManager.Get(storageOptions.storageMode, storageKey);
            if (storageItem && storageItem.hasExpired == false)
            {
                deferred.resolve(storageItem.data);
                return deferred.promise();
            }
        }

        ns.LogMessage('ns.BusinessDataManager.GetFooterData(): calling data source');

        // Query the High-Level configuration list
        var queryText = '(Path:"' + ns.Configuration.PortalAdminSiteAbsoluteUrl + '/' + ns.Configuration.ConfigurationListWebRelativeUrl + '" AND contentclass=STS_ListItem_GenericList AND ' +
            ns.Configuration.ManagedProp_PnPConfigKey + ':"' + ns.Configuration.ConfigurationListFooterKey + '")';
        var queryUrl = ns.Configuration.GetWebAppAbsoluteUrl() + "/_api/search/query?querytext='" + encodeURIComponent(queryText) + "'" +
            "&selectproperties='" + encodeURIComponent(ns.Configuration.ManagedProp_PnPConfigKey + "," + ns.Configuration.ManagedProp_PnPConfigValue) + "'" +
            "&trimduplicates=true&rowlimit=1";

        $.ajax({
            url: queryUrl,
            method: "GET",
            headers: { "ACCEPT": "application/json;odata=verbose" },
            cache: false
        })
        .done(function (data)
        {
            ns.LogMessage('ns.BusinessDataManager.GetFooterData(): processing data response');

            //  construct the BDO for the control
            var footer = new ns.BusinessDataManager.FooterData();

            var pqr = data.d.query.PrimaryQueryResult;
            if (pqr && pqr.RelevantResults && pqr.RelevantResults.RowCount > 0)
            {
                //There should be only one row, but let's be safe...
                var results = pqr.RelevantResults.Table.Rows.results;
                var numResults = results.length;
                for (var i = 0; i < numResults; i++)
                {
                    var result = results[i];
                    var resultProps = result.Cells.results;
                    footer.Html = ns.BusinessDataManager.GetPropertyValueFromResult(ns.Configuration.ManagedProp_PnPConfigValue, resultProps);
                }
            }
            else
            {
                ns.LogMessage('ns.BusinessDataManager.GetFooterData(): no results returned');
                // No results is a valid result; cache a default/empty BDO
                footer = new ns.BusinessDataManager.FooterData();
            }

            // If storage is in play, store the resulting BDO and return it to the caller
            if (ns.BusinessDataManager.UseStorage(storageOptions))
            {
                ns.StorageManager.Set(storageOptions.storageMode, storageKey, footer, storageOptions.useSlidingExpiration, storageOptions.timeout);
            }
            deferred.resolve(footer);
        })
        .fail(function (xhr, status, error)
        {
            ns.LogError('ns.BusinessDataManager.GetFooterData(): failed to get data - Status=' + status + '; error=' + error);
            if (storageItem)
            {
                // Store the stale BDO and return it to the caller; doing so provides reasonable display content and prevents a cascade of data source call attempts/failures.
                ns.LogWarning('ns.BusinessDataManager.GetFooterData(): storing/returning stale data as a fallback');
                ns.StorageManager.Set(storageOptions.storageMode, storageKey, storageItem.data, storageOptions.useSlidingExpiration, storageOptions.timeout);
            }
            // return the stale BDO if present; otherwise, return a null BDO and let the caller decide what to render
            // TODO: instead of returning null, consider returning an ErrorData BDO if you wish to pass verbose error data to the caller.
            deferred.resolve(storageItem ? storageItem.data : null);
        });
    }
    catch (ex)
    {
        ns.LogError('ns.BusinessDataManager.GetFooterData(): unexpected exception occurred; error=' + ex.message);
        if (storageItem)
        {
            // Store the stale BDO and return it to the caller; doing so provides reasonable display content and prevents a cascade of data source call attempts/failures.
            ns.LogWarning('ns.BusinessDataManager.GetFooterData(): storing/returning stale data as a fallback');
            ns.StorageManager.Set(storageOptions.storageMode, storageKey, storageItem.data, storageOptions.useSlidingExpiration, storageOptions.timeout);
        }
        // return the stale BDO if present; otherwise, return a null BDO and let the caller decide what to render
        // TODO: instead of returning null, consider returning an ErrorData BDO if you wish to pass verbose error data to the caller.
        deferred.resolve(storageItem ? storageItem.data : null);
    }
    return deferred.promise();
};
/// Consumes and persists an updated lightweight JSON Business Data Object for the Footer control
//  - myLinks: an instance of the FooterData object, containing the updated data
ns.BusinessDataManager.SetFooterData = function (storageOptions, myLinks) {
    var jsonContract = JSON.stringify(myLinks);
    console.log("ns.BusinessDataManager.SetFooterData()  JSON: " + jsonContract);

    //TODO: implement the peristance model only if you leverage a custom admin UX that has a SAVE button; otherwise, ignore and use the OOB admin UX
};

/// Generates and returns the lightweight JSON Business Data Object for the Company Links control
ns.BusinessDataManager.GetCompanyLinksData = function (storageOptions)
{
    var deferred = $.Deferred();

    // Company Links data is unique to this site collection; we must use a site collection-specific storage key.
    var storageKey = ns.BusinessDataManager.CompanyLinksStorageKeyRoot + _spPageContextInfo.siteServerRelativeUrl;

    // If null, the BDO is not in storage; otherwise, storageItem.data holds the BDO and storageItem.hasExpired indicates freshness 
    // By design, we store/return a stale BDO if an exception occurs while building the fresh BDO; doing so allows the control to 
    // continue showing reasonable content; it also prevents a cascade of data source call attempts/failures.
    var storageItem = null;

    try
    {
        // If storage is in play, request the BDO from storage
        if (ns.BusinessDataManager.UseStorage(storageOptions))
        {
            // Return the BDO to the caller if it is still fresh; if the BDO is stale, keep it around in case we encounter an issue building a fresh BDO
            storageItem = ns.StorageManager.Get(storageOptions.storageMode, storageKey);
            if (storageItem && storageItem.hasExpired == false)
            {
                deferred.resolve(storageItem.data);
                return deferred.promise();
            }
        }

        ns.LogMessage('ns.BusinessDataManager.GetCompanyLinksData(): calling data source');

        // Query the Company Links configuration list
        var queryText = '(Path:"' + _spPageContextInfo.siteAbsoluteUrl + '/' + ns.Configuration.CompanyLinksListWebRelativeUrl + '" AND contentclass=STS_ListItem_GenericList)';
        var queryUrl = ns.Configuration.GetWebAppAbsoluteUrl() + "/_api/search/query?querytext='" + encodeURIComponent(queryText) + "'" +
            "&selectproperties='" + encodeURIComponent(ns.Configuration.ManagedProp_PnPLinkText + "," + ns.Configuration.ManagedProp_PnPLinkUrl) + "'" +
            "&sortlist='" + ns.Configuration.ManagedProp_PnPDisplayOrder + ":ascending'&enablesorting=true&trimduplicates=false&rowlimit=20";

        $.ajax({
            url: queryUrl,
            method: "GET",
            headers: { "ACCEPT": "application/json;odata=verbose" },
            cache: false
        })
        .done(function (data)
        {
            ns.LogMessage('ns.BusinessDataManager.GetCompanyLinksData(): processing data response');

            //  construct the BDO for the control
            var companyLinks = new ns.BusinessDataManager.CompanyLinksData();

            var pqr = data.d.query.PrimaryQueryResult;
            if (pqr && pqr.RelevantResults && pqr.RelevantResults.RowCount > 0)
            {
                var results = pqr.RelevantResults.Table.Rows.results;
                var numResults = results.length;
                ns.LogMessage('ns.BusinessDataManager.GetCompanyLinksData(): ' + numResults + ' results returned');
                for (var i = 0; i < numResults; i++)
                {
                    var result = results[i];
                    var resultProps = result.Cells.results;
                    var linkText = ns.BusinessDataManager.GetPropertyValueFromResult(ns.Configuration.ManagedProp_PnPLinkText, resultProps);
                    var linkUrl = ns.BusinessDataManager.GetPropertyValueFromResult(ns.Configuration.ManagedProp_PnPLinkUrl, resultProps);

                    // For now, the nav data is simply a collection of Nav Links with no Nav Headers
                    companyLinks.Nodes[i] = new ns.BusinessDataManager.NavLink(linkText, linkUrl, null);
                }
            }
            else
            {
                ns.LogMessage('ns.BusinessDataManager.GetCompanyLinksData(): no results returned');
                // No results is a valid result; cache a default/empty BDO
                companyLinks = new ns.BusinessDataManager.CompanyLinksData();
            }

            // If storage is in play, store the resulting BDO and return it to the caller
            if (ns.BusinessDataManager.UseStorage(storageOptions))
            {
                ns.StorageManager.Set(storageOptions.storageMode, storageKey, companyLinks, storageOptions.useSlidingExpiration, storageOptions.timeout);
            }
            deferred.resolve(companyLinks);
        })
        .fail(function (xhr, status, error)
        {
            ns.LogError('ns.BusinessDataManager.GetCompanyLinksData(): failed to get data - Status=' + status + '; error=' + error);
            if (storageItem)
            {
                // Store the stale BDO and return it to the caller; doing so provides reasonable display content and prevents a cascade of data source call attempts/failures.
                ns.LogWarning('ns.BusinessDataManager.GetCompanyLinksData(): storing/returning stale data as a fallback');
                ns.StorageManager.Set(storageOptions.storageMode, storageKey, storageItem.data, storageOptions.useSlidingExpiration, storageOptions.timeout);
            }
            // return the stale BDO if present; otherwise, return a null BDO and let the caller decide what to render
            // TODO: instead of returning null, consider returning an ErrorData BDO if you wish to pass verbose error data to the caller.
            deferred.resolve(storageItem ? storageItem.data : null);
        });
    }
    catch (ex)
    {
        ns.LogError('ns.BusinessDataManager.GetCompanyLinksData(): unexpected exception occurred; error=' + ex.message);
        if (storageItem)
        {
            // Store the stale BDO and return it to the caller; doing so provides reasonable display content and prevents a cascade of data source call attempts/failures.
            ns.LogWarning('ns.BusinessDataManager.GetCompanyLinksData(): storing/returning stale data as a fallback');
            ns.StorageManager.Set(storageOptions.storageMode, storageKey, storageItem.data, storageOptions.useSlidingExpiration, storageOptions.timeout);
        }
        // return the stale BDO if present; otherwise, return a null BDO and let the caller decide what to render
        // TODO: instead of returning null, consider returning an ErrorData BDO if you wish to pass verbose error data to the caller.
        deferred.resolve(storageItem ? storageItem.data : null);
    }
    return deferred.promise();
};
/// Consumes and persists an updated lightweight JSON Business Data Object for the Company Links control
//  - companyLinks: an instance of the CompanyLinksData object, containing the updated data
ns.BusinessDataManager.SetCompanyLinksData = function (storageOptions, companyLinks)
{
    var jsonContract = JSON.stringify(companyLinks);
    console.log("ns.BusinessDataManager.SetCompanyLinksData()  JSON: " + jsonContract);

    //TODO: implement the peristance model only if you leverage a custom admin UX that has a SAVE button; otherwise, ignore and use the OOB admin UX
};

/// Generates and returns the lightweight JSON Business Data Object for the Local Navigation control
ns.BusinessDataManager.GetLocalNavData = function (storageOptions)
{
    var deferred = $.Deferred();

    // Local Nav data is unique to this web; we must use a web-specific storage key.
    var storageKey = ns.BusinessDataManager.LocalNavigationStorageKeyRoot + _spPageContextInfo.webServerRelativeUrl;

    // If null, the BDO is not in storage; otherwise, storageItem.data holds the BDO and storageItem.hasExpired indicates freshness 
    // By design, we store/return a stale BDO if an exception occurs while building the fresh BDO; doing so allows the control to 
    // continue showing reasonable content; it also prevents a cascade of data source call attempts/failures.
    var storageItem = null;

    try
    {
        // If storage is in play, request the BDO from storage
        if (ns.BusinessDataManager.UseStorage(storageOptions))
        {
            // Return the BDO to the caller if it is still fresh; if the BDO is stale, keep it around in case we encounter an issue building a fresh BDO
            storageItem = ns.StorageManager.Get(storageOptions.storageMode, storageKey);
            if (storageItem && storageItem.hasExpired == false)
            {
                deferred.resolve(storageItem.data);
                return deferred.promise();
            }
        }

        ns.LogMessage('ns.BusinessDataManager.GetLocalNavData(): calling data source');

        // Query the Company Links configuration list
        var queryText = '(Path:"' + _spPageContextInfo.webAbsoluteUrl + '/' + ns.Configuration.LocalNavListWebRelativeUrl + '" AND contentclass=STS_ListItem_GenericList)';
        var queryUrl = ns.Configuration.GetWebAppAbsoluteUrl() + "/_api/search/query?querytext='" + encodeURIComponent(queryText) + "'" +
            "&selectproperties='" + encodeURIComponent(ns.Configuration.ManagedProp_PnPLinkText + "," + ns.Configuration.ManagedProp_PnPLinkUrl) + "'" +
            "&sortlist='" + ns.Configuration.ManagedProp_PnPDisplayOrder + ":ascending'&enablesorting=true&trimduplicates=false&rowlimit=20";

        $.ajax({
            url: queryUrl,
            method: "GET",
            headers: { "ACCEPT": "application/json;odata=verbose" },
            cache: false
        })
        .done(function (data)
        {
            ns.LogMessage('ns.BusinessDataManager.GetLocalNavData(): processing data response');

            //  construct the BDO for the control
            var localNav = new ns.BusinessDataManager.LocalNavData();

            var pqr = data.d.query.PrimaryQueryResult;
            if (pqr && pqr.RelevantResults && pqr.RelevantResults.RowCount > 0)
            {
                var results = pqr.RelevantResults.Table.Rows.results;
                var numResults = results.length;
                ns.LogMessage('ns.BusinessDataManager.GetLocalNavData(): ' + numResults + ' results returned');
                for (var i = 0; i < numResults; i++)
                {
                    var result = results[i];
                    var resultProps = result.Cells.results;
                    var linkText = ns.BusinessDataManager.GetPropertyValueFromResult(ns.Configuration.ManagedProp_PnPLinkText, resultProps);
                    var linkUrl = ns.BusinessDataManager.GetPropertyValueFromResult(ns.Configuration.ManagedProp_PnPLinkUrl, resultProps);

                    // For now, the nav data is simply a collection of Nav Links with no Nav Headers
                    localNav.Nodes[i] = new ns.BusinessDataManager.NavLink(linkText, linkUrl, null);
                }
            }
            else
            {
                ns.LogMessage('ns.BusinessDataManager.GetLocalNavData(): no results returned');
                // No results is a valid result; cache a default/empty BDO
                localNav = new ns.BusinessDataManager.LocalNavData();
            }

            // If storage is in play, store the resulting BDO and return it to the caller
            if (ns.BusinessDataManager.UseStorage(storageOptions))
            {
                ns.StorageManager.Set(storageOptions.storageMode, storageKey, localNav, storageOptions.useSlidingExpiration, storageOptions.timeout);
            }
            deferred.resolve(localNav);
        })
        .fail(function (xhr, status, error)
        {
            ns.LogError('ns.BusinessDataManager.GetLocalNavData(): failed to get data - Status=' + status + '; error=' + error);
            if (storageItem)
            {
                // Store the stale BDO and return it to the caller; doing so provides reasonable display content and prevents a cascade of data source call attempts/failures.
                ns.LogWarning('ns.BusinessDataManager.GetLocalNavData(): storing/returning stale data as a fallback');
                ns.StorageManager.Set(storageOptions.storageMode, storageKey, storageItem.data, storageOptions.useSlidingExpiration, storageOptions.timeout);
            }
            // return the stale BDO if present; otherwise, return a null BDO and let the caller decide what to render
            // TODO: instead of returning null, consider returning an ErrorData BDO if you wish to pass verbose error data to the caller.
            deferred.resolve(storageItem ? storageItem.data : null);
        });
    }
    catch (ex)
    {
        ns.LogError('ns.BusinessDataManager.GetLocalNavData(): unexpected exception occurred; error=' + ex.message);
        if (storageItem)
        {
            // Store the stale BDO and return it to the caller; doing so provides reasonable display content and prevents a cascade of data source call attempts/failures.
            ns.LogWarning('ns.BusinessDataManager.GetLocalNavData(): storing/returning stale data as a fallback');
            ns.StorageManager.Set(storageOptions.storageMode, storageKey, storageItem.data, storageOptions.useSlidingExpiration, storageOptions.timeout);
        }
        // return the stale BDO if present; otherwise, return a null BDO and let the caller decide what to render
        // TODO: instead of returning null, consider returning an ErrorData BDO if you wish to pass verbose error data to the caller.
        deferred.resolve(storageItem ? storageItem.data : null);
    }
    return deferred.promise();
};
/// Consumes and persists an updated lightweight JSON Business Data Object for the Local Navigation control
//  - localNav: an instance of the LocalNavigationData object, containing the updated data
ns.BusinessDataManager.SetLocalNavData = function (storageOptions, localNav)
{
    var jsonContract = JSON.stringify(localNav);
    console.log("ns.BusinessDataManager.SetLocalNavData()  JSON: " + jsonContract);

    //TODO: implement the peristance model only if you leverage a custom admin UX that has a SAVE button; otherwise, ignore and use the OOB admin UX
};

/// Generates and returns the lightweight JSON Business Data Object for the User Info control
ns.BusinessDataManager.GetUserInfoData = function (storageOptions)
{
    var deferred = $.Deferred();

    // User Info data is obviously unique to this user; we must use a user-specific storage key.
    var storageKey = ns.BusinessDataManager.UserInfoStorageKeyRoot + ':' + _spPageContextInfo.userLoginName;

    // If null, the BDO is not in storage; otherwise, storageItem.data holds the BDO and storageItem.hasExpired indicates freshness 
    // By design, we store/return a stale BDO if an exception occurs while building the fresh BDO; doing so allows the control to 
    // continue showing reasonable content; it also prevents a cascade of data source call attempts/failures.
    var storageItem = null;

    try
    {
        // If storage is in play, request the BDO from storage
        if (ns.BusinessDataManager.UseStorage(storageOptions))
        {
            // Return the BDO to the caller if it is still fresh; if the BDO is stale, keep it around in case we encounter an issue building a fresh BDO
            storageItem = ns.StorageManager.Get(storageOptions.storageMode, storageKey);
            if (storageItem && storageItem.hasExpired == false)
            {
                deferred.resolve(storageItem.data);
                return deferred.promise();
            }
        }

        ns.LogMessage('ns.BusinessDataManager.GetUserInfoData(): calling data source');

        // Query the User Profile configuration list
        var queryUrl = ns.Configuration.GetWebAppAbsoluteUrl() + "/_api/sp.userprofiles.peoplemanager/getmyproperties";

        $.ajax({
            url: queryUrl,
            method: "GET",
            headers: { "ACCEPT": "application/json;odata=verbose" },
            cache: false
        })
        .done(function (data)
        {
            ns.LogMessage('ns.BusinessDataManager.GetUserInfoData(): processing data response');

            //  construct the BDO for the control
            var userInfo = new ns.BusinessDataManager.UserInfoData();

            var profile = data.d;
            if (profile)
            {
                var profileProps = profile.UserProfileProperties.results;
                ns.LogMessage('ns.BusinessDataManager.GetUserInfoData(): 1 result returned for ' + _spPageContextInfo.userLoginName);

                userInfo.Account = profile.AccountName;
                userInfo.Dept = ns.BusinessDataManager.GetPropertyValueFromResult("Department", profileProps);
                userInfo.Email = profile.Email;
                userInfo.First = ns.BusinessDataManager.GetPropertyValueFromResult("FirstName", profileProps);
                userInfo.Last = ns.BusinessDataManager.GetPropertyValueFromResult("LastName", profileProps);
                userInfo.Name = profile.DisplayName;
                userInfo.OneDriveUrl = profile.PersonalUrl;
                userInfo.ProfileUrl = profile.UserUrl;
                userInfo.Title = ns.BusinessDataManager.GetPropertyValueFromResult("Title", profileProps);
                userInfo.Phone = ns.BusinessDataManager.GetPropertyValueFromResult("WorkPhone", profileProps);
            }
            else
            {
                ns.LogMessage('ns.BusinessDataManager.GetUserInfoData(): no results returned');
                // No results is a valid result; cache a default/empty BDO
                userInfo = new ns.BusinessDataManager.UserInfoData();
            }

            // If storage is in play, store the resulting BDO and return it to the caller
            if (ns.BusinessDataManager.UseStorage(storageOptions))
            {
                ns.StorageManager.Set(storageOptions.storageMode, storageKey, userInfo, storageOptions.useSlidingExpiration, storageOptions.timeout);
            }
            deferred.resolve(userInfo);
        })
        .fail(function (xhr, status, error)
        {
            ns.LogError('ns.BusinessDataManager.GetUserInfoData(): failed to get data - Status=' + status + '; error=' + error);
            if (storageItem)
            {
                // Store the stale BDO and return it to the caller; doing so provides reasonable display content and prevents a cascade of data source call attempts/failures.
                ns.LogWarning('ns.BusinessDataManager.GetUserInfoData(): storing/returning stale data as a fallback');
                ns.StorageManager.Set(storageOptions.storageMode, storageKey, storageItem.data, storageOptions.useSlidingExpiration, storageOptions.timeout);
            }
            // return the stale BDO if present; otherwise, return a null BDO and let the caller decide what to render
            // TODO: instead of returning null, consider returning an ErrorData BDO if you wish to pass verbose error data to the caller.
            deferred.resolve(storageItem ? storageItem.data : null);
        });
    }
    catch (ex)
    {
        ns.LogError('ns.BusinessDataManager.GetUserInfoData(): unexpected exception occurred; error=' + ex.message);
        if (storageItem)
        {
            // Store the stale BDO and return it to the caller; doing so provides reasonable display content and prevents a cascade of data source call attempts/failures.
            ns.LogWarning('ns.BusinessDataManager.GetUserInfoData(): storing/returning stale data as a fallback');
            ns.StorageManager.Set(storageOptions.storageMode, storageKey, storageItem.data, storageOptions.useSlidingExpiration, storageOptions.timeout);
        }
        // return the stale BDO if present; otherwise, return a null BDO and let the caller decide what to render
        // TODO: instead of returning null, consider returning an ErrorData BDO if you wish to pass verbose error data to the caller.
        deferred.resolve(storageItem ? storageItem.data : null);
    }
    return deferred.promise();
};
/// Consumes and persists an updated lightweight JSON Business Data Object for the User Info control
//  - userInfo: an instance of the UserInfoData object, containing the updated data
ns.BusinessDataManager.SetUserInfoData = function (storageOptions, userInfo) {
    var jsonContract = JSON.stringify(userInfo);
    console.log("ns.BusinessDataManager.SetUserInfoData()  JSON: " + jsonContract);

    //TODO: implement the peristance model only if you leverage a custom admin UX that has a SAVE button; otherwise, ignore and use the OOB admin UX
};

/// Generates and returns the lightweight JSON Business Data Object for the Stock Ticker control
ns.BusinessDataManager.GetStockTickerData = function (storageOptions)
{
    var deferred = $.Deferred();

    // Stock Ticker data is global; we can use the global storage key.
    var storageKey = ns.BusinessDataManager.StockTickerStorageKey;

    // If null, the BDO is not in storage; otherwise, storageItem.data holds the BDO and storageItem.hasExpired indicates freshness 
    // By design, we store/return a stale BDO if an exception occurs while building the fresh BDO; doing so allows the control to 
    // continue showing reasonable content; it also prevents a cascade of data source call attempts/failures.
    var storageItem = null;

    try
    {
        // If storage is in play, request the BDO from storage
        if (ns.BusinessDataManager.UseStorage(storageOptions))
        {
            // Return the BDO to the caller if it is still fresh; if the BDO is stale, keep it around in case we encounter an issue building a fresh BDO
            storageItem = ns.StorageManager.Get(storageOptions.storageMode, storageKey);
            if (storageItem && storageItem.hasExpired == false)
            {
                deferred.resolve(storageItem.data);
                return deferred.promise();
            }
        }

        ns.LogMessage('ns.BusinessDataManager.GetStockTickerData(): calling data source');

        //  construct the BDO for the control       
        var stockTickerSymbol = ns.Configuration.StockTickerSymbol;
        var queryText = "select * from yahoo.finance.quotes where symbol in ('" + stockTickerSymbol + "')";
        var url = 'https://query.yahooapis.com/v1/public/yql?q=' + encodeURIComponent(queryText) + '&env=store://datatables.org/alltableswithkeys&format=json&callback=?';

        var deferred = $.Deferred();

        $.ajax({
            url: url,
            method: "GET",
            dataType: 'json',
            cache: false
        })
        .done(function (data)
        {
            ns.LogMessage('ns.BusinessDataManager.GetStockTickerData(): processing data response');

            var stockTicker = new ns.BusinessDataManager.StockTickerData();
            try
            {
                var quote = data.query.results.quote;

                // the symbol (even if invalid) is always returned; however, other properties are null if the symbol is invalid.
                var symbol = quote.Symbol.toString();
                var companyName = quote.Name.toString();
                var price = quote.LastTradePriceOnly.toString();
                var changeInPrice = quote.Change != null ? quote.Change.toString() : "";
                var percentChangeInPrice = quote.PercentChange != null ? quote.PercentChange.toString() : "";

                ns.LogMessage('ns.BusinessDataManager.GetStockTickerData(): 1 result returned for ' + stockTickerSymbol);
                stockTicker.Quotes[0] = new ns.BusinessDataManager.StockQuote(symbol, price, changeInPrice, percentChangeInPrice);
            }
            catch (ex)
            {
                ns.LogMessage('ns.BusinessDataManager.GetStockTickerData(): error processing result for ' + stockTickerSymbol + '; error=' + ex.message);
                // No results is a valid result; cache a default/empty BDO
                stockTicker = new ns.BusinessDataManager.StockTickerData();
            }

            // If storage is in play, store the resulting BDO and return it to the caller
            if (ns.BusinessDataManager.UseStorage(storageOptions))
            {
                ns.StorageManager.Set(storageOptions.storageMode, storageKey, stockTicker, storageOptions.useSlidingExpiration, storageOptions.timeout);
            }
            deferred.resolve(stockTicker);
        })
        .fail(function (xhr, status, error)
        {
            ns.LogError('ns.BusinessDataManager.GetStockTickerData(): failed to get data - Status=' + status + '; error=' + error);
            if (storageItem)
            {
                // Store the stale BDO and return it to the caller; doing so provides reasonable display content and prevents a cascade of data source call attempts/failures.
                ns.LogWarning('ns.BusinessDataManager.GetStockTickerData(): storing/returning stale data as a fallback');
                ns.StorageManager.Set(storageOptions.storageMode, storageKey, storageItem.data, storageOptions.useSlidingExpiration, storageOptions.timeout);
            }
            // return the stale BDO if present; otherwise, return a null BDO and let the caller decide what to render
            // TODO: instead of returning null, consider returning an ErrorData BDO if you wish to pass verbose error data to the caller.
            deferred.resolve(storageItem ? storageItem.data : null);
        });
    }
    catch (ex)
    {
        ns.LogError('ns.BusinessDataManager.GetStockTickerData(): unexpected exception occurred; error=' + ex.message);
        if (storageItem)
        {
            // Store the stale BDO and return it to the caller; doing so provides reasonable display content and prevents a cascade of data source call attempts/failures.
            ns.LogWarning('ns.BusinessDataManager.GetStockTickerData(): storing/returning stale data as a fallback');
            ns.StorageManager.Set(storageOptions.storageMode, storageKey, storageItem.data, storageOptions.useSlidingExpiration, storageOptions.timeout);
        }
        // return the stale BDO if present; otherwise, return a null BDO and let the caller decide what to render
        // TODO: instead of returning null, consider returning an ErrorData BDO if you wish to pass verbose error data to the caller.
        deferred.resolve(storageItem ? storageItem.data : null);
    }
    return deferred.promise();
};

// Process the Clear_Storage query string argument
if (ns.UtilityManager.GetQueryStringParameter('clearStorage') == '1')
{
    ns.LogMessage('ns.UtilityManager: query string argument (clearStorage=1) found; clearing BDO localStorage/sessionStorage...');
    ns.BusinessDataManager.ClearStorage(ns.StorageManager.DurableStorageMode);
    ns.BusinessDataManager.ClearStorage(ns.StorageManager.SessionStorageMode);
    ns.LogMessage('ns.UtilityManager: BDO localStorage/sessionStorage cleared.');
};
