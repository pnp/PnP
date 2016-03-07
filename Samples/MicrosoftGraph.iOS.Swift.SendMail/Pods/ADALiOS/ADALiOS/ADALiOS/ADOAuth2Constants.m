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

#import "ADOAuth2Constants.h"

NSString *const OAUTH2_ACCESS_TOKEN       = @"access_token";
NSString *const OAUTH2_AUTHORIZATION      = @"authorization";
NSString *const OAUTH2_AUTHORIZE_SUFFIX   = @"/oauth2/authorize";

NSString *const OAUTH2_AUTHORIZATION_CODE = @"authorization_code";
NSString *const OAUTH2_AUTHORIZATION_URI  = @"authorization_uri";
NSString *const OAUTH2_BEARER             = @"Bearer";
NSString *const OAUTH2_CLIENT_ID          = @"client_id";
NSString *const OAUTH2_CLIENT_SECRET      = @"client_secret";
NSString *const OAUTH2_CODE               = @"code";
NSString *const OAUTH2_ERROR              = @"error";
NSString *const OAUTH2_ERROR_DESCRIPTION  = @"error_description";
NSString *const OAUTH2_EXPIRES_IN         = @"expires_in";
NSString *const OAUTH2_GRANT_TYPE         = @"grant_type";
NSString *const OAUTH2_PLATFORM_ID        = @"platform_id";
NSString *const OAUTH2_REALM              = @"realm";
NSString *const OAUTH2_REDIRECT_URI       = @"redirect_uri";
NSString *const OAUTH2_REFRESH_TOKEN      = @"refresh_token";
NSString *const OAUTH2_RESOURCE           = @"resource";
NSString *const OAUTH2_RESPONSE_TYPE      = @"response_type";
NSString *const OAUTH2_SCOPE              = @"scope";
NSString *const OAUTH2_STATE              = @"state";
NSString *const OAUTH2_TOKEN              = @"token";
NSString *const OAUTH2_TOKEN_SUFFIX       = @"/oauth2/token";
NSString *const OAUTH2_INSTANCE_DISCOVERY_SUFFIX = @"common/discovery/instance";
NSString *const OAUTH2_TOKEN_TYPE         = @"token_type";
NSString *const OAUTH2_LOGIN_HINT         = @"login_hint";
NSString *const OAUTH2_ID_TOKEN           = @"id_token";
NSString *const OAUTH2_CORRELATION_ID_RESPONSE  = @"correlation_id";
NSString *const OAUTH2_CORRELATION_ID_REQUEST   = @"return-client-request-id";
NSString *const OAUTH2_CORRELATION_ID_REQUEST_VALUE = @"client-request-id";
NSString *const OAUTH2_ASSERTION = @"assertion";
NSString *const OAUTH2_SAML11_BEARER_VALUE = @"urn:ietf:params:oauth:grant-type:saml1_1-bearer";
NSString *const OAUTH2_SAML2_BEARER_VALUE = @"urn:ietf:params:oauth:grant-type:saml2-bearer";

//Diagnostic traces sent to the Azure Active Directory servers:
NSString *const ADAL_ID_PLATFORM          = @"x-client-SKU";//The ADAL platform. iOS or OSX
NSString *const ADAL_ID_VERSION           = @"x-client-Ver";
NSString *const ADAL_ID_CPU               = @"x-client-CPU";//E.g. ARM64
NSString *const ADAL_ID_OS_VER            = @"x-client-OS";//iOS/OSX version
NSString *const ADAL_ID_DEVICE_MODEL      = @"x-client-DM";//E.g. iPhone

//Internal constants:
NSString *const AUTH_FAILED               = @"Authentication Failed";
NSString *const AUTH_FAILED_ERROR_CODE    = @"Authentication Failed: %d";
NSString *const AUTH_NON_PROTOCOL_ERROR   = @"non_protocol_error";

NSString *const AUTH_FAILED_SERVER_ERROR   = @"The Authorization Server returned an unrecognized response";
NSString *const AUTH_FAILED_NO_STATE       = @"The Authorization Server response has incorrectly encoded state";
NSString *const AUTH_FAILED_BAD_STATE      = @"The Authorization Server response has no encoded state";
NSString *const AUTH_FAILED_NO_TOKEN       = @"The requested access token could not be found";
NSString *const AUTH_FAILED_BAD_PARAMETERS = @"Incorrect parameters for authorization call";
NSString *const AUTH_FAILED_NO_CLIENTID    = @"Unable to determine client identifier";
NSString *const AUTH_FAILED_NO_REDIRECTURI = @"Unable to determine redirect URL";
NSString *const AUTH_FAILED_BUSY           = @"Authorization call already in progress";
