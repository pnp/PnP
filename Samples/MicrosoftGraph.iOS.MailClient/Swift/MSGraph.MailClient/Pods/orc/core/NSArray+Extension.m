/*******************************************************************************
 * Copyright (c) Microsoft Open Technologies, Inc.
 * All Rights Reserved
 * Licensed under the Apache License, Version 2.0.
 * See License.txt in the project root for license information.
 ******************************************************************************/
 
#import "NSArray+Extension.h"
#import <objc/runtime.h>

@implementation NSArray(NSArrayExtension)

- (instancetype)initWithCollectionType:(NSString *)type {
    
    if (self = [self init]) {
        objc_setAssociatedObject(self, NSSelectorFromString(@"typeName"), type, OBJC_ASSOCIATION_RETAIN_NONATOMIC);

    }
    
    return self;
}

@end