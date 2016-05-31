/*******************************************************************************
 * Copyright (c) Microsoft Open Technologies, Inc.
 * All Rights Reserved
 * Licensed under the Apache License, Version 2.0.
 * See License.txt in the project root for license information.
 ******************************************************************************/

#import <Foundation/Foundation.h>

@protocol MSOrcRequest;

@protocol MSOrcCredentials <NSObject>

@required

- (void)prepareRequest:(id<MSOrcRequest>)request;

@property (copy, atomic, readwrite) NSString *token;

@end