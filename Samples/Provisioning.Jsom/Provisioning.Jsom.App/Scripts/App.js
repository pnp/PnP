'use strict';

var context = SP.ClientContext.get_current();
var user = context.get_web().get_currentUser();

var hostweburl;
var appweburl;

//Get the URI decoded URLs. 
hostweburl = decodeURIComponent(getQueryStringParameter("SPHostUrl"));
appweburl = decodeURIComponent(getQueryStringParameter("SPAppWebUrl"));

// This code runs when the DOM is ready and creates a context object which is needed to use the SharePoint object model
$(document).ready(function () {
    var clerk = new Contoso.JSOMProvisioning.ProvisioningClerk();
    clerk.set_appweburl(appweburl);
    clerk.set_hostweburl(hostweburl);
    clerk.set_webtitle($("#sitetitle").text());
    clerk.set_weburl($("#siteurl").text());
    clerk.set_webtemplate($("#sitetemplate").text());
    clerk.set_sitecolumnname($("#sitecolumnname").text());
    clerk.set_sitecolumndisplayname($("#sitecolumndisplayname").text());
    clerk.set_sitecolumntype($("#sitecolumntype").text());
    clerk.set_contenttypename($("#contenttypename").text());
    clerk.set_contenttypeid($("#contenttypeid").text());
    clerk.set_documentlibraryname($("#doclibname").text());
    clerk.set_filename($("#docname").text());
    clerk.set_filetitle($("#doctitle").text());
    clerk.set_filefavoritecolor($("#doccolor").text());
    $("#createsite").click(function () { provisionsite(); });
    $("#deletesite").click(function () { cleanup(); });
    function provisionsite() {
        clerk.createsitecolumn()
        .then(
            function () {
                showMessage("Site column provisioned");
                
                return clerk.createcontenttype();
            },
            function () { console.log("Site column could not be created."); })
        .then(
            function () {
                showMessage("Content type provisioned");
                
                return clerk.createsite();
            },
            function () { console.log("Content type could not be created."); })
        .then(
            function () {
                showMessage("Site provisioned");
                return clerk.createdocumentlibrary();
            },
            function () { console.log("Site could not be created.") })
        .then(
            function () {
                showMessage("Document library provisioned");
                return clerk.createfile();
            },
            function () { console.log("Document library could not be created.") })
        .then(
            function () {
                showMessage("File provisioned");
            },
            function () { console.log("File could not be created.") })
    };
    function cleanup() {
        clerk.deletedocumentlibrary()
        .then(
            function () {
                showMessage("Document library deleted");
                return clerk.deletesite();
            },
            function () {
                console.log("Document library cleanup failed.");
                return clerk.deletesite();
            }
        )
        .then(
            function () {
                showMessage("Site deleted");
                return clerk.deletecontenttype();
            },
            function () {
                showMessage("Site cleanup failed.");
                return clerk.deletecontenttype();
            }
        )
        .then(
            function () {
                showMessage("Content type deleted");
                return clerk.deletesitecolumn();
            },
            function () {
                showMessage("Content type cleanup failed.");
                return clerk.deletesitecolumn();
            }
        )
        .then(
            function () {
                showMessage("Site column deleted");
            },
            function () {
                console.log("Site column cleanup failed.");
            }
        )
    };
    function showMessage(message) {
        SP.UI.Notify.addNotification(message, false);
        console.log(message);
    }
});


// Function to retrieve a query string value.  
function getQueryStringParameter(paramToRetrieve) {
    var params = document.URL.split("?")[1].split("&");
    var strParams = "";

    for (var i = 0; i < params.length; i = i + 1) {
        var singleParam = params[i].split("=");
        if (singleParam[0] == paramToRetrieve)
            return singleParam[1];
    }
}