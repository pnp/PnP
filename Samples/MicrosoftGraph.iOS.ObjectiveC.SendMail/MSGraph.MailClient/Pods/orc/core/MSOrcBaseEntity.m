/*******************************************************************************
 * Copyright (c) Microsoft Open Technologies, Inc.
 * All Rights Reserved
 * Licensed under the Apache License, Version 2.0.
 * See License.txt in the project root for license information.
 ******************************************************************************/

#import "MSOrcBaseEntity.h"

@interface MSOrcBaseEntity()


@end

@implementation MSOrcBaseEntity

@synthesize updatedValues = _updatedValues;

- (instancetype)init {
    
    if (self = [super init]) {
        
        _updatedValues = [[NSMutableSet alloc] init];
    }
    
    return self;
}

- (void) valueChangedFor: (NSString *) property{
    [self.updatedValues addObject:property];
}


@end
