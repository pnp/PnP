// Copyright © Microsoft Open Technologies, Inc.
//
// All Rights Reserved
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// THIS CODE IS PROVIDED *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS
// OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION
// ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A
// PARTICULAR PURPOSE, MERCHANTABILITY OR NON-INFRINGEMENT.
//
// See the Apache License, Version 2.0 for the specific language
// governing permissions and limitations under the License.

#import <Foundation/Foundation.h>

#import "NSDictionary+ADExtensions.h"
#import "NSString+ADHelperMethods.h"

@implementation NSDictionary ( ADAL )

// Decodes a www-form-urlencoded string into a dictionary of key/value pairs.
// Always returns a dictionary, even if the string is nil, empty or contains no pairs
+ (NSDictionary *)adURLFormDecode:(NSString *)string
{
    NSMutableDictionary *parameters = [[NSMutableDictionary alloc] initWithCapacity:6];
    
    if ( nil != string && string.length != 0 )
    {
        NSArray *pairs = [string componentsSeparatedByString:@"&"];
        
        for ( NSString *pair in pairs )
        {
            NSArray *elements = [pair componentsSeparatedByString:@"="];
            
            if ( elements != nil && elements.count == 2 )
            {
                NSString *key     = [[[elements objectAtIndex:0] adTrimmedString] adUrlFormDecode];
                NSString *value   = [[[elements objectAtIndex:1] adTrimmedString] adUrlFormDecode];
                if ( nil != key && key.length != 0 )
                    [parameters setObject:value forKey:key];
            }
        }
    }
    
    return parameters;
}

// Encodes a dictionary consisting of a set of name/values pairs that are strings to www-form-urlencoded
// Returns nil if the dictionary is empty, otherwise the encoded value
- (NSString *)adURLFormEncode
{
    __block NSString *parameters = nil;
    
    [self enumerateKeysAndObjectsUsingBlock: ^(id key, id value, BOOL *stop)
    {
        *stop = NO;
        
        if ( parameters == nil )
        {
            parameters = [NSString stringWithFormat:@"%@=%@",
                           [[((NSString *)key) adTrimmedString] adUrlFormEncode],
                           [[((NSString *)value) adTrimmedString] adUrlFormEncode]];
        }
        else
        {
            parameters = [NSString stringWithFormat:@"%@&%@=%@",
                          parameters,
                          [[((NSString *)key) adTrimmedString] adUrlFormEncode],
                          [[((NSString *)value) adTrimmedString] adUrlFormEncode]];
        }
    }];
    
    return parameters;
}

@end
