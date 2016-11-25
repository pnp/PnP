/*******************************************************************************
 * Copyright (c) Microsoft Open Technologies, Inc.
 * All Rights Reserved
 * Licensed under the Apache License, Version 2.0.
 * See License.txt in the project root for license information.
 ******************************************************************************/

package com.microsoft.aad.adal;

import android.content.Intent;
import org.apache.cordova.CallbackContext;
import org.apache.cordova.CordovaPlugin;
import org.apache.cordova.PluginResult;
import org.json.JSONArray;
import org.json.JSONException;

import java.security.NoSuchAlgorithmException;
import java.util.ArrayList;
import java.util.Hashtable;
import java.util.Iterator;

import javax.crypto.NoSuchPaddingException;

import static com.microsoft.aad.adal.SimpleSerialization.tokenItemToJSON;

public class CordovaAdalPlugin extends CordovaPlugin {

    private static final PromptBehavior SHOW_PROMPT_ALWAYS = PromptBehavior.Always;

    private final Hashtable<String, AuthenticationContext> contexts = new Hashtable<String, AuthenticationContext>();
    private AuthenticationContext currentContext;
    private CallbackContext callbackContext;

    @Override
    public boolean execute(String action, JSONArray args, final CallbackContext callbackContext) throws JSONException {

        this.cordova.setActivityResultCallback(this);
        this.callbackContext = callbackContext;

        if (action.equals("createAsync")) {

            // Don't catch JSONException since it is already handled by Cordova
            String authority = args.getString(0);
            // Due to https://github.com/AzureAD/azure-activedirectory-library-for-android/blob/master/src/src/com/microsoft/aad/adal/AuthenticationContext.java#L158
            // AuthenticationContext constructor validates authority by default
            boolean validateAuthority = args.optBoolean(1, true);
            return createAsync(authority, validateAuthority);

        } else if (action.equals("acquireTokenAsync")) {

            String authority = args.getString(0);
            String resourceUrl = args.getString(1);
            String clientId = args.getString(2);
            String redirectUrl = args.getString(3);
            String userId = args.optString(4, null);
            userId = userId.equals("null") ? null : userId;
            String extraQueryParams = args.optString(5, null);
            extraQueryParams = extraQueryParams.equals("null") ? null : extraQueryParams;

            return acquireTokenAsync(authority, resourceUrl, clientId, redirectUrl, userId, extraQueryParams);

        } else if (action.equals("acquireTokenSilentAsync")) {

            String authority = args.getString(0);
            String resourceUrl = args.getString(1);
            String clientId = args.getString(2);
            String userId = args.getString(3);

            // This is a workaround for Cordova bridge issue. When null us passed from JS side
            // it is being translated to "null" string
            userId = userId.equals("null") ? null : userId;

            return acquireTokenSilentAsync(authority, resourceUrl, clientId, userId);

        } else if (action.equals("tokenCacheClear")){

            String authority = args.getString(0);
            return clearTokenCache(authority);

        } else if (action.equals("tokenCacheReadItems")){

            String authority = args.getString(0);
            return readTokenCacheItems(authority);

        } else if (action.equals("tokenCacheDeleteItem")){

            String authority = args.getString(0);
            String itemAuthority = args.getString(1);
            String resource = args.getString(2);
            resource = resource.equals("null") ? null : resource;
            String clientId = args.getString(3);
            String userId = args.getString(4);
            boolean isMultipleResourceRefreshToken = args.getBoolean(5);

            return deleteTokenCacheItem(authority, itemAuthority, resource, clientId, userId, isMultipleResourceRefreshToken);
        }

        return false;
    }

    private boolean createAsync(String authority, boolean validateAuthority) {

        try {
            getOrCreateContext(authority, validateAuthority);
        } catch (Exception e) {
            callbackContext.sendPluginResult(new PluginResult(PluginResult.Status.ERROR, e.getMessage()));
            return true;
        }

        callbackContext.success();
        return true;
    }

