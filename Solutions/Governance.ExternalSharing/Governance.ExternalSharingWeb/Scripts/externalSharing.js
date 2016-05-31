// SCRIPT TO HANDLE SETTING THE STATUS BAR THAT EXTERNAL SHARING IS ENABLED ON THIS SITE.
var jQuery = "https://ajax.aspnetcdn.com/ajax/jQuery/jquery-2.0.2.min.js";

// Is MDS enabled?
if ("undefined" != typeof g_MinimalDownload && g_MinimalDownload && (window.location.pathname.toLowerCase()).endsWith("/_layouts/15/start.aspx") && "undefined" != typeof asyncDeltaManager) {
    // Register script for MDS if possible
    RegisterModuleInit("externalSharing.js", JavaScript_Embed); //MDS registration
    JavaScript_Embed(); //non MDS run
} else {
    JavaScript_Embed();
}

function JavaScript_Embed() {

    loadScript(jQuery, function () {
        $(document).ready(function () {
            var message = "This site can be shared with people outside of Contoso"
            // Execute status setter only after SP.JS has been loaded
            SP.SOD.executeOrDelayUntilScriptLoaded(function () { SetStatusBar(message); }, 'sp.js');
        });
    });
}

function SetStatusBar(message) {
    var statusId = SP.UI.Status.addStatus(message);
    SP.UI.Status.setStatusPriColor(statusId, "#f0f0f0");


}

function IsOnPage(pageName) {
    if (window.location.href.toLowerCase().indexOf(pageName.toLowerCase()) > -1) {
        return true;
    } else {
        return false;
    }
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