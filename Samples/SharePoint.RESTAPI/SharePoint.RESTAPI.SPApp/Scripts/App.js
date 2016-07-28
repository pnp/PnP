var hostweburl;
var appweburl;
var eTag;

// This code runs when the DOM is ready and creates a context object which is needed to use the SharePoint object model
$(document).ready(function () {

    //Get the URI decoded URLs. 
    hostweburl =
        decodeURIComponent(
            getQueryStringParameter("SPHostUrl")
    );
    appweburl =
        decodeURIComponent(
            getQueryStringParameter("SPAppWebUrl")
    );

    var scriptbase = hostweburl + "/_layouts/15/";

    $.getScript(scriptbase + "SP.RequestExecutor.js", execCrossDomainRequest);
});

function execCrossDomainRequest() {

    var contextInfoUri = appweburl + "/_api/contextinfo"; 
    var itemUri = appweburl +
            "/_api/SP.AppContextSite(@target)/web/lists/GetByTitle('Documents')/Items(1)?@target='" +
            hostweburl + "'";

    var executor = new SP.RequestExecutor(appweburl);

    executor.executeAsync({ 
        url: contextInfoUri, 
        method: "POST", 
        headers: { "Accept": "application/json; odata=verbose" }, 
        success: function (data) { 
            var jsonObject = JSON.parse(data.body); 
            formDigestValue = jsonObject.d.GetContextWebInformation.FormDigestValue; 
            updateListItem(formDigestValue, itemUri);
        }, 
        error: function (data, errorCode, errorMessage) { 
            var errMsg = "Error retrieving the form digest value: " 
                + errorMessage; 
            $("#error").text(errMsg);
        } 
    });
}

function updateListItem(formDigestValue, itemUri) {

    var executor = new SP.RequestExecutor(appweburl);
    var newContent = JSON.stringify({ '__metadata': { 'type': 'SP.Data.Shared_x0020_DocumentsItem' }, 'Title': 'Changed by REST API' });

    executor.executeAsync({
        url: itemUri,
        method: "GET",
        headers: { "Accept": "application/json; odata=verbose" },
        success: function (data) {
            $("#message").text('ETag: ' + data.headers["ETAG"]);
            eTag = data.headers["ETAG"];
            internalUpdateListItem(formDigestValue, itemUri, eTag, newContent);
        },
        error: function (data, errorCode, errorMessage) {
            var errMsg = "Error retrieving the eTag value: "
                + errorMessage;
            $("#error").text(errMsg);
        }
    });
}

function internalUpdateListItem(formDigestValue, itemUri, eTag, newContent) {

    var executor = new SP.RequestExecutor(appweburl);

    executor.executeAsync({
        url:
            appweburl +
            "/_api/SP.AppContextSite(@target)/web/lists/GetByTitle('Documents')/Items(1)?@target='" +
            hostweburl + "'",
        method: "POST",
        body: newContent,
        headers: {
            "Accept": "application/json;odata=verbose",
            "content-type": "application/json;odata=verbose",
            "content-length": newContent.length,
            "X-RequestDigest": formDigestValue, // $("#__REQUESTDIGEST").val(),
            "X-HTTP-Method": "MERGE",
            "IF-MATCH": eTag
        },
        success: function (data) {
            $("#message").text('Item succesfully updated!');
        },
        error: function (data, errorCode, errorMessage) {
            var errMsg = "Error updating list item: "
                + errorMessage;
            $("#error").text(errMsg);
        }
    });
}

// Function to retrieve a query string value. 
// For production purposes you may want to use 
//  a library to handle the query string. 
function getQueryStringParameter(paramToRetrieve) {
    var params =
        document.URL.split("?")[1].split("&");
    var strParams = "";
    for (var i = 0; i < params.length; i = i + 1) {
        var singleParam = params[i].split("=");
        if (singleParam[0] == paramToRetrieve)
            return singleParam[1];
    }
}

function readCurrentUsername() {

    $.ajax({
        url: appweburl + "/_api/web/currentuser",
        method: "GET",
        headers: { "Accept": "application/json; odata=verbose" },
        success: function (data) {
            $("#message").text('Hello ' + data.d.Title);
        },
        error: function (err) {
            alert(err);
        }
    });

}