
if (document.readyState === "complete") {
    //Already loaded!
    SubSiteOverride_Inject(); //non MDS run
}
else {
    //Add onload or DOMContentLoaded event listeners Mozilla, Opera and webkit nightlies currently support this event
    if (document.addEventListener) {
        // Use the handy event callback
        document.addEventListener("DOMContentLoaded", function () { SubSiteOverride_Inject(); }, false);
        // If IE event model is used
    } else if (document.attachEvent) {
        // ensure firing before onload,
        // maybe late but safe also for iframes
        document.attachEvent("onreadystatechange", function () { SubSiteOverride_Inject(); });
    }
}

// Execution
function SubSiteOverride_Inject() {

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