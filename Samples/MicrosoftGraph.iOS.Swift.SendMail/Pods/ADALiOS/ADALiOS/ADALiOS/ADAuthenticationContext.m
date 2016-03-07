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
#import "ADAuthenticationContext.h"
#import "ADAuthenticationResult.h"
#import "ADAuthenticationResult+Internal.h"
#import "ADOAuth2Constants.h"
#import "ADAuthenticationBroker.h"
#import "ADAuthenticationSettings.h"
#import <libkern/OSAtomic.h>
#import "NSURL+ADExtensions.h"
#import "NSDictionary+ADExtensions.h"
#import "ADWebRequest.h"
#import "ADWebResponse.h"
#import "ADInstanceDiscovery.h"
#import "ADTokenCacheStoreItem.h"
#import "ADTokenCacheStoreKey.h"
#import "ADUserInformation.h"
#import "ADWorkPlaceJoin.h"
#import "ADPkeyAuthHelper.h"
#import "ADWorkPlaceJoinConstants.h"
#import "ADClientMetrics.h"

NSString* const unknownError = @"Uknown error.";
NSString* const credentialsNeeded = @"The user credentials are need to obtain access token. Please call the non-silent acquireTokenWithResource methods.";
NSString* const serverError = @"The authentication server returned an error: %@.";

//Used for the callback of obtaining the OAuth2 code:
typedef void(^ADAuthorizationCodeCallback)(NSString*, ADAuthenticationError*);
static volatile int sDialogInProgress = 0;

BOOL isCorrelationIdUserProvided = NO;

@implementation ADAuthenticationContext

-(id) init
{
    //Ensure that the appropriate init function is called. This will cause the runtime to throw.
    [super doesNotRecognizeSelector:_cmd];
    return nil;
}

