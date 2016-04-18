/*******************************************************************************
 * Copyright (c) Microsoft Open Technologies, Inc.
 * All Rights Reserved
 * Licensed under the Apache License, Version 2.0.
 * See License.txt in the project root for license information.
 ******************************************************************************/

#import <Foundation/Foundation.h>

@protocol MSOrcHttpTransport;
@protocol MSOrcLogger;
@protocol MSOrcJsonSerializer;
@protocol MSOrcCredentials;
@protocol MSOrcRequest;

@protocol MSOrcDependencyResolver <NSObject>

@required

@property (strong, atomic, readwrite) id<MSOrcCredentials> credentials;
@property (strong, nonatomic, readonly) id<MSOrcHttpTransport> httpTransport;
@property (strong, nonatomic, readonly) id<MSOrcLogger> logger;

- (id<MSOrcRequest>)createOrcRequest;
- (NSString *)getPlatformUserAgent:(NSString *)productName;

@end
