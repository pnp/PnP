/*******************************************************************************
 * Copyright (c) Microsoft Open Technologies, Inc.
 * All Rights Reserved
 * Licensed under the Apache License, Version 2.0.
 * See License.txt in the project root for license information.
 ******************************************************************************/

#import "MSOrcExecutable.h"

@implementation MSOrcExecutable

@synthesize parent = _parent;
@synthesize urlComponent = _urlComponent;
@synthesize customHeaders = _customHeaders;
@synthesize customParameters = _customParameters;
@synthesize entityClass = _entityClass;
@synthesize resolver = _resolver;

- (instancetype)initWithUrl:(NSString *)urlComponent parent:(id<MSOrcExecutable>)parent asClass:(Class)entityClass {
    
    if (self = [super init]) {
        
        _resolver = [parent resolver];
        _urlComponent = urlComponent;
        _parent = parent;
        _entityClass = entityClass;
        _customParameters = [[NSMutableDictionary alloc] init];
        _customHeaders = [[NSMutableDictionary alloc] init];
    }
    
    return self;
}

- (id<MSOrcExecutable>)addCustomHeaderWithName:(NSString *)name value:(NSString *)value {
    
    NSDictionary *dicc = [[NSDictionary alloc] initWithObjectsAndKeys:value, name, nil];
    [_customHeaders addEntriesFromDictionary:dicc];
    
    return self;
}

- (id<MSOrcExecutable>)addCustomParametersWithName:(NSString *)name value:(id)value {
    
    NSDictionary *dicc = [[NSDictionary alloc] initWithObjectsAndKeys:value, name, nil];
    
    [self.customParameters addEntriesFromDictionary:dicc];
    
    return self;
}

@end