
// Copyright (c) Microsoft Open Technologies, Inc.  All rights reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

var AuthenticationContext;

var authority = 'https://login.windows.net/test353.onmicrosoft.com';
var resourceUrl = 'https://graph.windows.net/';
var appId = '1eed60fd-93bf-44c3-948b-d419b32b5ed6';
var redirectUrl = 'http://localhost:4400/services/aad/redirectTarget.html';
  
var tenantName = 'test353.onmicrosoft.com';
var endpointUrl = resourceUrl + tenantName;

function pre(json) {
    return '<pre>' + JSON.stringify(json, null, 4) + '</pre>';
}

var app = {
    // Application Constructor
    initialize: function () {
        this.bindEvents();
    },
    // Bind Event Listeners
    //
    // Bind any events that are required on startup. Common events are:
    // 'load', 'deviceready', 'offline', and 'online'.
    bindEvents: function () {
        document.addEventListener('deviceready', app.onDeviceReady, false);

        document.getElementById('create-context').addEventListener('click', app.createContext);
        document.getElementById('acquire-token').addEventListener('click', app.acquireToken);
        document.getElementById('acquire-token-silent').addEventListener('click', app.acquireTokenSilent);
        document.getElementById('read-tokencache').addEventListener('click', app.readTokenCache);
        document.getElementById('clear-tokencache').addEventListener('click', app.clearTokenCache);

        function toggleMenu() {
            // menu must be always shown on desktop/tablet
            if (document.body.clientWidth > 480) return;
            var cl = document.body.classList;
            if (cl.contains('left-nav')) { cl.remove('left-nav'); }
            else { cl.add('left-nav'); }
        }

        document.getElementById('slide-menu-button').addEventListener('click', toggleMenu);
    },
    // deviceready Event Handler
    //
    // The scope of 'this' is the event. In order to call the 'receivedEvent'
    // function, we must explicitly call 'app.receivedEvent(...);'
    onDeviceReady: function () {
        // app.receivedEvent('deviceready');
        app.logArea = document.getElementById("log-area");
        app.log("Cordova initialized, 'deviceready' event was fired");
        AuthenticationContext = Microsoft.ADAL.AuthenticationContext;
    },
    // Update DOM on a Received Event
    receivedEvent: function (id) {
        var parentElement = document.getElementById(id);
        var listeningElement = parentElement.querySelector('.listening');
        var receivedElement = parentElement.querySelector('.received');

        listeningElement.setAttribute('style', 'display:none;');
        receivedElement.setAttribute('style', 'display:block;');

        console.log('Received Event: ' + id);
    },

    log: function (message, isError) {
        isError ? console.error(message) : console.log(message);
        var logItem = document.createElement('li');
        logItem.classList.add("topcoat-list__item");
        isError && logItem.classList.add("error-item");
        var timestamp = '<span class="timestamp">' + new Date().toLocaleTimeString() + ': </span>';
        logItem.innerHTML = (timestamp + message);
        app.logArea.insertBefore(logItem, app.logArea.firstChild);
    },
    error: function (message) {
        app.log(message, true);
    },
    createContext: function() {
        AuthenticationContext.createAsync(authority)
        .then(function (context) {
            app.authContext = context;
            app.log("Created authentication context for authority URL: " + context.authority);
        }, app.error);
    },
    acquireToken: function () {
        if (app.authContext == null) {
            app.error('Authentication context isn\'t created yet. Create context first');
            return;
        }

        app.authContext.acquireTokenAsync(resourceUrl, appId, redirectUrl)
            .then(function(authResult) {
                app.log('Acquired token successfully: ' + pre(authResult));
            }, function(err) {
                app.error("Failed to acquire token: " + pre(err));
            });
    },
    acquireTokenSilent: function() {
        if (app.authContext == null) {
            app.error('Authentication context isn\'t created yet. Create context first');
            return;
        }

        // testUserId parameter is needed if you have > 1 token cache items to avoid "multiple_matching_tokens_detected" error
        // Note: This is for the test purposes only
        var testUserId;
        app.authContext.tokenCache.readItems().then(function (cacheItems) {
            if (cacheItems.length > 0) {
                testUserId = cacheItems[0].userInfo.userId;
            }

            app.authContext.acquireTokenSilentAsync(resourceUrl, appId, testUserId).then(function (authResult) {
                app.log('Acquired token successfully: ' + pre(authResult));
            }, function(err) {
                app.error("Failed to acquire token silently: " + pre(err));
            });
        }, function(err) {
            app.error("Unable to get User ID from token cache. Have you acquired token already? " + pre(err));
        });
    },
    readTokenCache: function () {
        if (app.authContext == null) {
            app.error('Authentication context isn\'t created yet. Create context first');
            return;
        }

        app.authContext.tokenCache.readItems()
        .then(function (res) {
            var text = "Read token cache successfully. There is " + res.length + " items stored.";
            if (res.length > 0) {
                text += "The first one is: " + pre(res[0]);
            }
            app.log(text);

        }, function (err) {
            app.error("Failed to read token cache: " + pre(err));
        });
    },
    clearTokenCache: function () {
        if (app.authContext == null) {
            app.error('Authentication context isn\'t created yet. Create context first');
            return;
        }

        app.authContext.tokenCache.clear().then(function () {
            app.log("Cache cleaned up successfully.");
        }, function (err) {
            app.error("Failed to clear token cache: " + pre(err));
        });
    }
};
