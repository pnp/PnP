var hostweburl;
var appweburl;
var eTag;
var formDigestValue;

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

    $.getScript(scriptbase + "SP.RequestExecutor.js", retrieveFormDigest);
});

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

function clearStatus() {
    $("#message").text("");
    $("#error").text("");
}

// Listing 9-9
function retrieveFormDigest() {

    var contextInfoUri = appweburl + "/_api/contextinfo";
    var executor = new SP.RequestExecutor(appweburl);

    executor.executeAsync({
        url: contextInfoUri,
        method: "POST",
        headers: { "Accept": "application/json; odata=verbose" },
        success: function (data) {
            var jsonObject = JSON.parse(data.body);
            formDigestValue = jsonObject.d.GetContextWebInformation.FormDigestValue;
        },
        error: function (data, errorCode, errorMessage) {
            var errMsg = "Error retrieving the form digest value: "
                + errorMessage;
            $("#error").text(errMsg);
        }
    });
}

// Listing 9-10
function createNewList() {
    clearStatus();
    retrieveFormDigest();

    var executor = new SP.RequestExecutor(appweburl);
    var operationUri = appweburl +
        "/_api/SP.AppContextSite(@target)/web/lists?@target='" +
        hostweburl + "'";

    var bodyContent = JSON.stringify(
        {
            '__metadata': { 'type': 'SP.List' },
            'AllowContentTypes': true,
            'BaseTemplate': 100,
            'ContentTypesEnabled': true,
            'Description': 'Custom List created via REST API',
            'Title': 'RESTCreatedList'
        }
    );

    executor.executeAsync({
        url: operationUri,
        method: "POST",
        headers: {
            "Accept": "application/json;odata=verbose",
            "content-type": "application/json;odata=verbose",
            "content-length": bodyContent.length,
            "X-RequestDigest": formDigestValue, // $("#__REQUESTDIGEST").val(),
        },
        body: bodyContent,
        success: function (data) {
            var jsonObject = JSON.parse(data.body);
            $("#message").text("Operation completed!");
        },
        error: function (data, errorCode, errorMessage) {
            var jsonObject = JSON.parse(data.body);
            var errMsg = "Error: " + jsonObject.error.message.value;
            $("#error").text(errMsg);
        }
    });
}

// Listing 9-11
function createNewListItem() {
    clearStatus();
    retrieveFormDigest();

    var executor = new SP.RequestExecutor(appweburl);
    var operationUri = appweburl +
        "/_api/SP.AppContextSite(@target)/web/lists/GetByTitle('RESTCreatedList')/Items?@target='" +
        hostweburl + "'";

    var bodyContent = JSON.stringify(
        {
            '__metadata': { 'type': 'SP.Data.RESTCreatedListListItem' },
            'Title': 'Item created via REST API'
        }
    );

    executor.executeAsync({
        url: operationUri,
        method: "POST",
        headers: {
            "Accept": "application/json;odata=verbose",
            "content-type": "application/json;odata=verbose",
            "content-length": bodyContent.length,
            "X-RequestDigest": formDigestValue, // $("#__REQUESTDIGEST").val(),
        },
        body: bodyContent,
        success: function (data) {
            var jsonObject = JSON.parse(data.body);
            $("#message").text("Operation completed!");
        },
        error: function (data, errorCode, errorMessage) {
            var jsonObject = JSON.parse(data.body);
            var errMsg = "Error: " + jsonObject.error.message.value;
            $("#error").text(errMsg);
        }
    });
}

