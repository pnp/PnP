var jQuery = "https://ajax.aspnetcdn.com/ajax/jQuery/jquery-2.0.2.min.js";
// Is MDS enabled?
if ("undefined" != typeof g_MinimalDownload && g_MinimalDownload && (window.location.pathname.toLowerCase()).endsWith("/_layouts/15/start.aspx") && "undefined" != typeof asyncDeltaManager) {
    // Register script for MDS if possible
    RegisterModuleInit("scenario1.js", RemoteManager_InjectMDS); //MDS registration
    RemoteManager_Inject(); //non MDS run

    if (typeof (Sys) != "undefined" && Boolean(Sys) && Boolean(Sys.Application)) {
        Sys.Application.notifyScriptLoaded();
    }

    if (typeof (NotifyScriptLoadedAndExecuteWaitingJobs) == "function") {
        NotifyScriptLoadedAndExecuteWaitingJobs("scenario1.js");
    }

} else {
    loadScript(jQuery, function () {
        $(document).ready(function () {
            RemoteManager_Inject();
        });
    });
}

function RemoteManager_InjectMDS() {
    loadScript(jQuery, function () {
        RemoteManager_Inject();
    });
}

function RemoteManager_Inject() {

    var message = "<img src='/_Layouts/Images/STS_ListItem_43216.gif' align='absmiddle'> <font color='#AA0000'>JavaScript customization is <i>fun</i>!</font>"
    SetStatusBar(message);

    // Customize the viewlsts.aspx page
    if (IsOnPage("viewlsts.aspx")) {
        //hide the subsites link on the viewlsts.aspx page
        $("#createnewsite").parent().hide();
    }
}

function SetStatusBar(message) {
    var strStatusID = SP.UI.Status.addStatus("Information : ", message, true);
    SP.UI.Status.setStatusPriColor(strStatusID, "yellow");
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