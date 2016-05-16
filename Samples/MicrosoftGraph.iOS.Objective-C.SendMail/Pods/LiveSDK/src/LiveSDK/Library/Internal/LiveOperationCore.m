//
//  LiveOperationCore.m
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
#import "LiveAuthHelper.h"
#import "LiveConnectClientCore.h"
#import "LiveConnectionHelper.h"
#import "LiveConstants.h"
#import "LiveOperationCore.h"
#import "StringHelper.h"
#import "UrlHelper.h"

@class LiveConnectClientCore;

@implementation LiveOperationCore

@synthesize method = _method,
              path = _path, 
       requestBody = _requestBody,
          delegate = _delegate,
         userState = _userState,
        liveClient = _liveClient, 
       inputStream = _inputStream,
      streamReader,
           request,
   publicOperation,
         rawResult, 
            result,
        connection,
      httpResponse,
      responseData,
         completed,
         httpError;

- (id) initWithMethod:(NSString *)method
                 path:(NSString *)path
          requestBody:(NSData *)requestBody
             delegate:(id)delegate
            userState:(id)userState
           liveClient:(LiveConnectClientCore *)liveClient
{
    self = [super init];
    if (self) 
    {
        _method = [method copy];
        _path = [path copy];
        _requestBody = [requestBody retain];
        _delegate = delegate;
        _userState = [userState retain]; 
        _liveClient = [liveClient retain];
        httpError = nil;
        completed = NO;
    }
    
    return self;
}

- (id) initWithMethod:(NSString *)method
                 path:(NSString *)path
          inputStream:(NSInputStream *)inputStream
             delegate:(id)delegate
            userState:(id)userState
           liveClient:(LiveConnectClientCore *)liveClient
{
    self = [super init];
    if (self) 
    {
        _method = [method copy];
        _path = [path copy];
        _inputStream = [inputStream retain];
        _delegate = delegate;
        _userState = [userState retain]; 
        _liveClient = [liveClient retain];
        completed = NO;
    }
    
    return self;
}

- (void)dealloc 
{
    [_method release];
    [_path release];
    [_requestBody release];
    [_userState release];
    [_liveClient release];
    [_inputStream release];
    [streamReader release];
    [request release];
    [rawResult release];
    [result release];
    [connection release];
    [responseData release];
    [publicOperation release];
    [httpResponse release];
    [httpError release];
    
    [super dealloc];
}

- (void) execute 
{
    [_liveClient refreshSessionWithDelegate:self
                                  userState:nil];
}

- (void) authCompleted: (LiveConnectSessionStatus) status
               session: (LiveConnectSession *) session
             userState: (id) userState
{
    [self sendRequest];
}

- (void) cancel
{
    NSError *error = [LiveAuthHelper createAuthError:LIVE_ERROR_CODE_API_CANCELED
                                            errorStr:LIVE_ERROR_CODE_S_REQUEST_CANCELED
                                         description:LIVE_ERROR_DESC_API_CANCELED
                                          innerError:nil];
    [self operationFailed:error];
    
    [self dismissCurrentRequest];
}

- (void) dismissCurrentRequest
{
    [connection cancel]; 
}

- (NSURL *)requestUrl
{
    return [LiveApiHelper buildAPIUrl:self.path
                               params:[NSMutableDictionary dictionaryWithObjectsAndKeys:
                                            @"true", LIVE_API_PARAM_SUPPRESS_RESPONSE_CODES,
                                            @"true", LIVE_API_PARAM_SUPPRESS_REDIRECTS,
                                            nil ]];
}

- (void) setRequestContentType
{
    [request setValue:LIVE_API_HEADER_CONTENTTYPE_JSON 
   forHTTPHeaderField:LIVE_API_HEADER_CONTENTTYPE];
}

- (void) readInputStream
{
    self.streamReader = [[[StreamReader alloc]initWithStream:_inputStream
                                                    delegate:self]
                         autorelease ];
    [self.streamReader start];
}

- (void)streamReadingCompleted:(NSData *)data
{
    self.requestBody = data;
    [self sendRequest];
}

- (void)streamReadingFailed:(NSError *)error
{
    [self operationFailed:error];
}

