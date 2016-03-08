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

#import "ADWebResponse.h"

@implementation ADWebResponse
{
    NSHTTPURLResponse *_response;
    NSData            *_body;
    NSString          *_bodyText;
}

@synthesize body = _body;

- (id)init
{
    return nil;
}

- (id)initWithResponse:(NSHTTPURLResponse *)response data:(NSData *)data
{
    if ( response == nil )
    {
        NSAssert( false, @"Invalid Parameters" );
        return nil;
    }
    
    if ( ( self = [super init] ) != nil )
    {
        _response = response;
        _body     = data;
        _bodyText = nil;
    }
    
    return self;
}

- (NSDictionary *)headers
{
    return _response.allHeaderFields;
}

- (NSInteger)statusCode
{
    return _response.statusCode;
}

@end
