//
//  LiveOperationCore.h
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


#import <Foundation/Foundation.h>
#import "LiveAuthDelegate.h"
#import "LiveOperationDelegate.h"
#import "StreamReader.h"

@class LiveConnectClientCore;
@class LiveOperation;

@interface LiveOperationCore : NSObject <StreamReaderDelegate, LiveAuthDelegate>

@property (nonatomic, readonly) NSString *path;
@property (nonatomic, readonly) NSString *method;
@property (nonatomic, retain) NSData *requestBody;
@property (nonatomic, readonly) id userState; 
@property (nonatomic, assign) id delegate;
@property (nonatomic, readonly) LiveConnectClientCore *liveClient;
@property (nonatomic, retain) NSInputStream *inputStream;
@property (nonatomic, readonly) NSURL *requestUrl;
@property (nonatomic, retain) StreamReader *streamReader;
@property (nonatomic, retain) NSMutableURLRequest *request; 

@property (nonatomic) BOOL completed;
@property (nonatomic, retain) NSString *rawResult;
@property (nonatomic, retain) NSDictionary *result;
@property (nonatomic, retain) id connection;
@property (nonatomic, retain) NSMutableData *responseData;
@property (nonatomic, retain) id publicOperation;
@property (nonatomic, retain) NSHTTPURLResponse *httpResponse;
@property (nonatomic, retain) NSError *httpError;

- (id) initWithMethod:(NSString *)method
                 path:(NSString *)path
          requestBody:(NSData *)requestBody
             delegate:(id)delegate
            userState:(id)userState
           liveClient:(LiveConnectClientCore *)liveClient;

- (id) initWithMethod:(NSString *)method
                 path:(NSString *)path
          inputStream:(NSInputStream *)inputStream
             delegate:(id)delegate
            userState:(id)userState
           liveClient:(LiveConnectClientCore *)liveClient;

- (void) execute;

- (void) cancel;

- (void) dismissCurrentRequest;

- (void) setRequestContentType;

- (void) sendRequest;

- (void) operationFailed:(NSError *)error;

- (void) operationCompleted;

- (void) operationReceivedData:(NSData *)data;

@end
