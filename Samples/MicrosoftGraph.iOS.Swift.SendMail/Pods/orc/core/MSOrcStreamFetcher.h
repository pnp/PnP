/*******************************************************************************
 * Copyright (c) Microsoft Open Technologies, Inc.
 * All Rights Reserved
 * Licensed under the Apache License, Version 2.0.
 * See License.txt in the project root for license information.
 ******************************************************************************/

#import "core/MSOrcEntityFetcher.h"

#define OPT_STREAM_DOWNLOAD @"MUST_STREAM_DOWNLOAD_CONTENT"
#define OPT_STREAM_UPLOAD @"MUST_STREAM_UPLOAD_CONTENT"

@interface MSOrcStreamFetcher : MSOrcEntityFetcher

- (id)initWithUrl:(NSString *)urlComponent parent:(id<MSOrcReadable>)parent;

- (void)getContentWithCallback:(void (^)(NSData *content, MSOrcError * error))callback;

- (void)getStreamedContentWithCallback:(void (^)(NSInputStream *content, MSOrcError *error))callback;

- (void)putContent:(NSData *)content
          callback:(void (^)(NSInteger statusCode, MSOrcError *error))callback;

- (void)putContent:(NSInputStream *)content
          withSize:(NSInteger)size
          callback:(void (^)(NSInteger statusCode, MSOrcError *error))callback;

@end