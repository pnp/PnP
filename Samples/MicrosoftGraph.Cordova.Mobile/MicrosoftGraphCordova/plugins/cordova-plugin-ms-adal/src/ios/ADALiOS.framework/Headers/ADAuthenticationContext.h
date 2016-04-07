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
#import <Foundation/Foundation.h>

#import "ADTokenCacheStoring.h"
#import "ADAuthenticationError.h"
#import "ADAuthenticationResult.h"
#import "ADTokenCacheStoreItem.h"
#import "ADUserInformation.h"
#import "ADTokenCacheStoreKey.h"

#if TARGET_OS_IPHONE
//iOS:
#   include <UIKit/UIKit.h>
typedef UIWebView WebViewType;
#else
//OS X:
#   include <WebKit/WebKit.h>
typedef WebView   WebViewType;
#endif

@class UIViewController;

typedef enum
{
    /*! Default option. Assumes the assertion provided is of type SAML 1.1. */
    AD_SAML1_1,
    
    /*! Assumes the assertion provided is of type SAML 2. */
    AD_SAML2,
} ADAssertionType;


typedef enum
{
    /*! Default option. Users will be prompted only if their attention is needed. First the cache will
     be checked for a suitable access token (non-expired). If none is found, the cache will be checked
     for a suitable refresh token to be used for obtaining a new access token. If this attempt fails
     too, it depends on the acquireToken method being called.
     acquireTokenWithResource methods will prompt the user to re-authorize the resource usage by providing
     credentials. If user login cookies are present from previous authorization, the webview will be
     displayed and automatically dismiss itself without asking the user to re-enter credentials.
     acquireTokenSilentWithResource methods will not show UI in this case, but fail with error code
     AD_ERROR_USER_INPUT_NEEDED. */
    AD_PROMPT_AUTO,
    
    /*! The user will be prompted explicitly for credentials, consent or any other prompts. This option
     is useful in multi-user scenarios. Example is authenticating for the same e-mail service with different
     user. */
    AD_PROMPT_ALWAYS,
    
    /*! Re-authorizes (through displaying webview) the resource usage, making sure that the resulting access
     token contains updated claims. If user logon cookies are available, the user will not be asked for
     credentials again and the logon dialog will dismiss automatically. This is equivalent to passing
     prompt=refresh_session as an extra query parameter during the authorization. */
    AD_PROMPT_REFRESH_SESSION,
} ADPromptBehavior;

@class ADAuthenticationResult;

/*! The completion block declaration. */
typedef void(^ADAuthenticationCallback)(ADAuthenticationResult* result);

/*! The central class for managing multiple tokens. Usage: create one per AAD or ADFS authority.
 As authority is required, the class cannot be used with "new" or the parameterless "init" selectors.
 Attempt to call [ADAuthenticationContext new] or [[ADAuthenticationContext alloc] init] will throw an exception.
 @class ADAuthenticationContext
 */
@interface ADAuthenticationContext : NSObject

/*! The method allows subclassing of ADAuthenticationContext. For direct class usage, the static factory methods
 are recommended due to their simplicity.
 @param authority: The AAD or ADFS authority. Example: @"https://login.windows.net/contoso.com"
 @param validateAuthority: Specifies if the authority should be validated.
 @param tokenCacheStore: Allows the user to specify a dictionary object that will implement the token caching. If this
 parameter is null, tokens will not be cached.
 @param error: the method will fill this parameter with the error details, if such error occurs. This parameter can
 be nil.
 */
-(id) initWithAuthority: (NSString*) authority
      validateAuthority: (BOOL) validateAuthority        tokenCacheStore: (id<ADTokenCacheStoring>)tokenCache
                  error: (ADAuthenticationError* __autoreleasing *) error;

/*! Creates the object, setting the authority, default cache and enables the authority validation. In case of an error
 the function will return nil and if the error parameter is supplied, it will be filled with error details.
 @param authority: The AAD or ADFS authority. Example: @"https://login.windows.net/contoso.com"
 @param error: the method will fill this parameter with the error details, if such error occurs. This parameter can
 be nil. */
+(ADAuthenticationContext*) authenticationContextWithAuthority: (NSString*) authority
                                                         error: (ADAuthenticationError* __autoreleasing *) error;

