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

@class ADAuthenticationError;

@interface ADNTLMHandler : NSObject

/* Starts intercepting HTTPS connections to enable NTLM authentication over webview. 
 If this method succeeds, it should be paired with endWebViewNTLMHandler. The method attempts
 to retrieve the workplace join identity and certificate. The interception should be as 
 short as possible, as this is a very specific fix to overcome the webview limitations. */
+(BOOL) startWebViewNTLMHandlerWithError: (ADAuthenticationError* __autoreleasing*) error;
/* Stops the HTTPS interception. */
+(void) endWebViewNTLMHandler;

/* Handles a client authentication NTLM challenge by collecting user credentials. Returns YES,
 if the challenge has been handled. */
+(BOOL) handleNTLMChallenge:(NSURLAuthenticationChallenge *)challenge
                 urlRequest:(NSURLRequest*) request
             customProtocol:(NSURLProtocol*) protocol;

+(BOOL) isChallengeCancelled;

+(void) setCancellationUrl:(NSString*) url;
@end
