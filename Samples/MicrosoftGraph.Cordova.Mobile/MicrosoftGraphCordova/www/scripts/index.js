// To debug code on page load in Ripple or on Android devices/emulators: launch your app, set breakpoints, 
// and then run "window.location.reload()" in the JavaScript Console.
(function () {
  "use strict";
  var tenantName, authority, authContext, output;
  var resourceUrl = 'https://graph.microsoft.com/';
  var appId = "92f98787-c980-4c15-9be0-348ba4244408";
  var redirectUrl = "http://localhost:4400/www/index.html";

  document.addEventListener('deviceready', onDeviceReady.bind(this), false);

  function onDeviceReady() {
    // Handle the Cordova pause and resume events
    document.addEventListener('pause', onPause.bind(this), false);
    document.addEventListener('resume', onResume.bind(this), false);
    document.getElementById("loaddata").addEventListener('click', onLoadData.bind(this), false);
    document.getElementById("clearCache").addEventListener('click', onClearCache.bind(this), false);
    document.getElementById("clearOutput").addEventListener('click', onClearOutput.bind(this), false);
    document.getElementById("queries").addEventListener('change', onQueriesChanged.bind(this), false);
    output = document.getElementById("output");
  };

  function fail(err) {
    output.innerHTML = err.message;
  }

  function onLoadData() {
    if (tenantName !== document.getElementById("tenantname").value) {
      // tenantName has changed - reset auth context 
      tenantName = document.getElementById("tenantname").value;
      if (tenantName.length > 0) authority = "https://login.windows.net/" + tenantName + ".onmicrosoft.com";
      else authority = "https://login.windows.net/common";
      authContext = null;
    }
    document.getElementById("data").innerHTML = "";

    getAccessToken(resourceUrl, appId, redirectUrl, function (response) {
      output.innerHTML = displayTokenResponse(response);

      var endPointUri = "https://graph.microsoft.com/";
      // Demo only: Don't build query out of unsanitized user input in production code!
      var query = document.getElementById("query").value;
      var requestUri = endPointUri + query;
      var bearerToken = "Bearer " + response.accessToken;
      var xhr = new XMLHttpRequest();
      xhr.open('GET', requestUri);
      if (xhr.msCaching) xhr.msCaching = 'disabled'; // disable aggressive caching on IE
      xhr.setRequestHeader("Authorization", bearerToken);
      xhr.setRequestHeader("Accept", "application/json;odata.metadata=minimal");
      xhr.onload = function () {
        if (xhr.status === 200) {
          var response = JSON.parse(xhr.responseText);
          var data = "";
          if (response.value)
            for (var i = 0; i < response.value.length; i++)
              data += "<li>" + displayValue(response.value[i]) + "</li>";
          document.getElementById("data").innerHTML = data;
        }
        else {
          output.innerHTML += '<br />Request failed.  Returned status of ' + xhr.status;
        }
      };
      xhr.send();
    }, fail);
  };

  function onClearCache() {
    if (authContext) authContext.tokenCache.clear();
  }

  function onClearOutput() {
    document.getElementById("output").innerHTML = "";
    document.getElementById("data").innerHTML = "";
  }

  function onQueriesChanged(event) {
    document.getElementById("query").value = document.getElementById("queries").value;
  }

  function displayValue(value) {
    // return a display value for various possible objects
    var display = value.displayName;
    if (display === undefined) display = value.title;
    if (display === undefined) display = value.subject;
    if (display === undefined) display = value.name;
    return display;
  }

  function getAccessTokenFromContext(context, resourceUrl, appId, redirectUrl, success, fail) {
    try {
      // Current recommended pattern is to call acquireTokenSilentAsync first.
      // If token can't be obtained silently, the "fail" callback invokes acquireTokenAsync.
      // Intention is that acquireTokenAsync will always prompt in ADAL libraries moving forward.
      context.acquireTokenSilentAsync(resourceUrl, appId).then(success, function () {
        context.acquireTokenAsync(resourceUrl, appId, redirectUrl).then(success, fail);
      });
    } catch (ex) {
      fail(ex.message);
    }
  }

  function getContextFromCachedAuthority() {
    if (authContext && authContext.tokenCache) authContext.tokenCache.readItems().then(function (items) {
      if (items.length > 0) {
        authority = items[0].authority;
        authContext = new Microsoft.ADAL.AuthenticationContext(authority);
      }
    });
  }

  function getAccessToken(resourceUrl, appId, redirectUrl, success, fail) {
    try {
      if (!authContext) {
        Microsoft.ADAL.AuthenticationContext.createAsync(authority).then(function (context) {
          authContext = context;
          // If you use the common endpoint the user will be prompted each time
          // the app is run unless authContext uses the authority from the cache
          if (tenantName.length === 0) getContextFromCachedAuthority();
          getAccessTokenFromContext(authContext, resourceUrl, appId, redirectUrl, success, fail);
        }, fail);
      }
      else getAccessTokenFromContext(authContext, resourceUrl, appId, redirectUrl, success, fail);
    } catch (ex) {
      fail(ex.message);
    }
  }

  function displayTokenResponse(response) {
    var message = "";
    message += "<b>Access token:</b> " + response.accessToken;
    message += "<br />\r\n";
    message += decodeJWT(response.accessToken);
    message += "<br />\r\n";
    message += "<b>Token will expire on:</b> " + response.expiresOn;
    message += "<br />\r\n";
    message += "<b>UserInfo:</b> " + response.userInfo.displayableId + " (" + response.userInfo.uniqueId + ")";
    message += "<br />\r\n";
    message += "<b>Identity Provider:</b> " + response.userInfo.identityProvider;
    message += "<br />\r\n";
    return message;
  }

  function decodeJWT(encodedJWT) {
    // Decode JWT token for demonstration purposes only.
    // It's recommend that you treat the token as opaque
    // and don't try to use the contents directly.
    var decodedJWT = "";
    try {
      var sections = encodedJWT.split(".");
      decodedJWT += "<br /><b>Header:</b> " + atob(sections[0]);
      decodedJWT += "<br /><b>Payload:</b> " + atob(sections[1]);
      decodedJWT += "<br /><b>Signature:</b> " + sections[2];
      decodedJWT += "<br />";
    } catch (ex) {
      decodedJWT += "<br />Error: " + ex.message;
    }
    return decodedJWT;
  }

  function onPause() {
    // TODO: This application has been suspended. Save application state here.
  };

  function onResume() {
    // TODO: This application has been reactivated. Restore application state here.
  };
})();