var jQuery = "https://ajax.aspnetcdn.com/ajax/jQuery/jquery-2.0.2.min.js";

// Register script for MDS if possible
// Is MDS enabled?
if ("undefined" != typeof g_MinimalDownload && g_MinimalDownload && (window.location.pathname.toLowerCase()).endsWith("/_layouts/15/start.aspx") && "undefined" != typeof asyncDeltaManager) {
    // Register script for MDS if possible
    RegisterModuleInit("siteClassification.js", JavaScript_Embed); //MDS registration
    JavaScript_Embed(); //non MDS run
} else {
    JavaScript_Embed();
}


function JavaScript_Embed() {

    loadScript(jQuery, function () {
        $(document).ready(function () {
            // Execute status setter only after SP.JS has been loaded
            SP.SOD.executeOrDelayUntilScriptLoaded(function () { setClassifier(); }, 'sp.js');
        });
    });
}

function setClassifier() {
    var clientContext = SP.ClientContext.get_current();
    var web = clientContext.get_web();
    var props = web.get_allProperties();
    clientContext.load(web);
    clientContext.load(props);

    clientContext.executeQueryAsync(Function.createDelegate(this, function (sender, args) {
        var policy = props.get_item('PolicyName');
        if (policy == 'HBI') {
            var img = $("<a id='ctl_pnp_classifier' class='ms-promotedActionButton' style='display: inline-block;' href='#'><span class='s4-clust ms-promotedActionButton-icon' style='width: 28px; height: 30px; overflow: hidden; display: inline-block; position: relative;'><img alt='Share' src='" + hbiImageSource + "'></span>");
            $('#ctl00_site_share_button').before(img);
            classified = true;
        }
        else if (policy == 'MBI') {
            var img = $("<a id='ctl_pnp_classifier' class='ms-promotedActionButton' style='display: inline-block;' href='#'><span class='s4-clust ms-promotedActionButton-icon' style='width: 28px; height: 30px; overflow: hidden; display: inline-block; position: relative;'><img alt='Share' src='" + mbiImageSource + "'></span>");
            $('#ctl00_site_share_button').before(img);
            classified = true;
        }
        else if (policy == 'LBI') {
            var img = $("<a id='ctl_pnp_classifier' class='ms-promotedActionButton' style='display: inline-block;' href='#'><span class='s4-clust ms-promotedActionButton-icon' style='width: 28px; height: 30px; overflow: hidden; display: inline-block; position: relative;'><img alt='Share' src='" + lbiImageSource + "'></span>");
            $('#ctl00_site_share_button').before(img);
            classified = true;
        }
    }));
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