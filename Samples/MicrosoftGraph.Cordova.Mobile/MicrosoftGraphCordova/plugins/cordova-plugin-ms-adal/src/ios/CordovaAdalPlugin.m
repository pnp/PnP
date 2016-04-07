/*******************************************************************************
 * Copyright (c) Microsoft Open Technologies, Inc.
 * All Rights Reserved
 * See License in the project root for license information.
 ******************************************************************************/

#import "CordovaAdalPlugin.h"
#import "CordovaAdalUtils.h"

@implementation CordovaAdalPlugin

- (void)createAsync:(CDVInvokedUrlCommand *)command
{
    [self.commandDelegate runInBackground:^{
        @try
        {
            NSString *authority = ObjectOrNil([command.arguments objectAtIndex:0]);
            BOOL validateAuthority = [[command.arguments objectAtIndex:1] boolValue];

            [CordovaAdalPlugin getOrCreateAuthContext:authority
                                    validateAuthority:validateAuthority];

            CDVPluginResult *pluginResult = [CDVPluginResult resultWithStatus:CDVCommandStatus_OK];

            [self.commandDelegate sendPluginResult:pluginResult callbackId:command.callbackId];
        }
        @catch (ADAuthenticationError *error)
        {
            CDVPluginResult *pluginResult = [CDVPluginResult resultWithStatus:CDVCommandStatus_ERROR
                                                          messageAsDictionary:[CordovaAdalUtils ADAuthenticationErrorToDictionary:error]];
            [self.commandDelegate sendPluginResult:pluginResult callbackId:command.callbackId];
        }
    }];
}

- (void)acquireTokenAsync:(CDVInvokedUrlCommand *)command
{
    [self.commandDelegate runInBackground:^{
        @try
        {
            NSString *authority = ObjectOrNil([command.arguments objectAtIndex:0]);
            NSString *resourceId = ObjectOrNil([command.arguments objectAtIndex:1]);
            NSString *clientId = ObjectOrNil([command.arguments objectAtIndex:2]);
            NSURL *redirectUri = [NSURL URLWithString:[command.arguments objectAtIndex:3]];
            NSString *userId = ObjectOrNil([command.arguments objectAtIndex:4]);
            NSString *extraQueryParameters = ObjectOrNil([command.arguments objectAtIndex:5]);

            ADAuthenticationContext *authContext = [CordovaAdalPlugin getOrCreateAuthContext:authority
                                                                           validateAuthority:FALSE];

            // TODO iOS sdk requires user name instead of guid so we should map provided id to a known user name
            userId = [CordovaAdalUtils mapUserIdToUserName:authContext
                                                    userId:userId];

            [authContext acquireTokenWithResource:resourceId
                                         clientId:clientId
                                      redirectUri:redirectUri
                                   promptBehavior:AD_PROMPT_ALWAYS
                                           userId:userId
                             extraQueryParameters:extraQueryParameters
                                  completionBlock:^(ADAuthenticationResult *result) {

                                      NSMutableDictionary *msg = [CordovaAdalUtils ADAuthenticationResultToDictionary: result];
                                      CDVCommandStatus status = (AD_SUCCEEDED != result.status) ? CDVCommandStatus_ERROR : CDVCommandStatus_OK;
                                      CDVPluginResult *pluginResult = [CDVPluginResult resultWithStatus:status messageAsDictionary: msg];
                                      [self.commandDelegate sendPluginResult:pluginResult callbackId:command.callbackId];
                                  }];
        }
        @catch (ADAuthenticationError *error)
        {
            CDVPluginResult *pluginResult = [CDVPluginResult resultWithStatus:CDVCommandStatus_ERROR
                                                          messageAsDictionary:[CordovaAdalUtils ADAuthenticationErrorToDictionary:error]];
            [self.commandDelegate sendPluginResult:pluginResult callbackId:command.callbackId];
        }
    }];
}

- (void)acquireTokenSilentAsync:(CDVInvokedUrlCommand *)command
{
    [self.commandDelegate runInBackground:^{
        @try
        {
            NSString *authority = ObjectOrNil([command.arguments objectAtIndex:0]);
            NSString *resourceId = ObjectOrNil([command.arguments objectAtIndex:1]);
            NSString *clientId = ObjectOrNil([command.arguments objectAtIndex:2]);
            NSString *userId = ObjectOrNil([command.arguments objectAtIndex:3]);

            ADAuthenticationContext *authContext = [CordovaAdalPlugin getOrCreateAuthContext:authority
                                                                           validateAuthority:FALSE];

            // TODO iOS sdk requires user name instead of guid so we should map provided id to a known user name
            userId = [CordovaAdalUtils mapUserIdToUserName:authContext
                                                    userId:userId];

            [authContext acquireTokenSilentWithResource:resourceId
                                               clientId:clientId
                                            redirectUri:nil
                                                 userId:userId
                                        completionBlock:^(ADAuthenticationResult *result) {
                                            NSMutableDictionary *msg = [CordovaAdalUtils ADAuthenticationResultToDictionary: result];
                                            CDVCommandStatus status = (AD_SUCCEEDED != result.status) ? CDVCommandStatus_ERROR : CDVCommandStatus_OK;
                                            CDVPluginResult *pluginResult = [CDVPluginResult resultWithStatus:status messageAsDictionary: msg];
                                            [self.commandDelegate sendPluginResult:pluginResult callbackId:command.callbackId];
                                        }];
        }
        @catch (ADAuthenticationError *error)
        {
            CDVPluginResult *pluginResult = [CDVPluginResult resultWithStatus:CDVCommandStatus_ERROR
                                                          messageAsDictionary:[CordovaAdalUtils ADAuthenticationErrorToDictionary:error]];
            [self.commandDelegate sendPluginResult:pluginResult callbackId:command.callbackId];
        }
    }];
}

