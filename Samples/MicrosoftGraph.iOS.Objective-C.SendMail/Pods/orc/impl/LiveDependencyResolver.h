/*******************************************************************************
 * Copyright (c) Microsoft Open Technologies, Inc.
 * All Rights Reserved
 * Licensed under the Apache License, Version 2.0.
 * See License.txt in the project root for license information.
 ******************************************************************************/

#import "MSOrcDefaultDependencyResolver.h"
#import "MSOrcOAuthCredentials.h"
#import <UIKit/UIKit.h>

@protocol LiveAuthDelegate;
@class LiveConnectClient;

@interface LiveDependencyResolver : MSOrcDefaultDependencyResolver

@property (copy, nonatomic, readonly) LiveConnectClient *liveClient;
@property (copy, nonatomic, readonly) UIViewController *handlerController;
@property (copy, nonatomic, readonly) id<LiveAuthDelegate> delegate;

- (id)initWithClientId:(NSString *)clientId
             andScopes:(NSArray *)scopes
         andLiveClient:(LiveConnectClient *)client
           andDelegate:(id<LiveAuthDelegate>)delegate
               andView:(UIViewController*)controller;

@end