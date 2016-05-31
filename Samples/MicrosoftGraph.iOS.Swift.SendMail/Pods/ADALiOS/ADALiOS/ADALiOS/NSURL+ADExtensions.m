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

#import "NSURL+ADExtensions.h"
#import "NSDictionary+ADExtensions.h"
#import "NSString+ADHelperMethods.h"

const unichar fragmentSeparator = '#';
const unichar queryStringSeparator = '?';

@implementation NSURL ( ADAL )

- (NSString *) adAuthority
{
    NSInteger port = self.port.integerValue;
    
    if ( port == 0 )
    {
        if ( [self.scheme isEqualToString:@"http"] )
        {
            port = 80;
        }
        else if ( [self.scheme isEqualToString:@"https"] )
        {
            port = 443;
        }
    }
    
    return [NSString stringWithFormat:@"%@:%ld", self.host, (long)port];
}

//Used for getting the parameters from either the fragment or the query
//string. This internal helper method attempts to extract the parameters
//for the substring of the URL succeeding the separator. Also, if the
//separator is present more than once, the method returns null.
//Unlike standard NSURL implementation, the method handles well URNs.
-(NSDictionary*) getParametersAfter: (unichar) startSeparator
                              until: (unichar) endSeparator
{
    NSArray* parts = [[self absoluteString] componentsSeparatedByCharactersInSet:[NSCharacterSet characterSetWithRange:(NSRange){startSeparator, 1}]];
    if (parts.count != 2)
    {
        return nil;
    }
    NSString* last = [parts lastObject];
    if (endSeparator)
    {
        long index = [last adFindCharacter:endSeparator start:0];
        last = [last substringWithRange:(NSRange){0, index}];
    }
    if ([NSString adIsStringNilOrBlank:last])
    {
        return nil;
    }
    return [NSDictionary adURLFormDecode:last];
}

// Decodes parameters contained in a URL fragment
- (NSDictionary *) adFragmentParameters
{
    return [self getParametersAfter:fragmentSeparator until:0];
}

// Decodes parameters contains in a URL query
- (NSDictionary *) adQueryParameters
{
    return [self getParametersAfter:queryStringSeparator until:fragmentSeparator];
}

@end