- (void)tokenCacheClear:(CDVInvokedUrlCommand *)command
{
    [self.commandDelegate runInBackground:^{
        @try
        {
            ADAuthenticationError *error;

            NSString *authority = ObjectOrNil([command.arguments objectAtIndex:0]);

            ADAuthenticationContext *authContext = [CordovaAdalPlugin getOrCreateAuthContext:authority
                                                                           validateAuthority:FALSE];

            [authContext.tokenCacheStore removeAllWithError:&error];

            if (error != nil)
            {
                @throw(error);
            }

            CDVPluginResult *pluginResult = [CDVPluginResult resultWithStatus:CDVCommandStatus_OK];

            [self.commandDelegate sendPluginResult:pluginResult callbackId:command.callbackId];
        }
        @catch (ADAuthenticationError *error)
        {
            CDVPluginResult *pluginResult = [CDVPluginResult resultWithStatus:CDVCommandStatus_ERROR
                                                          messageAsDictionary:[CordovaAdalUtils ADAuthenticationErrorToDictionary:error]];
            [self.commandDelegate sendPluginResult:pluginResult callbackId:command.callbackId];
        }
    }];
}

- (void)tokenCacheReadItems:(CDVInvokedUrlCommand *)command
{
    [self.commandDelegate runInBackground:^{
        @try
        {
            ADAuthenticationError *error;

            NSString *authority = ObjectOrNil([command.arguments objectAtIndex:0]);

            ADAuthenticationContext *authContext = [CordovaAdalPlugin getOrCreateAuthContext:authority
                                                                           validateAuthority:FALSE];

            NSArray *cacheItems = [authContext.tokenCacheStore allItemsWithError:&error];

            NSMutableArray *items = [NSMutableArray arrayWithCapacity:cacheItems.count];

            if (error != nil)
            {
                @throw(error);
            }

            for (id obj in cacheItems)
            {
                [items addObject:[CordovaAdalUtils ADTokenCacheStoreItemToDictionary:obj]];
            }

            CDVPluginResult *pluginResult = [CDVPluginResult resultWithStatus:CDVCommandStatus_OK
                                                               messageAsArray:items];

            [self.commandDelegate sendPluginResult:pluginResult callbackId:command.callbackId];
        }
        @catch (ADAuthenticationError *error)
        {
            CDVPluginResult *pluginResult = [CDVPluginResult resultWithStatus:CDVCommandStatus_ERROR
                                                          messageAsDictionary:[CordovaAdalUtils ADAuthenticationErrorToDictionary:error]];
            [self.commandDelegate sendPluginResult:pluginResult callbackId:command.callbackId];
        }
    }];
}
- (void)tokenCacheDeleteItem:(CDVInvokedUrlCommand *)command
{
    [self.commandDelegate runInBackground:^{
        @try
        {
            ADAuthenticationError *error;

            NSString *authority = ObjectOrNil([command.arguments objectAtIndex:0]);
            NSString *itemAuthority = ObjectOrNil([command.arguments objectAtIndex:1]);
            NSString *resourceId = ObjectOrNil([command.arguments objectAtIndex:2]);
            NSString *clientId = ObjectOrNil([command.arguments objectAtIndex:3]);
            NSString *userId = ObjectOrNil([command.arguments objectAtIndex:4]);

            ADAuthenticationContext *authContext = [CordovaAdalPlugin getOrCreateAuthContext:authority
                                                                           validateAuthority:FALSE];

            // TODO iOS sdk requires user name instead of guid so we should map provided id to a known user name
            userId = [CordovaAdalUtils mapUserIdToUserName:authContext
                                                    userId:userId];

            ADTokenCacheStoreKey *key = [ADTokenCacheStoreKey keyWithAuthority:itemAuthority resource:resourceId clientId:clientId error:&error];

            if (error != nil)
            {
                @throw(error);
            }

            [authContext.tokenCacheStore removeItemWithKey:key userId:userId error:&error];

            if (error != nil)
            {
                @throw(error);
            }

            CDVPluginResult *pluginResult = [CDVPluginResult resultWithStatus:CDVCommandStatus_OK];
            [self.commandDelegate sendPluginResult:pluginResult callbackId:command.callbackId];
        }
        @catch (ADAuthenticationError *error)
        {
            CDVPluginResult *pluginResult = [CDVPluginResult resultWithStatus:CDVCommandStatus_ERROR
                                                          messageAsDictionary:[CordovaAdalUtils ADAuthenticationErrorToDictionary:error]];
            [self.commandDelegate sendPluginResult:pluginResult callbackId:command.callbackId];
        }
    }];
}

static NSMutableDictionary *existingContexts = nil;

+ (ADAuthenticationContext *)getOrCreateAuthContext:(NSString *)authority
                                  validateAuthority:(BOOL)validate
{
    if (!existingContexts)
    {
        existingContexts = [NSMutableDictionary dictionaryWithCapacity:1];
    }

    ADAuthenticationContext *authContext = [existingContexts objectForKey:authority];

    if (!authContext)
    {
        ADAuthenticationError *error;

        authContext = [ADAuthenticationContext authenticationContextWithAuthority:authority
                                                                validateAuthority:validate
                                                                            error:&error];
        if (error != nil)
        {
            @throw(error);
        }

        [existingContexts setObject:authContext forKey:authority];
    }

    return authContext;
}

static id ObjectOrNil(id object)
{
    return [object isKindOfClass:[NSNull class]] ? nil : object;
}

@end