/*! Creates the object, setting the authority, default cache and desired authority validation flag. In case of an error
 the function will return nil and if the error parameter is supplied, it will be filled with error details.
 @param authority: The AAD or ADFS authority. Example: @"https://login.windows.net/contoso.com"
 @param validateAuthority: Specifies if the authority should be validated.
 @param error: the method will fill this parameter with the error details, if such error occurs. This parameter can
 be nil. */
+(ADAuthenticationContext*) authenticationContextWithAuthority: (NSString*) authority
                                             validateAuthority: (BOOL) validate
                                                         error: (ADAuthenticationError* __autoreleasing *) error;

/*! Creates the object, setting the authority, desired cache and enables the authority validation. In case of an error
 the function will return nil and if the error parameter is supplied, it will be filled with error details.
 @param authority The AAD or ADFS authority. Example: @"https://login.windows.net/contoso.com"
 @param validateAuthority: Specifies if the authority should be validated.
 @param tokenCacheStore: Allows the user to specify a dictionary object that will implement the token caching. If this
 parameter is null, tokens will not be cached.
 @param error: the method will fill this parameter with the error details, if such error occurs. This parameter can
 be nil. */
+(ADAuthenticationContext*) authenticationContextWithAuthority: (NSString*) authority
                                               tokenCacheStore: (id<ADTokenCacheStoring>) tokenCache
                                                         error: (ADAuthenticationError* __autoreleasing *) error;

/*! Creates the object, setting the authority, desired cache and desired authority validation flag. In case of an error
 the function will return nil and if the error parameter is supplied, it will be filled with error details.
 @param authority The AAD or ADFS authority. Example: @"https://login.windows.net/contoso.com"
 @param validateAuthority Specifies if the authority should be validated.
 @param tokenCacheStore Allows the user to specify a dictionary object that will implement the token caching. If this
 parameter is null, the library will use a shared, internally implemented static instance instead.
 @param error: the method will fill this parameter with the error details, if such error occurs. This parameter can
 be nil. */
+(ADAuthenticationContext*) authenticationContextWithAuthority: (NSString*) authority
                                             validateAuthority: (BOOL) validate
                                               tokenCacheStore: (id<ADTokenCacheStoring>) tokenCache
                                                         error: (ADAuthenticationError* __autoreleasing *) error;

/*!
 */
+(BOOL) isResponseFromBroker:(NSURL*) response;

/*!
 */
+(void) handleBrokerResponse:(NSURL*) response
             completionBlock: (ADAuthenticationCallback) completionBlock;


/*! Represents the authority used by the context. */
@property (readonly) NSString* authority;

/*! Controls authority validation in acquire token calls. */
@property BOOL validateAuthority;

/*! Represents the URL scheme of the application. If nil, the API selects the first value in an array of URL schemes. */
@property NSString* applicationURLScheme;

/*! Provides access to the token cache used in this context. If null, tokens will not be cached. */
@property id<ADTokenCacheStoring> tokenCacheStore;

/*! Unique identifier passed to the server and returned back with errors. Useful during investigations to correlate the
 requests and the responses from the server. If nil, a new UUID is generated on every request. */
@property (strong, getter=getCorrelationId, setter=setCorrelationId:) NSUUID* correlationId;

/*! The parent view controller for the authentication view controller UI. This property will be used only if
 a custom web view is NOT specified. */
@property (weak) UIViewController* parentController;


/*! Gets or sets the webview, which will be used for the credentials. If nil, the library will create a webview object
 when needed, leveraging the parentController property. */
@property (weak) WebViewType* webView;

/*! Follows the OAuth2 protocol (RFC 6749). The function will first look at the cache and automatically check for token
 expiration. Additionally, if no suitable access token is found in the cache, but refresh token is available,
 the function will use the refresh token automatically. If neither of these attempts succeeds, the method will use the provided assertion to get an 
 access token from the service.
 
 @param samlAssertion: the assertion representing the authenticated user.
 @param assertionType: the assertion type of the user assertion.
 @param resource: the resource whose token is needed.
 @param clientId: the client identifier
 @param userId: the required user id of the authenticated user.
 @param completionBlock: the block to execute upon completion. You can use embedded block, e.g. "^(ADAuthenticationResult res){ <your logic here> }"
 */
