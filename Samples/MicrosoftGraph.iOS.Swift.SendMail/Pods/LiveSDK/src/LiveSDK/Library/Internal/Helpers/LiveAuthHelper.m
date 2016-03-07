//
//  LiveAuthHelper.m
//  Live SDK for iOS
//
//  Copyright 2015 Microsoft Corporation
//
//  Permission is hereby granted, free of charge, to any person obtaining a copy
//  of this software and associated documentation files (the "Software"), to deal
//  in the Software without restriction, including without limitation the rights
//  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//  copies of the Software, and to permit persons to whom the Software is
//  furnished to do so, subject to the following conditions:
//  
//  The above copyright notice and this permission notice shall be included in
//  all copies or substantial portions of the Software.
//  
//  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
//  THE SOFTWARE.
//

#import <UIKit/UIKit.h>
#import "LiveAuthHelper.h"
#import "LiveConstants.h"
#import "LiveConnectSession.h"
#import "JsonParser.h"
#import "StringHelper.h"
#import "UrlHelper.h"

NSString * LIVE_ENDPOINT_API_HOST = @"apis.live.net";
NSString * LIVE_ENDPOINT_LOGIN_HOST = @"login.live.com";

@implementation LiveAuthHelper

+ (NSBundle *) getSDKBundle
{
    NSString *sdkPath = [[NSBundle mainBundle] pathForResource:@"LiveSDK"
                                                        ofType:@"framework"];
    return (sdkPath)? [NSBundle bundleWithPath:sdkPath] : [NSBundle mainBundle];
}

+ (UIImage *) getBackButtonImage
{
    NSString * path = [[NSBundle mainBundle] pathForResource:@"LiveSDK.framework/Resources/backArrow_black"
                                                      ofType:@"png"];
    if (path) {
        return [UIImage imageWithContentsOfFile:path];
    }
    else
    {
        return [UIImage imageNamed:@"backArrow_black"];
    }
}

+ (NSArray *) normalizeScopes:(NSArray *)scopes
{
    NSMutableArray *normalScopes = [NSMutableArray array];
    for (NSUInteger i = 0; i < scopes.count; i++) 
    {
        NSString *scope = [scopes objectAtIndex:i];
        if (![StringHelper isNullOrEmpty:scope])
        {
            [normalScopes addObject: [scope lowercaseString]];
        }
    }
    
    return normalScopes;
}

+ (BOOL) isScopes:(NSArray *)scopes1
         subSetOf:(NSArray *)scopes2
{
    return [[NSSet setWithArray:scopes1] isSubsetOfSet:[NSSet setWithArray:scopes2]];
}

+ (BOOL) isiPad
{
    return [[[UIDevice currentDevice] model] hasPrefix:@"iPad"];
}

+ (NSString *) getAuthorizeUrl
{
    return [NSString stringWithFormat:@"https://%@/oauth20_authorize.srf", LIVE_ENDPOINT_LOGIN_HOST];
}

+ (NSURL *) getRetrieveTokenUrl
{
    return [NSURL URLWithString: [NSString stringWithFormat:@"https://%@/oauth20_token.srf", LIVE_ENDPOINT_LOGIN_HOST]];
}

+ (NSString *) getDefaultRedirectUrlString
{
    return [NSString stringWithFormat:@"https://%@/oauth20_desktop.srf", LIVE_ENDPOINT_LOGIN_HOST];
}

+ (NSString *) getAuthDisplayValue
{
    return [LiveAuthHelper isiPad] ? LIVE_AUTH_DISPLAY_IOS_TABLET : LIVE_AUTH_DISPLAY_IOS_PHONE;
}

+ (NSURL *) buildAuthUrlWithClientId:(NSString *)clientId
                         redirectUri:(NSString *)redirectUri
                              scopes:(NSArray *)scopes
{
    NSString *language = [[NSLocale preferredLanguages] objectAtIndex:0];
    NSString * scopesString = [scopes componentsJoinedByString:@" "];
    NSMutableDictionary *parameters = [NSMutableDictionary dictionaryWithObjectsAndKeys:
                                              clientId, LIVE_AUTH_CLIENTID,
                                        LIVE_AUTH_CODE, LIVE_AUTH_RESPONSE_TYPE,
                                           redirectUri, LIVE_AUTH_REDIRECT_URI,
                  [LiveAuthHelper getAuthDisplayValue], LIVE_AUTH_DISPLAY,
                                          scopesString, LIVE_AUTH_SCOPE,
                                              language, LIVE_AUTH_LOCALE,
                                                   nil];
    
    return [UrlHelper constructUrl:[LiveAuthHelper getAuthorizeUrl] params:parameters];
}

+ (NSData *) buildGetTokenBodyDataWithClientId:(NSString *)clientId
                                   redirectUri:(NSString *)redirectUri
                                      authCode:(NSString *)authCode
{
    NSMutableDictionary *parameters = [NSMutableDictionary dictionaryWithObjectsAndKeys:
                                                            clientId, LIVE_AUTH_CLIENTID,
                                                            authCode, LIVE_AUTH_CODE,
                                                         redirectUri, LIVE_AUTH_REDIRECT_URI,
                                       LIVE_AUTH_GRANT_TYPE_AUTHCODE, LIVE_AUTH_GRANT_TYPE,                                                                      
                                                                 nil];
    
    NSString *bodyString =  [UrlHelper encodeUrlParameters:parameters];
    
    return [bodyString dataUsingEncoding:NSUTF8StringEncoding];
}

