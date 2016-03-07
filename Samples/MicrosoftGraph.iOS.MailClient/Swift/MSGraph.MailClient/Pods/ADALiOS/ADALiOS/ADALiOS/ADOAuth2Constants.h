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

#pragma once

extern NSString *const OAUTH2_ACCESS_TOKEN;
extern NSString *const OAUTH2_AUTHORIZATION;
extern NSString *const OAUTH2_AUTHORIZATION_CODE;
extern NSString *const OAUTH2_AUTHORIZATION_URI;
extern NSString *const OAUTH2_AUTHORIZE_SUFFIX;
extern NSString *const OAUTH2_BEARER;
extern NSString *const OAUTH2_CLIENT_ID;
extern NSString *const OAUTH2_CLIENT_SECRET;
extern NSString *const OAUTH2_CODE;
extern NSString *const OAUTH2_ERROR;
extern NSString *const OAUTH2_ERROR_DESCRIPTION;
extern NSString *const OAUTH2_EXPIRES_IN;
extern NSString *const OAUTH2_GRANT_TYPE;
extern NSString *const OAUTH2_PLATFORM_ID;
extern NSString *const OAUTH2_REALM;
extern NSString *const OAUTH2_REDIRECT_URI;
extern NSString *const OAUTH2_REFRESH_TOKEN;
extern NSString *const OAUTH2_RESOURCE;
extern NSString *const OAUTH2_RESPONSE_TYPE;
extern NSString *const OAUTH2_SCOPE;
extern NSString *const OAUTH2_STATE;
extern NSString *const OAUTH2_TOKEN;
extern NSString *const OAUTH2_TOKEN_SUFFIX;
extern NSString *const OAUTH2_INSTANCE_DISCOVERY_SUFFIX;
extern NSString *const OAUTH2_TOKEN_TYPE;
extern NSString *const OAUTH2_LOGIN_HINT;
extern NSString *const OAUTH2_ID_TOKEN;
extern NSString *const OAUTH2_CORRELATION_ID_RESPONSE;
extern NSString *const OAUTH2_CORRELATION_ID_REQUEST;
extern NSString *const OAUTH2_CORRELATION_ID_REQUEST_VALUE;
extern NSString *const OAUTH2_SAML11_BEARER_VALUE;
extern NSString *const OAUTH2_SAML2_BEARER_VALUE;
extern NSString *const OAUTH2_ASSERTION;

//Diagnostic traces sent to the Azure Active Directory servers:
extern NSString *const ADAL_ID_PLATFORM;//The ADAL platform. iOS or OSX
extern NSString *const ADAL_ID_VERSION;
extern NSString *const ADAL_ID_CPU;//E.g. ARM64
extern NSString *const ADAL_ID_OS_VER;//iOS/OSX version
extern NSString *const ADAL_ID_DEVICE_MODEL;//E.g. iPhone 5S


extern NSString *const AUTH_FAILED; //Generic error.
extern NSString *const AUTH_FAILED_ERROR_CODE;
extern NSString *const AUTH_NON_PROTOCOL_ERROR; //A special error to denote that the error was not part of the protocol. E.g. a connection error.

extern NSString *const AUTH_FAILED_SERVER_ERROR;
extern NSString *const AUTH_FAILED_NO_STATE;
extern NSString *const AUTH_FAILED_BAD_STATE;
extern NSString *const AUTH_FAILED_NO_TOKEN;
extern NSString *const AUTH_FAILED_BAD_PARAMETERS;
extern NSString *const AUTH_FAILED_NO_CLIENTID;
extern NSString *const AUTH_FAILED_NO_REDIRECTURI;
extern NSString *const AUTH_FAILED_BUSY;
