
// Copyright (c) Microsoft Open Technologies, Inc.  All rights reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

/*jshint jasmine: true */
/*global require, module, Microsoft*/

var TENANT_NAME = '17bf7168-5251-44ed-a3cf-37a5997cc451';
var APP_ID = '3cfa20df-bca4-4131-ab92-626fb800ebb5';
var REDIRECT_URL = "http://test.com";

var RESOURCE_URL = 'https://graph.windows.net/';

var AUTHORITY_URL = 'https://login.windows.net/' + TENANT_NAME + '/';
var INVALID_AUTHORITY_URL = 'https://invalid.authority.url';
var TEST_USER_ID = '';
var INVALID_USER_ID = 'invalid@user.id';

var AuthenticationContext = Microsoft.ADAL.AuthenticationContext;
var AuthenticationResult = require('cordova-plugin-ms-adal.AuthenticationResult');
var TokenCacheItem = require('cordova-plugin-ms-adal.TokenCacheItem');
var TokenCache = require('cordova-plugin-ms-adal.TokenCache');
var UserInfo = require('cordova-plugin-ms-adal.UserInfo');

module.exports.defineAutoTests = function () {

    describe("Authentication Context", function () {

        it("Should have a constructor", function () {
            expect(AuthenticationContext).toBeDefined();
            expect(typeof AuthenticationContext).toEqual("function");
        });

        it("Should have createAsync static method", function () {
            expect(AuthenticationContext.createAsync).toBeDefined();
            expect(typeof AuthenticationContext.createAsync).toEqual("function");
        });

        it("Should not have any other static properties", function () {
            var ownProperties = [],
                context = AuthenticationContext;

            for (var p in context) {
                if (context.hasOwnProperty(p)) {
                    ownProperties.push(p);
                }
            }

            expect(ownProperties.length).toEqual(1);
            expect(ownProperties[0]).toEqual("createAsync");
        });

        it("Should have been created properly using constructor", function () {
            var context;
            try {
                context = new AuthenticationContext(AUTHORITY_URL);
                expect(context instanceof AuthenticationContext).toBeTruthy();
                expect(context.authority).toEqual(AUTHORITY_URL);
                expect(context.tokenCache instanceof TokenCache).toBeTruthy();
            } catch (err) {
                expect(err).not.toBeDefined();
            }
        });

        // We need to test this case here because we need to be sure
        // that context for this authority hadn't been created already
        it("Should get token successfully if created using constructor", function (done) {
            var context = new AuthenticationContext(AUTHORITY_URL);
            context.acquireTokenSilentAsync(RESOURCE_URL, APP_ID, TEST_USER_ID)
            .then(function (authResult) {
                expect(authResult).toBeDefined();
                expect(authResult instanceof AuthenticationResult).toBeTruthy();
                expect(authResult.accessToken).toBeDefined();
                expect(authResult.expiresOn).toBeDefined();
                expect(typeof authResult.accessToken).toEqual("string");
                expect(authResult.expiresOn instanceof Date).toBeTruthy();
                done();
            }, function (err) {
                expect(err).not.toBeDefined();
                done();
            });
        });

        it("Should have been created properly via 'createAsync' method", function (done) {
            AuthenticationContext.createAsync(AUTHORITY_URL)
            .then(function (context) {
                expect(context instanceof AuthenticationContext).toBeTruthy();
                expect(context.authority).toEqual(AUTHORITY_URL);
                expect(context.tokenCache instanceof TokenCache).toBeTruthy();
                done();
            }, function (err) {
                expect(err).not.toBeDefined();
                done();
            });
        });

        it("Should accept validateAuthority flag", function (done) {
            AuthenticationContext.createAsync(AUTHORITY_URL, false)
            .then(function (context) {
                expect(context instanceof AuthenticationContext).toBeTruthy();
                expect(context.authority).toEqual(AUTHORITY_URL);
                expect(context.validateAuthority).toBeFalsy();
                done();
            }, function (err) {
                expect(err).not.toBeDefined();
                done();
            });
        });

        it("Should fail to create context if AUTHORITY_URL is not valid", function (done) {
            AuthenticationContext.createAsync(INVALID_AUTHORITY_URL, true)
            .then(function (context) {
                expect(context).not.toBeDefined();
                done();
            }, function (err) {
                expect(err).toBeDefined();
                done();
            });
        });

        describe("Token acquisition", function () {

            var authContext;

            beforeEach(function (done) {
                AuthenticationContext.createAsync(AUTHORITY_URL)
                .then(function (context) {
                    authContext = context;
                    done();
                });
            });

            afterEach(function () {
                authContext = null;
            });

            it("Should have an 'acquireTokenAsync' method", function (done) {
                expect(authContext.acquireTokenAsync).toBeDefined();
                expect(typeof authContext.acquireTokenAsync).toEqual("function");
                done();
            });

            // This test is pended since acquireTokenAsync will always bypass cookies and show UI
            xit("Should acquire token via 'acquireTokenAsync' method", function (done) {
                authContext.acquireTokenAsync(RESOURCE_URL, APP_ID, REDIRECT_URL)
                .then(function (authResult) {
                    expect(authResult).toBeDefined();
                    expect(authResult instanceof AuthenticationResult).toBeTruthy();
                    expect(authResult.accessToken).toBeDefined();
                    expect(authResult.expiresOn).toBeDefined();
                    expect(typeof authResult.accessToken).toEqual("string");
                    expect(authResult.expiresOn instanceof Date).toBeTruthy();
                    expect(authResult.userInfo instanceof UserInfo).toBeTruthy();
                    // Save acquired userId for further usage
                    TEST_USER_ID = authResult.userInfo.userId;
                    done();
                }, function (err) {
                    expect(err).not.toBeDefined();
                    done();
                });
            });

            it("Should have an 'acquireTokenSilentAsync' method", function (done) {
                expect(authContext.acquireTokenSilentAsync).toBeDefined();
                expect(typeof authContext.acquireTokenSilentAsync).toEqual("function");
                done();
            });

            it("Should acquire token via 'acquireTokenSilentAsync' method", function (done) {
                authContext.acquireTokenSilentAsync(RESOURCE_URL, APP_ID, TEST_USER_ID)
                .then(function (authResult) {
                    expect(authResult).toBeDefined();
                    expect(authResult instanceof AuthenticationResult).toBeTruthy();
                    expect(authResult.accessToken).toBeDefined();
                    expect(authResult.expiresOn).toBeDefined();
                    expect(typeof authResult.accessToken).toEqual("string");
                    expect(authResult.expiresOn instanceof Date).toBeTruthy();
                    expect(authResult.userInfo instanceof UserInfo).toBeTruthy();
                    done();
                }, function (err) {
                    expect(err).not.toBeDefined();
                    done();
                });
            });

            it("Should fail to acquire token via 'acquireTokenSilentAsync' method if username is not valid", function (done) {
                authContext.acquireTokenSilentAsync(RESOURCE_URL, APP_ID, INVALID_USER_ID)
                .then(function (authResult) {
                    expect(authResult).not.toBeDefined();
                    done();
                }, function (err) {
                    expect(err).toBeDefined();
                    done();
                });
            });
        });
    });

    describe("Token Cache", function () {

        var context, cache;

        beforeEach(function() {
            context = new AuthenticationContext(AUTHORITY_URL);
            cache = context.tokenCache;
        });

        afterEach(function () {
            context = cache = null;
        });

        it("Should exist in authentication context instance", function () {
            expect(context.tokenCache).toBeDefined();
            expect(context.tokenCache instanceof TokenCache).toBeTruthy();
        });

        it("Should contain proper fields and methods", function () {
            expect(cache.contextAuthority).toBeDefined();
            expect(typeof cache.contextAuthority).toBe("string");
            expect(cache.contextAuthority).toEqual(context.authority);
            expect(typeof cache.clear).toBe("function");
            expect(typeof cache.readItems).toBe("function");
            expect(typeof cache.deleteItem).toBe("function");
        });

        it("Should acquire native cache via 'readItems' method", function (done) {
            cache.readItems()
            .then(function (cacheItems) {
                expect(cacheItems.constructor).toBe(Array);
                expect(cacheItems.length).toBeGreaterThan(0);
                expect(cacheItems[0] instanceof TokenCacheItem).toBeTruthy();
                done();
            }, function (err) {
                expect(err).not.toBeDefined();
                done();
            });
        });

        it("Should be able to delete item via 'deleteItem' method", function(done) {

            var fail = function (err) {
                expect(err).not.toBeDefined();
                done();
            };

            var initialLength;

            cache.readItems().then(function (cacheItems) {
                var item = cacheItems[0];
                initialLength = cacheItems.length;
                return cache.deleteItem(item);
            }, fail).then(function () {
                return cache.readItems();
            }, fail).then(function (cacheItems) {
                expect(cacheItems.length).toEqual(initialLength - 1);
                done();
            }, fail);
        });

        it("Should be able to clear native cache via 'clear' method", function(done) {

            var fail = function (err) {
                expect(err).not.toBeDefined();
                done();
            };

            cache.readItems().then(function () {
                return cache.clear();
            }, fail).then(function () {
                return cache.readItems();
            }, fail).then(function (cacheItems) {
                expect(cacheItems.length).toEqual(0);
                done();
            }, fail);
        });
    });
};