-(void)  acquireTokenForAssertion: (NSString*) assertion
                    assertionType: (ADAssertionType) assertionType
                         resource: (NSString*) resource
                         clientId: (NSString*) clientId
                           userId: (NSString*) userId
                  completionBlock: (ADAuthenticationCallback) completionBlock;


/*! Follows the OAuth2 protocol (RFC 6749). The function will first look at the cache and automatically check for token
 expiration. Additionally, if no suitable access token is found in the cache, but refresh token is available,
 the function will use the refresh token automatically. If neither of these attempts succeeds, the method will display
 credentials web UI for the user to re-authorize the resource usage. Logon cookie from previous authorization may be
 leveraged by the web UI, so user may not be actuall prompted. Use the other overloads if a more precise control of the
 UI displaying is desired.
 @param resource: the resource whose token is needed.
 @param clientId: the client identifier
 @param redirectUri: The redirect URI according to OAuth2 protocol.
 @param completionBlock: the block to execute upon completion. You can use embedded block, e.g. "^(ADAuthenticationResult res){ <your logic here> }"
 */
-(void) acquireTokenWithResource: (NSString*) resource
                        clientId: (NSString*) clientId
                     redirectUri: (NSURL*) redirectUri
                 completionBlock: (ADAuthenticationCallback) completionBlock;

/*! Follows the OAuth2 protocol (RFC 6749). The function will first look at the cache and automatically check for token
 expiration. Additionally, if no suitable access token is found in the cache, but refresh token is available,
 the function will use the refresh token automatically. If neither of these attempts succeeds, the method will display
 credentials web UI for the user to re-authorize the resource usage. Logon cookie from previous authorization may be
 leveraged by the web UI, so user may not be actuall prompted. Use the other overloads if a more precise control of the
 UI displaying is desired.
 @param resource: the resource whose token is needed.
 @param clientId: the client identifier
 @param redirectUri: The redirect URI according to OAuth2 protocol
 @param userId: The user to be prepopulated in the credentials form. Additionally, if token is found in the cache,
 it may not be used if it belongs to different token. This parameter can be nil.
 @param completionBlock: the block to execute upon completion. You can use embedded block, e.g. "^(ADAuthenticationResult res){ <your logic here> }"
 */
-(void) acquireTokenWithResource: (NSString*) resource
                        clientId: (NSString*) clientId
                     redirectUri: (NSURL*) redirectUri
                          userId: (NSString*) userId
                 completionBlock: (ADAuthenticationCallback) completionBlock;

/*! Follows the OAuth2 protocol (RFC 6749). The function will first look at the cache and automatically check for token
 expiration. Additionally, if no suitable access token is found in the cache, but refresh token is available,
 the function will use the refresh token automatically. If neither of these attempts succeeds, the method will display
 credentials web UI for the user to re-authorize the resource usage. Logon cookie from previous authorization may be
 leveraged by the web UI, so user may not be actuall prompted. Use the other overloads if a more precise control of the
 UI displaying is desired.
 @param resource: the resource whose token is needed.
 @param clientId: the client identifier
 @param redirectUri: The redirect URI according to OAuth2 protocol
 @param userId: The user to be prepopulated in the credentials form. Additionally, if token is found in the cache,
 it may not be used if it belongs to different token. This parameter can be nil.
 @param extraQueryParameters: will be appended to the HTTP request to the authorization endpoint. This parameter can be nil.
 @param completionBlock: the block to execute upon completion. You can use embedded block, e.g. "^(ADAuthenticationResult res){ <your logic here> }"
 */
-(void)  acquireTokenWithResource: (NSString*) resource
                         clientId: (NSString*) clientId
                      redirectUri: (NSURL*) redirectUri
                           userId: (NSString*) userId
             extraQueryParameters: (NSString*) queryParams
                  completionBlock: (ADAuthenticationCallback) completionBlock;

/*! Follows the OAuth2 protocol (RFC 6749). The behavior is controlled by the promptBehavior parameter on whether to re-authorize the
 resource usage (through webview credentials UI) or attempt to use the cached tokens first.
 @param resource: the resource for whom token is needed.
 @param clientId: the client identifier
 @param redirectUri: The redirect URI according to OAuth2 protocol
 @param userId: The user to be prepopulated in the credentials form. Additionally, if token is found in the cache,
 it may not be used if it belongs to different token. This parameter can be nil.
 @param extraQueryParameters: will be appended to the HTTP request to the authorization endpoint. This parameter can be nil.
 @param credentialsType: controls the way of obtaining client credentials if such are needed.
 @param promptBehavior: controls if any credentials UI will be shownt.
 @param completionBlock: the block to execute upon completion. You can use embedded block, e.g. "^(ADAuthenticationResult res){ <your logic here> }"
 */