    private boolean acquireTokenAsync(String authority, String resourceUrl, String clientId, String redirectUrl, String userId, String extraQueryParams) {

        final AuthenticationContext authContext;
        try{
            authContext = getOrCreateContext(authority);
        } catch (Exception e) {
            callbackContext.sendPluginResult(new PluginResult(PluginResult.Status.ERROR, e.getMessage()));
            return true;
        }

        if (userId != null) {
            ITokenCacheStore cache = authContext.getCache();
            if (cache instanceof ITokenStoreQuery) {

                ArrayList<TokenCacheItem> tokensForUserId = ((ITokenStoreQuery)cache).getTokensForUser(userId);
                if (tokensForUserId.size() > 0) {
                    // Try to acquire alias for specified userId
                    userId = tokensForUserId.get(0).getUserInfo().getDisplayableId();
                }
            }
        }

        authContext.acquireToken(this.cordova.getActivity(), resourceUrl, clientId, redirectUrl,
                userId, SHOW_PROMPT_ALWAYS, extraQueryParams, new DefaultAuthenticationCallback(callbackContext));

        return true;

    }

    private boolean acquireTokenSilentAsync(String authority, String resourceUrl, String clientId, String userId) {

        final AuthenticationContext authContext;
        try{
            authContext = getOrCreateContext(authority);
        } catch (Exception e) {
            callbackContext.sendPluginResult(new PluginResult(PluginResult.Status.ERROR, e.getMessage()));
            return true;
        }

        authContext.acquireTokenSilent(resourceUrl, clientId, userId, new DefaultAuthenticationCallback(callbackContext));
        return true;
    }

    private boolean readTokenCacheItems(String authority) throws JSONException {

        final AuthenticationContext authContext;
        try{
            authContext = getOrCreateContext(authority);
        } catch (Exception e) {
            callbackContext.sendPluginResult(new PluginResult(PluginResult.Status.ERROR, e.getMessage()));
            return true;
        }

        JSONArray result = new JSONArray();
        ITokenCacheStore cache = authContext.getCache();

        if (cache instanceof ITokenStoreQuery) {
            Iterator<TokenCacheItem> cacheItems = ((ITokenStoreQuery)cache).getAll();

            while (cacheItems.hasNext()){
                TokenCacheItem item = cacheItems.next();
                result.put(tokenItemToJSON(item));
            }
        }

        callbackContext.sendPluginResult(new PluginResult(PluginResult.Status.OK, result));

        return true;
    }

    private boolean deleteTokenCacheItem(String authority, String itemAuthority,  String resource,
                                         String clientId, String userId, boolean isMultipleResourceRefreshToken) {

        final AuthenticationContext authContext;
        try{
            authContext = getOrCreateContext(authority);
        } catch (Exception e) {
            callbackContext.sendPluginResult(new PluginResult(PluginResult.Status.ERROR, e.getMessage()));
            return true;
        }

        String key = CacheKey.createCacheKey(itemAuthority, resource, clientId, isMultipleResourceRefreshToken, userId);
        authContext.getCache().removeItem(key);

        callbackContext.sendPluginResult(new PluginResult(PluginResult.Status.OK));
        return true;
    }

    private boolean clearTokenCache(String authority) {
        final AuthenticationContext authContext;
        try{
            authContext = getOrCreateContext(authority);
        } catch (Exception e) {
            callbackContext.sendPluginResult(new PluginResult(PluginResult.Status.ERROR, e.getMessage()));
            return true;
        }

        authContext.getCache().removeAll();
        callbackContext.sendPluginResult(new PluginResult(PluginResult.Status.OK));
        return true;
    }

    @Override
    public void onActivityResult(int requestCode, int resultCode, Intent data) {
        super.onActivityResult(requestCode, resultCode, data);
        if (currentContext != null) {
            currentContext.onActivityResult(requestCode, resultCode, data);
        }
    }

    private AuthenticationContext getOrCreateContext (String authority, boolean validateAuthority) throws NoSuchPaddingException, NoSuchAlgorithmException {

        AuthenticationContext result;
        if (!contexts.containsKey(authority)) {
            result = new AuthenticationContext(this.cordova.getActivity(), authority, validateAuthority);
            this.contexts.put(authority, result);
        } else {
            result = contexts.get(authority);
        }

        currentContext = result;
        return result;
    }

    private AuthenticationContext getOrCreateContext (String authority) throws NoSuchPaddingException, NoSuchAlgorithmException {
        return getOrCreateContext(authority, false);
    }
}
