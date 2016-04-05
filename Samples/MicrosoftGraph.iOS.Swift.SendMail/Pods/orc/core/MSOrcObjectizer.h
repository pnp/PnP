/*******************************************************************************
 * Copyright (c) Microsoft Open Technologies, Inc.
 * All Rights Reserved
 * Licensed under the Apache License, Version 2.0.
 * See License.txt in the project root for license information.
 ******************************************************************************/


#ifndef MSORCOBJECTIZER_H
#define MSORCOBJECTIZER_H

#import <Foundation/Foundation.h>
#import "api/MSOrcInteroperableWithDictionary.h"
#import "api/MSOrcSerializer.h"

/**
 * Converts an MSOrc object or collection of MSOrc objects to/from a dictionary/array representation.
 * It can also directly convert to/from a string if a MSOrcSerializer is used
 */

@interface MSOrcObjectizer : NSObject

+ (Class<MSOrcSerializer>) getCurrentSerializer;

+ (id<MSOrcInteroperableWithDictionary>) objectize:(id)dictionaryOrArray toType: (Class) type;
+ (id<MSOrcInteroperableWithDictionary>) objectizeFromString: (NSString *) string toType: (Class) type;

+ (id) deobjectize: (id) obj;
+ (NSString *) deobjectizeToString: (id) obj;

+ (NSString *) stringFromBool: (bool) value;
+ (bool) boolFromString: (NSString *) value;

+ (NSString *) stringFromInt: (int) value;
+ (int) intFromString: (NSString *) value;

+ (NSString *) stringFromDouble: (double) value;
+ (double) doubleFromString: (NSString *) value;

+ (NSString *) stringFromFloat: (float) value;
+ (float) floatFromString: (NSString *) value;

+ (NSString *) stringFromLongLong: (long long) date;
+ (long long) longLongFromString: (NSString *) string;

+ (NSString *) stringFromDate: (NSDate *) date;
+ (NSDate *) dateFromString: (NSString *) string;

+ (NSString *) stringFromData: (NSData *) data;
+ (NSData *) dataFromString: (NSString *) string;

+ (NSString *) stringFromTimeInterval: (NSTimeInterval) interval;
+ (NSTimeInterval) timeIntervalFromString: (NSString *) string;


@end 

#endif
