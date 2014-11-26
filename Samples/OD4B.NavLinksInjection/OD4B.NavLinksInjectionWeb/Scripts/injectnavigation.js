// Register script for MDS if possible
RegisterModuleInit("injectnavigation.js", RemoteManager_Inject); //MDS registration
RemoteManager_Inject(); //non MDS run

var cacheTimeout = 1800;
var currentTime;
var timeStamp;
var secondaryNavInjected;
var context = SP.ClientContext.get_current();
var user = context.get_web().get_currentUser();
var buildSecondaryNavigation;

if (typeof (Sys) != "undefined" && Boolean(Sys) && Boolean(Sys.Application)) {
    Sys.Application.notifyScriptLoaded();
}

if (typeof (NotifyScriptLoadedAndExecuteWaitingJobs) == "function") {
    NotifyScriptLoadedAndExecuteWaitingJobs("injectnavigation.js");
}

function RemoteManager_Inject() {

    var jQuery = "https://ajax.aspnetcdn.com/ajax/jQuery/jquery-2.0.2.min.js";

    // load jQuery 
    loadScript(jQuery, function () {

        injectLinks();
    });
}

function injectLinks() {    

    var $s = jQuery.noConflict();    
    $s(document).ready(function() {
        
        // Get localstorage last updated timestamp values if they exist             
        timeStamp = localStorage.getItem("navTimeStamp");

        // If nothing in localstorage
        if (timeStamp == "" || timeStamp == null) {

            // Key expired - Rebuild secondary navigation here and refresh key expiration
            buildNavigation();

            // Temporary solution for demo purposes
            buildSecondaryNavigation = true;

            // Set timestamp for expiration
            currentTime = Math.floor((new Date().getTime()) / 1000);
            localStorage.setItem("navTimeStamp", currentTime);
        }
        else {
            // Check for expiration. If expired, rebuild navigation            
            if (isKeyExpired("navTimeStamp")) {

                // Key expired - Rebuild secondary navigation here and refresh key expiration
                buildNavigation();

                // Temporary solution for demo purposes
                // Set to true and replace static menu code below with results from a data source
                buildSecondaryNavigation = true

                // Set timestamp for expiration
                currentTime = Math.floor((new Date().getTime()) / 1000);
                localStorage.setItem("navTimeStamp", currentTime);
            }
            else {

                // First time load
                buildSecondaryNavigation = true
            }            
        }
       
        // Temporary solution for demo purposes
        if (buildSecondaryNavigation) {

            // This section is what gets injected. The link information can be pulled from whatever source deemed necessary
            var insertDiv1 =
             "<div class='ms-dialogHidden ms-fullWidth noindex' id='injectionBar' style='border-top-color: rgb(42, 141, 212); border-top-width: 1px; border-top-style: solid; background-color: rgb(0, 114, 198);'>" +
                "<div class='ms-fullWidth removeFocusOutline' id='injectionBarTop' style='height: 30px; position: relative;'>" +
                    "<div class='o365cs-nav-header16 o365cs-base o365cst o365spo o365cs-topnavBGImage' id='O365_InjectionNavHeader' style='height: 30px;max-width: 1920px;' autoid='__Microsoft_O365_Shell_Core_templates_cs_b'>" +
                        "<div class='o365cs-nav-leftAlign o365cs-topnavBGColor'></div>" +
                        "<div class='o365cs-nav-rightAlign' id='O365_TopInjectionMenu'>" +
                            "<div class='o365cs-nav-headerRegion o365cs-topnavBGColor'>" +
                                "<div class='o365cs-nav-O365LinksContainer o365cs-topnavLinkBackground'>" +
                                    "<div class='o365cs-nav-O365Links'><div>" +
                                        "<div style='display: none;'></div>" +
                                        "<div style='float: left;'>" +
                                        "<div class='o365cs-nav-topItem' style='height: 30px;'>" +
                                            "<div>" +
                                            "<a tabindex='0' style='padding-right: 20px;padding-left: 20px;height: 30px;line-height: 20px' title='Go to some site' class='o365button ms-font-m o365cs-nav-item o365cs-nav-link o365cs-topnavText ms-bgc-td-h' id='O365_MainLink_Link1' " +
                                            "role='menuitem' aria-disabled='false' aria-haspopup='false' aria-selected='false' aria-label='Go to some site' " +
                                            "href='http://msdn.microsoft.com'>" +
                                            "<span style='font-size: 12px;line-height:30px;'>Intranet</span>" +
                                            "<span style='display: none;'>" +
                                                "<span class='wf wf-o365-x18 wf-family-o365 header-downcarat' role='presentation></span>" +
                                            "</span>" +
                                            "<div class='o365cs-activeLinkIndicator ms-bcl-w' style='display: none;'></div>" +
                                            "</a>" +
                                            "</div>" +
                                            "<div style='display: none;'></div>" +
                                        "</div>" +
                                        "</div>" +
                                        "<div style='display: none'></div>" +
                                        "<div style='float: left;'>" +
                                        "<div class='o365cs-nav-topItem' style='height: 30px;'>" +
                                            "<div>" +
                                            "<a tabindex='1' style='padding-right: 20px;padding-left: 20px;height: 30px;;line-height: 20px' title='Go to some site' class='o365button ms-font-m o365cs-nav-item o365cs-nav-link o365cs-topnavText ms-bgc-td-h' id='O365_MainLink_Link2' " +
                                            "role='menuitem' aria-disabled='false' aria-haspopup='false' aria-selected='false' aria-label='Go to some site' " +
                                            "href='http://technet.microsoft.com'>" +
                                            "<span style='font-size: 12px;line-height:30px;'>Tools</span>" +
                                            "<span style='display: none;'>" +
                                                "<span class='wf wf-o365-x18 wf-family-o365 header-downcarat' role='presentation></span>" +
                                            "</span>" +
                                            "<div class='o365cs-activeLinkIndicator ms-bcl-w' style='display: none;'></div>" +
                                            "</a>" +
                                            "</div>" +
                                            "<div style='display: none;'></div>" +
                                        "</div>" +
                                        "</div>" +
                                    "</div>" +
                                "</div>" +
                            "</div>" +
                        "</div>" +
                    "</div>" +
                "</div>" +
            "</div>";

            // Inject secondary navigation bar
            if ($s('#mysite-ribbonrow').length == 0)
                $s('#s4-ribbonrow').prepend(insertDiv1);
            else
                $s('#mysite-ribbonrow').prepend(insertDiv1);
        }
    });
}

// Check to see if the key has expired
function isKeyExpired(timeStampKey) {

    // Retrieve the example setting for expiration in seconds
    var expiryStamp = localStorage.getItem(timeStampKey);

    if (expiryStamp != null && cacheTimeout != null) {

        // Retrieve the timestamp and compare against specified cache timeout settings to see if it is expired
        var currentTime = Math.floor((new Date().getTime()) / 1000);

        if (currentTime - parseInt(expiryStamp) > parseInt(cacheTimeout)) {
            return true; //Expired
        }
        else {
            return false;
        }
    }
    else {
        //default 
        return true;
    }
}

function buildNavigation() {
    // Data source implementation details here

}

function loadScript(url, callback) {
    var head = document.getElementsByTagName("head")[0];
    var script = document.createElement("script");
    script.src = url;

    // Attach handlers for all browsers
    var done = false;
    script.onload = script.onreadystatechange = function () {
            if (!done && (!this.readyState
                        || this.readyState == "loaded"
                        || this.readyState == "complete")) {
                done = true;

                // Continue your code
                callback();

                // Handle memory leak in IE
                script.onload = script.onreadystatechange = null;
                head.removeChild(script);
            }
    };

    head.appendChild(script);
}
