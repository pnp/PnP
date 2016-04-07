// Copyright (c) Microsoft Open Technologies, Inc.  All rights reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

/*global module*/

var util = require('./utility');

/**
 * Represents information about authorized user.
 */
function UserInfo(userInfo) {

    userInfo = userInfo || {};

    this.displayableId = userInfo.displayableId;
    this.userId = userInfo.userId || userInfo.uniqueId;
    this.familyName = userInfo.familyName;
    this.givenName = userInfo.givenName;
    this.identityProvider = userInfo.identityProvider;
    this.passwordChangeUrl = userInfo.passwordChangeUrl; //uri
    this.passwordExpiresOn = userInfo.passwordExpiresOn ? new Date(userInfo.passwordExpiresOn) : null;
    this.uniqueId = userInfo.uniqueId;
}

/**
 * Parses jwt token that contains a use information and produces a valid UserInfo structure.
 * This method is intended for internal use and should not be used by end-user.
 *
 * @param  {String} jwtToken String that contains a valid JWT token, that contains user information.
 *                           Usually this is an idToken field of authenticationResult structure.
 *
 * @return {Object}          UserInfo object, created from token data.
 */
UserInfo.fromJWT = function function_name (jwtToken) {
    // JWT token passed here should be a non-empty string
    if (typeof jwtToken !== 'string' || jwtToken.length === 0) {
        return null;
    }

    var token;
    // If there is non-valid JWT token passed we don't want to
    // bubble error up and return null, as jwt isn't passed at all.
    try{
        token = util.parseJWT(jwtToken);
    } catch (e) {
        return null;
    }

    var result = new UserInfo();

    result.displayableId = token.name;
    result.familyName = token.family_name;
    result.givenName = token.given_name;
    // Due to https://msdn.microsoft.com/en-us/library/azure/dn195587.aspx this value is
    // identical to the value of the Issuer claim unless the user account is in a different tenant than the issuer.
    // In case when identity provider is not specified in token, we use 'issuer' field ('iss' claim) of token
    result.identityProvider = token.idp || token.iss;
    result.passwordChangeUrl = token.pwd_url;
    result.passwordExpiresOn = token.exp ? Date(token.exp) : null;
    result.uniqueId = token.unique_name;
    result.userId = token.oid;

    return result;
};

module.exports = UserInfo;
