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

//Protocol constants:
extern NSString* const OAuth2_Bearer;
extern NSString* const OAuth2_Authenticate_Header;
extern NSString* const OAuth2_Authorization;
extern NSString* const OAuth2_Authorization_Uri;
extern NSString* const OAuth2_Resource_Id;

//Error messages:
extern NSString* const InvalidHeader_NoBearer;
extern NSString* const MissingHeader;
extern NSString* const MissingAuthority;
extern NSString* const ConnectionError;
extern NSString* const InvalidResponse;
extern NSString* const UnauthorizedHTTStatusExpected;

/*! Contains non-public declarations of the ADAuthenticationParameters class.
 Exposed in a separate header for easier testing */
@interface ADAuthenticationParameters (Internal)

/*! Internal initializer, should be called only from within the class definitions
 or derived classes. */
-(id) initInternalWithParameters: (NSDictionary*) extractedParameters
                           error: (ADAuthenticationError* __autoreleasing*) error;

/*! Internal method. Extracts the challenge parameters from the Bearer contents in the authorize header. 
 Returns nil in case of error and if "error" parameter is not nil, adds the error details to this parameter. */
+ (NSDictionary*) extractChallengeParameters: (NSString*) headerContents
                                       error: (ADAuthenticationError* __autoreleasing*) error;

@end
