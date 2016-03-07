/*******************************************************************************
 * Copyright (c) Microsoft Open Technologies, Inc.
 * All Rights Reserved
 * Licensed under the Apache License, Version 2.0.
 * See License.txt in the project root for license information.
 ******************************************************************************/

#import <Foundation/Foundation.h>

@protocol MSOrcURL <NSObject>

@required

@property (copy, nonatomic, readonly) NSMutableDictionary *queryStringParameters;
@property (copy, nonatomic, readonly) NSMutableArray *pathComponents;
@property (copy, nonatomic, readwrite) NSString *baseUrl;

- (void)appendPathComponent:(NSString *)pathComponent;
- (void)addQueryStringParameter:(NSString *)name value:(NSString *)value;
- (NSString *)toString;
                                                    
@end