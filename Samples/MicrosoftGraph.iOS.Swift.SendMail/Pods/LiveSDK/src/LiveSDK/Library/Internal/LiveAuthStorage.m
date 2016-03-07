//
//  LiveAuthStorage.m
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


#import "LiveAuthStorage.h"
#import "LiveConstants.h"

@interface LiveAuthStorage()

- (void) save;

@end

@implementation LiveAuthStorage

@synthesize refreshToken = _refreshToken;

- (id) initWithClientId:(NSString *)clientId
{
    self = [super init];
    if (self) 
    {
        // Find the file path
        NSString *libDirectory = [NSSearchPathForDirectoriesInDomains(NSLibraryDirectory, NSUserDomainMask, YES) objectAtIndex:0];
        _filePath = [[libDirectory stringByAppendingPathComponent:@"LiveService_auth.plist"] retain];
        _clientId = clientId;
        
        // If file exist, load the file
        if ([[NSFileManager defaultManager] fileExistsAtPath:_filePath])
        {
            assert(clientId != nil);
            
            NSDictionary *dictionary = [NSDictionary dictionaryWithContentsOfFile:_filePath];
            if ([clientId isEqualToString:[dictionary valueForKey: LIVE_AUTH_CLIENTID]]) 
            {
                _refreshToken = [[dictionary valueForKey:LIVE_AUTH_REFRESH_TOKEN] retain];
            }
            else
            {
                // The storage has a different client_id, flush it.
                [self save];
            }
        }
        
    }
    
    return self; 
}

- (void) dealloc
{
    [_filePath release];
    [_clientId release];
    [_refreshToken release];
    
    [super dealloc];
}

- (void) save
{
    NSMutableDictionary *data = [[NSMutableDictionary alloc] init];
    [data setValue:_clientId forKey:LIVE_AUTH_CLIENTID];
    [data setValue:_refreshToken forKey:LIVE_AUTH_REFRESH_TOKEN];
    
    [data writeToFile:_filePath atomically:YES];
    [data release];
}

- (void) setRefreshToken:(NSString *)refreshToken
{
    [_refreshToken release];    
    _refreshToken = [refreshToken retain];
    
    [self save];
}

@end