// Listing 9-12
function updateListItem() {
    clearStatus();
    retrieveFormDigest();

    var executor = new SP.RequestExecutor(appweburl);
    var operationUri = appweburl +
        "/_api/SP.AppContextSite(@target)/web/lists/GetByTitle('RESTCreatedList')/Items(1)?@target='" +
        hostweburl + "'";

    var bodyContent = JSON.stringify(
        {
            '__metadata': { 'type': 'SP.Data.RESTCreatedListListItem' },
            'Title': 'Item changed via REST API'
        }
    );

    // Retrieve the eTag value
    executor.executeAsync({
        url: operationUri,
        method: "GET",
        headers: { "Accept": "application/json; odata=verbose" },
        success: function (data) {
            $("#message").text('ETag: ' + data.headers["ETAG"]);
            eTag = data.headers["ETAG"];

            // Invoke the real update operation
            executor.executeAsync({
                url: operationUri,
                method: "POST",
                headers: {
                    "Accept": "application/json;odata=verbose",
                    "content-type": "application/json;odata=verbose",
                    "content-length": bodyContent.length,
                    "X-RequestDigest": formDigestValue, // $("#__REQUESTDIGEST").val(),
                    "X-HTTP-Method": "MERGE",
                    "IF-MATCH": eTag
                },
                body: bodyContent,
                success: function (data) {
                    $("#message").text("Operation completed!");
                },
                error: function (data, errorCode, errorMessage) {
                    var jsonObject = JSON.parse(data.body);
                    var errMsg = "Error: " + jsonObject.error.message.value;
                    $("#error").text(errMsg);
                }
            });
        },
        error: function (data, errorCode, errorMessage) {
            var jsonObject = JSON.parse(data.body);
            var errMsg = "Error retrieving the eTag value: " + jsonObject.error.message.value;
            $("#error").text(errMsg);
        }
    });
}

// Listing 9-13
function deleteListItem() {
    clearStatus();
    retrieveFormDigest();

    var executor = new SP.RequestExecutor(appweburl);
    var operationUri = appweburl +
        "/_api/SP.AppContextSite(@target)/web/lists/GetByTitle('RESTCreatedList')/Items(1)?@target='" +
        hostweburl + "'";

    // Retrieve the eTag value
    executor.executeAsync({
        url: operationUri,
        method: "GET",
        headers: { "Accept": "application/json; odata=verbose" },
        success: function (data) {
            $("#message").text('ETag: ' + data.headers["ETAG"]);
            eTag = data.headers["ETAG"];

            // Invoke the real update operation
            executor.executeAsync({
                url: operationUri,
                method: "POST",
                headers: {
                    "Accept": "application/json;odata=verbose",
                    "content-type": "application/json;odata=verbose",
                    "X-RequestDigest": formDigestValue, // $("#__REQUESTDIGEST").val(),
                    "X-HTTP-Method": "DELETE",
                    "IF-MATCH": eTag
                },
                success: function (data) {
                    $("#message").text("Operation completed!");
                },
                error: function (data, errorCode, errorMessage) {
                    var jsonObject = JSON.parse(data.body);
                    var errMsg = "Error: " + jsonObject.error.message.value;
                    $("#error").text(errMsg);
                }
            });
        },
        error: function (data, errorCode, errorMessage) {
            var jsonObject = JSON.parse(data.body);
            var errMsg = "Error retrieving the eTag value: " + jsonObject.error.message.value;
            $("#error").text(errMsg);
        }
    });
}

// Listing 9-14
function queryListItems() {
    clearStatus();

    var executor = new SP.RequestExecutor(appweburl);
    var operationUri = appweburl +
        "/_api/SP.AppContextSite(@target)/web/lists/GetByTitle('Sample%20Contacts')/Items?@target='" +
        hostweburl + "'&$filter=Company%20eq%20'OfficeDevPnP'";

    executor.executeAsync({
        url: operationUri,
        method: "GET",
        headers: { "Accept": "application/json;odata=verbose" },
        success: function (data) {
            var jsonObject = JSON.parse(data.body);
            $("#message").empty();

            for (var i = 0; i < jsonObject.d.results.length; i++)
            {
                var item = jsonObject.d.results[i];
                $("#message").append("<div>" + item.Title + "</div>");
            }
        },
        error: function (data, errorCode, errorMessage) {
            var jsonObject = JSON.parse(data.body);
            var errMsg = "Error: " + jsonObject.error.message.value;
            $("#error").text(errMsg);
        }
    });
}

