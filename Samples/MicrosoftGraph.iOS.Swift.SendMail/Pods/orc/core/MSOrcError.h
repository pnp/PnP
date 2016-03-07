/*******************************************************************************
 * Copyright (c) Microsoft Open Technologies, Inc.
 * All Rights Reserved
 * Licensed under the Apache License, Version 2.0.
 * See License.txt in the project root for license information.
 ******************************************************************************/

#import <Foundation/Foundation.h>
#import "api/MSOrcResponse.h"

@interface MSOrcError : NSError

@property (copy, nonatomic, readonly) id<MSOrcResponse> response;
@property (copy, nonatomic, readonly) NSData *payload;

- (instancetype)initWithResponse:(id<MSOrcResponse>)response andMessage:(NSString *)message;
- (instancetype)initWithResponse:(id<MSOrcResponse>)response andError:(NSError *)error;

@end