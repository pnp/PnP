/*******************************************************************************
 * Copyright (c) Microsoft Open Technologies, Inc.
 * All Rights Reserved
 * Licensed under the Apache License, Version 2.0.
 * See License.txt in the project root for license information.
 ******************************************************************************/

package com.microsoft.aad.adal;

import org.json.JSONException;
import org.json.JSONObject;

/**
 * Class that responsible for simple serialization of ADAL primitives
 */
class SimpleSerialization {

    /**
     * Convert UserInfo object to JSON representation
     * @param info UserInfo object
     * @return JSONObject that represents a UserInfo structure
     * @throws JSONException
     */
    static JSONObject userInfoToJSON(UserInfo info) throws JSONException {

        JSONObject userInfo = new JSONObject();

        if (info == null) {
            return userInfo;
        }

        userInfo.put("displayableId", info.getDisplayableId());
        userInfo.put("familyName", info.getFamilyName());
        userInfo.put("givenName", info.getGivenName());
        userInfo.put("identityProvider", info.getIdentityProvider());
        userInfo.put("passwordChangeUrl", info.getPasswordChangeUrl());
        userInfo.put("passwordExpiresOn", info.getPasswordExpiresOn());
        userInfo.put("uniqueId", info.getUserId());
        userInfo.put("userId", info.getUserId());

        return userInfo;
    }

    /**
     * Convert AuthenticationResult object to JSON representation. Nested userInfo field is being
     * serialized as well. In case if userInfo field is not exists in input object it will
     * be equal to null in resultant object
     * @param authenticationResult AuthenticationResult object
     * @return JSONObject that represents a AuthenticationResult structure
     * @throws JSONException
     */
    static JSONObject authenticationResultToJSON(AuthenticationResult authenticationResult) throws JSONException {
        JSONObject authResult = new JSONObject();

        authResult.put("accessToken", authenticationResult.getAccessToken());
        authResult.put("accessTokenType", authenticationResult.getAccessTokenType());
        authResult.put("expiresOn", authenticationResult.getExpiresOn());
        authResult.put("idToken", authenticationResult.getIdToken());
        authResult.put("isMultipleResourceRefreshToken", authenticationResult.getIsMultiResourceRefreshToken());
        authResult.put("statusCode", authenticationResult.getStatus());
        authResult.put("tenantId", authenticationResult.getTenantId());

        JSONObject userInfo = null;
        try {
            userInfo = userInfoToJSON(authenticationResult.getUserInfo());
        } catch (JSONException ignored) {}

        authResult.put("userInfo", userInfo);

        return authResult;
    }

    /**
     * Convert TokenCacheItem object to JSON representation. Nested userInfo field is being
     * serialized as well. In case if userInfo field is not exists in input object it will
     * be equal to null in resultant object
     * @param item TokenCacheItem object
     * @return JSONObject that represents a TokenCacheItem structure
     * @throws JSONException
     */
    static JSONObject tokenItemToJSON(TokenCacheItem item) throws JSONException {
        JSONObject result = new JSONObject();

        result.put("accessToken", item.getAccessToken());
        result.put("authority", item.getAuthority());
        result.put("clientId", item.getClientId());
        result.put("expiresOn", item.getExpiresOn());
        result.put("isMultipleResourceRefreshToken", item.getIsMultiResourceRefreshToken());
        result.put("resource", item.getResource());
        result.put("tenantId", item.getTenantId());
        result.put("idToken", item.getRawIdToken());

        JSONObject userInfo = null;
        try {
            userInfo = userInfoToJSON(item.getUserInfo());
        } catch (JSONException ignored) {}

        result.put("userInfo", userInfo);

        return result;
    }
}
