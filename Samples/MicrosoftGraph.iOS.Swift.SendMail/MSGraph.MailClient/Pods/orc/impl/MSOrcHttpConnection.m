/*******************************************************************************
 * Copyright (c) Microsoft Open Technologies, Inc.
 * All Rights Reserved
 * Licensed under the Apache License, Version 2.0.
 * See License.txt in the project root for license information.
 ******************************************************************************/

#import "MSOrcHttpConnection.h"
#import "MSOrcRequestImpl.h"
#import "MSOrcResponseImpl.h"
#import "MSOrcLoggerImpl.h"

#import "api/MSOrcRequest.h"
#import "api/MSOrcResponse.h"
#import "api/MSOrcURL.h"
#import "core/MSOrcError.h"

@interface MSOrcHttpConnection() <NSURLSessionDataDelegate>

@property (strong, atomic, readwrite) id<MSOrcRequest> request;
@property (strong, atomic, readwrite) NSMutableURLRequest *mutableRequest;
@property (strong, atomic, readonly) NSMutableData *mutableMutalbeData;
@property (copy, nonatomic, readonly) MSOrcLoggerImpl *logger;

@end

@implementation MSOrcHttpConnection

NSString *OPT_STREAM_UPLOAD = @"MUST_STREAM_UPLOAD_CONTENT";
NSString *OPT_STREAM_DOWNLOAD = @"MUST_STREAM_DOWNLOAD_CONTENT";

- (instancetype)init {
    
    if (self = [super init]) {
        
        _logger = [[MSOrcLoggerImpl alloc] init];
    }
    
    return self;
}

- (void)executeRequest:(id<MSOrcRequest>)request
                            callback:(void (^)(id<MSOrcResponse> response, MSOrcError *error))callback {
    
    self.request = request;
    
    [self initMutableRequest:request];
    
    if (request.options.count == 0) {
        
        return [[self executeWithCallBack:callback] resume];
    }
    
    NSString* isStreamedUpload   = [[[request options] valueForKey:OPT_STREAM_UPLOAD] objectAtIndex:0];
    NSString* isStreamedDownload = [[[request options] valueForKey:OPT_STREAM_DOWNLOAD] objectAtIndex:0];
    
    if (![isStreamedUpload isKindOfClass:[NSNull class]] && [isStreamedUpload isEqualToString:@"true"]) {
        
        return [[self executeWithDelegate:callback] resume];
    } else if (![isStreamedDownload isKindOfClass:[NSNull class]] && [isStreamedDownload isEqualToString:@"true"]) {
        
        return [[self downloadStream:callback] resume];
    }
    
    [self.logger logMessage:@"The options selected are not valid for the request." withLevel:LOG_LEVEL_ERROR];
    
    return;
}

- (NSURLSessionTask *)executeWithCallBack:(void (^)(id<MSOrcResponse> response, MSOrcError *error))callback {
    
    NSURLSession *session = [NSURLSession sharedSession];
    
    NSURLSessionDataTask *task = [session dataTaskWithRequest:self.mutableRequest
                                            completionHandler:^(NSData *data, NSURLResponse *response, NSError *error) {
                                                
                                                [self handleResponseData:data response:response error:error callback:callback];
                                            }];
    
    return task;
}

- (NSURLSessionTask *)downloadStream:(void (^)(id<MSOrcResponse> response, MSOrcError *error))callback {
    
    NSURLSession *session = [NSURLSession sharedSession];
    
    NSURLSessionDataTask *task = [session dataTaskWithRequest:self.mutableRequest
                                            completionHandler:^(NSData *data, NSURLResponse *response, NSError *error) {
                                                
                                                [self handleResponseData:data response:response error:error callback:callback];
                                            }];
    
    return task;
}

- (NSURLSessionTask *)executeWithDelegate:(void (^)(id<MSOrcResponse> response, MSOrcError *error))callback {
    
    [self.mutableRequest addValue:[NSString stringWithFormat:@"%ld",(long)self.request.size]
               forHTTPHeaderField:@"Content-Length"];
    
    NSURLSessionConfiguration *conf = [NSURLSessionConfiguration defaultSessionConfiguration];
    
    NSURLSession *session = [NSURLSession sessionWithConfiguration:conf
                                                          delegate:self
                                                     delegateQueue:[NSOperationQueue currentQueue]];
    
    __block NSURLSessionUploadTask *task = [session uploadTaskWithStreamedRequest:self.mutableRequest];
    
    dispatch_queue_t tasks = dispatch_queue_create("tasks", NULL);
    
    dispatch_async(tasks, ^{
        
        dispatch_semaphore_t sem = dispatch_semaphore_create(0);
        dispatch_semaphore_signal(sem);
        
        while ([task state] != NSURLSessionTaskStateCompleted) {
            
            dispatch_semaphore_wait(sem, 0.1);
        }
        
        [self handleResponseData:nil response:task.response error:task.error callback:callback];
    });
    
    return task;
}

- (id<MSOrcRequest>)createRequest {
    
    return [[MSOrcRequestImpl alloc] init];
}

