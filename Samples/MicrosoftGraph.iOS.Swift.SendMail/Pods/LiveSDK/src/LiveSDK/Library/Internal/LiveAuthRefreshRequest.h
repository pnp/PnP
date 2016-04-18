//
//  LiveAuthRefreshRequest.h
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
#import "LiveConnectClientCore.h"

@class LiveConnectClientCore;

@interface LiveAuthRefreshRequest : NSObject
{
@private
    NSString *_clientId;
    NSArray *_scopes;
    NSString *_refreshToken;
    id<LiveAuthDelegate> _delegate;
    id _userState;
    LiveConnectClientCore *_client;
}

@property (nonatomic, retain) id tokenConnection;
@property (nonatomic, retain) NSMutableData *tokenResponseData;

- (id) initWithClientId:(NSString *)clientId
                  scope:(NSArray *)scopes
           refreshToken:(NSString *)refreshToken
               delegate:(id<LiveAuthDelegate>)delegate
              userState:(id)userState
             clientStub:(LiveConnectClientCore *)client;

- (void) execute;

- (void) cancel;

@end