// Listing 9-15
function createNewLibrary() {
    clearStatus();
    retrieveFormDigest();

    var executor = new SP.RequestExecutor(appweburl);
    var operationUri = appweburl +
        "/_api/SP.AppContextSite(@target)/web/lists?@target='" +
        hostweburl + "'";

    var bodyContent = JSON.stringify(
        {
            '__metadata': { 'type': 'SP.List' },
            'AllowContentTypes': true,
            'BaseTemplate': 101,
            'ContentTypesEnabled': true,
            'Description': 'Custom Library created via REST API',
            'Title': 'RESTCreatedLibrary'
        }
    );

    executor.executeAsync({
        url: operationUri,
        method: "POST",
        headers: {
            "Accept": "application/json;odata=verbose",
            "content-type": "application/json;odata=verbose",
            "content-length": bodyContent.length,
            "X-RequestDigest": formDigestValue, // $("#__REQUESTDIGEST").val(),
        },
        body: bodyContent,
        success: function (data) {
            var jsonObject = JSON.parse(data.body);
            $("#message").text("Operation completed!");
        },
        error: function (data, errorCode, errorMessage) {
            var jsonObject = JSON.parse(data.body);
            var errMsg = "Error: " + jsonObject.error.message.value;
            $("#error").text(errMsg);
        }
    });
}

// Listing 9-16
function uploadFile() {
    clearStatus();
    retrieveFormDigest();

    var executor = new SP.RequestExecutor(appweburl);
    var operationUri = appweburl +
        "/_api/SP.AppContextSite(@target)/web/lists/GetByTitle('Documents')/RootFolder/Files/Add(url='SampleFile.xml',overwrite=true)?@target='" +
        hostweburl + "'";

    var xmlDocument = "<?xml version='1.0'?><document><title>Uploaded via REST API</title></document>";

    executor.executeAsync({
        url: operationUri,
        method: "POST",
        headers: {
            "Accept": "application/json;odata=verbose",
            "content-type": "text/xml",
            "content-length": xmlDocument.length,
            "X-RequestDigest": formDigestValue, // $("#__REQUESTDIGEST").val(),
        },
        body: xmlDocument,
        success: function (data) {
            var jsonObject = JSON.parse(data.body);
            $("#message").text("Operation completed!");
        },
        error: function (data, errorCode, errorMessage) {
            var jsonObject = JSON.parse(data.body);
            var errMsg = "Error: " + jsonObject.error.message.value;
            $("#error").text(errMsg);
        }
    });
}

// Listing 9-17
function updateFile() {
    clearStatus();
    retrieveFormDigest();

    var executor = new SP.RequestExecutor(appweburl);
    var operationUri = appweburl +
        "/_api/SP.AppContextSite(@target)/web/GetFileByServerRelativeUrl('/sites/PnPDev/Shared%20Documents/SampleFile.xml')/$value?@target='" +
        hostweburl + "'";

    var xmlDocument = "<?xml version='1.0'?><document><title>File updated via REST API</title></document>";

    executor.executeAsync({
        url: operationUri,
        method: "POST",
        headers: {
            "Accept": "application/json;odata=verbose",
            "content-type": "text/xml",
            "content-length": xmlDocument.length,
            "X-HTTP-Method": "PUT",
            "X-RequestDigest": formDigestValue, // $("#__REQUESTDIGEST").val(),
        },
        body: xmlDocument,
        success: function (data) {
            $("#message").text("Operation completed!");
        },
        error: function (data, errorCode, errorMessage) {
            var jsonObject = JSON.parse(data.body);
            var errMsg = "Error: " + jsonObject.error.message.value;
            $("#error").text(errMsg);
        }
    });
}

