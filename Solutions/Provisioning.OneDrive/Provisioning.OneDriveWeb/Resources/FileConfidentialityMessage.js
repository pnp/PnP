// Register script for MDS if possible
RegisterModuleInit("FileConfidentialityMessage.js", RemoteManager_Inject); //MDS registration
RemoteManager_Inject(); //non MDS run

if (typeof (Sys) != "undefined" && Boolean(Sys) && Boolean(Sys.Application)) {
    Sys.Application.notifyScriptLoaded();
}

if (typeof (NotifyScriptLoadedAndExecuteWaitingJobs) == "function") {
    NotifyScriptLoadedAndExecuteWaitingJobs("FileConfidentialityMessage.js");
}

function RemoteManager_Inject() {

    var jQuery = "https://ajax.aspnetcdn.com/ajax/jQuery/jquery-2.0.2.min.js";

    // load jQuery and if complete load the js resource file
    loadScript(jQuery, function () {

        // General changes that apply to all loaded pages come here
        // ----------------------------------------------------------
        var message = "OneDrive for Business should not be used for storing high confidentiality files - Check <a href='#'>company policy</a>. <img src='/_Layouts/Images/info16by16.gif' align='absmiddle'>"

        // Execute status setter only after SP.JS has been loaded
        SP.SOD.executeOrDelayUntilScriptLoaded(function () { SetStatusBar(message); }, 'sp.js');

        // Page specific changes are conditioned via an IsOnPage call
        // ----------------------------------------------------------

        // Customize the viewlsts.aspx page
        if (IsOnPage("viewlsts.aspx")) {
            //hide the subsites link on the viewlsts.aspx page
            $("#createnewsite").parent().hide();
        }

    });
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
