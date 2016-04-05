/*******************************************************************************
 * Copyright (c) Microsoft Open Technologies, Inc.
 * All Rights Reserved
 * Licensed under the Apache License, Version 2.0.
 * See License.txt in the project root for license information.
 ******************************************************************************/

#import <Foundation/Foundation.h>

@protocol MSOrcResponse <NSObject>

@required

@property (strong, atomic, readonly) NSData *data;
@property (strong, atomic, readonly) NSURLResponse *response;
@property (strong, atomic, readonly) NSInputStream *stream;
@property (atomic, readonly) int status;

- (instancetype)initWithData:(NSData *)data response:(NSURLResponse *)response;
- (instancetype)initWithStream:(NSInputStream *)stream response:(NSURLResponse *)response;

@optional
- (void)close;

@end