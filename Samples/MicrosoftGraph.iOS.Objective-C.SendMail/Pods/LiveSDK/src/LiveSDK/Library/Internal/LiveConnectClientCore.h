//
//  LiveConnectClientCore.h
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
#import "LiveAuthRequest.h"
#import "LiveAuthRefreshRequest.h"
#import "LiveAuthStorage.h"
#import "LiveConnectSession.h"
#import "LiveConstants.h"
#import "LiveDownloadOperationCore.h"
#import "LiveDownloadOperationDelegate.h"
#import "LiveOperationCore.h"
#import "LiveOperationDelegate.h"
#import "LiveUploadOperationDelegate.h"
#import "LiveUploadOverwriteOption.h"

@class LiveAuthRefreshRequest;

@interface LiveConnectClientCore : NSObject 
{
@private
    LiveAuthStorage *_storage;
}

@property (nonatomic, readonly) NSString *clientId;
@property (nonatomic, readonly) NSArray *scopes;

@property (nonatomic) LiveConnectSessionStatus status;
@property (nonatomic, retain) LiveConnectSession *session;

@property (nonatomic, retain) LiveAuthRequest *authRequest;
@property (nonatomic, retain) LiveAuthRefreshRequest *authRefreshRequest;
@property (nonatomic, readonly) BOOL hasPendingUIRequest;

- (id) initWithClientId:(NSString *)clientId
                 scopes:(NSArray *)scopes
               delegate:(id<LiveAuthDelegate>)delegate
              userState:(id)userState;

- (void) login:(UIViewController *)currentViewController
        scopes:(NSArray *)scopes
      delegate:(id<LiveAuthDelegate>)delegate
     userState:(id)userState;

- (void) logoutWithDelegate:(id<LiveAuthDelegate>)delegate
                  userState:(id)userState;

- (void) refreshSessionWithDelegate:(id<LiveAuthDelegate>)delegate
                          userState:(id)userState;

- (LiveOperation *) sendRequestWithMethod:(NSString *)method
                                     path:(NSString *)path
                                 jsonBody:(NSString *)jsonBody
                                 delegate:(id <LiveOperationDelegate>)delegate
                                userState:(id) userState;

- (LiveDownloadOperation *) downloadFromPath:(NSString *)path
                                    delegate:(id <LiveDownloadOperationDelegate>)delegate
                                   userState:(id)userState;

- (LiveOperation *) uploadToPath:(NSString *)path
                        fileName:(NSString *)fileName
                            data:(NSData *)data
                       overwrite:(LiveUploadOverwriteOption)overwrite
                        delegate:(id <LiveUploadOperationDelegate>)delegate
                       userState:(id)userState;

- (LiveOperation *) uploadToPath:(NSString *)path
                        fileName:(NSString *)fileName
                     inputStream:(NSInputStream *)inputStream
                       overwrite:(LiveUploadOverwriteOption)overwrite
                        delegate:(id <LiveUploadOperationDelegate>)delegate
                       userState:(id)userState;

- (void) sendAuthCompletedMessage:(NSArray *)eventArgs;
@end
