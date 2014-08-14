// Register script for MDS if possible
RegisterModuleInit("CustomInjectedJS.js", SubSiteOverride_Inject); //MDS registration
SubSiteOverride_Inject(); //non MDS run

// Actual execution
function SubSiteOverride_Inject() {

    // Run injection only for site content
    if ((window.location.href.toLowerCase().indexOf("viewlsts.aspx") > -1 && window.location.href.toLowerCase().indexOf("_layouts/15") > -1))
    {
        SubSiteOverride_OverrideLinkToAppUrl();
    }
}

// Actual link override. Checking the right URL from root site collection of the tenant/web application
function SubSiteOverride_OverrideLinkToAppUrl() {

    //Update create new site link point to our custom page.
    var link = document.getElementById('createnewsite');
    var url = "https://localhost:44339/pages/default.aspx?SPHostUrl=" + encodeURIComponent(_spPageContextInfo.webAbsoluteUrl);
    if (link != undefined) {
        // Could be get from SPSite root web property bag - now hard coded for demo purposes
        link.href = url;
    }

}


if (typeof (Sys) != "undefined" && Boolean(Sys) && Boolean(Sys.Application)) {
  Sys.Application.notifyScriptLoaded();
}
if (typeof (NotifyScriptLoadedAndExecuteWaitingJobs) == "function") {
    NotifyScriptLoadedAndExecuteWaitingJobs("CustomInjectedJS.js");
}