var jQuery = "https://ajax.aspnetcdn.com/ajax/jQuery/jquery-2.0.2.min.js";
var sharingStatus = "";
var policy = "";
var statusBarBackground;
var hostUrl;
var subSiteUrl;
var currentUrl;

// Register script for MDS if possible
// Is MDS enabled?
if ("undefined" != typeof g_MinimalDownload && g_MinimalDownload && (window.location.pathname.toLowerCase()).endsWith("/_layouts/15/start.aspx") && "undefined" != typeof asyncDeltaManager) {
    // Register script for MDS if possible
    RegisterModuleInit("siteprivacy.js", JavaScript_Embed); //MDS registration
    JavaScript_Embed(); //non MDS run
} else {
    JavaScript_Embed();
}

function JavaScript_Embed() {

    loadScript(jQuery, function () {
        $(document).ready(function () {
            var message = "";

            
           
            // Execute status setter only after SP.JS has been loaded
            SP.SOD.executeOrDelayUntilScriptLoaded(function () {
                //alert("Site Collection: " + window.location.host + _spPageContextInfo.siteServerRelativeUrl);
                //alert("Sub Site: " + _spPageContextInfo.webAbsoluteUrl);

                hostUrl = window.location.host + _spPageContextInfo.siteServerRelativeUrl;
                subSiteUrl = _spPageContextInfo.webAbsoluteUrl;
                if (hostUrl != subSiteUrl) {
                    currentUrl = subSiteUrl;
                }
                else {
                    currentUrl = hostUrl;
                }

                var appweb = _spPageContextInfo.isAppWeb;
                if (appweb == false && appweb != 'undefined')
                {
                    getClassifier();
                }

            }, 'sp.js');
        });
    });
}

function getClassifier() {    
    var clientContext = SP.ClientContext.get_current();
    var web = clientContext.get_web();

   
    
    var props = web.get_allProperties();
    clientContext.load(web);
    clientContext.load(props);

    clientContext.executeQueryAsync(Function.createDelegate(this, function (sender, args) {
        try{
            policy = props.get_item('PolicyName');
            getSiteSharingStatus();
        }
        catch(e){
            policy = "No Site Policy";
            getSiteSharingStatus();
        }
        
    }));

    //return policy;
}

function SetStatusBar(message, bgColor) {
    strUpdatedStatusID = SP.UI.Status.addStatus("Attention: ", message, true);
    SP.UI.Status.setStatusPriColor(strUpdatedStatusID, bgColor);

}

function IsOnPage(pageName) {
    if (window.location.href.toLowerCase().indexOf(pageName.toLowerCase()) > -1) {
        return true;
    } else {
        return false;
    }
}

function getSiteSharingStatus() {   
    if (hostUrl == subSiteUrl) {
        var clientContext = SP.ClientContext.get_current();
        var web = clientContext.get_web();
        var props = web.get_allProperties();
        clientContext.load(web);
        clientContext.load(props);

        clientContext.executeQueryAsync(Function.createDelegate(this, function (sender, args) {
            sharingStatus = props.get_item('_site_props_externalsharing');
            setUI();

        }));

        return sharingStatus;
    }
    else {
        var clientContext = SP.ClientContext.get_current();
        var site = clientContext.get_site();
        var web = site.get_rootWeb();
        var props = web.get_allProperties();
        clientContext.load(web);
        clientContext.load(props);

        clientContext.executeQueryAsync(Function.createDelegate(this, function (sender, args) {
            sharingStatus = props.get_item('_site_props_externalsharing');
            setUI();

        }));

        return sharingStatus;
    }
    
}

function setUI() {
    if (policy == 'High Business Impact') {
        //alert("Sharing: " + sharingStatus);
        if (sharingStatus == "true" || sharingStatus == "True") {
            message = "<font color='#000000'><a href='https://yoursitepolicy' target=_blank>Information on this site has been classified as <b>High Business Impact</b> and <b>Partner Sharing is enabled</b></a></font>";
        }
        else {
            message = "<font color='#000000'><a href='https://yoursitepolicy' target=_blank>Information on this site has been classified as <b>High Business Impact</b></a></font>";
        }
        statusBarBackground = "#f0f0f0";
    }
    else if (policy == 'Medium Business Impact') {
        if (sharingStatus == "true" || sharingStatus == "True") {
            message = "<font color='#000000'><a href='https://yoursitepolicy' target=_blank>Information on this site has been classified as <b>Medium Business Impact</b> and <b>Partner Sharing is enabled</b></a></font>";
        }
        else {
            message = "<font color='#000000'><a href='https://yoursitepolicy' target=_blank>Information on this site has been classified as <b>Medium Business Impact</b></a></font>";
        }
        statusBarBackground = "#f0f0f0";
    }
    else if (policy == 'Low Business Impact') {
        if (sharingStatus == "true" || sharingStatus == "True") {
            message = "<font color='#000000'><a href='https://yoursitepolicy' target=_blank>Information on this site has been classified as <b>Low Business Impact</b> and <b>Partner Sharing is enabled</b></a></font>";
        }
        else {
            message = "<font color='#000000'><a href='https://yoursitepolicy' target=_blank>Information on this site has been classified as <b>Low Business Impact</b></a></font>";
        }
        statusBarBackground = "#f0f0f0";
    }    
    else {
        if (sharingStatus == "true" || sharingStatus == "True") {
            message = "<font color='#000000'>Information on this site has not yet been classified.  Click <a href='" + currentUrl + "/_layouts/15/ProjectPolicyAndLifecycle.aspx'>here</a> to set the Policy. <b>Partner sharing is enabled</b></font>";
        }
        else {
            message = "<font color='#000000'>Information on this site has not yet been classified.  Click <a href='" + currentUrl + "/_layouts/15/ProjectPolicyAndLifecycle.aspx'>here</a> to set the Policy.</font>";
        }
        statusBarBackground = "yellow";
        
    }

    // add code to set a policy (reminder red) This sub-site does not have a policy set. Click here to set

    SetStatusBar(message, statusBarBackground);
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