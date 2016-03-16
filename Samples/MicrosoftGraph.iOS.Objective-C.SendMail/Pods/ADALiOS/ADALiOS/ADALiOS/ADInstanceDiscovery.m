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
#import "ADInstanceDiscovery.h"
#import "ADAuthenticationError.h"
#import "ADWebRequest.h"
#import "ADAuthenticationError.h"
#import "NSDictionary+ADExtensions.h"
#import "ADWebResponse.h"
#import "ADOAuth2Constants.h"
#import "ADAuthenticationSettings.h"
#import "NSString+ADHelperMethods.h"
#import "ADClientMetrics.h"

NSString* const sTrustedAuthority = @"https://login.windows.net";
NSString* const sApiVersionKey = @"api-version";
NSString* const sApiVersion = @"1.0";
NSString* const sAuthorizationEndPointKey = @"authorization_endpoint";
NSString* const sCommonAuthorizationEndpoint = @"common/oauth2/authorize";
NSString* const sTenantDiscoveryEndpoint = @"tenant_discovery_endpoint";

NSString* const sValidationServerError = @"The authority validation server returned an error: %@.";

@implementation ADInstanceDiscovery

-(id) init
{
    [super doesNotRecognizeSelector:_cmd];//Throws an exception.
    return nil;
}

-(id) initInternal
{
    self = [super init];
    if (self)
    {
        mValidatedAuthorities = [NSMutableSet new];
        //List of prevalidated authorities (Azure Active Directory cloud instances).
        //Only the sThrustedAuthority is used for validation of new authorities.
        [mValidatedAuthorities addObject:sTrustedAuthority];
        [mValidatedAuthorities addObject:@"https://login.chinacloudapi.cn"];
        [mValidatedAuthorities addObject:@"https://login.cloudgovapi.us"];
        [mValidatedAuthorities addObject:@"https://login.microsoftonline.com"];
    }
    
    return self;
}

/*! The getter of the public "validatedAuthorities" property. */
- (NSSet*) getValidatedAuthorities
{
    API_ENTRY;
    NSSet* copy;
    @synchronized (self)
    {
        copy = [NSSet setWithSet:mValidatedAuthorities];
    }
    return copy;
}

+(ADInstanceDiscovery*) sharedInstance
{
    API_ENTRY;
    @synchronized (self)
    {
        static ADInstanceDiscovery* instance;
        if (!instance)
        {
            instance = [[ADInstanceDiscovery alloc] initInternal];
        }
        return instance;
    }
}

/*! Extracts the base URL host, e.g. if the authority is
 "https://login.windows.net/mytenant.com/oauth2/authorize", the host will be
 "https://login.windows.net". Returns nil and reaises an error if the protocol
 is not https or the authority is not a valid URL.*/
-(NSString*) extractHost: (NSString*) authority
           correlationId: (NSUUID*) correlationId
                   error: (ADAuthenticationError* __autoreleasing *) error
{
    NSURL* fullUrl = [NSURL URLWithString:authority.lowercaseString];
    
    ADAuthenticationError* adError;
    if (!fullUrl || ![fullUrl.scheme isEqualToString:@"https"])
    {
        adError = [ADAuthenticationError errorFromArgument:authority argumentName:@"authority"];
    }
    else
    {
        NSArray* paths = fullUrl.pathComponents;
        if (paths.count < 2)
        {
            adError = [ADAuthenticationError errorFromAuthenticationError:AD_ERROR_INVALID_ARGUMENT protocolCode:nil errorDetails:
                       [NSString stringWithFormat:@"Missing tenant in the authority URL. Please add the tenant or use 'common', e.g. https://login.windows.net/example.com. CorrelationId: %@", [correlationId UUIDString]]];
        }
        else
        {
            NSString* tenant = [paths objectAtIndex:1];
            if ([@"adfs" isEqualToString:tenant])
            {
                adError = [ADAuthenticationError errorFromAuthenticationError:AD_ERROR_INVALID_ARGUMENT
                                                                 protocolCode:nil
                                                                 errorDetails:
                           [NSString stringWithFormat:@"Authority validation is not supported for ADFS instances. Consider disabling the authority validation in the authentication context. CorrelationId: %@", [correlationId UUIDString]]];
            }
        }
    }
    
    if (adError)
    {
        if (error)
        {
            *error = adError;
        }
        return nil;
    }
    
    return [NSString stringWithFormat:@"https://%@", fullUrl.host];
}

