/*******************************************************************************
 * Copyright (c) Microsoft Open Technologies, Inc.
 * All Rights Reserved
 * Licensed under the Apache License, Version 2.0.
 * See License.txt in the project root for license information.
 ******************************************************************************/

#import <Foundation/Foundation.h>

@interface MSOCalendarSerializer : NSObject

+ (NSDate *)deserialize:(NSString *)value;
+ (NSString *)serialize:(NSDate *)date;
    
@end