+ (NSData *) buildRefreshTokenBodyDataWithClientId:(NSString *)clientId
                                      refreshToken:(NSString *)refreshToken
                                             scope:(NSArray *)scopes
{
    NSString * scopesString = [scopes componentsJoinedByString:@" "];
    NSMutableDictionary *parameters = [NSMutableDictionary dictionaryWithObjectsAndKeys:
                                                      clientId, LIVE_AUTH_CLIENTID,
                                                  refreshToken, LIVE_AUTH_REFRESH_TOKEN,
                                                  scopesString, LIVE_AUTH_SCOPE,
                                       LIVE_AUTH_REFRESH_TOKEN, LIVE_AUTH_GRANT_TYPE,                                    
                                       nil];
    
    NSString *bodyString =  [UrlHelper encodeUrlParameters:parameters];
    
    return [bodyString dataUsingEncoding:NSUTF8StringEncoding];
}

+ (NSError *) createAuthError:(NSInteger)code
                         info:(NSDictionary *)info
{
    return [NSError errorWithDomain:LIVE_ERROR_DOMAIN 
                               code:LIVE_ERROR_CODE_LOGIN_FAILED 
                           userInfo:info];
}

+ (NSError *) createAuthError:(NSInteger)code
                     errorStr:(NSString *)errorStr
                  description:(NSString *)description
                   innerError:(NSError *)innerError
{
    return [LiveAuthHelper createAuthError:code
                                      info:[NSDictionary dictionaryWithObjectsAndKeys:
                                                 errorStr, LIVE_ERROR_KEY_ERROR,
                                              description, LIVE_ERROR_KEY_DESCRIPTION,
                                               innerError, LIVE_ERROR_KEY_INNER_ERROR,
                                                      nil]];
}

+ (id) readAuthResponse:(NSData *)data
{
    NSString* responseString = [[[NSString alloc] initWithData:data
                                                     encoding:NSUTF8StringEncoding]
                                autorelease];
    NSError *error = nil;
    NSDictionary *params = [MSJSONParser parseText:responseString 
                                             error:&error ];
    
    NSString * accessToken = [params valueForKey:LIVE_AUTH_ACCESS_TOKEN];
    if (accessToken != nil) 
    {
        NSString *authenticationToken = [params valueForKey: LIVE_AUTH_AUTHENTICATION_TOKEN];
        NSString *refreshToken = [params valueForKey: LIVE_AUTH_REFRESH_TOKEN];
        NSArray *scopes = [[params valueForKey:LIVE_AUTH_SCOPE] componentsSeparatedByString:@" "]; 
        NSString *expiresInStr = [params valueForKey:LIVE_AUTH_EXPIRES_IN];
        NSTimeInterval expiresIn = [expiresInStr doubleValue];
        NSDate *expires = [NSDate dateWithTimeIntervalSinceNow:expiresIn];
        
        LiveConnectSession *session = [[[LiveConnectSession alloc] initWithAccessToken:accessToken 
                                                                   authenticationToken:authenticationToken 
                                                                          refreshToken:refreshToken 
                                                                                scopes:scopes 
                                                                               expires:expires]
                                       autorelease];
        return session;
    }
    else 
    {
        if (error == nil)
        {
            return [LiveAuthHelper createAuthError:LIVE_ERROR_CODE_LOGIN_FAILED
                                              info:params];
        }
        else
        {
            return [LiveAuthHelper createAuthError:LIVE_ERROR_CODE_RETRIEVE_TOKEN_FAILED
                                          errorStr:LIVE_ERROR_CODE_S_RESPONSE_PARSING_FAILED 
                                       description:[NSString stringWithFormat:@"Unable to read response: %@", responseString]
                                        innerError:error];
        }
    }
}

+ (void) clearCookieForUrl:(NSString *)url
{
    NSHTTPCookieStorage* cookies = [NSHTTPCookieStorage sharedHTTPCookieStorage];
    NSArray* liveCookies = [cookies cookiesForURL:[NSURL URLWithString:url]];
    for (NSHTTPCookie* cookie in liveCookies) 
    {
        [cookies deleteCookie:cookie];
    }
}

+ (void) clearAuthCookie
{
    NSString *httpFormat =  @"http://%@";
    NSString *httpsFormat =  @"https://%@";
    NSMutableArray *authEndpoints = [NSMutableArray arrayWithObjects: 
                                     [NSString stringWithFormat: httpFormat, LIVE_ENDPOINT_LOGIN_HOST], 
                                     [NSString stringWithFormat: httpsFormat, LIVE_ENDPOINT_LOGIN_HOST],
                                     nil];
    
    for (NSString* authUrl in authEndpoints) 
    {
        [LiveAuthHelper clearCookieForUrl: authUrl];
    }
}

+ (BOOL) isSessionValid:(LiveConnectSession *)session
{
    // We give 3 seconds to allow for potential network transmission delay.
    return ([session.expires timeIntervalSinceNow] >= LIVE_AUTH_EXPIRE_VALUE_ADJUSTMENT);   
}

+ (BOOL) shouldRefreshToken:(LiveConnectSession *)session
               refreshToken:(NSString *)refreshToken
{
    BOOL hasRefreshToken = ![StringHelper isNullOrEmpty:refreshToken];
    if (session != nil) 
    {
        
        return hasRefreshToken && ([session.expires timeIntervalSinceNow] < LIVE_AUTH_REFRESH_TIME_BEFORE_EXPIRE);
    }
    else
    {
        // We have refresh token but no access token, we should get one.
        return hasRefreshToken;
    }
}

+ (void) overrideLoginServer:(NSString *)loginServer
                  apiServer:(NSString *)apiServer

{
    LIVE_ENDPOINT_LOGIN_HOST = loginServer;
    LIVE_ENDPOINT_API_HOST = apiServer;
}

@end
