/*******************************************************************************
 * Copyright (c) Microsoft Open Technologies, Inc.
 * All Rights Reserved
 * Licensed under the Apache License, Version 2.0.
 * See License.txt in the project root for license information.
 ******************************************************************************/

#import "LiveDependencyResolver.h"
#import "LiveConnectClient.h"
#import "LiveAuthDelegate.h"

@implementation LiveDependencyResolver

@synthesize handlerController = _handlerController;
@synthesize delegate = _delegate;
@synthesize liveClient = _liveClient;

- (instancetype)initWithClientId:(NSString *)clientId
                       andScopes:(NSArray *)scopes
                   andLiveClient:(LiveConnectClient *)client
                     andDelegate:(id<LiveAuthDelegate>)delegate
                         andView:(UIViewController *)controller {
    
    if (self = [super init]) {
        
         _liveClient = [client initWithClientId:clientId scopes:scopes
                                       delegate:delegate
                                      userState:@"init"];
         _delegate = delegate;
         _handlerController = controller;
    }
    
    return self;
}

- (id<MSOrcCredentials>)credentials {

    __block MSOrcOAuthCredentials *credentials = [[MSOrcOAuthCredentials alloc] init];

    [self.liveClient login:self.handlerController delegate:self.delegate];

    credentials.token = self.liveClient.session.accessToken;

    return credentials;
}

@end