module.exports.defineManualTests = function (contentEl, createActionButton) {

    var context;

    createActionButton("Create Authentication context", function () {
        AuthenticationContext.createAsync(AUTHORITY_URL)
        .then(function (ctx) {
            context = ctx;
            contentEl.innerHTML = JSON.stringify(ctx, null, 4);
        }, function (err) {
            contentEl.innerHTML = err ? err.error + ": " + err.errorDescription : "";
        });
    });

    createActionButton("Acquire token", function () {

        if (!context) {
            contentEl.innerHTML = "Create context first";
            return;
        }

        context.acquireTokenAsync(RESOURCE_URL, APP_ID, REDIRECT_URL).then(function (authRes) {
            // Save acquired userId for further usage
            TEST_USER_ID = authRes.userInfo.userId;
            contentEl.innerHTML = authRes;
            contentEl.innerHTML += "<br /> AccessToken: " + authRes.accessToken;
            contentEl.innerHTML += "<br /> ExpiresOn: " + authRes.expiresOn;
        }, function(err) {
            contentEl.innerHTML = err ? err.error + ": " + err.errorDescription : "";
        });
    });

    createActionButton("Acquire token with userId", function () {

        if (!context) {
            contentEl.innerHTML = "Create context first";
            return;
        }

        context.tokenCache.readItems()
        .then(function function_name (items) {
            var itemsWithUserId = items.filter(function(item) {
                return item.userInfo && item.userInfo.userId;
            });

            if (itemsWithUserId.length <= 0 ) {
                contentEl.innerHTML = "No users withUserId found in cache, please acquire token first";
                return;
            }

            context.acquireTokenAsync(RESOURCE_URL, APP_ID, REDIRECT_URL, itemsWithUserId[0].userInfo.userId).then(function (authRes) {
                // Save acquired userId for further usage
                TEST_USER_ID = authRes.userInfo.userId;
                contentEl.innerHTML = authRes;
                contentEl.innerHTML += "<br /> AccessToken: " + authRes.accessToken;
                contentEl.innerHTML += "<br /> ExpiresOn: " + authRes.expiresOn;
            }, function(err) {
                contentEl.innerHTML = err ? err.error + ": " + err.errorDescription : "";
            });

            contentEl.innerHTML = JSON.stringify(items, null, 4);
        }, function(err) {
            contentEl.innerHTML = err ? err.error + ": " + err.errorDescription : "";
        });
    });

    createActionButton("Acquire token silently", function () {

        if (!context) {
            contentEl.innerHTML = "Create context first";
            return;
        }

        context.acquireTokenSilentAsync(RESOURCE_URL, APP_ID, TEST_USER_ID).then(function (authRes) {
            contentEl.innerHTML = authRes;
            contentEl.innerHTML += "<br /> AccessToken: " + authRes.accessToken;
            contentEl.innerHTML += "<br /> ExpiresOn: " + authRes.expiresOn;
        }, function(err) {
            contentEl.innerHTML = err ? err.error + ": " + err.errorDescription : "";
        });
    });

    createActionButton("Read token cache items", function () {

        if (!context) {
            contentEl.innerHTML = "Create context first";
            return;
        }

        context.tokenCache.readItems()
        .then(function function_name (items) {
            contentEl.innerHTML = JSON.stringify(items, null, 4);
        }, function(err) {
            contentEl.innerHTML = err ? err.error + ": " + err.errorDescription : "";
        });
    });

    createActionButton("Clear cache", function () {

        if (!context) {
            contentEl.innerHTML = "Create context first";
            return;
        }

        context.tokenCache.clear().then(function() {
            contentEl.innerHTML = "Logged out";
        }, function(err) {
            contentEl.innerHTML = err ? err.error + ": " + err.errorDescription : "";
        });
    });

};
