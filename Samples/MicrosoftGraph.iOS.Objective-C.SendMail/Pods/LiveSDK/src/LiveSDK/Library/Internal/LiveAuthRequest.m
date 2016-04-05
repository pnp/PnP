//
//  LiveAuthRequest.m
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


#import "LiveAuthDialogDelegate.h"
#import "LiveAuthDialog.h"
#import "LiveAuthHelper.h"
#import "LiveAuthRequest.h"
#import "LiveConnectClientCore.h"
#import "LiveConnectionHelper.h"
#import "LiveConstants.h"
#import "UrlHelper.h"

@implementation LiveAuthRequest

@synthesize  authCode = _authCode,
              session = _session,
                error = _error,
currentViewController = _currentViewController,
   authViewController = _authViewController,
               status = _status,
      tokenConnection = _tokenConnection,
    tokenResponseData = _tokenResponseData;

- (id) initWithClient:(LiveConnectClientCore *)client
               scopes:(NSArray *)scopes
currentViewController:(UIViewController *)currentViewController
             delegate:(id<LiveAuthDelegate>)delegate
            userState:(id)userState
{
    self = [super init];
    if (self) 
    {
        _client = client;
        _scopes = [scopes copy];
        _currentViewController = [currentViewController retain];
        _delegate = delegate;
        _userState = [userState retain];
        _status = AuthNotStarted;
    }
    
    return self; 
}

- (void)dealloc
{    
    _authViewController.delegate = nil;
    [_tokenConnection cancel];
    
    [_scopes release];
    [_userState release];
    
    [_authCode release];
    [_session release];
    [_error release];
    [_currentViewController release];
    [_authViewController release];
    [_tokenConnection release];
    [_tokenResponseData release];
    
    [super dealloc];
}

- (BOOL)isUserInvolved 
{
    return (_currentViewController != nil);
}

- (void)execute
{
    [self process];
}

- (void)updateStatus:(LiveAuthRequstStatus)status
{
    if (_status != AuthCompleted) {
        // Only update the status when it is not completed yet to avoid rolling back 
        // the process and triggering callback more than once.
        _status = status;
        [self process];        
    }
}

- (void)sendRequestCompletedMessage
{
    if (self.error == nil) 
    {
        if ([_delegate respondsToSelector:@selector(authCompleted:session:userState:)]) 
        {
            [_delegate authCompleted:_client.status 
                             session:_client.session 
                           userState:_userState];
        }
    }
    else
    {
        if ([_delegate respondsToSelector:@selector(authFailed:userState:)])
        {
            [_delegate authFailed:self.error 
                        userState:_userState];
        }
    }
}

- (void)dismissModal
{
    if (self.currentViewController) 
    {
        // Adding a checking logic and wait for the modal dialog to appear before we can dismiss it.
        if (self.authViewController.canDismiss) 
        {
            [self.currentViewController dismissViewControllerAnimated:YES completion:nil];
            self.currentViewController = nil;  
            self.authViewController = nil;
        }
        else
        {
            [self performSelector:@selector(dismissModal) withObject:self afterDelay:0.1];
        }
    }  
}

- (void)complete
{
    [self dismissModal];
    [self updateStatus:AuthCompleted];
}

- (void)process
{
    switch (self.status) 
    {
        case AuthNotStarted:
            [self authorize];
            break;
        case AuthAuthorized:
            [self retrieveToken];
            break;
        case AuthTokenRetrieved:
        case AuthFailed:
            [self complete];
            break;
        default:
            break;
    }
}

- (void)authorize
{
    NSURL *authRequestUrl = [LiveAuthHelper buildAuthUrlWithClientId:_client.clientId 
                                                         redirectUri:[LiveAuthHelper getDefaultRedirectUrlString] 
                                                              scopes:_scopes];

    NSString *nibName = [LiveAuthHelper isiPad]? @"LiveAuthDialog_iPad" : @"LiveAuthDialog_iPhone";
    
    _authViewController = [[LiveAuthDialog alloc] initWithNibName:nibName
                                                           bundle:[LiveAuthHelper getSDKBundle] 
                                                         startUrl:authRequestUrl 
                                                           endUrl:[LiveAuthHelper getDefaultRedirectUrlString]
                                                         delegate:self];
    
    // Create a Navigation controller
    UINavigationController *modalDialog = [[[UINavigationController alloc]initWithRootViewController:self.authViewController]
                                          autorelease];
    
    [self.currentViewController presentViewController:modalDialog
                                             animated:YES
                                           completion:nil];
}

- (void)retrieveToken
{
    NSURL * url = [LiveAuthHelper getRetrieveTokenUrl];
    NSMutableURLRequest* request = [NSMutableURLRequest requestWithURL:url
                                                           cachePolicy:NSURLRequestReloadIgnoringLocalCacheData
                                                       timeoutInterval:HTTP_REQUEST_TIMEOUT_INTERVAL];
    
    [request setHTTPMethod:@"POST"];
    [request setValue:LIVE_AUTH_POST_CONTENT_TYPE forHTTPHeaderField:LIVE_API_HEADER_CONTENTTYPE];
    [request setHTTPBody:[LiveAuthHelper buildGetTokenBodyDataWithClientId:_client.clientId 
                                                               redirectUri:[LiveAuthHelper getDefaultRedirectUrlString] 
                                                                  authCode:self.authCode]];
    
    self.tokenConnection = [LiveConnectionHelper createConnectionWithRequest:request delegate:self];
}

