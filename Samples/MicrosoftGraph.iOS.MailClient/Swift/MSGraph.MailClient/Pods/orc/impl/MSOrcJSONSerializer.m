/*******************************************************************************
 * Copyright (c) Microsoft Open Technologies, Inc.
 * All Rights Reserved
 * Licensed under the Apache License, Version 2.0.
 * See License.txt in the project root for license information.
 ******************************************************************************/


#import "MSOrcJSONSerializer.h"

@implementation MSOrcJSONSerializer

+ (NSString *) serialize:(id)objectToSerialize
{
   return [[NSString alloc] initWithData:[NSJSONSerialization dataWithJSONObject:objectToSerialize options:0 error:nil] encoding:NSUTF8StringEncoding];
}

+ (id) deserializeString: (NSString *) string
{
    return [NSJSONSerialization JSONObjectWithData: [string dataUsingEncoding:NSUTF8StringEncoding] options:0 error:nil];
}

@end