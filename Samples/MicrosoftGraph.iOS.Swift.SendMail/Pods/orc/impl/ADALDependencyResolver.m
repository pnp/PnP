/*******************************************************************************
 * Copyright (c) Microsoft Open Technologies, Inc.
 * All Rights Reserved
 * Licensed under the Apache License, Version 2.0.
 * See License.txt in the project root for license information.
 ******************************************************************************/

#import "api.h"
#import "ADALDependencyResolver.h"
#import "ADAuthenticationResult.h"
#import "ADAuthenticationContext.h"
#import "ADKeychainTokenCacheStore.h"
#import "impl/MSOrcOAuthCredentials.h"

@interface ADALDependencyResolver ()

@property (strong, atomic, readonly) ADAuthenticationContext *context;
@property (strong, atomic, readonly) NSString *clientId;
@property (strong, atomic, readonly) NSURL *redirectUri;
@property (strong, nonatomic) NSDictionary *settings;

@end

@implementation ADALDependencyResolver


// Designated initializer
- (instancetype)initWithContext:(ADAuthenticationContext *)context
                     resourceId:(NSString *)resourceId
                       clientId:(NSString *)clientId
                    redirectUri:(NSURL *)redirectUri {
    
    if (self = [super init]) {
        _clientId = clientId;
        _context = context;
        _resourceId = resourceId;
        _redirectUri = redirectUri;
        
    }
    
    return self;
}

/*!
 Relies on adal_settings.plist
*/
- (instancetype)initWithPlist {
    
    self = [super init];
    
    NSString *path = [[NSBundle mainBundle] pathForResource:@"adal_settings" ofType:@"plist"];
    if (path) {
        _settings = [[NSDictionary alloc] initWithContentsOfFile:path];
    } else {
        @throw([[NSException alloc] initWithName:@"NO_SETTINGS_PLIST" reason:@"adal_settings.plist not found in bundle." userInfo:[[NSDictionary alloc] init]]);
    }
    
    ADAuthenticationError *adError;
    ADAuthenticationContext *ctx = [[ADAuthenticationContext alloc]
                                            initWithAuthority:[_settings valueForKey:@"AuthorityUrl"]
                                            validateAuthority:NO
                                              tokenCacheStore:[[ADKeychainTokenCacheStore alloc] init]
                                                        error:&adError];
    if (adError) {
        @throw(adError);
    }
    
    return [self initWithContext:ctx
                      resourceId:[self.settings valueForKey:@"ResourceId"]
                        clientId:[self.settings valueForKey:@"ClientId"]
                     redirectUri:[NSURL URLWithString:[self.settings valueForKey:@"RedirectUri"]]];
}

- (void)interactiveLogon {
    [self interactiveLogonWithCallback:nil];
}

- (void)interactiveLogonWithCallback:(void(^)(ADAuthenticationResult *result))callback {
    
    void(^theCompletionBlock)(ADAuthenticationResult *);
    
    void(^nullCallback)(ADAuthenticationResult *) = ^(ADAuthenticationResult *result) {
        if (result.status != AD_SUCCEEDED) {
            [self.logger logMessage:result.error.errorDetails withLevel:LOG_LEVEL_ERROR];
        } else {
            [self.logger logMessage:@"AD auth succeeded." withLevel:LOG_LEVEL_INFO];
        }
    };
    
    if (callback) {
        theCompletionBlock = callback;
    } else {
        theCompletionBlock = nullCallback;
    }
    
    [self.context acquireTokenWithResource:self.resourceId
                                  clientId:self.clientId
                               redirectUri:self.redirectUri
                            promptBehavior:AD_PROMPT_ALWAYS
                                    userId:nil
                      extraQueryParameters:nil
                            completionBlock:theCompletionBlock];
    
}

- (id<MSOrcCredentials>)credentials {
    
    __block MSOrcOAuthCredentials *credentials;
    
    dispatch_semaphore_t sem = dispatch_semaphore_create(0);
    
    [self.context acquireTokenSilentWithResource:self.resourceId
                                        clientId:self.clientId
                                     redirectUri:self.redirectUri
                                 completionBlock:^(ADAuthenticationResult *result) {
                                     
                                     credentials = [[MSOrcOAuthCredentials alloc] init];
                                     credentials.token = result.accessToken;
                                     
                                     dispatch_semaphore_signal(sem);
                                 }];
    
    dispatch_semaphore_wait(sem, 10);
    
    return credentials;
}

@end