#pragma mark -  LiveAuthDialogDelegate

- (void) authDialogCompletedWithResponse:(NSURL *)responseUrl
{
    NSDictionary *responseData = [UrlHelper parseUrl:responseUrl];
    NSString *authCode = [responseData valueForKey:LIVE_AUTH_CODE];
    
    if (authCode != nil)
    {
        self.authCode = authCode;
        [self updateStatus:AuthAuthorized];
    }
    else
    {
        // Without 'code=xxxxx', this should be an error case.
        NSDictionary *errInfo;
        if ([responseData valueForKey:LIVE_ERROR_KEY_ERROR] != nil)
        {
            errInfo = responseData;
        }
        else
        {
            errInfo = [NSDictionary dictionaryWithObjectsAndKeys:
                      LIVE_ERROR_CODE_S_REQUEST_FAILED, LIVE_ERROR_KEY_ERROR,
                           LIVE_ERROR_DESC_AUTH_FAILED, LIVE_ERROR_KEY_DESCRIPTION,
                                                  nil];
        }
        
        self.error = [NSError errorWithDomain:LIVE_ERROR_DOMAIN 
                                         code:LIVE_ERROR_CODE_LOGIN_FAILED 
                                     userInfo:errInfo];      
        [self updateStatus:AuthFailed];
    }
}

- (void) authDialogFailedWithError:(NSError *)error
{
    if (_status >= AuthAuthorized) 
    {
        // We already passed authorization phase, ignore any further callback. 
        return;
    }
    
    self.error = [NSError errorWithDomain:LIVE_ERROR_DOMAIN 
                                     code:LIVE_ERROR_CODE_LOGIN_FAILED 
                                 userInfo:[NSDictionary dictionaryWithObjectsAndKeys:
                                           LIVE_ERROR_CODE_S_REQUEST_FAILED, LIVE_ERROR_KEY_ERROR,
                                                LIVE_ERROR_DESC_AUTH_FAILED, LIVE_ERROR_KEY_DESCRIPTION,
                                                                      error, LIVE_ERROR_KEY_INNER_ERROR,
                                                                      nil]];
    [self updateStatus:AuthFailed];
}

- (void) authDialogCanceled;
{
    self.error =  [NSError errorWithDomain:LIVE_ERROR_DOMAIN
                                      code:LIVE_ERROR_CODE_LOGIN_CANCELED 
                                  userInfo:[NSDictionary dictionaryWithObjectsAndKeys:
                                             LIVE_ERROR_CODE_S_REQUEST_CANCELED, LIVE_ERROR_KEY_ERROR,
                                                  LIVE_ERROR_DESC_AUTH_CANCELED, LIVE_ERROR_KEY_DESCRIPTION,
                                                                          nil]];
    [self updateStatus:AuthFailed];
}

- (void) authDialogDisappeared
{
    // We do callback only after the modal dialog has disappeared. 
    // Otherwise, the app code may not be able to open a new modal dialog.
    [self sendRequestCompletedMessage];
}

#pragma mark -  NSURLConnection delegate methods

- (void)connection:(NSURLConnection *)connection 
didReceiveResponse:(NSURLResponse *)response
{
    NSHTTPURLResponse *httpResponse = (NSHTTPURLResponse *)response;
    if ((httpResponse.statusCode / 100) != 2) 
    {
        NSString *description = [NSString stringWithFormat:@"HTTP error %zd", (ssize_t)httpResponse.statusCode];
        self.error = [LiveAuthHelper createAuthError:LIVE_ERROR_CODE_LOGIN_FAILED
                                            errorStr:LIVE_ERROR_CODE_S_REQUEST_FAILED
                                         description:description
                                          innerError:nil];
        
        [self updateStatus:AuthFailed];
    }
    else
    {
        self.tokenResponseData = [[[NSMutableData alloc] init] autorelease];
    }
}

- (void)connection:(NSURLConnection *)connection didReceiveData:(NSData *)data 
{
    [self.tokenResponseData appendData:data];
}

- (NSCachedURLResponse *)connection:(NSURLConnection *)connection
                  willCacheResponse:(NSCachedURLResponse*)cachedResponse 
{
    return nil;
}

- (void)connectionDidFinishLoading:(NSURLConnection *)connection 
{
    id response = [LiveAuthHelper readAuthResponse:self.tokenResponseData];
    
    if ([response isKindOfClass:[LiveConnectSession class]])
    {
        _client.session = response;
        self.session = response;
        [self updateStatus:AuthTokenRetrieved];
    }
    else
    {
        self.error = response;
        [self updateStatus:AuthFailed];
    }   
    
    self.tokenResponseData = nil;
    self.tokenConnection = nil;
}

- (void)connection:(NSURLConnection *)connection didFailWithError:(NSError *)error 
{
    self.error = [LiveAuthHelper createAuthError:LIVE_ERROR_CODE_LOGIN_FAILED
                                        errorStr:LIVE_ERROR_CODE_S_REQUEST_FAILED
                                     description:LIVE_ERROR_DESC_AUTH_FAILED 
                                      innerError:error];
    
    [self updateStatus:AuthFailed];
    
    self.tokenResponseData = nil;
    self.tokenConnection = nil;
}

@end