- (void)initMutableRequest:(id<MSOrcRequest>)theRequest{
    
    MSOrcRequestImpl *reqImpl = (MSOrcRequestImpl *)theRequest;
    
    self.mutableRequest = [NSMutableURLRequest requestWithURL:[NSURL URLWithString:[reqImpl.url toString]]];
    self.mutableRequest.HTTPMethod = [reqImpl verbToString];
    
    if (reqImpl.streamContent != nil) {
        
        self.mutableRequest.HTTPBodyStream = reqImpl.streamContent;
    }
    else {
        
        self.mutableRequest.HTTPBody = reqImpl.content;
    }
    
    for (NSString *key in reqImpl.headers.allKeys) {
        
        NSString *value = [reqImpl.headers valueForKey:key];
        
        [self.mutableRequest addValue:value forHTTPHeaderField:key];
        
        [self.logger logMessage:[NSString stringWithFormat:@"HEADERS: %@ : %@", key, value]
                      withLevel:LOG_LEVEL_INFO];
    }
    
    [self.logger logMessage:[NSString stringWithFormat:@"VERB : %@", self.mutableRequest.HTTPMethod]
                  withLevel:LOG_LEVEL_INFO];
    [self.logger logMessage:[NSString stringWithFormat:@"URL : %@", self.mutableRequest.URL.absoluteString]
                  withLevel:LOG_LEVEL_INFO];
}

- (void)handleResponseDataWithStream:(NSInputStream *)stream response:(NSURLResponse *)response error:(NSError *)error
                  callback:(void (^)(id<MSOrcResponse> response, MSOrcError *error))callback {
    
    long statusCode = [(NSHTTPURLResponse *)response statusCode];
    
    if (statusCode < 200 || statusCode > 299) {
        
        if (error == nil) {
            
            error = [[NSError alloc] initWithDomain:@"Error in the Request"
                                               code:statusCode userInfo:nil];
        }
        
        MSOrcResponseImpl *res = [[MSOrcResponseImpl alloc] initWithStream:stream response:response];
        
        callback(res, [[MSOrcError alloc] initWithResponse:res andError:error]);
    }
    else{
        
        MSOrcResponseImpl *responseImpl = [[MSOrcResponseImpl alloc] initWithStream:stream response:response];
        callback(responseImpl, nil);
    }
}

- (void)handleResponseData:(NSData *)data response:(NSURLResponse *)response error:(NSError *)error
                  callback:(void (^)(id<MSOrcResponse> response, MSOrcError *error))callback {
    
    long statusCode = [(NSHTTPURLResponse *)response statusCode];
    
    if (statusCode < 200 || statusCode > 299) {
        
        if (error == nil) {
            
            NSArray *msg = [NSJSONSerialization JSONObjectWithData:data
                                                           options: NSJSONReadingMutableContainers error:nil];
            
            error = [[NSError alloc] initWithDomain:@"Error in the Request"
                                               code:statusCode userInfo:(NSDictionary*)msg];
        }
        
        MSOrcResponseImpl *res = [[MSOrcResponseImpl alloc] initWithData:data response:response];
        
        callback(res, [[MSOrcError alloc] initWithResponse:res andError:error]);
    }
    else{
        
        MSOrcResponseImpl *responseImpl = [[MSOrcResponseImpl alloc] initWithData:data response:response];
        callback(responseImpl, nil);
    }
}

#pragma mark - NSURLSessionDataDelegate

- (void)URLSession:(NSURLSession *)session task:(NSURLSessionTask *)task
   didSendBodyData:(int64_t)bytesSent
    totalBytesSent:(int64_t)totalBytesSent totalBytesExpectedToSend:(int64_t)totalBytesExpectedToSend {
    
    [self.logger logMessage:[NSString stringWithFormat:@"%lld %lld %lld", bytesSent, totalBytesSent, totalBytesExpectedToSend]
                  withLevel:LOG_LEVEL_INFO];
}

- (void)URLSession:(NSURLSession *)session task:(NSURLSessionTask *)task
 needNewBodyStream:(void (^)(NSInputStream *bodyStream))completionHandler {
    
    completionHandler(self.request.streamContent);
}

- (void)URLSession:(NSURLSession *)session task:(NSURLSessionTask *)task didCompleteWithError:(NSError *)error {
    
    [self.logger logMessage:[NSString stringWithFormat:@"%s: error = %@; data = %@", __PRETTY_FUNCTION__, error,
                             [[NSString alloc] initWithData:self.mutableMutalbeData encoding:NSUTF8StringEncoding]]
                  withLevel:LOG_LEVEL_INFO];
}

- (void)URLSession:(NSURLSession *)session dataTask:(NSURLSessionDataTask *)dataTask didReceiveResponse:(NSURLResponse *)response completionHandler:(void (^)(NSURLSessionResponseDisposition disposition))completionHandler {
    
    [self.request setContent:[NSMutableData data]];
    
    completionHandler(NSURLSessionResponseAllow);
}

- (void)URLSession:(NSURLSession *)session dataTask:(NSURLSessionDataTask *)dataTask didReceiveData:(NSData *)data {
    
    [self.mutableMutalbeData appendData:data];
}

@end