-(void)  acquireTokenWithResource: (NSString*) resource
                         clientId: (NSString*) clientId
                      redirectUri: (NSURL*) redirectUri
                   promptBehavior: (ADPromptBehavior) promptBehavior
                           userId: (NSString*) userId
             extraQueryParameters: (NSString*) queryParams
                  completionBlock: (ADAuthenticationCallback) completionBlock;

/*! Follows the OAuth2 protocol (RFC 6749). The function will first look at the cache and automatically check for token
 expiration. Additionally, if no suitable access token is found in the cache, but refresh token is available,
 the function will use the refresh token automatically. This method will not show UI for the user to reauthorize resource usage.
 If reauthorization is needed, the method will return an error with code AD_ERROR_USER_INPUT_NEEDED.
 @param resource: the resource whose token is needed.
 @param clientId: the client identifier
 @param redirectUri: The redirect URI according to OAuth2 protocol.
 @param completionBlock: the block to execute upon completion. You can use embedded block, e.g. "^(ADAuthenticationResult res){ <your logic here> }"
 */
-(void) acquireTokenSilentWithResource: (NSString*) resource
                              clientId: (NSString*) clientId
                           redirectUri: (NSURL*) redirectUri
                       completionBlock: (ADAuthenticationCallback) completionBlock;

/*! Follows the OAuth2 protocol (RFC 6749). The function will first look at the cache and automatically check for token
 expiration. Additionally, if no suitable access token is found in the cache, but refresh token is available,
 the function will use the refresh token automatically. This method will not show UI for the user to reauthorize resource usage.
 If reauthorization is needed, the method will return an error with code AD_ERROR_USER_INPUT_NEEDED.
 @param resource: the resource whose token is needed.
 @param clientId: the client identifier
 @param redirectUri: The redirect URI according to OAuth2 protocol
 @param userId: The user to be prepopulated in the credentials form. Additionally, if token is found in the cache,
 it may not be used if it belongs to different token. This parameter can be nil.
 @param completionBlock: the block to execute upon completion. You can use embedded block, e.g. "^(ADAuthenticationResult res){ <your logic here> }"
 */
-(void) acquireTokenSilentWithResource: (NSString*) resource
                              clientId: (NSString*) clientId
                           redirectUri: (NSURL*) redirectUri
                                userId: (NSString*) userId
                       completionBlock: (ADAuthenticationCallback) completionBlock;

/*! Follows the OAuth2 protocol (RFC 6749). Uses the refresh token to obtain an access token (and another refresh token). The method
 is superceded by acquireToken, which will implicitly use the refresh token if needed. Please use acquireTokenByRefreshToken
 only if you would like to store the access and refresh tokens, managing the expiration times in your application logic.
 @param refreshToken: the resource whose token is needed.
 @param clientId: the client identifier
 @param completionBlock: the block to execute upon completion. You can use embedded block, e.g. "^(ADAuthenticationResult res){ <your logic here }"
 */
-(void) acquireTokenByRefreshToken: (NSString*) refreshToken
                          clientId: (NSString*) clientId
                   completionBlock: (ADAuthenticationCallback) completionBlock;

/*! Follows the OAuth2 protocol (RFC 6749). Uses the refresh token to obtain an access token (and another refresh token). The method
 is superceded by acquireToken, which will implicitly use the refresh token if needed. Please use acquireTokenByRefreshToken
 only if you would like to store the access and refresh tokens, managing the expiration times in your application logic.
 @param refreshToken: the resource whose token is needed.
 @param clientId: the client identifier
 @param resource: the desired resource to which the token applies.
 @param completionBlock: the block to execute upon completion. You can use embedded block, e.g. "^(ADAuthenticationResult res){ <your logic here> }"
 */
-(void) acquireTokenByRefreshToken: (NSString*) refreshToken
                          clientId: (NSString*) clientId
                          resource: (NSString*) resource
                   completionBlock: (ADAuthenticationCallback) completionBlock;

@end










