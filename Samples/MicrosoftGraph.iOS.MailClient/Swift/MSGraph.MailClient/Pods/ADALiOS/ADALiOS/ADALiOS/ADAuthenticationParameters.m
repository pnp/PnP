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
#import "ADAuthenticationParameters.h"
#import "ADAuthenticationParameters+Internal.h"
#import "ADAuthenticationSettings.h"
#import "ADWebRequest.h"
#import "ADWebResponse.h"
#import "NSString+ADHelperMethods.h"

@implementation ADAuthenticationParameters

//These two are needed, as the instance variables will be accessed by the class category.
@synthesize authority = _authority;
@synthesize resource = _resource;

-(id) init
{
    //Throws exception as the method should not be called.
    [super doesNotRecognizeSelector:_cmd];
    return nil;
}

+(void) raiseErrorWithCode: (ADErrorCode) code
                   details: (NSString*) details
                     error: (ADAuthenticationError* __autoreleasing*) error
{
    //The error object should always be created to ensure propper logging, even if "error" is nil.
    ADAuthenticationError* raisedError = [ADAuthenticationError errorFromUnauthorizedResponse:code errorDetails:details];
    if (error)
    {
        *error = raisedError;
    }
}

-(NSDictionary*) getExtractedParameters
{
    return [NSDictionary dictionaryWithDictionary:_extractedParameters];
}

+(void) parametersFromResourceUrl:(NSURL*)resourceUrl
                  completionBlock:(ADParametersCompletion)completion
{
    API_ENTRY;
    THROW_ON_NIL_ARGUMENT(completion);//The block is required
    
    if (!resourceUrl)
    {
        //Nil passed, just call the callback on the same thread with the error:
        ADAuthenticationError* error = [ADAuthenticationError errorFromArgument:resourceUrl argumentName:@"resourceUrl"];
        completion(nil, error);
        return;
    }

    dispatch_async(dispatch_get_global_queue(DISPATCH_QUEUE_PRIORITY_HIGH, 0),^
    {
        ADWebRequest* request = [[ADWebRequest alloc] initWithURL:resourceUrl correlationId:nil];
        request.method = HTTPGet;
        AD_LOG_VERBOSE_F(@"Starting authorization challenge request", @"Resource: %@", resourceUrl);
        
        [request send:^(NSError * error, ADWebResponse *response) {
            ADAuthenticationError* adError;
            ADAuthenticationParameters* parameters;
            if (error)
            {
                adError = [ADAuthenticationError errorFromNSError:error
                                                     errorDetails:[NSString stringWithFormat:ConnectionError, error.description]];
            }
            else if (HTTP_UNAUTHORIZED != response.statusCode)
            {
                adError = [ADAuthenticationError errorFromUnauthorizedResponse:AD_ERROR_UNAUTHORIZED_CODE_EXPECTED
                                                                  errorDetails:[NSString stringWithFormat:UnauthorizedHTTStatusExpected,
                                                                                response.statusCode]];
            }
            else
            {
                //Request coming, attempt to process it:
                parameters = [self parametersFromResponseHeaders:response.headers error:&adError];
            }
            completion(parameters, adError);
        }];
    });
}

+(ADAuthenticationParameters*) parametersFromResponseHeaders:(NSDictionary*)headers
                                                       error:(ADAuthenticationError *__autoreleasing *)error
{
    // Handle 401 Unauthorized using the OAuth2 Implicit Profile
    NSString  *authenticateHeader = [headers valueForKey:OAuth2_Authenticate_Header];
    if ([NSString adIsStringNilOrBlank:authenticateHeader])
    {
        NSString* details = [NSString stringWithFormat:MissingHeader, OAuth2_Authenticate_Header];
        [self raiseErrorWithCode:AD_ERROR_MISSING_AUTHENTICATE_HEADER details:details error:error];
        
        return nil;
    }
    
    AD_LOG_INFO(@"Retrieved authenticate header", authenticateHeader);
    return [self parametersFromResponseAuthenticateHeader:authenticateHeader error:error];
}

+(ADAuthenticationParameters*) parametersFromResponse:(NSHTTPURLResponse*)response
                                                error:(ADAuthenticationError *__autoreleasing *)error
{
    API_ENTRY;
    RETURN_NIL_ON_NIL_ARGUMENT(response);
    
    return [self parametersFromResponseHeaders:response.allHeaderFields error:error];
}

+(ADAuthenticationParameters*) parametersFromResponseAuthenticateHeader:(NSString*)authenticateHeader
                                                                  error:(ADAuthenticationError *__autoreleasing *)error
{
    API_ENTRY;
    
    NSDictionary* params = [self extractChallengeParameters:authenticateHeader error:error];
    return params ? [[ADAuthenticationParameters alloc] initInternalWithParameters:params error:error]
                  : nil;
}


@end
