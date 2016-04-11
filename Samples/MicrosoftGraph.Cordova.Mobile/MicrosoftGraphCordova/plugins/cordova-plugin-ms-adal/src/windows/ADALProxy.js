
// Copyright (c) Microsoft Open Technologies, Inc.  All rights reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

/*global require, Microsoft, Windows, WinJS*/

var Deferred = require('./utility').Utility.Deferred;

var isPhone = WinJS.Utilities.isPhone;

var webAuthBrokerContinuationCallback = null;
var successWebAuthStatus = Windows.Security.Authentication.Web.WebAuthenticationStatus.success;
var activationKindWebAuthContinuation = Windows.ApplicationModel.Activation.ActivationKind.webAuthenticationBrokerContinuation;
var AUTH_RESULT_SUCCESS_STATUS = 0;
var REQUIRED_DISPLAYABLE_ID = Microsoft.IdentityModel.Clients.ActiveDirectory.UserIdentifierType.requiredDisplayableId,
    UNIQUE_ID = Microsoft.IdentityModel.Clients.ActiveDirectory.UserIdentifierType.uniqueId;

var ctxCache = {};

function handleAuthResult(win, fail, res) {
    if (res.status === AUTH_RESULT_SUCCESS_STATUS) {
        win(res);
    } else {
        fail(res);
    }
}

function mapUserUniqueIdToDisplayName(context, uniqueId) {
    var cacheItems = context.tokenCache.readItems();

    for (var i = 0; i < cacheItems.length; i++) {
        try {
            if (cacheItems[i].uniqueId === uniqueId) {
                return cacheItems[i].displayableId;
            }
        } catch (e) { }
    }
}

function wrapUserId(userId, type) {
    return (userId !== '' && userId != null) ? new Microsoft.IdentityModel.Clients.ActiveDirectory.UserIdentifier(userId, type)
        : Microsoft.IdentityModel.Clients.ActiveDirectory.UserIdentifier.anyUser;
}

