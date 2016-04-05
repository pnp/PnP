//
//  LiveConnectSession.m
//  Live SDK for iOS
//
//  Copyright 2015 Microsoft Corporation
//
//  Permission is hereby granted, free of charge, to any person obtaining a copy
//  of this software and associated documentation files (the "Software"), to deal
//  in the Software without restriction, including without limitation the rights
//  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//  copies of the Software, and to permit persons to whom the Software is
//  furnished to do so, subject to the following conditions:
//  
//  The above copyright notice and this permission notice shall be included in
//  all copies or substantial portions of the Software.
//  
//  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
//  THE SOFTWARE.
//


#import "LiveConnectSession.h"

@implementation LiveConnectSession

@synthesize accessToken = _accessToken, 
    authenticationToken = _authenticationToken,
           refreshToken = _refreshToken, 
                 scopes = _scopes, 
                expires = _expires;

- (id) initWithAccessToken:(NSString *)accessToken
       authenticationToken:(NSString *)authenticationToken
              refreshToken:(NSString *)refreshToken
                    scopes:(NSArray *)scopes
                   expires:(NSDate *)expires
{
    self = [super init];
    if (self) 
    {
        _accessToken = [accessToken retain];
        _authenticationToken = [authenticationToken retain];
        _refreshToken = [refreshToken retain];
        _scopes = [scopes retain];
        _expires = [expires retain];
    }
    
    return self;
}

- (void)dealloc 
{
    [_accessToken release];
    [_authenticationToken release];
    [_refreshToken release];
    [_scopes release];
    [_expires release];
    
    [super dealloc];
}

@end