- (void) sendRequest
{
    if (completed) 
    {
        return;
    }
    
    if (_inputStream != nil && _requestBody == nil)
    {
        // We have a stream to read.
        [self readInputStream];
        return;
    }
    
    self.request = [NSMutableURLRequest requestWithURL:self.requestUrl
                                           cachePolicy:NSURLRequestReloadIgnoringLocalCacheData
                                       timeoutInterval:HTTP_REQUEST_TIMEOUT_INTERVAL];
    
    [request setHTTPMethod:self.method];
    
    
    if ([LiveAuthHelper isSessionValid:_liveClient.session])
    {
        [request setValue:[NSString stringWithFormat:@"bearer %@", self.liveClient.session.accessToken ]
       forHTTPHeaderField:LIVE_API_HEADER_AUTHORIZATION];
    }
    
    // Set this header for SDK usage tracking purpose.
    [request setValue: [LiveApiHelper getXHTTPLiveLibraryHeaderValue]
   forHTTPHeaderField:LIVE_API_HEADER_X_HTTP_LIVE_LIBRARY];
    
    if (self.requestBody != nil)
    {
        [self setRequestContentType];
        [request setHTTPBody:self.requestBody];
    }
    
    self.connection = [LiveConnectionHelper createConnectionWithRequest:request delegate:self];    
}

- (NSMutableData *)responseData
{
    if (responseData == nil) 
    {
        responseData = [[NSMutableData alloc] init];
    }

    return responseData;
}

#pragma mark methods on response handling

- (void) operationFailed:(NSError *)error
{
    if (completed) {
        return;
    }
    
    completed = YES;
    if ([_delegate respondsToSelector:@selector(liveOperationFailed:operation:)]) 
    {
        [_delegate liveOperationFailed:error operation:publicOperation];
    }

    // LiveOperation was returned in the interface return. However, the app may not retain the object
    // In order to keep it alive, we keep LiveOperationCore and LiveOperation in circular reference.
    // After the event raised, we set this property to nil to break the circle, so that they are recycled.
    self.publicOperation = nil;
}

- (void) operationCompleted
{
    if (completed) 
    {
        return;
    }
    
    NSString *textResponse;
    NSDictionary *response;
    NSError *error = nil;
    
    [LiveApiHelper parseApiResponse:responseData 
                       textResponse:&textResponse 
                           response:&response 
                              error:&error];
    
    if (error == nil) 
    {
        error = self.httpError;
    }
    
    if (error == nil)
    {
        self.rawResult = textResponse  ;
        self.result = response;
        
        if ([_delegate respondsToSelector:@selector(liveOperationSucceeded:)])
        {
            [_delegate liveOperationSucceeded:self.publicOperation];
        }

        // LiveOperation was returned in the interface return. However, the app may not retain the object
        // In order to keep it alive, we keep LiveOperationCore and LiveOperation in circular reference.
        // After the event raised, we set this property to nil to break the circle, so that they are recycled.
        self.publicOperation = nil;
    }
    else
    {
        [self operationFailed:error];
    }
    
    completed = YES;
    self.responseData = nil;
}

- (void) operationReceivedData:(NSData *)data
{
    [self.responseData appendData:data];
}

#pragma mark NSURLConnection Delegate

- (void)connection:(NSURLConnection *)connection 
didReceiveResponse:(NSURLResponse *)response 
{   
    self.httpResponse = (NSHTTPURLResponse *)response;
    if ((httpResponse.statusCode / 100) != 2) 
    {
        NSString *message = [NSString stringWithFormat:@"HTTP error %zd", (ssize_t)httpResponse.statusCode];
        self.httpError = [LiveApiHelper createAPIError:LIVE_ERROR_CODE_S_REQUEST_FAILED
                                               message:message
                                            innerError:nil];
    }
}

- (void)connection:(NSURLConnection *)connection 
    didReceiveData:(NSData *)data 
{
    [self operationReceivedData:data];
}

- (NSCachedURLResponse *)connection:(NSURLConnection *)connection
                  willCacheResponse:(NSCachedURLResponse*)cachedResponse 
{
    return nil;
}

- (void)connectionDidFinishLoading:(NSURLConnection *)connection 
{
    [self operationCompleted];
    
    self.connection = nil;
}

- (void)connection:(NSURLConnection *)connection 
  didFailWithError:(NSError *)error 
{
    self.connection = nil;
    
    [self operationFailed:error];
}

@end
