/*******************************************************************************
 * Copyright (c) Microsoft Open Technologies, Inc.
 * All Rights Reserved
 * Licensed under the Apache License, Version 2.0.
 * See License.txt in the project root for license information.
 ******************************************************************************/

package com.microsoft.aad.adal;

import org.apache.cordova.CallbackContext;
import org.apache.cordova.PluginResult;
import org.json.JSONException;
import org.json.JSONObject;

import static com.microsoft.aad.adal.SimpleSerialization.authenticationResultToJSON;

/**
 * Class that provides implementation for passing AuthenticationResult from acquireToken* methods
 * to Cordova JS code
 */
class DefaultAuthenticationCallback implements AuthenticationCallback<AuthenticationResult> {

    /**
     * Private field that stores cordova callback context which is used to send results back to JS
     */
    private final CallbackContext callbackContext;

    /**
     * Default constructor
     * @param callbackContext Cordova callback context which is used to send results back to JS
     */
    DefaultAuthenticationCallback(CallbackContext callbackContext){
        this.callbackContext = callbackContext;
    }

    /**
     * Success callback that serializes AuthenticationResult instance and passes it to Cordova
     * @param authResult AuthenticationResult instance
     */
    @Override
    public void onSuccess(AuthenticationResult authResult) {

        JSONObject result;
        try {
            result = authenticationResultToJSON(authResult);
            callbackContext.sendPluginResult(new PluginResult(PluginResult.Status.OK, result));
        } catch (JSONException e) {
            callbackContext.sendPluginResult(new PluginResult(PluginResult.Status.JSON_EXCEPTION,
                    "Failed to serialize Authentication result"));
        }
    }

    /**
     * Error callback that passes error to Cordova
     * @param e AuthenticationException
     */
    @Override
    public void onError(Exception e) {
        callbackContext.sendPluginResult(new PluginResult(PluginResult.Status.ERROR, e.getMessage()));
    }
}
