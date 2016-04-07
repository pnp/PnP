// Copyright (c) Microsoft Open Technologies, Inc.  All rights reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

/*global module, require*/

var checkArgs = require('cordova/argscheck').checkArgs;

var bridge = require('./CordovaBridge');
var Deferred = require('./utility').Utility.Deferred;
var AuthenticationResult = require('./AuthenticationResult');
var TokenCache = require('./TokenCache');

/**
 * Constructs context to use with known authority to get the token. It reuses existing context
 * for this authority URL in native proxy or creates a new one if it doesn't exists.
 * Corresponding native context will be created at first time when it will be needed.
 *
 * @param   {String}  authority         Authority url to send code and token requests
 * @param   {Boolean} validateAuthority Validate authority before sending token request
 *                                      When context is being created syncronously using this constructor
 *                                      validateAuthority in native context will be disabled to prevent
 *                                      context initialization failure
 *
 * @returns {Object}  Newly created authentication context.
 */
function AuthenticationContext(authority, validateAuthority) {

    checkArgs('s*', 'AuthenticationContext', arguments);

    if (validateAuthority !== false) {
        validateAuthority = true;
    }

    this.authority = authority;
    this.validateAuthority = validateAuthority;
    this.tokenCache = new TokenCache(this.authority);
}

/**
 * Constructs context asynchronously to use with known authority to get the token.
 * It reuses existing context for this authority URL in native proxy or creates a new one if it doesn't exists.
 *
 * @param   {String}   authority         Authority url to send code and token requests
 * @param   {Boolean}  validateAuthority Validate authority before sending token request. True by default
 *
 * @returns {Promise}  Promise either fulfilled with newly created authentication context or rejected with error
 */
AuthenticationContext.createAsync = function (authority, validateAuthority) {

    checkArgs('s*', 'AuthenticationContext.createAsync', arguments);

    var d = new Deferred();

    if (validateAuthority !== false) {
        validateAuthority = true;
    }

    bridge.executeNativeMethod('createAsync', [authority, validateAuthority]).then(function () {
        d.resolve(new AuthenticationContext(authority, validateAuthority));
    }, function(err) {
        d.reject(err);
    });

    return d;
};

/**
 * Acquires token using interactive flow if needed. It checks the cache to return existing result
 * if not expired. It tries to use refresh token if available. If it fails to get token with
 * refresh token, it will remove this refresh token from cache and start authentication.
 *
 * @param   {String}  resourceUrl Resource identifier
 * @param   {String}  clientId    Client (application) identifier
 * @param   {String}  redirectUrl Redirect url for this application
 * @param   {String}  userId      User identifier (optional)
 * @param   {String}  extraQueryParameters
 *                                Extra query parameters (optional)
 *                                Parameters should be escaped before passing to this method (e.g. using 'encodeURI()')
 *
 * @returns {Promise} Promise either fulfilled with AuthenticationResult object or rejected with error
 */
AuthenticationContext.prototype.acquireTokenAsync = function (resourceUrl, clientId, redirectUrl, userId, extraQueryParameters) {

    checkArgs('sssSS', 'AuthenticationContext.acquireTokenAsync', arguments);

    var d = new Deferred();

    bridge.executeNativeMethod('acquireTokenAsync', [this.authority, resourceUrl, clientId, redirectUrl, userId, extraQueryParameters])
    .then(function(authResult){
        d.resolve(new AuthenticationResult(authResult));
    }, function(err) {
        d.reject(err);
    });

    return d;
};

/**
 * Acquires token WITHOUT using interactive flow. It checks the cache to return existing result
 * if not expired. It tries to use refresh token if available. If it fails to get token without
 * displaying UI it will fail. This method guarantees that no UI will be shown to user.
 *
 * @param   {String}  resourceUrl Resource identifier
 * @param   {String}  clientId    Client (application) identifier
 * @param   {String}  userId      User identifier (optional)
 *
 * @returns {Promise} Promise either fulfilled with AuthenticationResult object or rejected with error
 */
AuthenticationContext.prototype.acquireTokenSilentAsync = function (resourceUrl, clientId, userId) {

    checkArgs('ssS', 'AuthenticationContext.acquireTokenSilentAsync', arguments);

    var d = new Deferred();

    bridge.executeNativeMethod('acquireTokenSilentAsync', [this.authority, resourceUrl, clientId, userId])
    .then(function(authResult){
        d.resolve(new AuthenticationResult(authResult));
    }, function(err) {
        d.reject(err);
    });

    return d;
};

module.exports = AuthenticationContext;