-(void) validateAuthority: (NSString*) authority
            correlationId: (NSUUID*) correlationId
          completionBlock: (ADDiscoveryCallback) completionBlock;
{
    API_ENTRY;
    THROW_ON_NIL_ARGUMENT(completionBlock);
    if (!correlationId)
    {
        correlationId = [NSUUID UUID];//Create one if not passed.
    }
    
    NSString* message = [NSString stringWithFormat:@"Attempting to validate the authority: %@; CorrelationId: %@", authority, [correlationId UUIDString]];
    AD_LOG_VERBOSE(@"Instance discovery", message);
    
    ADAuthenticationError* error;
    NSString* authorityHost = [self extractHost:authority correlationId:correlationId error:&error];
    if (error)
    {
        completionBlock(NO, error);
        return;
    }
    
    //Cache poll:
    if ([self isAuthorityValidated:authorityHost])
    {
        completionBlock(YES, nil);
        return;
    }
    
    
    dispatch_async(dispatch_get_global_queue(DISPATCH_QUEUE_PRIORITY_HIGH, 0), ^
                   {
                       //Nothing in the cache, ask the server:
                       [self requestValidationOfAuthority:authority
                                                     host:authorityHost
                                         trustedAuthority:sTrustedAuthority
                                            correlationId:correlationId
                                          completionBlock:completionBlock];
                   });
}

//Checks the cache for previously validated authority.
//Note that the authority host should be normalized: no ending "/" and lowercase.
-(BOOL) isAuthorityValidated: (NSString*) authorityHost
{
    THROW_ON_NIL_EMPTY_ARGUMENT(authorityHost);
    
    BOOL validated;
    @synchronized(self)
    {
        validated = [mValidatedAuthorities containsObject:authorityHost];
    }
    
    NSString* message = [NSString stringWithFormat:@"Checking cache for '%@'. Result: %d", authorityHost, validated];
    AD_LOG_VERBOSE(@"Authority Validation Cache", message);
    return validated;
}

//Note that the authority host should be normalized: no ending "/" and lowercase.
-(void) setAuthorityValidation: (NSString*) authorityHost
{
    THROW_ON_NIL_EMPTY_ARGUMENT(authorityHost);
    
    @synchronized(self)
    {
        [mValidatedAuthorities addObject:authorityHost];
    }
    
    NSString* message = [NSString stringWithFormat:@"Setting validation set to YES for authority '%@'", authorityHost];
    AD_LOG_VERBOSE(@"Authority Validation Cache", message);
}

