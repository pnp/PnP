var jQuery = "https://ajax.aspnetcdn.com/ajax/jQuery/jquery-2.0.2.min.js";

// Register script for MDS if possible
// Is MDS enabled?
if ("undefined" != typeof g_MinimalDownload && g_MinimalDownload && (window.location.pathname.toLowerCase()).endsWith("/_layouts/15/start.aspx") && "undefined" != typeof asyncDeltaManager) {
    // Register script for MDS if possible
    RegisterModuleInit("SubSiteOverride.js", JavaScript_Embed); //MDS registration
    SubSiteOverride_Embed(); //non MDS run
} else {
    SubSiteOverride_Embed();
}


// Execution
function SubSiteOverride_Embed() {

    // Run injection only on specific pages
    if ((window.location.href.toLowerCase().indexOf("viewlsts.aspx") > -1 && window.location.href.toLowerCase().indexOf("_layouts/15") > -1)) {
        SubSiteOverride_ViewlstPage();
    }
    if ((window.location.href.toLowerCase().indexOf("mngsubwebs.aspx") > -1 && window.location.href.toLowerCase().indexOf("_layouts/15") > -1)) {
        SubSiteOverride_ManageWebPage();
    }
}

// Actual link override. Checking the right URL from root site collection of the tenant/web application
function SubSiteOverride_ViewlstPage() {

    //Update create new site link point to our custom page.
    var link = document.getElementById('createnewsite');
    var url = SubSiteSettings_Web_Url + encodeURIComponent(_spPageContextInfo.webAbsoluteUrl);
    if (link != undefined) {
        link.href = url;
    }
}

function SubSiteOverride_ManageWebPage() {
        var link1 = document.getElementById('ctl00_PlaceHolderMain_MngSubwebToolBar_RptControls_newsite');
        var link2 = document.getElementById('ctl00_PlaceHolderMain_MngSubwebToolBar_RptControls_newsite_LinkImage');
        var link3 = document.getElementById('ctl00_PlaceHolderMain_MngSubwebToolBar_RptControls_newsite_LinkText');
        var url = SubSiteSettings_Web_Url + encodeURIComponent(_spPageContextInfo.webAbsoluteUrl);
        if (link1 != undefined) {
            link1.href = url;
            link2.href = url;
            link3.href = url;
        }
}

if (typeof (Sys) != "undefined" && Boolean(Sys) && Boolean(Sys.Application)) {
    Sys.Application.notifyScriptLoaded();
}
if (typeof (NotifyScriptLoadedAndExecuteWaitingJobs) == "function") {
    NotifyScriptLoadedAndExecuteWaitingJobs("SubSiteOverride.js");
}