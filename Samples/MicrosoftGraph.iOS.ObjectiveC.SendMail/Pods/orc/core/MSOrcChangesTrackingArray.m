/*******************************************************************************
 * Copyright (c) Microsoft Open Technologies, Inc.
 * All Rights Reserved
 * Licensed under the Apache License, Version 2.0.
 * See License.txt in the project root for license information.
 ******************************************************************************/

#include "core/MSOrcChangesTrackingArray.h"
#include "api/MSOrcInteroperableWithDictionary.h"

@implementation MSOrcChangesTrackingArray{
    bool changed;
    NSMutableArray *backing;
}

- (instancetype) init {
    if(self=[super init])
    {
        changed=false;
        backing=[[NSMutableArray alloc]init];
    }
    return self;
}

- (NSUInteger) count {
    return [backing count];
}

- (id)objectAtIndex:(NSUInteger)index {
    return [backing objectAtIndex:index];
}

- (void)addObject:(id)anObject {
    changed=true;
    [backing addObject:anObject];
}

- (void)insertObject:(id)anObject atIndex:(NSUInteger)index {
    changed=true;
    [backing insertObject: anObject atIndex:index];
}

- (void)replaceObjectAtIndex:(NSUInteger)index withObject:(id)anObject {
    changed=true;
    [backing replaceObjectAtIndex:index withObject:anObject];
}

- (void)removeObjectAtIndex:(NSUInteger)index {
    changed=true;
    [backing removeObjectAtIndex: index];
}

- (void)removeLastObject {
    changed=true;
    [backing removeLastObject];
}

- (void)resetChangedFlag {
    changed=false;
}

- (bool)hasChanged {
    if(changed) return true;
    for(id obj in backing) {
        
        if([obj conformsToProtocol:@protocol(MSOrcInteroperableWithDictionary)]) {
            
            NSDictionary *updatedDic=[obj toUpdatedValuesDictionary];
            
            if(updatedDic!=nil && [updatedDic count]>0)
            {
                return true;
            }
            
        }
    }
    return false;
}

@end