//Sends authority validation to the trustedAuthority by leveraging the instance discovery endpoint
//If the authority is known, the server will set the "tenant_discovery_endpoint" parameter in the response.
//The method should be executed on a thread that is guarranteed to exist upon completion, e.g. the UI thread.
-(void) requestValidationOfAuthority: (NSString*) authority
                                host: (NSString*) authorityHost
                    trustedAuthority: (NSString*) trustedAuthority
                       correlationId: (NSUUID*) correlationId
                     completionBlock: (ADDiscoveryCallback) completionBlock
{
    THROW_ON_NIL_ARGUMENT(completionBlock);
    THROW_ON_NIL_ARGUMENT(correlationId);//Should be set by the caller
    
    //All attempts to complete are done. Now try to validate the authorization ednpoint:
    NSString* authorizationEndpoint = [authority stringByAppendingString:OAUTH2_AUTHORIZE_SUFFIX];
    
    NSMutableDictionary *request_data = [NSMutableDictionary dictionaryWithObjectsAndKeys:
                                         sApiVersion, sApiVersionKey,
                                         authorizationEndpoint, sAuthorizationEndPointKey,
                                         nil];
    
    NSString* endPoint = [NSString stringWithFormat:@"%@/%@?%@", trustedAuthority, OAUTH2_INSTANCE_DISCOVERY_SUFFIX, [request_data adURLFormEncode]];
    
    AD_LOG_VERBOSE(@"Authority Validation Request", endPoint);
    ADWebRequest *webRequest = [[ADWebRequest alloc] initWithURL:[NSURL URLWithString:endPoint] correlationId:correlationId];
    
    webRequest.method = HTTPGet;
    [webRequest.headers setObject:@"application/json" forKey:@"Accept"];
    [webRequest.headers setObject:@"application/x-www-form-urlencoded" forKey:@"Content-Type"];
    [[ADClientMetrics getInstance] beginClientMetricsRecordForEndpoint:endPoint correlationId:[correlationId UUIDString] requestHeader:webRequest.headers];
    
    [webRequest send:^( NSError *error, ADWebResponse *webResponse )
     {
         // Request completion callback
         NSDictionary *response = nil;
         
         BOOL verified = NO;
         ADAuthenticationError* adError = nil;
         if ( error == nil )
         {
             switch (webResponse.statusCode)
             {
                 case 200:
                 case 400:
                 case 401:
                 {
                     NSError   *jsonError  = nil;
                     id         jsonObject = [NSJSONSerialization JSONObjectWithData:webResponse.body options:0 error:&jsonError];
                     
                     if ( nil != jsonObject && [jsonObject isKindOfClass:[NSDictionary class]] )
                     {
                         // Load the response
                         response = (NSDictionary *)jsonObject;
                         AD_LOG_VERBOSE(@"Discovery response", response.description);
                         verified = ![NSString adIsStringNilOrBlank:[response objectForKey:sTenantDiscoveryEndpoint]];
                         if (verified)
                         {
                             [self setAuthorityValidation:authorityHost];
                         }
                         else
                         {
                             //First check for explicit OAuth2 protocol error:
                             NSString* serverOAuth2Error = [response objectForKey:OAUTH2_ERROR];
                             NSString* errorDetails = [response objectForKey:OAUTH2_ERROR_DESCRIPTION];
                             // Error response from the server
                             adError = [ADAuthenticationError errorFromAuthenticationError:AD_ERROR_AUTHORITY_VALIDATION
                                                                              protocolCode:serverOAuth2Error
                                                                              errorDetails:(errorDetails) ? errorDetails : [NSString stringWithFormat:sValidationServerError, serverOAuth2Error]];
                             
                         }
                     }
                     else
                     {
                         if (jsonError)
                         {
                             adError = [ADAuthenticationError errorFromAuthenticationError:AD_ERROR_AUTHORITY_VALIDATION
                                                                              protocolCode:nil
                                                                              errorDetails:jsonError.localizedDescription];
                         }
                         else
                         {
                             NSString* errorMessage = [NSString stringWithFormat:@"Unexpected object type: %@", [jsonObject class]];
                             adError = [ADAuthenticationError errorFromAuthenticationError:AD_ERROR_AUTHORITY_VALIDATION
                                                                              protocolCode:nil
                                                                              errorDetails:errorMessage];
                         }
                     }
                 }
                     break;
                 default:
                 {
                     // Request failure
                     NSString* logMessage = [NSString stringWithFormat:@"Server HTTP Status %ld", (long)webResponse.statusCode];
                     NSString* errorData = [NSString stringWithFormat:@"Server HTTP Response %@", [[NSString alloc] initWithData:webResponse.body encoding:NSUTF8StringEncoding]];
                     AD_LOG_WARN(logMessage, errorData);
                     adError = [ADAuthenticationError errorFromAuthenticationError:AD_ERROR_AUTHORITY_VALIDATION protocolCode:nil errorDetails:errorData];
                 }
             }
         }
         else
         {
             AD_LOG_WARN(@"System error while making request.", error.description);
             // System error
             adError = [ADAuthenticationError errorFromNSError:error errorDetails:error.localizedDescription];
         }
         
         if(adError)
         {
             [[ADClientMetrics getInstance] endClientMetricsRecord:[adError description]];
         }
         else
         {
             [[ADClientMetrics getInstance] endClientMetricsRecord:nil];
         }
         
         completionBlock( verified, adError );
     }];
}

+(NSString*) canonicalizeAuthority: (NSString*) authority
{
    if ([NSString adIsStringNilOrBlank:authority])
    {
        return nil;
    }
    
    NSString* trimmedAuthority = [[authority adTrimmedString] lowercaseString];
    NSURL* url = [NSURL URLWithString:trimmedAuthority];
    if (!url)
    {
        AD_LOG_WARN_F(@"The authority is not a valid URL", @"Authority %@", authority);
        return nil;
    }
    NSString* scheme = url.scheme;
    if (![scheme isEqualToString:@"https"])
    {
        AD_LOG_WARN_F(@"Non HTTPS protocol for the authority", @"Authority %@", authority);
        return nil;
    }
    
    url = url.absoluteURL;//Resolve any relative paths.
    NSArray* paths = url.pathComponents;//Returns '/' as the first and the tenant as the second element.
    if (paths.count < 2)
        return nil;//No path component: invalid URL
    
    NSString* tenant = [paths objectAtIndex:1];
    if ([NSString adIsStringNilOrBlank:tenant])
    {
        return nil;
    }
    
    NSString* host = url.host;
    if ([NSString adIsStringNilOrBlank:host])
    {
        return nil;
    }
    trimmedAuthority = [NSString stringWithFormat:@"%@://%@/%@", scheme, host, tenant];
    
    return trimmedAuthority;
}

@end
