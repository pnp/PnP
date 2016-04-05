/*******************************************************************************
 * Copyright (c) Microsoft Open Technologies, Inc.
 * All Rights Reserved
 * Licensed under the Apache License, Version 2.0.
 * See License.txt in the project root for license information.
 ******************************************************************************/

#import "MSOCalendarSerializer.h"

@implementation MSOCalendarSerializer

+ (NSDate *)deserialize:(NSString *)value {

    NSDateFormatter *dateFormatter = [[NSDateFormatter alloc] init];
    
    [dateFormatter setDateFormat:@"yyyy-MM-dd'T'HH:mm:ss"];

    return [dateFormatter dateFromString:[value substringToIndex:19]];
}

+ (NSString *)serialize:(NSDate *)date {
    
    NSDateFormatter *dateFormatter = [[NSDateFormatter alloc] init];
    
    [dateFormatter setDateFormat:@"yyyy-MM-dd'T'HH:mm:ssz"];

    return [[[dateFormatter stringFromDate:date] substringToIndex:19] stringByAppendingString:@"Z"];
}

@end