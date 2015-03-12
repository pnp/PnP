if ("undefined" != typeof g_MinimalDownload && g_MinimalDownload && (window.location.pathname.toLowerCase()).endsWith("/_layouts/15/start.aspx") && "undefined" != typeof asyncDeltaManager) {
    // Register script for MDS if possible
    RegisterModuleInit("scenario3.js", RemoteManager_Inject); //MDS registration
    RemoteManager_Inject(); //non MDS run
} else {
    RemoteManager_Inject();
}

//Variables used to control the asynchronous requests
var asyncReqExecutedTime = 350; //Milliseconds
var columnReqExecuted = false;
var columnReqCount = 1
var pendingReqExecuted = false;
var pendingReqCount = 1

function RemoteManager_Inject() {

    var jQuery = "https://ajax.aspnetcdn.com/ajax/jQuery/jquery-2.0.2.min.js";

    // load jQuery and if complete load the js resource file
    loadScript(jQuery, function () {

        var scriptUrl = "";
        var scriptRevision = "";
        // iterate the loaded scripts to find the scenario3 script. We use the script URL to dynamically build the url for the resource file to be loaded.
        $('script').each(function (i, el) {
            if (el.src.toLowerCase().indexOf('scenario3.js') > -1) {
                scriptUrl = el.src;
                scriptRevision = scriptUrl.substring(scriptUrl.indexOf('.js') + 3);
                scriptUrl = scriptUrl.substring(0, scriptUrl.indexOf('.js'));
            }
        })

        var resourcesFile = scriptUrl + "." + _spPageContextInfo.currentUICultureName.toLowerCase() + ".js" + scriptRevision;
        // load the JS resource file based on the user's language
        loadScript(resourcesFile, function () {

            // General changes that apply to all loaded pages come here
            // ----------------------------------------------------------



            // Page specific changes are conditioned via an IsOnPage call
            // ----------------------------------------------------------
            // Update the column headers for the demo list
            if (IsOnPage("Lists/Demo/AllItems.aspx")) {
                setTimeout(ColumnReq, asyncReqExecutedTime);
            }

            // Update the number of roles shown in the access request screen. This sceen is dynimically loaded when the user clicks on the ellipsis (...) and 
            // since the screen does not trigger a pageload and the content only is available after a user click we need to use a pattern that
            // allows us to "plug in" our code when first the async load of the XSLT based list is done and secondly when the user clicks on the ellipsis (...). 
            // Below script demonstrates this pattern
            if (IsOnPage("pendingreq.aspx")) {
                setTimeout(PendingReq, asyncReqExecutedTime);
            }
        });
    });
}

function ColumnReq() {
    // If we managed to execute this code then we're done
    if (columnReqExecuted) {
        return false;
    }

    //Place your actual "customization logic" here and don't forget the flag the request to be true when you're code did succeed
    //when the selector returns data this means that the XSLT based listview has finished loading and thus the request can be flagged as done
    $(".ms-vh-div").each(function () {
        if (this.innerText.toLowerCase().indexOf("column1") > -1) {
            $(this).text(Column1_Title);
        } if (this.innerText.toLowerCase().indexOf("column2") > -1) {
            $(this).text(Column2_Title);
        }
        columnReqExecuted = true;
    });    

    // we apparently did not manage to do the change since the element where not yet loaded. Schedule another retry
    if (!columnReqExecuted) {
        // We've tried too many times...something must be wrong here
        if (columnReqCount > 15) {
            columnReqExecuted = true;
        }
        else {
            columnReqCount = columnReqCount + 1;
            // setup the next attempt
            setTimeout(ColumnReq, asyncReqExecutedTime);
        }
    }
}

function PendingReq() {

    // If we managed to execute this code then we're done
    if (pendingReqExecuted) {
        return false;
    }

    //Place your actual "customization logic" here and don't forget the flag the request to be true when you're code did succeed
    $("a.ms-ellipsis-a").each(function (i, v) {
        if ($(this).attr('onclick')) {
            pendingReqExecuted = true;
            // bind our click event after the original click event, meaning SharePoint will first show the content on click and then our code 
            // will run and update the content loaded by SharePoint
            $(this).bind('click', function (e) {
                $(".ms-accRqCllOt-PrmCmbBx").find('option').each(function () {
                    if ($(this).html().toLowerCase().indexOf("[edit]") > -1 ||
                        $(this).html().toLowerCase().indexOf("[read]") > -1 ) {
                        // do nothing as we want to leave this entries in the list
                    } else {
                        // all entries, except the "placeholder" do have an # in their value and thus need to be deleted
                        if ($(this).attr("value").indexOf("#") > -1) {
                            $(this).remove();
                        }
                    }
                });
            });
        }
    });

    // we apparently did not manage to do the change since the element where not yet loaded. Schedule another retry
    if (!pendingReqExecuted) {
        // We've tried too many times...something must be wrong here
        if (pendingReqCount > 15) {
            pendingReqExecuted = true;
        }
        else {
            pendingReqCount = pendingReqCount + 1;
            // setup the next attempt
            setTimeout(PendingReq, asyncReqExecutedTime);
        }
    }
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
