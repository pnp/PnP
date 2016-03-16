/*******************************************************************************
 * Copyright (c) Microsoft Open Technologies, Inc.
 * All Rights Reserved
 * Licensed under the Apache License, Version 2.0.
 * See License.txt in the project root for license information.
 ******************************************************************************/

#import <Foundation/Foundation.h>

@protocol MSOrcMultiPartElement

@required

@property (copy, nonatomic, readonly) NSData *content;
@property (copy, nonatomic, readonly) NSString *name;
@property (copy, nonatomic, readonly) NSString *contentType;

- (instancetype)initWithName:(NSString *)name andContentString:(NSString *)content;
- (instancetype)initWithName:(NSString *)name andContentType:(NSString *)contentType andContent:(NSData *)content;

@end

@interface MSOrcMultiPartElement : NSObject<MSOrcMultiPartElement>

@end