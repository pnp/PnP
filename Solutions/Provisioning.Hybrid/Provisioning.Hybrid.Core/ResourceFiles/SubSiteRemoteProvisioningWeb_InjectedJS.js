// Register script for MDS if possible
RegisterModuleInit("SubSiteRemoteProvisioningWeb_InjectedJS.js", SubSiteRemoteProvisioningWeb_Inject); //MDS registration
SubSiteRemoteProvisioningWeb_Inject(); //non MDS run

if (typeof (Sys) != "undefined" && Boolean(Sys) && Boolean(Sys.Application)) {
    Sys.Application.notifyScriptLoaded();
}

if (typeof (NotifyScriptLoadedAndExecuteWaitingJobs) == "function") {
    NotifyScriptLoadedAndExecuteWaitingJobs("SubSiteRemoteProvisioningWeb_InjectedJS.js");
}

//var - sub site provider hosted app URL set in configuration list
var SubSiteRemoteProvisioningWeb_SubSiteAppUrl;
var SubSiteRemoteProvisioningWeb_clientContext;
var SubSiteRemoteProvisioningWeb_oWebsite;
var SubSiteRemoteProvisioningWeb_oList;
var SubSiteRemoteProvisioningWeb_listItemEnumerator;
var SubSiteRemoteProvisioningWeb_collListItem;

// Execute only to specific pages.
function SubSiteRemoteProvisioningWeb_Inject() {

    // Go and resolve the app URL using config list from root site collection
    //SubSiteRemoteProvisioningWeb_RetrieveConfigItems();

    // Run injection only for site content page
    if (window.location.href.toLowerCase().indexOf("viewlsts.aspx") > -1 && window.location.href.toLowerCase().indexOf("_layouts/15") > -1) {
        SubSiteRemoteProvisioningWeb_SiteContent();
    }

    // Run injection only for site content page
    if (window.location.href.toLowerCase().indexOf("mngsubwebs.aspx") > -1 && window.location.href.toLowerCase().indexOf("_layouts/15") > -1) {
        SubSiteRemoteProvisioningWeb_ManageSubWebs();
    }
}

// For viewlsts.aspx - Notice that this is demo solution, so it uses hardcoded URL. 
function SubSiteRemoteProvisioningWeb_SiteContent() {
    //Update create new site link point to our custom page.
    var link = document.getElementById('createnewsite');
    var icon = document.getElementById('ctl00_onetidHeadbnnr2').src;
    var url = SubSiteRemoteProvisioningWeb_ResolveUrl() + "&SPHostLogoUrl=" + encodeURIComponent(icon);
    if (link != undefined) {
        // Could be get from SPSite root web property bag - now hardcdoded for demo purposes
        link.href = url;
    }
}

function SubSiteRemoteProvisioningWeb_ManageSubWebs() {
    var link1 = document.getElementById('ctl00_PlaceHolderMain_MngSubwebToolBar_RptControls_newsite');
    var link2 = document.getElementById('ctl00_PlaceHolderMain_MngSubwebToolBar_RptControls_newsite_LinkImage');
    var link3 = document.getElementById('ctl00_PlaceHolderMain_MngSubwebToolBar_RptControls_newsite_LinkText');

    var url = SubSiteRemoteProvisioningWeb_ResolveUrl();
    if (link1 != undefined) {
        link1.href = url;
        link2.href = url;
        link3.href = url;
    }
}

function SubSiteRemoteProvisioningWeb_ResolveUrl() {

    // Movign one, waiting for time to fix this properly
    // return SubSiteRemoteProvisioningWeb_SubSiteAppUrl + "?SPHostUrl=" + encodeURIComponent(_spPageContextInfo.webAbsoluteUrl) + "&IsDlg=0";
    return "https://localhost:44310/pages/default.aspx?SPHostUrl=" + encodeURIComponent(_spPageContextInfo.webAbsoluteUrl) + "&IsDlg=0";
}


function SubSiteRemoteProvisioningWeb_RetrieveConfigItems()
{
    var appRootsiteUrl = _spPageContextInfo.siteAbsoluteUrl.replace(_spPageContextInfo.siteServerRelativeUrl, "/");
    SubSiteRemoteProvisioningWeb_clientContext = new SP.ClientContext(appRootsiteUrl);
    SubSiteRemoteProvisioningWeb_oWebsite = SubSiteRemoteProvisioningWeb_clientContext.get_web();
    SubSiteRemoteProvisioningWeb_oList = SubSiteRemoteProvisioningWeb_oWebsite.get_lists().getByTitle('Configuration');
    SubSiteRemoteProvisioningWeb_clientContext.load(SubSiteRemoteProvisioningWeb_oList);
    SubSiteRemoteProvisioningWeb_clientContext.executeQueryAsync(Function.createDelegate(this, SubSiteRemoteProvisioningWeb_ConfigListExistsQuerySucceeded), Function.createDelegate(this, SubSiteRemoteProvisioningWeb_ConfigListExistsQueryFailed));
}

