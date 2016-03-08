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
#import "ADHelpers.h"
#import "NSString+ADHelperMethods.h"

@implementation ADHelpers

+(BOOL) isADFSInstance:(NSString*) endpoint
{
    if([NSString adIsStringNilOrBlank:endpoint]){
        return NO;
    }
    
    return[ADHelpers isADFSInstanceURL: [NSURL URLWithString:endpoint.lowercaseString]];
}


+(BOOL) isADFSInstanceURL:(NSURL*) endpointUrl
{
    
    NSArray* paths = endpointUrl.pathComponents;
    if (paths.count >= 2)
    {
        NSString* tenant = [paths objectAtIndex:1];
        return [@"adfs" isEqualToString:tenant];
    }
    return false;
}


+(NSString*) getEndpointName:(NSString*) fullEndpoint
{
    if([NSString adIsStringNilOrBlank:fullEndpoint])
    {
        return nil;
    }
    
    NSURL* endpointUrl = [NSURL URLWithString:fullEndpoint.lowercaseString];
    NSArray* paths = endpointUrl.pathComponents;
    if (paths.count >= 2)
    {
        return[paths objectAtIndex:[paths count]-1];
    }
    return nil;
}

@end