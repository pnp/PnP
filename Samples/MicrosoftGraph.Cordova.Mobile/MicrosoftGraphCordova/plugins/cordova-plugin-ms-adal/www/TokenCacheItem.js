// Copyright (c) Microsoft Open Technologies, Inc.  All rights reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

/*global module, require*/

var UserInfo = require('./UserInfo');

/**
 * Represents token cache item.
 */
function TokenCacheItem(cacheItem) {

    cacheItem = cacheItem || {};

    this.accessToken = cacheItem.accessToken;
    this.authority = cacheItem.authority;
    this.clientId = cacheItem.clientId;
    this.displayableId = cacheItem.displayableId;
    this.expiresOn = cacheItem.expiresOn ? new Date(cacheItem.expiresOn) : null;
    this.isMultipleResourceRefreshToken = cacheItem.isMultipleResourceRefreshToken;
    this.resource = cacheItem.resource;
    this.tenantId = cacheItem.tenantId;

    this.userInfo = cacheItem.idToken ? UserInfo.fromJWT(cacheItem.idToken) : null;
}

module.exports = TokenCacheItem;
