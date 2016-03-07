/*******************************************************************************
 * Copyright (c) Microsoft Open Technologies, Inc.
 * All Rights Reserved
 * Licensed under the Apache License, Version 2.0.
 * See License.txt in the project root for license information.
 ******************************************************************************/

#import "MSOrcError.h"

@protocol MSOrcReadable

@optional

- (void)readWithCallback:(void (^)(id response, MSOrcError *error))callback;
- (void)readRawWithCallback:(void (^)(NSString *responseString, MSOrcError *error))callback;

@end