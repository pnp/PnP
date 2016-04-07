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

/*! The class contains an incrementally expanding list of errors */
typedef enum
{
    /*! No error occurred. The value is added to make easier usage of functions that take error code,
     but no error condition occurred.*/
    AD_ERROR_SUCCEEDED = 0,
    
    /*! The user has cancelled the applicable UI prompts */
    AD_ERROR_USER_CANCEL = 1,
    
    /*! The method call contains one or more invalid arguments */
    AD_ERROR_INVALID_ARGUMENT = 2,
    
    /*! HTTP 401 (Unauthorized) response does not contain the OAUTH2 required header */
    AD_ERROR_MISSING_AUTHENTICATE_HEADER = 3,
    
    /*! HTTP 401 (Unauthorized) response's authentication header is in invalid format
     or does not contain expected values. */
    AD_ERROR_AUTHENTICATE_HEADER_BAD_FORMAT = 4,
    
    /*! An internal error occurs when the library did not receive
     a response from the server */
    AD_ERROR_CONNECTION_MISSING_RESPONSE = 5,
    
    /*! The logic expects the server to return HTTP_UNAUTHORIZED */
    AD_ERROR_UNAUTHORIZED_CODE_EXPECTED = 6,
    
    /*! The refresh token cannot be used for extracting an access token. */
    AD_ERROR_INVALID_REFRESH_TOKEN = 7,
    
    /*! An unexpected internal error occurred. */
    AD_ERROR_UNEXPECTED = 8,
    
    /*! Access tokens for multiple users exist in the token cache. Please specify the userId. */
    AD_ERROR_MULTIPLE_USERS = 9,
    
    /*! User needs to re-authorize resource usage. This error is raised when access token cannot 
     be obtained without user explicitly re-authorizing, but the developer has called 
     acquireTokenSilentWithResource method. To obtain the token, the application will need to call
     acquireTokenWithResource after this error to allow the library to give user abitlity
     to re-authorize (with web UI involved). */
    AD_ERROR_USER_INPUT_NEEDED = 10,
    
    /*! The cache store cannot be persisted to the specified location. This error is raised only if
     the application called explicitly to persist the cache. Else, the errors are only logged
     as warnings. */
    AD_ERROR_CACHE_PERSISTENCE = 11,
    
    /*! An issue occurred while attempting to read the persisted token cache store. */
    AD_ERROR_BAD_CACHE_FORMAT = 12,
    
    /*! The user is currently prompted for another authentication. The library chose to raise this
     error instead of waiting to avoid multiple sequential prompts. It is up to the application
     developer to chose to retry later. */
    AD_ERROR_USER_PROMPTED = 13,
    
    /*! This type of error occurs when something went wrong with the application stack, e.g.
     the resource bundle cannot be loaded. */
    AD_ERROR_APPLICATION = 14,
    
    /*! A generic error code for all of the authentication errors. */
    AD_ERROR_AUTHENTICATION = 15,
    
    /*! An error was raised during the process of validating the authorization authority. */
    AD_ERROR_AUTHORITY_VALIDATION = 16,
    
    /*! Failed to extract the main view controller of the application. Make sure that the application
     has UI elements.*/
    AD_ERROR_NO_MAIN_VIEW_CONTROLLER = 17,
    
    /*! Failed to extract the framework resources (e.g. storyboards). Please read the readme and documentation
     for the library on how to link the ADAL library with its resources to your project.*/
    AD_ERROR_MISSING_RESOURCES = 18,
    
    /*! Token requested for user A, but obtained for user B. This can happen if the user explicitly authenticated
     as user B in the login UI, or if cookies for user B are already present.*/
    AD_ERROR_WRONG_USER = 19,
    
    /*! When client authentication is requested by TLS, the library attempts to extract the authentication
     certificate. The error is generated if more than one certificate is found in the keychain. */
    AD_ERROR_MULTIPLE_TLS_CERTIFICATES = 20,
    
    /*! When the hash of the decrypted broker response does not match the hash returned from broker. */
    AD_ERROR_BROKER_RESPONSE_HASH_MISMATCH = 21,
    
    /*! When the application waiting for broker is activated without broker response. */
    AD_ERROR_BROKER_RESPONSE_NOT_RECEIVED = 22,
    
    /*! When work place join is required by the service. */
    AD_ERROR_WPJ_REQUIRED = 23,
    
    /*! The redirect URI cannot be used for invoking broker. */
    AD_ERROR_INVALID_REDIRECT_URI = 23,
    
    /*! The error code was not sent to us due to an older version of the broker */
    AD_ERROR_BROKER_UNKNOWN = 24,
    
    /*! Server redirects authentication process to a non-https url */
    AD_ERROR_NON_HTTPS_REDIRECT = 25
    
} ADErrorCode;

/* HTTP status codes used by the library */
typedef enum
{
    HTTP_UNAUTHORIZED = 401,
} HTTPStatusCodes;