//A wrapper around checkAndHandleBadArgument. Assumes that "completionMethod" is in scope:
#define HANDLE_ARGUMENT(ARG) \
if (![self checkAndHandleBadArgument:ARG \
argumentName:TO_NSSTRING(#ARG) \
completionBlock:completionBlock]) \
{ \
return; \
}


- (BOOL) handleNilOrEmptyAsResult:(NSObject*) argumentValue
                     argumentName: (NSString*) argumentName
             authenticationResult: (ADAuthenticationResult**)authenticationResult
{
    if (!argumentValue || ([argumentValue isKindOfClass:[NSString class]] && [NSString adIsStringNilOrBlank:(NSString*)argumentValue]))
    {
        ADAuthenticationError* argumentError = [ADAuthenticationError errorFromArgument:argumentValue argumentName:argumentName];
        *authenticationResult = [ADAuthenticationResult resultFromError:argumentError];
        return NO;
    }
    
    return YES;
}

/*! Verifies that the string parameter is not nil or empty. If it is,
 the method generates an error and set it to an authentication result.
 Then the method calls the callback with the result.
 The method returns if the argument is valid. If the method returns false,
 the calling method should return. */
-(BOOL) checkAndHandleBadArgument: (NSObject*) argumentValue
                     argumentName: (NSString*) argumentName
                  completionBlock: (ADAuthenticationCallback)completionBlock
{
    if (!argumentValue || ([argumentValue isKindOfClass:[NSString class]] && [NSString adIsStringNilOrBlank:(NSString*)argumentValue]))
    {
        ADAuthenticationError* argumentError = [ADAuthenticationError errorFromArgument:argumentValue argumentName:argumentName];
        ADAuthenticationResult* result = [ADAuthenticationResult resultFromError:argumentError];
        completionBlock(result);//Call the callback to tell about the result
        return NO;
    }
    else
    {
        return YES;
    }
}

//Translates the ADPromptBehavior into prompt query parameter. May return nil, if such
//parameter is not needed.
+(NSString*) getPromptParameter: (ADPromptBehavior) prompt
{
    switch (prompt) {
        case AD_PROMPT_ALWAYS:
            return @"login";
        case AD_PROMPT_REFRESH_SESSION:
            return @"refresh_session";
        default:
            return nil;
    }
}

+(BOOL) isForcedAuthorization: (ADPromptBehavior) prompt
{
    //If prompt parameter needs to be passed, re-authorization is needed.
    return [self getPromptParameter:prompt] != nil;
}


-(id) initWithAuthority: (NSString*) authority
      validateAuthority: (BOOL)bValidate
        tokenCacheStore: (id<ADTokenCacheStoring>)tokenCache
                  error: (ADAuthenticationError* __autoreleasing *) error
{
    API_ENTRY;
    NSString* extractedAuthority = [ADInstanceDiscovery canonicalizeAuthority:authority];
    RETURN_ON_INVALID_ARGUMENT(!extractedAuthority, authority, nil);
    
    self = [super init];
    if (self)
    {
        _authority = extractedAuthority;
        _validateAuthority = bValidate;
        _tokenCacheStore = tokenCache;
    }
    return self;
}


+(ADAuthenticationContext*) authenticationContextWithAuthority: (NSString*) authority
                                                         error: (ADAuthenticationError* __autoreleasing *) error
{
    API_ENTRY;
    return [self authenticationContextWithAuthority: authority
                                  validateAuthority: YES
                                    tokenCacheStore: [ADAuthenticationSettings sharedInstance].defaultTokenCacheStore
                                              error: error];
}

+(ADAuthenticationContext*) authenticationContextWithAuthority: (NSString*) authority
                                             validateAuthority: (BOOL) bValidate
                                                         error: (ADAuthenticationError* __autoreleasing *) error
{
    API_ENTRY
    return [self authenticationContextWithAuthority: authority
                                  validateAuthority: bValidate
                                    tokenCacheStore: [ADAuthenticationSettings sharedInstance].defaultTokenCacheStore
                                              error: error];
}

+(ADAuthenticationContext*) authenticationContextWithAuthority: (NSString*) authority
                                               tokenCacheStore: (id<ADTokenCacheStoring>) tokenCache
                                                         error: (ADAuthenticationError* __autoreleasing *) error
{
    API_ENTRY;
    return [self authenticationContextWithAuthority:authority
                                  validateAuthority:YES
                                    tokenCacheStore:tokenCache
                                              error:error];
}

+(ADAuthenticationContext*) authenticationContextWithAuthority: (NSString*) authority
                                             validateAuthority: (BOOL)bValidate
                                               tokenCacheStore: (id<ADTokenCacheStoring>)tokenCache
                                                         error: (ADAuthenticationError* __autoreleasing *) error
{
    API_ENTRY;
    RETURN_NIL_ON_NIL_EMPTY_ARGUMENT(authority);
    
    return [[self alloc] initWithAuthority: authority
                         validateAuthority: bValidate
                           tokenCacheStore: tokenCache
                                     error: error];
}

- (NSUUID*) getCorrelationId
{
    return ADLogger.getCorrelationId;
}

- (void) setCorrelationId:(NSUUID*) correlationId
{
    [ADLogger setCorrelationId: correlationId];
    isCorrelationIdUserProvided = YES;
}

-(void)  acquireTokenForAssertion: (NSString*) assertion
                    assertionType: (ADAssertionType) assertionType
                         resource: (NSString*) resource
                         clientId: (NSString*) clientId
                           userId: (NSString*) userId
                  completionBlock: (ADAuthenticationCallback) completionBlock{
    API_ENTRY;
    return [self internalAcquireTokenForAssertion:assertion
                                         clientId:clientId
                                         resource: resource
                                    assertionType:  assertionType
                                           userId:userId
                                            scope:nil
                                         tryCache:YES
                                validateAuthority:self.validateAuthority
                                    correlationId:[self getCorrelationId]
                                  completionBlock:completionBlock];
    
}


-(void) acquireTokenWithResource: (NSString*) resource
                        clientId: (NSString*) clientId
                     redirectUri: (NSURL*) redirectUri
                 completionBlock: (ADAuthenticationCallback) completionBlock
{
    API_ENTRY;
    return [self internalAcquireTokenWithResource:resource
                                         clientId:clientId
                                      redirectUri:redirectUri
                                   promptBehavior:AD_PROMPT_AUTO
                                           silent:NO
                                           userId:nil
                                            scope:nil
                             extraQueryParameters:nil
                                         tryCache:YES
                                validateAuthority:self.validateAuthority
                                    correlationId:[self getCorrelationId]
                                  completionBlock:completionBlock];
}

-(void) acquireTokenWithResource: (NSString*) resource
                        clientId: (NSString*) clientId
                     redirectUri: (NSURL*) redirectUri
                          userId: (NSString*) userId
                 completionBlock: (ADAuthenticationCallback) completionBlock
{
    API_ENTRY;
    [self internalAcquireTokenWithResource:resource
                                  clientId:clientId
                               redirectUri:redirectUri
                            promptBehavior:AD_PROMPT_AUTO
                                    silent:NO
                                    userId:userId
                                     scope:nil
                      extraQueryParameters:nil
                                  tryCache:YES
                         validateAuthority:self.validateAuthority
                             correlationId:[self getCorrelationId]
                           completionBlock:completionBlock];
}


-(void) acquireTokenWithResource: (NSString*) resource
                        clientId: (NSString*)clientId
                     redirectUri: (NSURL*) redirectUri
                          userId: (NSString*) userId
            extraQueryParameters: (NSString*) queryParams
                 completionBlock: (ADAuthenticationCallback) completionBlock
{
    API_ENTRY;
    [self internalAcquireTokenWithResource:resource
                                  clientId:clientId
                               redirectUri:redirectUri
                            promptBehavior:AD_PROMPT_AUTO
                                    silent:NO
                                    userId:userId
                                     scope:nil
                      extraQueryParameters:queryParams
                                  tryCache:YES
                         validateAuthority:self.validateAuthority
                             correlationId:[self getCorrelationId]
                           completionBlock:completionBlock];
}

-(void) acquireTokenSilentWithResource: (NSString*) resource
                              clientId: (NSString*) clientId
                           redirectUri: (NSURL*) redirectUri
                       completionBlock: (ADAuthenticationCallback) completionBlock
{
    API_ENTRY;
    return [self internalAcquireTokenWithResource:resource
                                         clientId:clientId
                                      redirectUri:redirectUri
                                   promptBehavior:AD_PROMPT_AUTO
                                           silent:YES
                                           userId:nil
                                            scope:nil
                             extraQueryParameters:nil
                                         tryCache:YES
                                validateAuthority:self.validateAuthority
                                    correlationId:[self getCorrelationId]
                                  completionBlock:completionBlock];
}

-(void) acquireTokenSilentWithResource: (NSString*) resource
                              clientId: (NSString*) clientId
                           redirectUri: (NSURL*) redirectUri
                                userId: (NSString*) userId
                       completionBlock: (ADAuthenticationCallback) completionBlock
{
    API_ENTRY;
    return [self internalAcquireTokenWithResource:resource
                                         clientId:clientId
                                      redirectUri:redirectUri
                                   promptBehavior:AD_PROMPT_AUTO
                                           silent:YES
                                           userId:userId
                                            scope:nil
                             extraQueryParameters:nil
                                         tryCache:YES
                                validateAuthority:self.validateAuthority
                                    correlationId:[self getCorrelationId]
                                  completionBlock:completionBlock];
}

//Returns YES if we shouldn't attempt other means to get access token.
//
-(BOOL) isFinalResult: (ADAuthenticationResult*) result
{
    return (AD_SUCCEEDED == result.status) /* access token provided, no need to try anything else */
    || (result.error && !result.error.protocolCode); //Connection is down, server is unreachable or DNS error. No need to try refresh tokens.
}


/*Attemps to use the cache. Returns YES if an attempt was successful or if an
 internal asynchronous call will proceed the processing. */
-(void) attemptToUseCacheItem: (ADTokenCacheStoreItem*) item
               useAccessToken: (BOOL) useAccessToken
                samlAssertion: (NSString*) samlAssertion
                assertionType: (ADAssertionType) assertionType
                     resource: (NSString*) resource
                     clientId: (NSString*) clientId
                       userId: (NSString*) userId
                correlationId: (NSUUID*) correlationId
              completionBlock: (ADAuthenticationCallback)completionBlock
{
    //All of these should be set before calling this method:
    THROW_ON_NIL_ARGUMENT(completionBlock);
    HANDLE_ARGUMENT(item);
    HANDLE_ARGUMENT(resource);
    HANDLE_ARGUMENT(clientId);
    HANDLE_ARGUMENT(correlationId);//Should have been set before this call
    
    if (useAccessToken)
    {
        //Access token is good, just use it:
        [ADLogger logToken:item.accessToken tokenType:@"access token" expiresOn:item.expiresOn correlationId:nil];
        ADAuthenticationResult* result = [ADAuthenticationResult resultFromTokenCacheStoreItem:item multiResourceRefreshToken:NO];
        completionBlock(result);
        return;
    }
    
    if ([NSString adIsStringNilOrBlank:item.refreshToken])
    {
        completionBlock([ADAuthenticationResult resultFromError:
                         [ADAuthenticationError unexpectedInternalError:@"Attempting to use an item without refresh token."]]);
        return;
    }
    
    //Now attempt to use the refresh token of the passed cache item:
    [self internalAcquireTokenByRefreshToken:item.refreshToken
                                    clientId:clientId
                                    resource:resource
                                      userId:item.userInformation.userId
                                   cacheItem:item
                           validateAuthority:NO /* Done by the caller. */
                               correlationId:correlationId
                             completionBlock:^(ADAuthenticationResult *result)
     {
         //Asynchronous block:
         if ([self isFinalResult:result])
         {
             completionBlock(result);
             return;
         }
         
         //Try other means of getting access token result:
         if (!item.multiResourceRefreshToken)//Try multi-resource refresh token if not currently trying it
         {
             ADTokenCacheStoreKey* broadKey = [ADTokenCacheStoreKey keyWithAuthority:self.authority resource:nil clientId:clientId error:nil];
             if (broadKey)
             {
                 BOOL useAccessToken;
                 ADAuthenticationError* error = nil;
                 ADTokenCacheStoreItem* broadItem = [self findCacheItemWithKey:broadKey userId:userId useAccessToken:&useAccessToken error:&error];
                 if (error)
                 {
                     completionBlock([ADAuthenticationResult resultFromError:error]);
                     return;
                 }
                 
                 if (broadItem)
                 {
                     if (!broadItem.multiResourceRefreshToken)
                     {
                         AD_LOG_WARN(@"Unexpected", @"Multi-resource refresh token expected here.");
                         //Recover (avoid infinite recursion):
                         completionBlock(result);
                         return;
                     }
                     
                     //Call recursively with the cache item containing a multi-resource refresh token:
                     [self attemptToUseCacheItem:broadItem
                                  useAccessToken:NO
                                   samlAssertion:samlAssertion
                                   assertionType:assertionType
                                        resource:resource
                                        clientId:clientId
                                          userId:userId
                                   correlationId:correlationId
                                 completionBlock:completionBlock];
                     return;//The call above takes over, no more processing
                 }//broad item
             }//key
         }//!item.multiResourceRefreshToken
         
         //The refresh token attempt failed and no other suitable refresh token found
         //call acquireToken
         [self internalAcquireTokenForAssertion:samlAssertion
                                       clientId:clientId
                                       resource:resource
                                  assertionType: assertionType
                                         userId:userId
                                          scope:nil
                                       tryCache:NO
                              validateAuthority:NO /* Already validated in this block. */
                                  correlationId:correlationId
                                completionBlock:completionBlock];
     }];//End of the refreshing token completion block, executed asynchronously.
}


/*Attemps to use the cache. Returns YES if an attempt was successful or if an
 internal asynchronous call will proceed the processing. */
-(void) attemptToUseCacheItem: (ADTokenCacheStoreItem*) item
               useAccessToken: (BOOL) useAccessToken
                     resource: (NSString*) resource
                     clientId: (NSString*) clientId
                  redirectUri: (NSURL*) redirectUri
               promptBehavior: (ADPromptBehavior) promptBehavior
                       silent: (BOOL) silent
                       userId: (NSString*) userId
         extraQueryParameters: (NSString*) queryParams
                correlationId: (NSUUID*) correlationId
              completionBlock: (ADAuthenticationCallback)completionBlock
{
    //All of these should be set before calling this method:
    THROW_ON_NIL_ARGUMENT(completionBlock);
    HANDLE_ARGUMENT(item);
    HANDLE_ARGUMENT(resource);
    HANDLE_ARGUMENT(clientId);
    HANDLE_ARGUMENT(correlationId);//Should have been set before this call
    
    if (useAccessToken)
    {
        //Access token is good, just use it:
        [ADLogger logToken:item.accessToken tokenType:@"access token" expiresOn:item.expiresOn correlationId:nil];
        ADAuthenticationResult* result = [ADAuthenticationResult resultFromTokenCacheStoreItem:item multiResourceRefreshToken:NO];
        completionBlock(result);
        return;
    }
    
    if ([NSString adIsStringNilOrBlank:item.refreshToken])
    {
        completionBlock([ADAuthenticationResult resultFromError:
                         [ADAuthenticationError unexpectedInternalError:@"Attempting to use an item without refresh token."]]);
        return;
    }
    
    //Now attempt to use the refresh token of the passed cache item:
    [self internalAcquireTokenByRefreshToken:item.refreshToken
                                    clientId:clientId
                                    resource:resource
                                      userId:item.userInformation.userId
                                   cacheItem:item
                           validateAuthority:NO /* Done by the caller. */
                               correlationId:correlationId
                             completionBlock:^(ADAuthenticationResult *result)
     {
         //Asynchronous block:
         if ([self isFinalResult:result])
         {
             completionBlock(result);
             return;
         }
         
         //Try other means of getting access token result:
         if (!item.multiResourceRefreshToken)//Try multi-resource refresh token if not currently trying it
         {
             ADTokenCacheStoreKey* broadKey = [ADTokenCacheStoreKey keyWithAuthority:self.authority resource:nil clientId:clientId error:nil];
             if (broadKey)
             {
                 BOOL useAccessToken;
                 ADAuthenticationError* error;
                 ADTokenCacheStoreItem* broadItem = [self findCacheItemWithKey:broadKey userId:userId useAccessToken:&useAccessToken error:&error];
                 if (error)
                 {
                     completionBlock([ADAuthenticationResult resultFromError:error]);
                     return;
                 }
                 
                 if (broadItem)
                 {
                     if (!broadItem.multiResourceRefreshToken)
                     {
                         AD_LOG_WARN(@"Unexpected", @"Multi-resource refresh token expected here.");
                         //Recover (avoid infinite recursion):
                         completionBlock(result);
                         return;
                     }
                     
                     //Call recursively with the cache item containing a multi-resource refresh token:
                     [self attemptToUseCacheItem:broadItem
                                  useAccessToken:NO
                                        resource:resource
                                        clientId:clientId
                                     redirectUri:redirectUri
                                  promptBehavior:promptBehavior
                                          silent:silent
                                          userId:userId
                            extraQueryParameters:queryParams
                                   correlationId:correlationId
                                 completionBlock:completionBlock];
                     return;//The call above takes over, no more processing
                 }//broad item
             }//key
         }//!item.multiResourceRefreshToken
         
         //The refresh token attempt failed and no other suitable refresh token found
         //call acquireToken
         [self internalAcquireTokenWithResource: resource
                                       clientId: clientId
                                    redirectUri: redirectUri
                                 promptBehavior: promptBehavior
                                         silent: silent
                                         userId: userId
                                          scope: nil
                           extraQueryParameters: queryParams
                                       tryCache: NO
                              validateAuthority: NO
                                  correlationId:correlationId
                                completionBlock: completionBlock];
     }];//End of the refreshing token completion block, executed asynchronously.
}

-(void) acquireTokenWithResource: (NSString*) resource
                        clientId: (NSString*) clientId
                     redirectUri: (NSURL*) redirectUri
                  promptBehavior: (ADPromptBehavior) promptBehavior
                          userId: (NSString*) userId
            extraQueryParameters: (NSString*) queryParams
                 completionBlock: (ADAuthenticationCallback)completionBlock
{
    API_ENTRY;
    THROW_ON_NIL_ARGUMENT(completionBlock);//The only argument that throws
    [self internalAcquireTokenWithResource:resource
                                  clientId:clientId
                               redirectUri:redirectUri
                            promptBehavior:promptBehavior
                                    silent:NO
                                    userId:userId
                                     scope:nil
                      extraQueryParameters:queryParams
                                  tryCache:YES
                         validateAuthority:self.validateAuthority
                             correlationId:[self getCorrelationId]
                           completionBlock:completionBlock];
}

//Gets an item from the cache, where userId may be nil. Raises error, if items for multiple users
//are present and user id is not specified.
-(ADTokenCacheStoreItem*) extractCacheItemWithKey: (ADTokenCacheStoreKey*) key
                                           userId: (NSString*) userId
                                            error: (ADAuthenticationError* __autoreleasing*) error
{
    if (!key || !self.tokenCacheStore)
    {
        return nil;//Nothing to return
    }
    
    ADAuthenticationError* localError;
    ADTokenCacheStoreItem* item = [self.tokenCacheStore getItemWithKey:key userId:userId error:&localError];
    if (!item && !localError && userId)
    {//ADFS fix, where the userId is not received by the server, but can be passed to the API:
        //We didn't find element with the userId, try finding an item with nil userId:
        NSArray* items = [self.tokenCacheStore getItemsWithKey:key error:&localError];
        if(items.count) {
            item = items.firstObject;
        }else{
            item = nil;
        }
        
        if (item && item.userInformation)
        {
            item = nil;//Different user id, just clear.
        }
    }
    if (error && localError)
    {
        *error = localError;
    }
    return item;
}

//Checks the cache for item that can be used to get directly or indirectly an access token.
//Checks the multi-resource refresh tokens too.
-(ADTokenCacheStoreItem*) findCacheItemWithKey: (ADTokenCacheStoreKey*) key
                                        userId: (NSString*) userId
                                useAccessToken: (BOOL*) useAccessToken
                                         error: (ADAuthenticationError* __autoreleasing*) error
{
    if (!key || !self.tokenCacheStore)
    {
        return nil;//Nothing to return
    }
    ADAuthenticationError* localError;
    ADTokenCacheStoreItem* item = [self extractCacheItemWithKey:key userId:userId error:&localError];
    if (localError)
    {
        if (error)
        {
            *error = localError;
        }
        return nil;//Quick return if an error was detected.
    }
    
    if (item)
    {
        *useAccessToken = item.accessToken && !item.isExpired;
        if (*useAccessToken)
        {
            return item;
        }
        else if (![NSString adIsStringNilOrBlank:item.refreshToken])
        {
            return item;//Suitable direct refresh token found.
        }
        else
        {
            //We have a cache item that cannot be used anymore, remove it from the cache:
            [self.tokenCacheStore removeItemWithKey:key userId:userId error:nil];
        }
    }
    *useAccessToken = false;//No item with suitable access token exists
    
    if (![NSString adIsStringNilOrBlank:key.resource])
    {
        //The request came for specific resource. Try returning a multi-resource refresh token:
        ADTokenCacheStoreKey* broadKey = [ADTokenCacheStoreKey keyWithAuthority:self.authority
                                                                       resource:nil
                                                                       clientId:key.clientId
                                                                          error:&localError];
        if (!broadKey)
        {
            AD_LOG_WARN(@"Unexpected error", localError.errorDetails);
            return nil;//Recover
        }
        ADTokenCacheStoreItem* broadItem = [self extractCacheItemWithKey:broadKey userId:userId error:&localError];
        if (localError)
        {
            if (error)
            {
                *error = localError;
            }
            return nil;
        }
        return broadItem;
    }
    return nil;//Nothing suitable
}

//Makes sure that the correlation id contains a valid UUID.
//Generates a new one if needed.
-(void) updateCorrelationId: (NSUUID* __autoreleasing*) correlationId
{
    THROW_ON_NIL_ARGUMENT(correlationId);
    if (!*correlationId || !isCorrelationIdUserProvided)
    {
       [ADLogger setCorrelationId:[NSUUID UUID]];
        *correlationId = [self getCorrelationId];
    }
}


-(void) internalAcquireTokenForAssertion: (NSString*) samlAssertion
                                clientId: (NSString*) clientId
                                resource: (NSString*) resource
                           assertionType: (ADAssertionType) assertionType
                                  userId: (NSString*) userId
                                   scope: (NSString*) scope
                                tryCache:(BOOL) tryCache
                       validateAuthority: (BOOL) validateAuthority
                           correlationId: (NSUUID*) correlationId
                         completionBlock: (ADAuthenticationCallback)completionBlock
{
    
    THROW_ON_NIL_ARGUMENT(completionBlock);
    HANDLE_ARGUMENT(resource);
    HANDLE_ARGUMENT(samlAssertion);
    
    [self updateCorrelationId:&correlationId];
    
    if (validateAuthority)
    {
        [[ADInstanceDiscovery sharedInstance] validateAuthority:self.authority correlationId:correlationId completionBlock:^(BOOL validated, ADAuthenticationError *error)
         {
             //Error should always be raised if the authority cannot be validated
#pragma unused(validated)
             if (error)
             {
                 completionBlock([ADAuthenticationResult resultFromError:error]);
             }
             else
             {
                 [self internalAcquireTokenForAssertion:samlAssertion
                                               clientId:clientId
                                               resource:resource
                                          assertionType: assertionType
                                                 userId:userId
                                                  scope:scope
                                               tryCache:tryCache
                                      validateAuthority:NO /* Already validated in this block. */
                                          correlationId:correlationId
                                        completionBlock:completionBlock];
             }
         }];
        return;//The asynchronous handler above will do the work.
    }
    
    //Check the cache:
    ADAuthenticationError* error = nil;
    //We are explicitly creating a key first to ensure indirectly that all of the required arguments are correct.
    //This is the safest way to guarantee it, it will raise an error, if the the any argument is not correct:
    ADTokenCacheStoreKey* key = [ADTokenCacheStoreKey keyWithAuthority:self.authority resource:resource clientId:clientId error:&error];
    if (!key)
    {
        //If the key cannot be extracted, call the callback with the information:
        ADAuthenticationResult* result = [ADAuthenticationResult resultFromError:error];
        completionBlock(result);
        return;
    }
    
    if (tryCache && self.tokenCacheStore)
    {
        //Cache should be used in this case:
        BOOL accessTokenUsable;
        ADTokenCacheStoreItem* cacheItem = [self findCacheItemWithKey:key userId:userId useAccessToken:&accessTokenUsable error:&error];
        if (error)
        {
            completionBlock([ADAuthenticationResult resultFromError:error]);
            return;
        }
        
        if (cacheItem)
        {
            //Found a promising item in the cache, try using it:
            [self attemptToUseCacheItem:cacheItem
                         useAccessToken:accessTokenUsable
                          samlAssertion:samlAssertion
                          assertionType:assertionType
                               resource:resource
                               clientId:clientId
                                 userId:userId
                          correlationId:correlationId
                        completionBlock:completionBlock];
            return; //The tryRefreshingFromCacheItem has taken care of the token obtaining
        }
    }
    
    dispatch_async(dispatch_get_global_queue(DISPATCH_QUEUE_PRIORITY_HIGH, 0), ^
                   {
                       [self requestTokenByAssertion: samlAssertion
                                       assertionType: assertionType
                                            resource: resource
                                            clientId: clientId
                                               scope: nil//For future use
                                       correlationId: correlationId
                                          completion: completionBlock];
                   });
}



-(void) internalAcquireTokenWithResource: (NSString*) resource
                                clientId: (NSString*) clientId
                             redirectUri: (NSURL*) redirectUri
                          promptBehavior: (ADPromptBehavior) promptBehavior
                                  silent: (BOOL) silent /* Do not show web UI for authorization. */
                                  userId: (NSString*) userId
                                   scope: (NSString*) scope
                    extraQueryParameters: (NSString*) queryParams
                                tryCache: (BOOL) tryCache /* set internally to avoid infinite recursion */
                       validateAuthority: (BOOL) validateAuthority
                           correlationId: (NSUUID*) correlationId
                         completionBlock: (ADAuthenticationCallback)completionBlock
{
    THROW_ON_NIL_ARGUMENT(completionBlock);
    HANDLE_ARGUMENT(resource);
    
    [self updateCorrelationId:&correlationId];
    
    if (validateAuthority)
    {
        [[ADInstanceDiscovery sharedInstance] validateAuthority:self.authority correlationId:correlationId completionBlock:^(BOOL validated, ADAuthenticationError *error)
         {
             if (error)
             {
                 completionBlock([ADAuthenticationResult resultFromError:error]);
             }
             else
             {
                 [self internalAcquireTokenWithResource:resource
                                               clientId:clientId
                                            redirectUri:redirectUri
                                         promptBehavior:promptBehavior
                                                 silent:silent
                                                 userId:userId
                                                  scope:scope
                                   extraQueryParameters:queryParams
                                               tryCache:tryCache
                                      validateAuthority:NO /* Already validated in this block. */
                                          correlationId:correlationId
                                        completionBlock:completionBlock];
             }
         }];
        return;//The asynchronous handler above will do the work.
    }
    
    //Check the cache:
    ADAuthenticationError* error;
    //We are explicitly creating a key first to ensure indirectly that all of the required arguments are correct.
    //This is the safest way to guarantee it, it will raise an error, if the the any argument is not correct:
    ADTokenCacheStoreKey* key = [ADTokenCacheStoreKey keyWithAuthority:self.authority resource:resource clientId:clientId error:&error];
    if (!key)
    {
        //If the key cannot be extracted, call the callback with the information:
        ADAuthenticationResult* result = [ADAuthenticationResult resultFromError:error];
        completionBlock(result);
        return;
    }
    
    if (tryCache && ![self.class isForcedAuthorization:promptBehavior] && self.tokenCacheStore)
    {
        //Cache should be used in this case:
        BOOL accessTokenUsable;
        ADTokenCacheStoreItem* cacheItem = [self findCacheItemWithKey:key userId:userId useAccessToken:&accessTokenUsable error:&error];
        if (error)
        {
            completionBlock([ADAuthenticationResult resultFromError:error]);
            return;
        }
        
        if (cacheItem)
        {
            //Found a promising item in the cache, try using it:
            [self attemptToUseCacheItem:cacheItem
                         useAccessToken:accessTokenUsable
                               resource:resource
                               clientId:clientId
                            redirectUri:redirectUri
                         promptBehavior:promptBehavior
                                 silent:silent
                                 userId:userId
                   extraQueryParameters:queryParams
                          correlationId:correlationId
                        completionBlock:completionBlock];
            return; //The tryRefreshingFromCacheItem has taken care of the token obtaining
        }
    }
    
    if (silent)
    {
        //The cache lookup and refresh token attempt have been unsuccessful,
        //so credentials are needed to get an access token, but the developer, requested
        //no UI to be shown:
        ADAuthenticationError* error =
        [ADAuthenticationError errorFromAuthenticationError:AD_ERROR_USER_INPUT_NEEDED
                                               protocolCode:nil
                                               errorDetails:credentialsNeeded];
        ADAuthenticationResult* result = [ADAuthenticationResult resultFromError:error];
        completionBlock(result);
        return;
    }
    
    dispatch_async([ADAuthenticationSettings sharedInstance].dispatchQueue, ^
                   {
                       //Get the code first:
                       [self requestCodeByResource:resource
                                          clientId:clientId
                                       redirectUri:redirectUri
                                             scope:scope
                                            userId:userId
                                    promptBehavior:promptBehavior
                              extraQueryParameters:queryParams
                                     correlationId:correlationId
                                        completion:^(NSString * code, ADAuthenticationError *error)
                        {
                            if (error)
                            {
                                ADAuthenticationResult* result = (AD_ERROR_USER_CANCEL == error.code) ? [ADAuthenticationResult resultFromCancellation]
                                : [ADAuthenticationResult resultFromError:error];
                                completionBlock(result);
                            }
                            else
                            {
                                [self requestTokenByCode:code
                                                resource:resource
                                                clientId:clientId
                                             redirectUri:redirectUri
                                                   scope:scope
                                           correlationId:correlationId
                                              completion:^(ADAuthenticationResult *result)
                                 {
                                     if (AD_SUCCEEDED == result.status)
                                     {
                                         [self updateCacheToResult:result cacheItem:nil withRefreshToken:nil];
                                         result = [self updateResult:result toUser:userId];
                                     }
                                     completionBlock(result);
                                 }];
                            }
                        }];
                   });
}

-(void) acquireTokenByRefreshToken: (NSString*)refreshToken
                          clientId: (NSString*)clientId
                   completionBlock: (ADAuthenticationCallback)completionBlock
{
    API_ENTRY;
    [self internalAcquireTokenByRefreshToken:refreshToken
                                    clientId:clientId
                                    resource:nil
                                      userId:nil
                                   cacheItem:nil
                           validateAuthority:self.validateAuthority
                               correlationId:[self getCorrelationId]
                             completionBlock:completionBlock];
}

-(void) acquireTokenByRefreshToken:(NSString*)refreshToken
                          clientId:(NSString*)clientId
                          resource:(NSString*)resource
                   completionBlock:(ADAuthenticationCallback)completionBlock
{
    API_ENTRY;
    [self internalAcquireTokenByRefreshToken:refreshToken
                                    clientId:clientId
                                    resource:resource
                                      userId:nil
                                   cacheItem:nil
                           validateAuthority:self.validateAuthority
                               correlationId:[self getCorrelationId]
                             completionBlock:completionBlock];
}

//Stores the result in the cache. cacheItem parameter may be nil, if the result is successfull and contains
//the item to be stored.
-(void) updateCacheToResult: (ADAuthenticationResult*) result
                  cacheItem: (ADTokenCacheStoreItem*) cacheItem
           withRefreshToken: (NSString*) refreshToken
{
    if(![self handleNilOrEmptyAsResult:result argumentName:@"result" authenticationResult:&result]){
        return;
    }
    
    if (!self.tokenCacheStore)
        return;//No cache to update
    
    if (AD_SUCCEEDED == result.status)
    {
        if(![self handleNilOrEmptyAsResult:result.tokenCacheStoreItem argumentName:@"tokenCacheStoreItem" authenticationResult:&result]
           || ![self handleNilOrEmptyAsResult:result.tokenCacheStoreItem.resource argumentName:@"resource" authenticationResult:&result]
           || ![self handleNilOrEmptyAsResult:result.tokenCacheStoreItem.accessToken argumentName:@"accessToken" authenticationResult:&result])
        {
            return;
        }
        
        //In case of success we use explicitly the item that comes back in the result:
        cacheItem = result.tokenCacheStoreItem;
        NSString* savedRefreshToken = cacheItem.refreshToken;
        if (result.multiResourceRefreshToken)
        {
            AD_LOG_VERBOSE_F(@"Token cache store", @"Storing multi-resource refresh token for authority: %@", self.authority);
            
            //If the server returned a multi-resource refresh token, we break
            //the item into two: one with the access token and no refresh token and
            //another one with the broad refresh token and no access token and no resource.
            //This breaking is useful for further updates on the cache and quick lookups
            ADTokenCacheStoreItem* multiRefreshTokenItem = [cacheItem copy];
            cacheItem.refreshToken = nil;
            
            multiRefreshTokenItem.accessToken = nil;
            multiRefreshTokenItem.resource = nil;
            multiRefreshTokenItem.expiresOn = nil;
            [self.tokenCacheStore addOrUpdateItem:multiRefreshTokenItem error:nil];
        }
        
        AD_LOG_VERBOSE_F(@"Token cache store", @"Storing access token for resource: %@", cacheItem.resource);
        [self.tokenCacheStore addOrUpdateItem:cacheItem error:nil];
        cacheItem.refreshToken = savedRefreshToken;//Restore for the result
    }
    else
    {
        if (AD_ERROR_INVALID_REFRESH_TOKEN == result.error.code)
        {//Bad refresh token. Remove it from the cache:
            if(![self handleNilOrEmptyAsResult:cacheItem argumentName:@"cacheItem" authenticationResult:&result]
               || ![self handleNilOrEmptyAsResult:cacheItem.resource argumentName:@"cacheItem.resource" authenticationResult:&result]
               || ![self handleNilOrEmptyAsResult:refreshToken argumentName:@"refreshToken" authenticationResult:&result])
            {
                return;
            }
            
            BOOL removed = NO;
            //The refresh token didn't work. We need to clear this refresh item from the cache.
            ADTokenCacheStoreKey* exactKey = [cacheItem extractKeyWithError:nil];
            if (exactKey)
            {
                ADTokenCacheStoreItem* existing = [self.tokenCacheStore getItemWithKey:exactKey userId:cacheItem.userInformation.userId error:nil];
                if ([refreshToken isEqualToString:existing.refreshToken])//If still there, attempt to remove
                {
                    AD_LOG_VERBOSE_F(@"Token cache store", @"Removing cache for resource: %@", cacheItem.resource);
                    [self.tokenCacheStore removeItemWithKey:exactKey userId:existing.userInformation.userId error:nil];
                    removed = YES;
                }
            }
            
            if (!removed)
            {
                //Now try finding a broad refresh token in the cache and remove it accordingly
                ADTokenCacheStoreKey* broadKey = [ADTokenCacheStoreKey keyWithAuthority:self.authority resource:nil clientId:cacheItem.clientId error:nil];
                if (broadKey)
                {
                    ADTokenCacheStoreItem* broadItem = [self.tokenCacheStore getItemWithKey:broadKey userId:cacheItem.userInformation.userId error:nil];
                    if (broadItem && [refreshToken isEqualToString:broadItem.refreshToken])//Remove if still there
                    {
                        AD_LOG_VERBOSE_F(@"Token cache store", @"Removing multi-resource refresh token for authority: %@", self.authority);
                        [self.tokenCacheStore removeItemWithKey:broadKey userId:cacheItem.userInformation.userId error:nil];
                    }
                }
            }
        }
    }
}

//Obtains an access token from the passed refresh token. If "cacheItem" is passed, updates it with the additional
//information and updates the cache:
-(void) internalAcquireTokenByRefreshToken: (NSString*) refreshToken
                                  clientId: (NSString*) clientId
                                  resource: (NSString*) resource
                                    userId: (NSString*) userId
                                 cacheItem: (ADTokenCacheStoreItem*) cacheItem
                         validateAuthority: (BOOL) validateAuthority
                             correlationId: correlationId
                           completionBlock: (ADAuthenticationCallback)completionBlock
{
    THROW_ON_NIL_ARGUMENT(completionBlock);
    HANDLE_ARGUMENT(refreshToken);
    HANDLE_ARGUMENT(clientId);
    
    AD_LOG_VERBOSE_F(@"Attempting to acquire an access token from refresh token.", @"Resource: %@", resource);
    
    [self updateCorrelationId:&correlationId];
    if (validateAuthority)
    {
        [[ADInstanceDiscovery sharedInstance] validateAuthority:self.authority correlationId:correlationId completionBlock:^(BOOL validated, ADAuthenticationError *error)
         {
             if (error)
             {
                 completionBlock([ADAuthenticationResult resultFromError:error]);
             }
             else
             {
                 [self internalAcquireTokenByRefreshToken:refreshToken
                                                 clientId:clientId
                                                 resource:resource
                                                   userId:userId
                                                cacheItem:cacheItem
                                        validateAuthority:NO /*Already validated in this block. */
                                            correlationId:correlationId
                                          completionBlock:completionBlock];
             }
         }];
        return;//The asynchronous block above will handle everything;
    }
    
    [ADLogger logToken:refreshToken tokenType:@"refresh token" expiresOn:nil correlationId:nil];
    //Fill the data for the token refreshing:
    NSMutableDictionary *request_data = [NSMutableDictionary dictionaryWithObjectsAndKeys:
                                         OAUTH2_REFRESH_TOKEN, OAUTH2_GRANT_TYPE,
                                         refreshToken, OAUTH2_REFRESH_TOKEN,
                                         clientId, OAUTH2_CLIENT_ID,
                                         nil];
    
    //The clang analyzer has some issues with the logic inside adIsStringNilOrBlank, as it is defined in a category.
#ifndef __clang_analyzer__
    if (![NSString adIsStringNilOrBlank:resource])
#else
        if (resource && ![NSString adIsStringNilOrBlank:resource])
#endif
        {
            [request_data setObject:resource forKey:OAUTH2_RESOURCE];
        }
    
    dispatch_async(dispatch_get_global_queue(DISPATCH_QUEUE_PRIORITY_HIGH, 0), ^
                   {
                       AD_LOG_INFO_F(@"Sending request for refreshing token.", @"Client id: '%@'; resource: '%@';", clientId, resource);
                       [self request:self.authority
                         requestData:request_data
                requestCorrelationId:correlationId
         isHandlingPKeyAuthChallenge:FALSE
                   additionalHeaders:nil
                          completion:^(NSDictionary *response)
                        {
                            ADTokenCacheStoreItem* resultItem = (cacheItem) ? cacheItem : [ADTokenCacheStoreItem new];
                            
                            //Always ensure that the cache item has all of these set, especially in the broad token case, where the passed item
                            //may have empty "resource" property:
                            resultItem.resource = resource;
                            resultItem.clientId = clientId;
                            resultItem.authority = self.authority;
                            
                            
                            ADAuthenticationResult *result = [self processTokenResponse:response forItem:resultItem fromRefresh:YES requestCorrelationId:correlationId];
                            if (cacheItem)//The request came from the cache item, update it:
                            {
                                [self updateCacheToResult:result
                                                cacheItem:resultItem
                                         withRefreshToken:refreshToken];
                            }
                            result = [self updateResult:result toUser:userId];//Verify the user (just in case)
                            
                            completionBlock(result);
                        }];
                   });
}

//Used in the flows, where developer requested an explicit user. The method compares
//the user for the obtained tokens (if provided by the server). If the user is different,
//an error result is returned. Returns the same result, if no issues are found.
-(ADAuthenticationResult*) updateResult: (ADAuthenticationResult*) result
                                 toUser: (NSString*) userId
{
    if (!result)
    {
        ADAuthenticationError* error =
        [ADAuthenticationError errorFromAuthenticationError:AD_ERROR_INVALID_ARGUMENT
                                               protocolCode:nil
                                               errorDetails:@"ADAuthenticationResult is nil"];
        return [ADAuthenticationResult resultFromError:error];
    }
    
    userId = [ADUserInformation normalizeUserId:userId];
    NSString* actualUser = result.tokenCacheStoreItem.userInformation.userId;
    if (!userId || AD_SUCCEEDED != result.status || !actualUser)
    {
        //No user to compare - either no specific user id requested, or no specific userId obtained:
        return result;
    }
    
    
    if (![userId isEqualToString:actualUser])
    {
        NSString* errorText = [NSString stringWithFormat:@"Different user was authenticated. Expected: '%@'; Actual: '%@'. Either the user entered credentials for different user, or cookie for different logged user is present. Consider calling acquireToken with AD_PROMPT_ALWAYS to ignore the cookie.",
                               userId, actualUser];
        
        ADAuthenticationError* error =
        [ADAuthenticationError errorFromAuthenticationError:AD_ERROR_WRONG_USER
                                               protocolCode:nil
                                               errorDetails:errorText];
        return [ADAuthenticationResult resultFromError:error];
    }
    
    return result;
}

//Understands and processes the access token response:
- (ADAuthenticationResult *)processTokenResponse: (NSDictionary *)response
                                         forItem: (ADTokenCacheStoreItem*)item
                                     fromRefresh: (BOOL) fromRefreshTokenWorkflow
                            requestCorrelationId: (NSUUID*) requestCorrelationId
{
    THROW_ON_NIL_ARGUMENT(response);
    THROW_ON_NIL_ARGUMENT(item);
    AD_LOG_VERBOSE(@"Token extraction", @"Attempt to extract the data from the server response.");
    
    NSString* responseId = [response objectForKey:OAUTH2_CORRELATION_ID_RESPONSE];
    NSUUID* responseUUID;
    if (![NSString adIsStringNilOrBlank:responseId])
    {
        responseUUID = [[NSUUID alloc] initWithUUIDString:responseId];
        if (!responseUUID)
        {
            AD_LOG_INFO_F(@"Bad correlation id", @"The received correlation id is not a valid UUID. Sent: %@; Received: %@", requestCorrelationId, responseId);
        }
        else if (![requestCorrelationId isEqual:responseUUID])
        {
            AD_LOG_INFO_F(@"Correlation id mismatch", @"Mismatch between the sent correlation id and the received one. Sent: %@; Received: %@", requestCorrelationId, responseId);
        }
    }
    else
    {
        AD_LOG_INFO_F(@"Missing correlation id", @"No correlation id received for request with correlation id: %@", [requestCorrelationId UUIDString]);
    }
    
    ADAuthenticationError* error = [self errorFromDictionary:response errorCode:(fromRefreshTokenWorkflow) ? AD_ERROR_INVALID_REFRESH_TOKEN : AD_ERROR_AUTHENTICATION];
    if (error)
    {
        return [ADAuthenticationResult resultFromError:error];
    }
    
    NSString* accessToken = [response objectForKey:OAUTH2_ACCESS_TOKEN];
    if (![NSString adIsStringNilOrBlank:accessToken])
    {
        item.authority = self.authority;
        item.accessToken = accessToken;
        
        // Token response
        id      expires_in = [response objectForKey:@"expires_in"];
        NSDate *expires    = nil;
        
        if ( expires_in != nil )
        {
            if ( [expires_in isKindOfClass:[NSString class]] )
            {
                NSNumberFormatter *formatter = [[NSNumberFormatter alloc] init];
                
                expires = [NSDate dateWithTimeIntervalSinceNow:[formatter numberFromString:expires_in].longValue];
            }
            else if ( [expires_in isKindOfClass:[NSNumber class]] )
            {
                expires = [NSDate dateWithTimeIntervalSinceNow:((NSNumber *)expires_in).longValue];
            }
            else
            {
                AD_LOG_WARN_F(@"Unparsable time", @"The response value for the access token expiration cannot be parsed: %@", expires);
                // Unparseable, use default value
                expires = [NSDate dateWithTimeIntervalSinceNow:3600.0];//1 hour
            }
        }
        else
        {
            AD_LOG_WARN(@"Missing expiration time.", @"The server did not return the expiration time for the access token.");
            expires = [NSDate dateWithTimeIntervalSinceNow:3600.0];//Assume 1hr expiration
        }
        
        item.accessTokenType = [response objectForKey:OAUTH2_TOKEN_TYPE];
        item.expiresOn       = expires;
        [ADLogger logToken:accessToken tokenType:@"access token" expiresOn:expires correlationId:responseUUID];
        
        if([response objectForKey:OAUTH2_REFRESH_TOKEN]){
            item.refreshToken    = [response objectForKey:OAUTH2_REFRESH_TOKEN];
        }
        
        NSString* resource   = [response objectForKey:OAUTH2_RESOURCE];
        BOOL multiResourceRefreshToken = NO;
        if (![NSString adIsStringNilOrBlank:resource])
        {
            if (item.resource && ![item.resource isEqualToString:resource])
            {
                AD_LOG_WARN_F(@"Wrong resource returned by the server.", @"Expected resource: '%@'; Server returned: '%@'", item.resource, resource);
            }
            //Currently, if the server has returned a "resource" parameter and we have a refresh token,
            //this token is a multi-resource refresh token:
            multiResourceRefreshToken = ![NSString adIsStringNilOrBlank:item.refreshToken];
        }
        [ADLogger logToken:item.refreshToken
                 tokenType:multiResourceRefreshToken ? @"multi-resource refresh token": @"refresh token"
                 expiresOn:nil
             correlationId:responseUUID];
        
        NSString* idToken = [response objectForKey:OAUTH2_ID_TOKEN];
        if (idToken)
        {
            ADUserInformation* userInfo = [ADUserInformation userInformationWithIdToken:idToken error:nil];
            if (userInfo)
            {
                item.userInformation = userInfo;
            }
        }
        
        return [ADAuthenticationResult resultFromTokenCacheStoreItem:item multiResourceRefreshToken:multiResourceRefreshToken];
    }
    
    //No access token and no error, we assume that there was another kind of error (connection, server down, etc.).
    //Note that for security reasons we log only the keys, not the values returned by the user:
    NSString* errorMessage = [NSString stringWithFormat:@"The server returned without providing an error. Keys returned: %@", [response allKeys]];
    error = [ADAuthenticationError errorFromAuthenticationError:AD_ERROR_AUTHENTICATION
                                                   protocolCode:nil
                                                   errorDetails:errorMessage];
    return [ADAuthenticationResult resultFromError:error];
}

//Ensures that a single UI login dialog can be requested at a time.
//Returns true if successfully acquired the lock. If not, calls the callback with
//the error and returns false.
-(BOOL) takeExclusionLockWithCallback: (ADAuthorizationCodeCallback) completionBlock
{
    THROW_ON_NIL_ARGUMENT(completionBlock);
    if ( !OSAtomicCompareAndSwapInt( 0, 1, &sDialogInProgress) )
    {
        NSString* message = @"The user is currently prompted for credentials as result of another acquireToken request. Please retry the acquireToken call later.";
        ADAuthenticationError* error = [ADAuthenticationError errorFromAuthenticationError:AD_ERROR_USER_PROMPTED
                                                                              protocolCode:nil
                                                                              errorDetails:message];
        completionBlock(nil, error);
        return NO;
    }
    return YES;
}

//Attempts to release the lock. Logs warning if the lock was already released.
-(void) releaseExclusionLock
{
    if ( !OSAtomicCompareAndSwapInt( 1, 0, &sDialogInProgress) )
    {
        AD_LOG_WARN(@"UI Locking", @"The UI lock has already been released.")
    }
}

//Generates the query string, encoding the state:
-(NSString*) queryStringFromResource: (NSString*) resource
                            clientId: (NSString*) clientId
                         redirectUri: (NSURL*) redirectUri
                               scope: (NSString*) scope /* for future use */
                              userId: (NSString*) userId
                         requestType: (NSString*) requestType
                      promptBehavior: (ADPromptBehavior) promptBehavior
                extraQueryParameters: (NSString*) queryParams
{
    NSString *state    = [self encodeProtocolStateWithResource:resource scope:scope];
    // Start the web navigation process for the Implicit grant profile.
    NSMutableString *startUrl = [NSMutableString stringWithFormat:@"%@?%@=%@&%@=%@&%@=%@&%@=%@&%@=%@",
                                 [self.authority stringByAppendingString:OAUTH2_AUTHORIZE_SUFFIX],
                                 OAUTH2_RESPONSE_TYPE, requestType,
                                 OAUTH2_CLIENT_ID, [clientId adUrlFormEncode],
                                 OAUTH2_RESOURCE, [resource adUrlFormEncode],
                                 OAUTH2_REDIRECT_URI, [[redirectUri absoluteString] adUrlFormEncode],
                                 OAUTH2_STATE, state];
    
    [startUrl appendFormat:@"&%@", [[ADLogger adalId] adURLFormEncode]];
    
    if (![NSString adIsStringNilOrBlank:userId])
    {
        [startUrl appendFormat:@"&%@=%@", OAUTH2_LOGIN_HINT, [userId adUrlFormEncode]];
    }
    NSString* promptParam = [self.class getPromptParameter:promptBehavior];
    if (promptParam)
    {
        //Force the server to ignore cookies, by specifying explicitly the prompt behavior:
        [startUrl appendString:[NSString stringWithFormat:@"&prompt=%@", promptParam]];
    }
    if (![NSString adIsStringNilOrBlank:queryParams])
    {//Append the additional query parameters if specified:
        queryParams = queryParams.adTrimmedString;
        
        //Add the '&' for the additional params if not there already:
        if ([queryParams hasPrefix:@"&"])
        {
            [startUrl appendString:queryParams];
        }
        else
        {
            [startUrl appendFormat:@"&%@", queryParams];
        }
    }
    
    return startUrl;
}

//Obtains a protocol error from the response:
-(ADAuthenticationError*) errorFromDictionary: (NSDictionary*) dictionary
                                    errorCode: (ADErrorCode) errorCode
{
    //First check for explicit OAuth2 protocol error:
    NSString* serverOAuth2Error = [dictionary objectForKey:OAUTH2_ERROR];
    if (![NSString adIsStringNilOrBlank:serverOAuth2Error])
    {
        NSString* errorDetails = [dictionary objectForKey:OAUTH2_ERROR_DESCRIPTION];
        // Error response from the server
        return [ADAuthenticationError errorFromAuthenticationError:errorCode
                                                      protocolCode:serverOAuth2Error
                                                      errorDetails:(errorDetails) ? errorDetails : [NSString stringWithFormat:serverError, serverOAuth2Error]];
    }
    //In the case of more generic error, e.g. server unavailable, DNS error or no internet connection, the error object will be directly placed in the dictionary:
    return [dictionary objectForKey:AUTH_NON_PROTOCOL_ERROR];
}

//Ensures that the state comes back in the response:
-(BOOL) verifyStateFromDictionary: (NSDictionary*) dictionary
{
    NSDictionary *state = [NSDictionary adURLFormDecode:[[dictionary objectForKey:OAUTH2_STATE] adBase64UrlDecode]];
    if (state.count != 0)
    {
        NSString *authorizationServer = [state objectForKey:@"a"];
        NSString *resource            = [state objectForKey:@"r"];
        
        if (![NSString adIsStringNilOrBlank:authorizationServer] && ![NSString adIsStringNilOrBlank:resource])
        {
            AD_LOG_VERBOSE_F(@"State", @"The authorization server returned the following state: %@", state);
            return YES;
        }
    }
    AD_LOG_WARN_F(@"State error", @"Missing or invalid state returned: %@", state);
    return NO;
}

//Requests an OAuth2 code to be used for obtaining a token:
-(void) requestCodeByResource: (NSString*) resource
                     clientId: (NSString*) clientId
                  redirectUri: (NSURL*) redirectUri
                        scope: (NSString*) scope /*for future use */
                       userId: (NSString*) userId
               promptBehavior: (ADPromptBehavior) promptBehavior
         extraQueryParameters: (NSString*) queryParams
                correlationId: (NSUUID*) correlationId
                   completion: (ADAuthorizationCodeCallback) completionBlock
{
    THROW_ON_NIL_ARGUMENT(completionBlock);
    if(!correlationId){
        completionBlock(nil, [ADAuthenticationError errorFromArgument:correlationId argumentName:@"correlationId"]);
        return;
    }
    
    AD_LOG_VERBOSE_F(@"Requesting authorization code.", @"Requesting authorization code for resource: %@", resource);
    if (![self takeExclusionLockWithCallback:completionBlock])
    {
        return;
    }
    
    ADAuthenticationSettings* settings = [ADAuthenticationSettings sharedInstance];
    NSString* startUrl = [self queryStringFromResource:resource
                                              clientId:clientId
                                           redirectUri:redirectUri
                                                 scope:scope
                                                userId:userId
                                           requestType:OAUTH2_CODE
                                        promptBehavior:promptBehavior
                                  extraQueryParameters:queryParams];
    
    [[ADAuthenticationBroker sharedInstance] start:[NSURL URLWithString:startUrl]
                                               end:[NSURL URLWithString:[redirectUri absoluteString]]
                                  parentController:self.parentController
                                           webView:self.webView
                                        fullScreen:settings.enableFullScreen
                                     correlationId:correlationId
                                        completion:^( ADAuthenticationError *error, NSURL *end )
     {
         [self releaseExclusionLock]; // Allow other operations that use the UI for credentials.
         
         NSString* code = nil;
         if (!error)
         {
             //Try both the URL and the fragment parameters:
             NSDictionary *parameters = [end adFragmentParameters];
             if ( parameters.count == 0 )
             {
                 parameters = [end adQueryParameters];
             }
             
             //OAuth2 error may be passed by the server:
             error = [self errorFromDictionary:parameters errorCode:AD_ERROR_AUTHENTICATION];
             if (!error)
             {
                 //Note that we do not enforce the state, just log it:
                 [self verifyStateFromDictionary:parameters];
                 code = [parameters objectForKey:OAUTH2_CODE];
                 if ([NSString adIsStringNilOrBlank:code])
                 {
                     error = [ADAuthenticationError errorFromAuthenticationError:AD_ERROR_AUTHENTICATION
                                                                    protocolCode:nil
                                                                    errorDetails:@"The authorization server did not return a valid authorization code."];
                 }
             }
         }
         
         completionBlock(code, error);
     }];
}



// Generic OAuth2 Authorization Request, obtains a token from a SAML assertion.
- (void)requestTokenByAssertion: (NSString *) samlAssertion
                  assertionType: (ADAssertionType) assertionType
                       resource: (NSString *) resource
                       clientId: (NSString*) clientId
                          scope: (NSString*) scope //For future use
                  correlationId: (NSUUID*) correlationId
                     completion: (ADAuthenticationCallback) completionBlock
{
#pragma unused(scope)
    HANDLE_ARGUMENT(correlationId);//Should be set by the caller
    AD_LOG_VERBOSE_F(@"Requesting token from authorization code.", @"Requesting token by authorization code for resource: %@", resource);
    
    //samlAssertion = [NSString samlAssertion adBase64];
    NSData *encodeData = [samlAssertion dataUsingEncoding:NSUTF8StringEncoding];
    NSString *base64String = [encodeData base64EncodedStringWithOptions:0];
    //Fill the data for the token refreshing:
    NSMutableDictionary *request_data = [NSMutableDictionary dictionaryWithObjectsAndKeys:
                                         [self getAssertionTypeGrantValue:assertionType], OAUTH2_GRANT_TYPE,
                                         base64String, OAUTH2_ASSERTION,
                                         clientId, OAUTH2_CLIENT_ID,
                                         resource, OAUTH2_RESOURCE,
                                         nil];
    [self executeRequest:self.authority requestData:request_data resource:resource clientId:clientId requestCorrelationId:correlationId isHandlingPKeyAuthChallenge:NO additionalHeaders:nil completion:completionBlock];
}

- (NSString*) getAssertionTypeGrantValue:(ADAssertionType) assertionType
{
    if(assertionType == AD_SAML1_1){
        return OAUTH2_SAML11_BEARER_VALUE;
    }
    
    if(assertionType == AD_SAML2){
        return OAUTH2_SAML2_BEARER_VALUE;
    }
    
    return nil;
}

// Generic OAuth2 Authorization Request, obtains a token from an authorization code.
- (void)requestTokenByCode: (NSString *) code
                  resource: (NSString *) resource
                  clientId: (NSString*) clientId
               redirectUri: (NSURL*) redirectUri
                     scope: (NSString*) scope
             correlationId: (NSUUID*) correlationId
                completion: (ADAuthenticationCallback) completionBlock
{
    HANDLE_ARGUMENT(code);
    HANDLE_ARGUMENT(correlationId);//Should be set by the caller
    AD_LOG_VERBOSE_F(@"Requesting token from authorization code.", @"Requesting token by authorization code for resource: %@", resource);
    
    //Fill the data for the token refreshing:
    NSMutableDictionary *request_data = [NSMutableDictionary dictionaryWithObjectsAndKeys:
                                         OAUTH2_AUTHORIZATION_CODE, OAUTH2_GRANT_TYPE,
                                         code, OAUTH2_CODE,
                                         clientId, OAUTH2_CLIENT_ID,
                                         [redirectUri absoluteString], OAUTH2_REDIRECT_URI,
                                         nil];
    
    [self executeRequest:self.authority requestData:request_data resource:resource clientId:clientId requestCorrelationId:correlationId isHandlingPKeyAuthChallenge:NO additionalHeaders:nil completion:completionBlock];
}


-(void) executeRequest:(NSString *)authorizationServer
           requestData:(NSDictionary *)request_data
              resource: (NSString *) resource
              clientId: (NSString*) clientId
  requestCorrelationId: (NSUUID*) requestCorrelationId
isHandlingPKeyAuthChallenge: (BOOL) isHandlingPKeyAuthChallenge
     additionalHeaders:(NSDictionary *)additionalHeaders
            completion: (ADAuthenticationCallback) completionBlock
{
    [self request:authorizationServer
      requestData:request_data
requestCorrelationId:requestCorrelationId
isHandlingPKeyAuthChallenge:isHandlingPKeyAuthChallenge
additionalHeaders:additionalHeaders
       completion:^(NSDictionary *response)
     {
         //Prefill the known elements in the item. These can be overridden by the response:
         ADTokenCacheStoreItem* item = [ADTokenCacheStoreItem new];
         item.resource = resource;
         item.clientId = clientId;
         completionBlock([self processTokenResponse:response forItem:item fromRefresh:NO requestCorrelationId:requestCorrelationId]);
     }];
}


// Performs an OAuth2 token request using the supplied request dictionary and executes the completion block
// If the request generates an HTTP error, the method adds details to the "error" parameters of the dictionary.
- (void)request:(NSString *)authorizationServer
    requestData:(NSDictionary *)request_data
requestCorrelationId: (NSUUID*) requestCorrelationId
isHandlingPKeyAuthChallenge: (BOOL) isHandlingPKeyAuthChallenge
additionalHeaders:(NSDictionary *)additionalHeaders
     completion:( void (^)(NSDictionary *) )completionBlock
{
    NSString* endPoint = authorizationServer;
    
    if(!isHandlingPKeyAuthChallenge){
        endPoint = [authorizationServer stringByAppendingString:OAUTH2_TOKEN_SUFFIX];
    }
    
    ADWebRequest *webRequest = [[ADWebRequest alloc] initWithURL:[NSURL URLWithString:endPoint]
                                                   correlationId:requestCorrelationId];
    
    webRequest.method = HTTPPost;
    [webRequest.headers setObject:@"application/json" forKey:@"Accept"];
    [webRequest.headers setObject:@"application/x-www-form-urlencoded" forKey:@"Content-Type"];
    [webRequest.headers setObject:pKeyAuthHeaderVersion forKey:pKeyAuthHeader];    
    if(additionalHeaders){
        for (NSString* key in [additionalHeaders allKeys] ) {
            [webRequest.headers setObject:[additionalHeaders objectForKey:key ] forKey:key];
        }
    }
    
    AD_LOG_VERBOSE_F(@"Post request", @"Sending POST request to %@ with client-request-id %@", endPoint, [requestCorrelationId UUIDString]);
    
    webRequest.body = [[request_data adURLFormEncode] dataUsingEncoding:NSUTF8StringEncoding];
    [[ADClientMetrics getInstance] beginClientMetricsRecordForEndpoint:endPoint correlationId:[requestCorrelationId UUIDString] requestHeader:webRequest.headers];
    
    [webRequest send:^( NSError *error, ADWebResponse *webResponse ) {
        // Request completion callback
        NSMutableDictionary *response = [NSMutableDictionary new];
        
        if ( error == nil )
        {
            NSDictionary* headers = webResponse.headers;
            //In most cases the correlation id is returned as a separate header
            NSString* responseCorrelationId = [headers objectForKey:OAUTH2_CORRELATION_ID_REQUEST_VALUE];
            if (![NSString adIsStringNilOrBlank:responseCorrelationId])
            {
                [response setObject:responseCorrelationId forKey:OAUTH2_CORRELATION_ID_RESPONSE];//Add it to the dictionary to be logged and checked later.
            }
            
            switch (webResponse.statusCode)
            {
                case 200:
                case 400:
                case 401:
                {
                    if(!isHandlingPKeyAuthChallenge){
                        NSString* wwwAuthValue = [headers valueForKey:wwwAuthenticateHeader];
                        if(![NSString adIsStringNilOrBlank:wwwAuthValue] && [wwwAuthValue adContainsString:pKeyAuthName]){
                            [self handlePKeyAuthChallenge:endPoint wwwAuthHeaderValue:wwwAuthValue requestData:request_data requestCorrelationId:requestCorrelationId completion:completionBlock];
                            return;
                        }
                    }
                    NSError   *jsonError  = nil;
                    id         jsonObject = [NSJSONSerialization JSONObjectWithData:webResponse.body options:0 error:&jsonError];
                    
                    if ( nil != jsonObject && [jsonObject isKindOfClass:[NSDictionary class]] )
                    {
                        // Load the response
                        [response addEntriesFromDictionary:(NSDictionary*)jsonObject];
                    }
                    else
                    {
                        ADAuthenticationError* adError;
                        if (jsonError)
                        {
                            // Unrecognized JSON response
                            NSString* bodyStr = [[NSString alloc] initWithData:webResponse.body encoding:NSUTF8StringEncoding];
                            AD_LOG_ERROR_F(@"JSON deserialization", jsonError.code, @"Error: %@. Body text: '%@'. HTTPS Code: %ld. Response correlation id: %@", jsonError.description, bodyStr, (long)webResponse.statusCode, responseCorrelationId);
                            adError = [ADAuthenticationError errorFromNSError:jsonError errorDetails:jsonError.localizedDescription];
                        }
                        else
                        {
                            adError = [ADAuthenticationError unexpectedInternalError:[NSString stringWithFormat:@"Unexpected object type: %@", [jsonObject class]]];
                        }
                        [response setObject:adError forKey:AUTH_NON_PROTOCOL_ERROR];
                    }
                }
                    break;
                default:
                {
                    // Request failure
                    NSString* body = [[NSString alloc] initWithData:webResponse.body encoding:NSUTF8StringEncoding];
                    NSString* errorData = [NSString stringWithFormat:@"Server HTTP status code: %ld. Full response %@", (long)webResponse.statusCode, body];
                    AD_LOG_WARN(@"HTTP Error", errorData);
                    
                    //Now add the information to the dictionary, so that the parser can extract it:
                    [response setObject:[ADAuthenticationError errorFromAuthenticationError:AD_ERROR_AUTHENTICATION protocolCode:@(webResponse.statusCode).stringValue errorDetails:errorData]
                                 forKey:AUTH_NON_PROTOCOL_ERROR];
                }
            }
        }
        else
        {
            AD_LOG_WARN(@"System error while making request.", error.description);
            // System error
            [response setObject:[ADAuthenticationError errorFromNSError:error errorDetails:error.localizedDescription]
                         forKey:AUTH_NON_PROTOCOL_ERROR];
        }
        
        if([response valueForKey:AUTH_NON_PROTOCOL_ERROR]){
            [[ADClientMetrics getInstance] endClientMetricsRecord:[[response valueForKey:AUTH_NON_PROTOCOL_ERROR] errorDetails]];
        }
        else
        {
            [[ADClientMetrics getInstance] endClientMetricsRecord:nil];
        }
        
        completionBlock( response );
    }];
}

// Encodes the state parameter for a protocol message
- (NSString *)encodeProtocolStateWithResource:(NSString *)resource scope:(NSString *)scope
{
    return [[[NSMutableDictionary dictionaryWithObjectsAndKeys:self.authority, @"a", resource, @"r", scope, @"s", nil]
             adURLFormEncode] adBase64UrlEncode];
}


- (void) handlePKeyAuthChallenge:(NSString *)authorizationServer
              wwwAuthHeaderValue:(NSString *)wwwAuthHeaderValue
                     requestData:(NSDictionary *)request_data
            requestCorrelationId: (NSUUID*) requestCorrelationId
                      completion:( void (^)(NSDictionary *) )completionBlock
{
    //pkeyauth word length=8 + 1 whitespace
    wwwAuthHeaderValue = [wwwAuthHeaderValue substringFromIndex:[pKeyAuthName length] + 1];
    wwwAuthHeaderValue = [wwwAuthHeaderValue stringByReplacingOccurrencesOfString:@"\""
                                                                       withString:@""];
    NSArray* headerPairs = [wwwAuthHeaderValue componentsSeparatedByString:@","];
    NSMutableDictionary* headerKeyValuePair = [[NSMutableDictionary alloc]init];
    for(int i=0; i<[headerPairs count]; ++i) {
        NSArray* pair = [headerPairs[i] componentsSeparatedByString:@"="];
        [headerKeyValuePair setValue:pair[1] forKey:[pair[0] adTrimmedString]];
    }
    NSString* authHeader = [ADPkeyAuthHelper createDeviceAuthResponse:authorizationServer challengeData:headerKeyValuePair challengeType:AD_THUMBPRINT];
    [headerKeyValuePair removeAllObjects];
    [headerKeyValuePair setObject:authHeader forKey:@"Authorization"];
    
    [self request:authorizationServer requestData:request_data requestCorrelationId:requestCorrelationId isHandlingPKeyAuthChallenge:TRUE additionalHeaders:headerKeyValuePair completion:completionBlock];
}

@end

