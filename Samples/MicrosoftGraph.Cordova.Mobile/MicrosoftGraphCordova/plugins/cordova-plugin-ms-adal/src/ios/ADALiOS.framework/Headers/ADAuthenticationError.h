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

extern NSString* const ADAuthenticationErrorDomain;
/*! Incorrect argument passed */
extern NSString* const ADInvalidArgumentDomain;
/*! Error related to extracting authority from the 401 (Unauthorized) challenge response */
extern NSString* const ADUnauthorizedResponseErrorDomain;
/*! Error returned by Broker */
extern NSString* const ADBrokerResponseErrorDomain;

@interface ADAuthenticationError : NSError

/*! The error code, returned by the server. Can be null. */
@property (readonly) NSString* protocolCode;

/*! The full details of the error. Can contain details from an inner error. */
@property (readonly) NSString* errorDetails;

/*! Generates an error for invalid method argument. */
+(ADAuthenticationError*) errorFromArgument: (id) argument
                               argumentName: (NSString*) argumentName;

/*! Generates an error related to the 401 Bearer challenge handling */
+(ADAuthenticationError*) errorFromUnauthorizedResponse: (NSInteger) responseCode
                                           errorDetails: (NSString*) errorDetails;

/*! Generates an error object from an internally encountered error condition. Preserves the error
 code and domain of the original error and adds the custom details in the "errorDetails" property. */
+(ADAuthenticationError*) errorFromNSError: (NSError*) error errorDetails: (NSString*) errorDetails;

/*! Genearates an error from the code and details of an authentication error */
+(ADAuthenticationError*) errorFromAuthenticationError: (NSInteger) code
                                          protocolCode: (NSString*) protocolCode
                                          errorDetails: (NSString*) errorDetails;

/*! Generates an error when an unexpected internal library conditions occurs. */
+(ADAuthenticationError*) unexpectedInternalError: (NSString*) errorDetails;

/*! Generates an error from cancel operations. E.g. the user pressed "Cancel" button
 on the authorization UI page. */
+(ADAuthenticationError*) errorFromCancellation;

/*! Generates an error for the case that server redirects authentication process to a non-https url */
+ (ADAuthenticationError*)errorFromNonHttpsRedirect;


@end
