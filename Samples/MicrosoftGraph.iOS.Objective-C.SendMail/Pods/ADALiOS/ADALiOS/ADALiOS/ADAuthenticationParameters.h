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

/*! Contains authentication parameters based on unauthorized
 response from resource server */
@interface ADAuthenticationParameters : NSObject
{
    @protected
    NSDictionary* _extractedParameters;
    NSString* _authority;
    NSString* _resource;
}

/*! The extracted authority. Can be null in case of an error. See the status field */
@property (readonly) NSString* authority;

/*! The resource URI, as returned by the server. */
@property (readonly) NSString* resource;

@property (readonly, getter = getExtractedParameters) NSDictionary* extractedParameters;

/*! The completion block declaration. In case of success, NSException parameter is nil and ADAuthenticationParameters
 is a valid pointer. If an error occurs, ADAuthenticationParameters will be nil and the NSException parameter
 contains all of the details.
*/
typedef void (^ADParametersCompletion)(ADAuthenticationParameters* parameters, ADAuthenticationError* error);

/*! Creates authentication parameters from the response received from the resource. The method 
 creates an HTTP GET request and expects the resource to have unauthorized status (401) and "WWW-Authenticate" 
 header, containing authentication parameters.
 @param: response: the response received from the server with the requirements above. May return null if
 an error has occurred.
 @param: error: Can be nil. If this parameter is not nil and an error occurred, it will be set to
 contain the error
 */
+(ADAuthenticationParameters*) parametersFromResponse: (NSHTTPURLResponse*) response
                                                error: (ADAuthenticationError*__autoreleasing*) error;

/*! Creates authentication parameters from "WWW-Authenticate" header of the response received
 from the resource. The method expects the header to contain authentication parameters.
 @param: authenticateHeader: the http response header, containing the authentication parameters.
 @param: error: Can be nil. If this parameter is not nil and an error occurred, it will be set to
 contain the error
 */
+(ADAuthenticationParameters*) parametersFromResponseAuthenticateHeader: (NSString*) authenticateHeader
                                                                  error: (ADAuthenticationError*__autoreleasing*) error;

/*! Extracts the authority from the the error code 401 http error code response. The method
 expects that the resource will respond with a HTTP 401 and "WWW-Authenticate" header, containing the
 authentication parameters.
 @param resourceUrl: address of the resource.
 @param completionBlock: the callback block to be executed upon completion.
 */
+(void) parametersFromResourceUrl: (NSURL*)resourceUrl
                  completionBlock: (ADParametersCompletion) completion;

/*! Returns a readonly copy of the extracted parameters from the authenticate header. */
-(NSDictionary*) getExtractedParameters;

@end
