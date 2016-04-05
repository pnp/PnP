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
#import "ADALiOS.h"
#import "ADAuthenticationDelegate.h"
#import "ADAuthenticationWebViewController.h"
#import "ADAuthenticationSettings.h"
#import "ADErrorCodes.h"
#import "ADLogger.h"
#import "ADPkeyAuthHelper.h"
#import "ADWorkPlaceJoinUtil.h"
#import "ADWorkPlaceJoin.h"
#import "ADWorkPlaceJoinConstants.h"
#import "NSDictionary+ADExtensions.h"
#import "ADAuthenticationSettings.h"
#import "ADNTLMHandler.h"

@implementation ADAuthenticationWebViewController
{
    __weak UIWebView *_webView;
    
    NSURL    *_startURL;
    NSString *_endURL;
    BOOL      _complete;
    float _timeout;
}

#pragma mark - Initialization
NSTimer *timer;

- (id)initWithWebView:(UIWebView *)webView startAtURL:(NSURL *)startURL endAtURL:(NSURL *)endURL
{
    if ( nil == startURL || nil == endURL )
        return nil;
    
    if ( nil == webView )
        return nil;
    
    if ( ( self = [super init] ) != nil )
    {
        _startURL  = [startURL copy];
        _endURL    = [endURL absoluteString];
        _complete  = NO;
        _timeout = [[ADAuthenticationSettings sharedInstance] requestTimeOut];
        _webView          = webView;
        _webView.delegate = self;
        [ADNTLMHandler setCancellationUrl:[_startURL absoluteString]];
    }
    
    return self;
}

- (void)dealloc
{
    // The ADAuthenticationWebViewController can be released before the
    // UIWebView that it is managing is released in the hosted case and
    // so it is important that to stop listening for events from the
    // UIWebView when we are released.
    _webView.delegate = nil;
    _webView          = nil;
}

#pragma mark - Public Methods

- (void)start
{
    NSMutableURLRequest* request = [[NSMutableURLRequest alloc] initWithURL:_startURL];
    [_webView loadRequest:request];
}

- (void)stop
{
}

- (void) handlePKeyAuthChallenge:(NSString *)challengeUrl
{
    
    AD_LOG_VERBOSE(@"Handling PKeyAuth Challenge", nil);

    NSArray * parts = [challengeUrl componentsSeparatedByString:@"?"];
    NSString *qp = [parts objectAtIndex:1];
    NSDictionary* queryParamsMap = [NSDictionary adURLFormDecode:qp];
    NSString* value = [queryParamsMap valueForKey:@"SubmitUrl"];
    
    NSArray * authorityParts = [value componentsSeparatedByString:@"?"];
    NSString *authority = [authorityParts objectAtIndex:0];
    
    NSMutableURLRequest* responseUrl = [[NSMutableURLRequest alloc] initWithURL: [NSURL URLWithString: value]];
    
    NSString* authHeader = [ADPkeyAuthHelper createDeviceAuthResponse:authority challengeData:queryParamsMap challengeType:AD_ISSUER];
    
    [responseUrl setValue:pKeyAuthHeaderVersion forHTTPHeaderField: pKeyAuthHeader];
    [responseUrl setValue:authHeader forHTTPHeaderField:@"Authorization"];
    [_webView loadRequest:responseUrl];
}


#pragma mark - UIWebViewDelegate Protocol

- (BOOL)webView:(UIWebView *)webView shouldStartLoadWithRequest:(NSURLRequest *)request navigationType:(UIWebViewNavigationType)navigationType
{
#pragma unused(webView)
#pragma unused(navigationType)
    
    if([ADNTLMHandler isChallengeCancelled]){
        _complete = YES;
        dispatch_async( dispatch_get_main_queue(), ^{[_delegate webAuthenticationDidCancel];});
        return NO;
    }
    
    NSString *requestURL = [request.URL absoluteString];
    if ([[[request.URL scheme] lowercaseString] isEqualToString:@"browser"]) {
        _complete = YES;
        dispatch_async( dispatch_get_main_queue(), ^{[_delegate webAuthenticationDidCancel];});
        
        requestURL = [requestURL stringByReplacingOccurrencesOfString:@"browser://" withString:@"https://"];
        [[UIApplication sharedApplication] openURL:[[NSURL alloc] initWithString:requestURL]];
        
        return NO;
    }
    
    // check for pkeyauth challenge.
    if ([requestURL hasPrefix: pKeyAuthUrn] )
    {
        [self handlePKeyAuthChallenge: requestURL];
        return NO;
    }
    
    // Stop at the end URL.
    if ( [[requestURL lowercaseString] hasPrefix:[_endURL lowercaseString]] )
    {
        // iOS generates a 102, Frame load interrupted error from stopLoading, so we set a flag
        // here to note that it was this code that halted the frame load in order that we can ignore
        // the error when we are notified later.
        _complete = YES;
        
        // Schedule the finish event; we do this so that the web view gets a chance to stop
        // This event is explicitly scheduled on the main thread as it is UI related.
        NSAssert( nil != _delegate, @"Delegate object was lost" );
        
        dispatch_async( dispatch_get_main_queue(), ^{ [_delegate webAuthenticationDidCompleteWithURL:request.URL]; } );
        
        // Tell the web view that this URL should not be loaded.
        return NO;
    }
    
    return YES;
}

- (void)webViewDidStartLoad:(UIWebView *)webView
{
    if (timer != nil){
        [timer invalidate];
    }
#pragma unused(webView)
    timer = [NSTimer scheduledTimerWithTimeInterval:_timeout target:self selector:@selector(failWithTimeout) userInfo:nil repeats:NO];
}

- (void)webViewDidFinishLoad:(UIWebView *)webView
{
#pragma unused(webView)
    [timer invalidate];
    timer = nil;
}

- (void)webView:(UIWebView *)webView didFailLoadWithError:(NSError *)error
{
#pragma unused(webView)
    if(timer && [timer isValid]){
        [timer invalidate];
        timer = nil;
    }
    
    if (NSURLErrorCancelled == error.code)
    {
        //This is a common error that webview generates and could be ignored.
        //See this thread for details: https://discussions.apple.com/thread/1727260
        return;
    }

    if([error.domain isEqual:@"WebKitErrorDomain"]){
        return;
    }
    
    // Ignore failures that are triggered after we have found the end URL
    if ( _complete == YES )
    {
        //We expect to get an error here, as we intentionally fail to navigate to the final redirect URL.
        AD_LOG_VERBOSE(@"Expected error", [error localizedDescription]);
        return;
    }
    
    // Tell our delegate that we are done after an error.
    if (_delegate)
    {
        AD_LOG_ERROR(@"authorization error", error.code, [error localizedDescription]);
        if([ADNTLMHandler isChallengeCancelled]){
            dispatch_async( dispatch_get_main_queue(), ^{ [_delegate webAuthenticationDidCancel]; } );
        } else{
            dispatch_async( dispatch_get_main_queue(), ^{ [_delegate webAuthenticationDidFailWithError:error]; } );
        }
    }
    else
    {
        AD_LOG_ERROR(@"Delegate object is lost", AD_ERROR_APPLICATION, @"The delegate object was lost, potentially due to another concurrent request.");
    }
}

- (void) failWithTimeout{
    
    AD_LOG_ERROR(@"Request load timeout", NSURLErrorTimedOut, nil);
    [self webView:_webView didFailLoadWithError:[NSError errorWithDomain:NSURLErrorDomain
                                                                    code:NSURLErrorTimedOut
                                                                userInfo:nil]];
}

@end
