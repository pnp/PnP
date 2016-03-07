//
//  LiveDownloadOperationCore.m
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


#import "LiveApiHelper.h"
#import "LiveDownloadOperationCore.h"
#import "LiveDownloadOperationInternal.h"
#import "LiveOperationInternal.h"
#import "LiveOperationProgress.h"

@class LiveDownloadOperation;

@implementation LiveDownloadOperationCore

- (id) initWithPath:(NSString *)path
           delegate:(id <LiveDownloadOperationDelegate>)delegate
          userState:(id)userState
         liveClient:(LiveConnectClientCore *)liveClient
{
    self = [super initWithMethod:@"GET" 
                            path:path 
                     requestBody:nil 
                        delegate:delegate 
                       userState:userState 
                      liveClient:liveClient];
    if (self)
    {
        contentLength = 0;
    }
    
    return self;
}

#pragma mark override methods

- (NSURL *)requestUrl
{
    // We don't use suppress_redirects for download, since redirect maybe expected.
    return [LiveApiHelper buildAPIUrl:self.path
                               params:nil];
}

- (void) setRequestContentType
{
    // override the behaviour in LiveOperation.
}

 - (void) operationCompleted
{
    if (self.completed) 
    {
        return;
    }
    
    if (self.httpError) 
    {
        // If there is httpError, try read the error information from the server.
        NSString *textResponse;
        NSDictionary *response;
        NSError *error = nil;
        
        [LiveApiHelper parseApiResponse:self.responseData 
                           textResponse:&textResponse 
                               response:&response 
                                  error:&error];
        error = (error != nil)? error : self.httpError;
        [self operationFailed:error];
    }
    else 
    {
        if ([self.delegate respondsToSelector:@selector(liveOperationSucceeded:)]) 
        {
            [self.delegate liveOperationSucceeded:self.publicOperation];
        }
        
        // LiveOperation was returned in the interface return. However, the app may not retain the object
        // In order to keep it alive, we keep LiveOperationCore and LiveOperation in circular reference.
        // After the event raised, we set this property to nil to break the circle, so that they are recycled.
        self.publicOperation = nil;
        
        self.completed = YES;
    }
}

- (void) operationReceivedData:(NSData *)data
{
    [self.responseData appendData:data];
    
    if ([self.delegate respondsToSelector:@selector(liveDownloadOperationProgressed:data:operation:)])
    {
        if (contentLength == 0)
        {
            contentLength = [[self.httpResponse.allHeaderFields valueForKey:@"Content-Length"] intValue];
        }
        
        LiveOperationProgress *progress = [[[LiveOperationProgress alloc] 
                                            initWithBytesTransferred:self.responseData.length 
                                                          totalBytes:contentLength]
                                           autorelease];
        
        [self.delegate liveDownloadOperationProgressed:progress 
                                                  data:data 
                                             operation:self.publicOperation];
    }
}

@end
