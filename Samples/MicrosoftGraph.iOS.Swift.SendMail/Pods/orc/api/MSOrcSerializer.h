/*******************************************************************************
 * Copyright (c) Microsoft Open Technologies, Inc.
 * All Rights Reserved
 * Licensed under the Apache License, Version 2.0.
 * See License.txt in the project root for license information.
 ******************************************************************************/

#import <Foundation/Foundation.h>

/**
 * A serializer to convert between a  NSArray/NSDictionary and a string representation
 */

@protocol MSOrcSerializer <NSObject>

@required

+ (NSString *) serialize:(id)objectToSerialize;
+ (id) deserializeString: (NSString *) string;

@end
