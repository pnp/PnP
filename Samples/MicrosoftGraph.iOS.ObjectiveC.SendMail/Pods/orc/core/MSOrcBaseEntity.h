/*******************************************************************************
 * Copyright (c) Microsoft Open Technologies, Inc.
 * All Rights Reserved
 * Licensed under the Apache License, Version 2.0.
 * See License.txt in the project root for license information.
 ******************************************************************************/

#import <Foundation/Foundation.h>

@interface MSOrcBaseEntity : NSObject 

@property (retain, nonatomic) NSString *odataType;
@property (retain, nonatomic) NSMutableSet *updatedValues;

- (void)valueChangedFor: (NSString *) property;


@end