function SubSiteRemoteProvisioningWeb_ConfigListExistsQuerySucceeded(sender, args) {
    // alert("List found");
    var camlQuery = new SP.CamlQuery();
    camlQuery.set_viewXml('<View><ViewFields><FieldRef Name="Title"/><FieldRef Name="URL"/></ViewFields><RowLimit>10</RowLimit></View>');
    SubSiteRemoteProvisioningWeb_collListItem = SubSiteRemoteProvisioningWeb_oList.getItems(camlQuery);
    SubSiteRemoteProvisioningWeb_clientContext.load(SubSiteRemoteProvisioningWeb_collListItem);
    SubSiteRemoteProvisioningWeb_clientContext.executeQueryAsync(Function.createDelegate(this, this.SubSiteRemoteProvisioningWeb_ConfigListQuerySucceeded), Function.createDelegate(this, this.ConfigListQueryFailed));
}

function SubSiteRemoteProvisioningWeb_ConfigListExistsQueryFailed(sender, args) {
    
    var listCreationInfo = new SP.ListCreationInformation();
    listCreationInfo.set_title('Configuration'); // list name
    listCreationInfo.set_description('App Configuration'); // list description
    listCreationInfo.set_templateType(SP.ListTemplateType.genericList); //list type

    SubSiteRemoteProvisioningWeb_oList = SubSiteRemoteProvisioningWeb_oWebsite.get_lists().add(listCreationInfo);

    SubSiteRemoteProvisioningWeb_clientContext.executeQueryAsync(
        Function.createDelegate(this, this.SubSiteRemoteProvisioningWeb_ConfigListCreationSucceeded),// when success
        Function.createDelegate(this, this.SubSiteRemoteProvisioningWeb_ConfigListCreationFailed) // when failed
        )
}


function SubSiteRemoteProvisioningWeb_ConfigListCreationSucceeded(sender, args) {
    // Get filed collection
    var fldCollection = SubSiteRemoteProvisioningWeb_oList.get_fields();

    var f1 = SubSiteRemoteProvisioningWeb_clientContext.castTo(fldCollection.addFieldAsXml('<Field Type="Text" DisplayName="URL" Name="URL" />', true, SP.AddFieldOptions.addToDefaultContentType), SP.FieldText);
    f1.set_title("URL");
    f1.set_description("URL");
    f1.update();

    SubSiteRemoteProvisioningWeb_clientContext.executeQueryAsync(
        Function.createDelegate(this, this.this.SubSiteRemoteProvisioningWeb_OnFieldInfoQuerySucceeded),
        Function.createDelegate(this, this.SubSiteRemoteProvisioningWeb_OnFieldInfoQueryFailed)
        );
}

function SubSiteRemoteProvisioningWeb_OnFieldInfoQuerySucceeded(sender, args) {

    var itemCreateInfo = new SP.ListItemCreationInformation();
    this.SubSiteRemoteProvisioningWeb_oListItem = SubSiteRemoteProvisioningWeb_oList.addItem(itemCreateInfo);
    SubSiteRemoteProvisioningWeb_oListItem.set_item('Title', 'SubSiteAppUrl');
    SubSiteRemoteProvisioningWeb_oListItem.set_item('URL', 'https://localhost:44310/pages/default.aspx');
    SubSiteRemoteProvisioningWeb_oListItem.update();

    SubSiteRemoteProvisioningWeb_clientContext.load(SubSiteRemoteProvisioningWeb_oListItem);

    SubSiteRemoteProvisioningWeb_clientContext.executeQueryAsync(Function.createDelegate(this, this.SubSiteRemoteProvisioningWeb_ConfigListItemQuerySucceeded), Function.createDelegate(this, this.SubSiteRemoteProvisioningWeb_ConfigListItemQuerFailed));
}


function SubSiteRemoteProvisioningWeb_ConfigListQuerySucceeded(sender, args) {
    SubSiteRemoteProvisioningWeb_listItemEnumerator = SubSiteRemoteProvisioningWeb_collListItem.getEnumerator();
    while (SubSiteRemoteProvisioningWeb_listItemEnumerator.moveNext()) {
        var SubSiteRemoteProvisioningWeb_oListItem = SubSiteRemoteProvisioningWeb_listItemEnumerator.get_current();
        if (SubSiteRemoteProvisioningWeb_oListItem.get_item('Title') === "SubSiteAppUrl") {
            SubSiteRemoteProvisioningWeb_SubSiteAppUrl = SubSiteRemoteProvisioningWeb_oListItem.get_item('URL'); 
        }
    }
}

function SubSiteRemoteProvisioningWeb_ConfigListItemQuerySucceeded(sender, args) {
}

function SubSiteRemoteProvisioningWeb_ConfigListItemQuerFailed(sender, args) {
}

function SubSiteRemoteProvisioningWeb_OnFieldInfoQueryFailed(sender, args) {
}

function SubSiteRemoteProvisioningWeb_ConfigListCreationFailed(sender, args) {
}

function onQueryFailed(sender, args) {
}

