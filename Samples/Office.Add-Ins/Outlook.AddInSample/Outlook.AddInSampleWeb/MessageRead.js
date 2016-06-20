/// <reference path="/Scripts/FabricUI/MessageBanner.js" />

// Create ADAL.JS config and 
// get the AuthenticationContext
var azureADTenant = "<your tenant name>"; // Target Azure AD tenant 
var azureADClientID = "<your Client ID>"; // App ClientID

// General settings for ADAL.JS
window.config = {
    tenant: azureADTenant + ".onmicrosoft.com",
    clientId: azureADClientID,
    postLogoutRedirectUri: window.location.origin,
    endpoints: {
        graphApiUri: "https://graph.microsoft.com",
        sharePointUri: "https://" + azureADTenant + ".sharepoint.com",
    },
    cacheLocation: "localStorage"
};

// Create the AuthenticationContext object to play with ADAL.JS
var authContext = new AuthenticationContext(config);

// Check For & Handle Redirect From AAD After Login
var isCallback = authContext.isCallback(window.location.hash);
authContext.handleWindowCallback();

// Check Login Status, Update UI
if (isCallback && !authContext.getLoginError()) {
    window.location = authContext._getItem(authContext.CONSTANTS.STORAGE.LOGIN_REQUEST);
}
else {
    var user = authContext.getCachedUser();
    if (!user) {
        authContext.login();
    }
}

(function () {
  "use strict";

  var messageBanner;

  // The Office initialize function must be run each time a new page is loaded.
  Office.initialize = function (reason) {
    $(document).ready(function () {
      var element = document.querySelector('.ms-MessageBanner');
      messageBanner = new fabric.MessageBanner(element);
      messageBanner.hideBanner();
      loadContextData();
    });
  };

  function loadContextData()
  {
      var item = Office.context.mailbox.item;
      var senderDisplayName = item.sender.displayName;

      authContext.acquireToken(config.endpoints.graphApiUri, function (error, token) {
          if (error || !token) {
              console.log("ADAL error occurred: " + error);
              return;
          }
          else {
              var senderContactUri = config.endpoints.graphApiUri + "/v1.0/me/contacts?$filter=displayName%20eq%20'" + senderDisplayName + "'&$top=1";

              $.ajax({
                  type: "GET",
                  url: senderContactUri,
                  headers: {
                      "Authorization": "Bearer " + token
                  }
              }).done(function (response) {
                  console.log("Query for sender contact executed.");
                  var items = response.value;
                  for (var i = 0; i < items.length; i++) {
                      console.log(items[i].displayName);
                      $("#senderDisplayName").text(items[i].displayName);
                      $("#senderCompanyName").text(items[i].companyName);
                      $("#senderMobilePhone").text(items[i].mobilePhone);
                  }
              }).fail(function () {
                  console.log("Error while searching for sender contact.");
              });

              var filesUri = config.endpoints.graphApiUri + "/v1.0/me/drive/root/search(q='" + senderDisplayName + "')";

              $.ajax({
                  type: "GET",
                  url: filesUri,
                  headers: {
                      "Authorization": "Bearer " + token
                  }
              }).done(function (response) {
                  console.log("Successfully fetched files from OneDrive.");
                  var items = response.value;
                  for (var i = 0; i < items.length; i++) {
                      console.log(items[i].name);
                      $("#filesTable").append("<div class='ms-Table-row'><span class='ms-Table-cell'><a href='" + items[i].webUrl + "'>" + items[i].name + "</a></span></div>");
                  }
              }).fail(function () {
                  console.log("Fetching files from OneDrive failed.");
              });
          }
      });

  }

  // Helper function for displaying notifications
  function showNotification(header, content) {
    $("#notificationHeader").text(header);
    $("#notificationBody").text(content);
    messageBanner.showBanner();
    messageBanner.toggleExpansion();
  }
})();