// Listing 9-18
function checkOutFile() {
    clearStatus();
    retrieveFormDigest();

    var executor = new SP.RequestExecutor(appweburl);
    var operationUri = appweburl +
        "/_api/SP.AppContextSite(@target)/web/GetFileByServerRelativeUrl('/sites/PnPDev/Shared%20Documents/SampleFile.xml')/CheckOut()?@target='" +
        hostweburl + "'";

    executor.executeAsync({
        url: operationUri,
        method: "POST",
        headers: {
            "Accept": "application/json;odata=verbose",
            "X-RequestDigest": formDigestValue, // $("#__REQUESTDIGEST").val(),
        },
        success: function (data) {
            var jsonObject = JSON.parse(data.body);
            $("#message").text("Operation completed!");
        },
        error: function (data, errorCode, errorMessage) {
            var jsonObject = JSON.parse(data.body);
            var errMsg = "Error: " + jsonObject.error.message.value;
            $("#error").text(errMsg);
        }
    });
}

// Listing 9-19
function checkInFile() {
    clearStatus();
    retrieveFormDigest();

    var executor = new SP.RequestExecutor(appweburl);
    var operationUri = appweburl +
        "/_api/SP.AppContextSite(@target)/web/GetFileByServerRelativeUrl('/sites/PnPDev/Shared%20Documents/SampleFile.xml')/CheckIn?@target='" +
        hostweburl + "'";
    
    var bodyContent = JSON.stringify(
        { 
            'comment': 'Checked in via REST',
            'checkInType': 1
        });

    executor.executeAsync({
        url: operationUri,
        method: "POST",
        headers: {
            "Accept": "application/json;odata=verbose",
            "Content-type": "application/json;odata=verbose",
            "Content-length": bodyContent.length,
            "X-RequestDigest": formDigestValue, // $("#__REQUESTDIGEST").val(),
        },
        body: bodyContent,
        success: function (data) {
            var jsonObject = JSON.parse(data.body);
            $("#message").text("Operation completed!");
        },
        error: function (data, errorCode, errorMessage) {
            var jsonObject = JSON.parse(data.body);
            var errMsg = "Error: " + jsonObject.error.message.value;
            $("#error").text(errMsg);
        }
    });
}

// Listing 9-20
function deleteFile() {
    clearStatus();
    retrieveFormDigest();

    var executor = new SP.RequestExecutor(appweburl);
    var operationUri = appweburl +
        "/_api/SP.AppContextSite(@target)/web/GetFileByServerRelativeUrl('/sites/PnPDev/Shared%20Documents/SampleFile.xml')?@target='" +
        hostweburl + "'";

    executor.executeAsync({
        url: operationUri,
        method: "POST",
        headers: {
            "Accept": "application/json;odata=verbose",
            "X-HTTP-Method": "DELETE",
            "X-RequestDigest": formDigestValue, // $("#__REQUESTDIGEST").val(),
            "IF-MATCH": "*", // Discard concurrency checks
        },
        success: function (data) {
            $("#message").text("Operation completed!");
        },
        error: function (data, errorCode, errorMessage) {
            var jsonObject = JSON.parse(data.body);
            var errMsg = "Error: " + jsonObject.error.message.value;
            $("#error").text(errMsg);
        }
    });
}

// Listing 9-21
function queryDocuments() {
    clearStatus();

    var executor = new SP.RequestExecutor(appweburl);
    var operationUri = appweburl +
        "/_api/SP.AppContextSite(@target)/web/lists/GetByTitle('Documents')/RootFolder/Files?@target='" +
        hostweburl + "'";

    executor.executeAsync({
        url: operationUri,
        method: "GET",
        headers: { "Accept": "application/json;odata=verbose" },
        success: function (data) {
            var jsonObject = JSON.parse(data.body);
            $("#message").empty();

            for (var i = 0; i < jsonObject.d.results.length; i++) {
                var item = jsonObject.d.results[i];
                $("#message").append("<div>" + item.Name + "</div>");
            }
        },
        error: function (data, errorCode, errorMessage) {
            var jsonObject = JSON.parse(data.body);
            var errMsg = "Error: " + jsonObject.error.message.value;
            $("#error").text(errMsg);
        }
    });
}
