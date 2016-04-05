/*******************************************************************************
 * Copyright (c) Microsoft Open Technologies, Inc.
 * All Rights Reserved
 * Licensed under the Apache License, Version 2.0.
 * See License.txt in the project root for license information.
 ******************************************************************************/

#import "MSOrcReadable.h"
#import "MSOrcError.h"
#import "api/MSOrcDependencyResolver.h"
#import "api/MSOrcRequest.h"
#import "api/MSOrcResponse.h"

@protocol MSOrcExecutable<MSOrcReadable>

@optional

@property (copy, nonatomic, readonly) id<MSOrcDependencyResolver> resolver;
- (void)orcExecuteRequest:(id<MSOrcRequest>)request callback:(void (^)(id<MSOrcResponse> response, MSOrcError *error))callback;

@end

@interface MSOrcExecutable : NSObject<MSOrcExecutable>

@property (copy, nonatomic, readonly) NSMutableDictionary *customParameters;
@property (copy, nonatomic, readonly) NSMutableDictionary *customHeaders;
@property (copy, nonatomic, readonly) NSString *urlComponent;
@property (copy, nonatomic, readonly) id<MSOrcExecutable> parent;
@property (copy, nonatomic, readonly) Class entityClass;

- (instancetype)initWithUrl:(NSString *)urlComponent parent:(id<MSOrcExecutable>)parent asClass:(Class)entityClass;

- (id<MSOrcExecutable>)addCustomHeaderWithName:(NSString *)name value:(NSString *)value;
- (id<MSOrcExecutable>)addCustomParametersWithName:(NSString *)name value:(NSString *)value;

@end