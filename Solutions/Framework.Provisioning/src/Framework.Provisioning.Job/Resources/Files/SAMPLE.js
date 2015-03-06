// Register script for MDS if possible
RegisterModuleInit("SAMPLE.js", PnPSubSite_Inject); //MDS registration
PnPSubSite_Inject(); //non MDS run

// Actual execution
function PnPSubSite_Inject() {

    // Run injection only for site content or manage workspaces page
    if ((window.location.href.toLowerCase().indexOf("viewlsts.aspx") > -1 || window.location.href.toLowerCase().indexOf("mngsubwebs.aspx") > -1)
        && window.location.href.toLowerCase().indexOf("_layouts/15") > -1) {
        PnPSubSite_OverrideLinkToAppUrl();
    }
}

// Actual link override. Checking the right URL from root site collection of the tenant/web application
function PnPSubSite_OverrideLinkToAppUrl() {
    //THIS IS A SAMPLE SCRIPT
  
}
if (typeof (Sys) != "undefined" && Boolean(Sys) && Boolean(Sys.Application)) {
  Sys.Application.notifyScriptLoaded();
}

if (typeof (NotifyScriptLoadedAndExecuteWaitingJobs) == "function") {
    NotifyScriptLoadedAndExecuteWaitingJobs("PnP_EmbeddedJS.js");
}
