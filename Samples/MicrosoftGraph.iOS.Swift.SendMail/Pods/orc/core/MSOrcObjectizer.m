/*******************************************************************************
 * Copyright (c) Microsoft Open Technologies, Inc.
 * All Rights Reserved
 * Licensed under the Apache License, Version 2.0.
 * See License.txt in the project root for license information.
 ******************************************************************************/


#import <MSOrcObjectizer.h>
#import "impl/MSOrcJSONSerializer.h"
#import <objc/runtime.h>

@implementation MSOrcObjectizer

static Class<MSOrcSerializer> currentSerializer = nil;

+ (Class<MSOrcSerializer>) getCurrentSerializer
{
    //set json serializer
    if(currentSerializer == nil) currentSerializer = [MSOrcJSONSerializer class];
    return currentSerializer;
}


+ (id<MSOrcInteroperableWithDictionary>) objectize:(id)dictionaryOrArray toType: (Class) type{
    
    if(![type conformsToProtocol:@protocol(MSOrcInteroperableWithDictionary)])
    {
        [NSException raise:@"Cannot call objectize for type not conforming to MSOrcInteroperableWithDictionary" format:@""];
        return nil;
    }
    
    if([dictionaryOrArray isKindOfClass:[NSDictionary class]])
    {
        return [[type alloc] initWithDictionary: dictionaryOrArray];
    }
        
    return nil;
}

+ (id<MSOrcInteroperableWithDictionary>) objectizeFromString: (NSString *) string toType: (Class) type {
    return [self objectize: [[self getCurrentSerializer] deserializeString: string] toType: type];
}

+ (id) deobjectize: (id) obj {
    Class type=[obj class];
    
    if(![type conformsToProtocol:@protocol(MSOrcInteroperableWithDictionary)]) return obj;
    
    return [obj toDictionary];
}

+ (NSString *) deobjectizeToString: (id) obj {
    
    return [[self getCurrentSerializer] serialize:[self deobjectize: obj]];
}

+ (NSString *) stringFromBool: (bool) value {
    return value?@"\"true\"":@"\"false\"";
}

+ (bool) boolFromString: (NSString *) value {
    return [value isEqualToString:@"true"];
}

+ (NSString *) stringFromInt: (int) value {
    return [NSString stringWithFormat:@"%d",value];
}

+ (int) intFromString: (NSString *) value {
    return [value intValue];
}

+ (NSString *) stringFromDouble: (double) value {
    return [NSString stringWithFormat:@"%f",value];
}

+ (double) doubleFromString: (NSString *) value {
    return [value doubleValue];
}

+ (NSString *) stringFromFloat: (float) value {
    return [NSString stringWithFormat:@"%f",(double)value];
}

+ (float) floatFromString: (NSString *) value {
    return [value floatValue];
}

+ (NSString *) stringFromLongLong: (long long) value {
    return [NSString stringWithFormat:@"%lld",value];
}

+ (long long) longLongFromString: (NSString *) value {
    return [value longLongValue];
}

+ (NSString *) stringFromDate: (NSDate *) date {

    NSDateFormatter *dateFormatter = [[NSDateFormatter alloc] init];

    [dateFormatter setDateFormat:@"yyyy-MM-dd'T'HH:mm:ssz"];
    
    return [[[dateFormatter stringFromDate:date] substringToIndex:19] stringByAppendingString:@"Z"];
}

+ (NSDate *) dateFromString: (NSString *) string {
    if(!string || [string isKindOfClass:[NSNull class]]) return nil;
    
    NSDateFormatter *dateFormatter = [[NSDateFormatter alloc] init];
    [dateFormatter setDateFormat:@"yyyy-MM-dd'T'HH:mm:ssz"];
    
    return [dateFormatter dateFromString:string];
}

+ (NSString *) stringFromData: (NSData *) data {
    return [data base64EncodedStringWithOptions:0];
}

+ (NSData *) dataFromString: (NSString *) string {
    if(!string || [string isKindOfClass:[NSNull class]]) return nil;
    
    return [[NSData alloc] initWithBase64EncodedString:string options:0];
}

+ (NSString *) stringFromTimeInterval: (NSTimeInterval) interval {
    return nil;
}

+ (NSTimeInterval) timeIntervalFromString: (NSString *) string {
    return 0.0;
}



@end