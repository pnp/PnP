/*******************************************************************************
 * Copyright (c) Microsoft Open Technologies, Inc.
 * All Rights Reserved
 * See License in the project root for license information.
 ******************************************************************************/

#import <Foundation/Foundation.h>
#import <Cordova/CDVPlugin.h>

#import <ADALiOS/ADAuthenticationContext.h>

// Implements Apache Cordova plugin for Microsoft Azure ADAL
@interface CordovaAdalPlugin : CDVPlugin

// AuthenticationContext methods
- (void)createAsync:(CDVInvokedUrlCommand *)command;
- (void)acquireTokenAsync:(CDVInvokedUrlCommand *)command;
- (void)acquireTokenSilentAsync:(CDVInvokedUrlCommand *)command;

// TokenCache methods
- (void)tokenCacheClear:(CDVInvokedUrlCommand *)command;
- (void)tokenCacheReadItems:(CDVInvokedUrlCommand *)command;
- (void)tokenCacheDeleteItem:(CDVInvokedUrlCommand *)command;

+ (ADAuthenticationContext *)getOrCreateAuthContext:(NSString *)authority
                                  validateAuthority:(BOOL)validate;
@end
