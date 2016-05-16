var jQuery = "https://ajax.aspnetcdn.com/ajax/jQuery/jquery-2.0.2.min.js";

// Register script for MDS if possible
// Is MDS enabled?
if ("undefined" != typeof g_MinimalDownload && g_MinimalDownload && (window.location.pathname.toLowerCase()).endsWith("/_layouts/15/start.aspx") && "undefined" != typeof asyncDeltaManager) {
    // Register script for MDS if possible
    RegisterModuleInit("breadcrumb.js", BreadcrumbJavaScript_Embed); //MDS registration
    BreadcrumbJavaScript_Embed(); //non MDS run
} else {
    BreadcrumbJavaScript_Embed();
}

function BreadcrumbJavaScript_Embed() {

    // load jQuery 
    loadScript(jQuery, function () {

        $(document).ready(function () {
            UpdateBreadcrumbs();
           
        });

    });
}

function UpdateBreadcrumbs()
{
    //var css = '.ms-breadcrumb-dropdownBox { display: inline-block !important; }';
    //$('<link rel="stylesheet" href="../styles/app.css" />').appendTo(document.head);
    
    //head = document.head || document.getElementsByTagName('head')[0];
    //style = document.createElement('style');

    //style.type = 'text/css';
    //if (style.styleSheet) {
    //    style.styleSheet.cssText = css;
    //} else {
    //    style.appendChild(document.createTextNode(css));
    //}

    //head.appendChild(style);

    var deltaBreadCrumbDropdown = document.getElementsByClassName("ms-breadcrumb-dropdownbox");
    var globalBreadCrumbNavPopout = document.getElementsByTagName("SharePoint:PopoutMenu")[0];

    if (deltaBreadCrumbDropdown != null)
    {
        var img = $("<style type='text/css'>.ms-breadcrumb-dropdownBox { display: inline-block !important; }</script>");
        $('zz12_RootAspMenu').before(img);
    }

    if (globalBreadCrumbNavPopout != null)
    {
        globalBreadCrumbNavPopout.attributes["Visible"] = "true";
        globalBreadCrumbNavPopout.attributes["IconUrl"] = "/_layouts/15/images/spcommon.png?rev=23";
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
