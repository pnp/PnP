// Register script for MDS if possible
RegisterModuleInit("OfficeAMSSubSite_InjectedJS.js", OfficeAMSSubSite_Inject); //MDS registration
OfficeAMSSubSite_Inject(); //non MDS run

if (typeof (Sys) != "undefined" && Boolean(Sys) && Boolean(Sys.Application)) {
    Sys.Application.notifyScriptLoaded();
}

// Actual execution
function OfficeAMSSubSite_Inject() {

    // Run injection only for site content or manage workspaces page
    if ((window.location.href.toLowerCase().indexOf("viewlsts.aspx") > -1 || window.location.href.toLowerCase().indexOf("mngsubwebs.aspx") > -1)
        && window.location.href.toLowerCase().indexOf("_layouts/15") > -1) {
        OfficeAMSSubSite_OverrideLinkToAppUrl();
    }
}

// Actual link override. Checking the right URL from root site collection of the tenant/web application
function OfficeAMSSubSite_OverrideLinkToAppUrl() {

    var appRootsiteUrl = _spPageContextInfo.siteAbsoluteUrl.replace(_spPageContextInfo.siteServerRelativeUrl, "/");
    var value;
    ctx = new SP.ClientContext(appRootsiteUrl);
    rootWeb = ctx.get_web();
    list = rootWeb.get_lists().getByTitle('OfficeAMSConfig');
    ctx.load(list);
    ctx.executeQueryAsync(function () {
        // Get list items
        var camlQuery = new SP.CamlQuery();
        camlQuery.set_viewXml('<Where><Eq><FieldRef Name=\'Title\' /><Value Type=\'Text\'>SubSiteAppUrl</Value></Eq></Where><ViewFields><FieldRef Name=\'Title\' /><FieldRef Name=\'Value\' /></ViewFields>');
        items = list.getItems(camlQuery);
        ctx.load(items);
        ctx.executeQueryAsync(function () {

            // Get items
            listItemEnumerator = items.getEnumerator();
            while (listItemEnumerator.moveNext()) {
                var item = listItemEnumerator.get_current();
                if (item.get_item('Title') === "SubSiteAppUrl") {
                    value = item.get_item('Value');
                }
            }

            //Update create new site link point to our custom page.
            var icon = document.getElementById('ctl00_onetidHeadbnnr2').src;
            var link = document.getElementById('createnewsite');
            var url = value + "/pages/default.aspx?SPHostUrl=" + encodeURIComponent(_spPageContextInfo.webAbsoluteUrl) + "&IsDlg=0&SPHostLogoUrl=" + encodeURIComponent(icon);
            if (link != undefined) {
                // Could be get from SPSite root web property bag - now hardcdoded for demo purposes
                link.href = url;
            }
            else if (window.location.href.toLowerCase().indexOf("mngsubwebs.aspx") > -1) {
                var link1 = document.getElementById('ctl00_PlaceHolderMain_MngSubwebToolBar_RptControls_newsite');
                var link2 = document.getElementById('ctl00_PlaceHolderMain_MngSubwebToolBar_RptControls_newsite_LinkImage');
                var link3 = document.getElementById('ctl00_PlaceHolderMain_MngSubwebToolBar_RptControls_newsite_LinkText');
                if (link1 != undefined) {
                    link1.href = url;
                    link2.href = url;
                    link3.href = url;
                }
            }

        }, Function.createDelegate(this, this.OfficeAMSSubSite_QueryFailed));

    }, Function.createDelegate(this, OfficeAMSSubSite_QueryFailed));
}

function OfficeAMSSubSite_QueryFailed(sender, args) { }

if (typeof (NotifyScriptLoadedAndExecuteWaitingJobs) == "function") {
    NotifyScriptLoadedAndExecuteWaitingJobs("OfficeAMSSubSite_InjectedJS.js");
}
