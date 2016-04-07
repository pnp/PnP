// Copyright (c) Microsoft Open Technologies, Inc.  All rights reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

/*global module, require*/

var bridge = require('./CordovaBridge');
var TokenCacheItem = require('./TokenCacheItem');
var Deferred = require('./utility').Utility.Deferred;
var checkArgs = require('cordova/argscheck').checkArgs;

/**
 * Token cache class used by {AuthenticationContext} to store access and refresh tokens.
 */
function TokenCache(contextAuthority) {
    this.contextAuthority = contextAuthority;
}

/**
 * Clears the cache by deleting all the items.
 *
 * @returns {Promise} Promise either fulfilled when operation is completed or rejected with error.
 */
TokenCache.prototype.clear = function () {
    return bridge.executeNativeMethod('tokenCacheClear', [this.contextAuthority]);
};

/**
 * Gets all cached items.
 *
 * @returns {Promise} Promise either fulfilled with array of cached items or rejected with error.
 */
TokenCache.prototype.readItems = function () {
    checkArgs('', 'TokenCache.readItems', arguments);
    var result = [];

    var d = new Deferred();

    bridge.executeNativeMethod('tokenCacheReadItems', [this.contextAuthority])
    .then(function (tokenCacheItems) {
        tokenCacheItems.forEach(function (item) {
            result.push(new TokenCacheItem(item));
        });
        d.resolve(result);
    }, function(err) {
        d.reject(err);
    });

    return d;
};

/**
 * Deletes cached item.
 *
 * @param   {TokenCacheItem}  item Cached item to delete from cache
 *
 * @returns {Promise} Promise either fulfilled when operation is completed or rejected with error.
 */
TokenCache.prototype.deleteItem = function (item) {
    checkArgs('*', 'TokenCache.deleteItem', arguments);

    var args = [
        this.contextAuthority,
        item.authority,
        item.resource,
        item.clientId,
        item.userInfo && item.userInfo.userId,
        item.isMultipleResourceRefreshToken
    ];

    return bridge.executeNativeMethod('tokenCacheDeleteItem', args);
};

module.exports = TokenCache;