var ADALProxy = {
    createAsync: function (win, fail, args) {
        var authority = args[0];
        var validateAuthority = args[1] !== false; // true by default

        if (isPhone) {
            try {
                // WP 8.1
                Microsoft.IdentityModel.Clients.ActiveDirectory.AuthenticationContext.createAsync(authority, validateAuthority).then(function (ctx) {
                    ctx.useCorporateNetwork = window.ADAL_DEFAULT_USE_CORPORATE_NETWORK === true;
                    ctxCache[authority] = ctx;
                    win(ctx);
                }, function(e) {
                    fail(e);
                });
            } catch (e) {
                fail(e);
            }
        } else {
            // Win 8.0 / 8.1
            try {
                var nativeContext = new Microsoft.IdentityModel.Clients.ActiveDirectory.AuthenticationContext(authority, validateAuthority);
                nativeContext.useCorporateNetwork = window.ADAL_DEFAULT_USE_CORPORATE_NETWORK === true;
                ctxCache[authority] = nativeContext;
                win(nativeContext);
            } catch (e) {
                fail(e);
            }
        }
    },

    getOrCreateCtx: function(authority) {
        var d = new Deferred();

        if (typeof ctxCache[authority] !== 'undefined') {
            d.resolve(ctxCache[authority]);
        } else {
            ADALProxy.createAsync(function (ctx) {
                d.resolve(ctx);
            }, function (err) {
                d.reject(err);
            }, [authority]);
        }

        return d;
    },

    acquireTokenAsync: function (win, fail, args) {
        try {
            var authority = args[0];
            var resourceUrl = args[1];
            var clientId = args[2];
            var redirectUrl = new Windows.Foundation.Uri(args[3]);
            var userId = args[4];
            var extraQueryParameters = args[5];

            var userIdentifier;
            var displayName;

            ADALProxy.getOrCreateCtx(authority).then(function (context) {
                displayName = mapUserUniqueIdToDisplayName(context, userId);

                if (typeof displayName !== 'undefined') {
                    userIdentifier = wrapUserId(displayName, REQUIRED_DISPLAYABLE_ID);
                } else {
                    userIdentifier = wrapUserId(userId, UNIQUE_ID);
                }

                if (isPhone) {
                    // Continuation callback is used when we're running on WindowsPhone which uses
                    // AuthenticateAndContinue method instead of AuthenticateAsync, which uses different async model
                    // Continuation callback need to be assigned to Application's 'activated' event.
                    webAuthBrokerContinuationCallback = function (activationArgs) {
                        if (activationArgs.detail.kind === activationKindWebAuthContinuation) {
                            var result = activationArgs.detail.webAuthenticationResult;
                            if (result.responseStatus == successWebAuthStatus) {
                                context.continueAcquireTokenAsync(activationArgs.detail);
                            } else {
                                fail(result);
                            }
                            WinJS.Application.removeEventListener('activated', webAuthBrokerContinuationCallback, true);
                        }
                    };

                    WinJS.Application.addEventListener('activated', webAuthBrokerContinuationCallback, true);

                    try {
                        if (typeof userIdentifier !== 'undefined') {
                            if (typeof extraQueryParameters === 'undefined') {
                                context.acquireTokenAndContinue(resourceUrl, clientId, redirectUrl, userIdentifier, function (res) {
                                    handleAuthResult(win, fail, res);
                                });
                            } else {
                                context.acquireTokenAndContinue(resourceUrl, clientId, redirectUrl, userIdentifier, extraQueryParameters, function (res) {
                                    handleAuthResult(win, fail, res);
                                });
                            }
                        } else {
                            context.acquireTokenAndContinue(resourceUrl, clientId, redirectUrl, function (res) {
                                handleAuthResult(win, fail, res);
                            });
                        }
                    } catch (e) {
                        fail(e);
                    }
                } else {
                    if (context.useCorporateNetwork) {
                        // Try to SSO first
                        context.acquireTokenAsync(resourceUrl, clientId, Windows.Security.Authentication.Web.WebAuthenticationBroker.getCurrentApplicationCallbackUri(), Microsoft.IdentityModel.Clients.ActiveDirectory.PromptBehavior.never, userIdentifier, extraQueryParameters).then(function (res) {
                            handleAuthResult(win, function() {
                                context.acquireTokenAsync(resourceUrl, clientId, Windows.Security.Authentication.Web.WebAuthenticationBroker.getCurrentApplicationCallbackUri(), Microsoft.IdentityModel.Clients.ActiveDirectory.PromptBehavior.always, userIdentifier, extraQueryParameters).then(function (res) {
                                    handleAuthResult(win, fail, res);
                                }, fail);
                            }, res);
                        }, fail);
                    } else {
                        context.acquireTokenAsync(resourceUrl, clientId, redirectUrl, Microsoft.IdentityModel.Clients.ActiveDirectory.PromptBehavior.always, userIdentifier, extraQueryParameters).then(function (res) {
                            handleAuthResult(win, fail, res);
                        }, fail);
                    }
                }
            }, fail);
        } catch (e) {
            fail(e);
        }
    },

    acquireTokenSilentAsync: function (win, fail, args) {
        try {
            var authority = args[0];
            var resourceUrl = args[1];
            var clientId = args[2];
            var userId = args[3];

            var userIdentifier = wrapUserId(userId, UNIQUE_ID);

            ADALProxy.getOrCreateCtx(authority).then(function (context) {
                context.acquireTokenSilentAsync(resourceUrl, clientId, userIdentifier).then(function (res) {
                    handleAuthResult(win, fail, res);
                }, fail);
            }, fail);
        } catch (e) {
            fail(e);
        }
    },

    tokenCacheClear: function (win, fail, args) {
        try {
            var authority = args[0];

            ADALProxy.getOrCreateCtx(authority).then(function (context) {
                context.tokenCache.clear();
                ctxCache = {};
                win();
            }, fail);
        } catch (e) {
            fail(e);
        }
    },

    tokenCacheReadItems: function (win, fail, args) {
        try {
            var authority = args[0];

            ADALProxy.getOrCreateCtx(authority).then(function (context) {
                win(context.tokenCache.readItems().map(function(item) {
                    var copy = {};

                    try {
                        copy.accessToken = item.accessToken;
                    } catch (e) { }

                    try {
                        copy.authority = item.authority;
                    } catch (e) { }

                    try {
                        copy.clientId = item.clientId;
                    } catch (e) { }

                    try {
                        copy.displayableId = item.displayableId;
                    } catch (e) { }

                    try {
                        copy.expiresOn = item.expiresOn;
                    } catch (e) { }

                    try {
                        copy.isMultipleResourceRefreshToken = item.isMultipleResourceRefreshToken;
                    } catch (e) { }

                    try {
                        copy.resource = item.resource;
                    } catch (e) { }

                    try {
                        copy.tenantId = item.tenantId;
                    } catch (e) { }


                    try {
                        copy.idToken = item.idToken;
                    } catch (e) { }

                    return copy;
                }));
            }, fail);
        } catch (e) {
            fail(e);
        }
    },

    tokenCacheDeleteItem: function (win, fail, args) {
        try {
            var contextAuthority = args[0];
            var itemAuthority = args[1];
            var itemResource = args[2];
            var itemClientId = args[3];
            var itemUserId = args[4];
            var itemIsMultipleResourceRefreshToken = args[5];

            ADALProxy.getOrCreateCtx(contextAuthority).then(function (context) {
                var allItems = context.tokenCache.readItems();

                for (var i = 0; i < allItems.length; i++) {
                    if (allItems[i].clientId === itemClientId
                        && allItems[i].resource === itemResource
                        && allItems[i].uniqueId === itemUserId
                        && allItems[i].authority === itemAuthority
                        && allItems[i].isMultipleResourceRefreshToken === itemIsMultipleResourceRefreshToken) {
                        context.tokenCache.deleteItem(allItems[i]);
                        win();
                        return;
                    }
                }

                fail('No such item found');
            }, fail);
        } catch (e) {
            fail(e);
        }
    }
};

require("cordova/exec/proxy").add("ADALProxy", ADALProxy);
