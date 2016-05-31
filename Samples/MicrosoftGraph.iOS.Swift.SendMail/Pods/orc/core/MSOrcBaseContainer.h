/*******************************************************************************
 * Copyright (c) Microsoft Open Technologies, Inc.
 * All Rights Reserved
 * Licensed under the Apache License, Version 2.0.
 * See License.txt in the project root for license information.
 ******************************************************************************/

#import "MSOrcExecutable.h"
#import "api/MSOrcDependencyResolver.h"
#import "api/MSOrcRequest.h"

@interface MSOrcBaseContainer : MSOrcExecutable

- (instancetype)initWithUrl:(NSString *)url dependencyResolver:(id<MSOrcDependencyResolver>)resolver;

+ (NSString *)generatePayloadWithParameters:(NSArray *)parameters
                         dependencyResolver:(id<MSOrcDependencyResolver>)resolver;

+ (void)addCustomParametersToOrcURLWithRequest:(id<MSOrcRequest>)request
                                      parameters:(NSDictionary *)parameters
                                         headers:(NSDictionary *)headers
                              dependencyResolver:(id<MSOrcDependencyResolver>)resolver;

+ (NSString *)getFunctionParameters:(NSDictionary *)parameters;

@end