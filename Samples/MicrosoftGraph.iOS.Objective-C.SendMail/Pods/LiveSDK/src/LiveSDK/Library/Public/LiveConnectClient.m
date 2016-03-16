//
//  LiveConnectClient.m
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


#import "LiveApiHelper.h"
#import "LiveAuthHelper.h"
#import "LiveConnectClient.h"
#import "LiveConnectClientCore.h"
#import "LiveConstants.h"
#import "JsonWriter.h"
#import "StringHelper.h"
#import "UrlHelper.h"

@interface LiveConnectClient()
{
@private
    LiveConnectClientCore *_liveClientCore;
}

@end

@implementation LiveConnectClient

- (id) initWithClientId:(NSString *)clientId
               delegate:(id<LiveAuthDelegate>)delegate
{
    return [self initWithClientId:clientId 
                         delegate:delegate 
                        userState:nil];
}

- (id) initWithClientId:(NSString *)clientId
               delegate:(id<LiveAuthDelegate>)delegate
              userState:(id) userState
{    
    return [self initWithClientId:clientId
                           scopes:nil
                         delegate:delegate
                        userState:userState];
}

- (id) initWithClientId:(NSString *)clientId
                 scopes:(NSArray *)scopes
               delegate:(id<LiveAuthDelegate>)delegate
{
    return [self initWithClientId:clientId  
                           scopes:scopes 
                         delegate:delegate 
                        userState:nil];
}

- (id) initWithClientId:(NSString *)clientId
                 scopes:(NSArray *)scopes
               delegate:(id<LiveAuthDelegate>)delegate
              userState:(id)userState
{
    if ([StringHelper isNullOrEmpty:clientId])
    {
        [NSException raise:NSInvalidArgumentException format:LIVE_ERROR_DESC_MISSING_PARAMETER, @"clientId", @"initWithClientId:redirectUri:scopes:delegate:userState"];
    }
    
    if (_liveClientCore) 
    {
        // We already initialized, so silently ignore it.
        return self;
    }
    
    self = [super init];
    if (self) 
    {
        _liveClientCore = [[LiveConnectClientCore alloc] initWithClientId:clientId 
                                                                   scopes:[LiveAuthHelper normalizeScopes:scopes] 
                                                                 delegate:delegate 
                                                                userState:userState];
    }
    
    return self;
}

- (void)dealloc
{
    [_liveClientCore release];
    
    [super dealloc];
}

#pragma mark Parameter validation

- (void) validateInit
{
    if (_liveClientCore == nil || 
        _liveClientCore.clientId == nil)
    {
        [NSException raise:LIVE_EXCEPTION format:LIVE_ERROR_DESC_MUST_INIT];
    }
}

- (void) validateRequiredParam:(id)value
                     paramName:(NSString *)name
                    methodName:(NSString *)methodName
{
    if (value == nil)
    {
        [NSException raise:NSInvalidArgumentException 
                    format:LIVE_ERROR_DESC_MISSING_PARAMETER, name, methodName];
    }
}

- (void) validateRequiredDictionaryParam:(NSDictionary *)value
                               paramName:(NSString *)name
                              methodName:(NSString *)methodName
{
    if (value == nil || value.count == 0)
    {
        [NSException raise:NSInvalidArgumentException 
                    format:LIVE_ERROR_DESC_MISSING_PARAMETER, name, methodName];
    }
}

- (void) validateStringParam:(NSString *)value
                   paramName:(NSString *)name
                  methodName:(NSString *)methodName
{
    if ([StringHelper isNullOrEmpty:value])
    {
        [NSException raise:NSInvalidArgumentException 
                    format:LIVE_ERROR_DESC_MISSING_PARAMETER, name, methodName];
    }
}

- (void) validatePath:(NSString *)path
           methodName:(NSString *)methodName
             relative:(BOOL)relative
{
    [self validateStringParam:path paramName:@"path" methodName:methodName];
    if (relative && [UrlHelper isFullUrl:path]) 
    {
        [NSException raise:NSInvalidArgumentException 
                    format:LIVE_ERROR_DESC_REQUIRE_RELATIVE_PATH, methodName];
    }
}

- (void) validateCopyMoveDestination:(NSString *)destination
                          methodName:(NSString *)methodName
{
    [self validateStringParam:destination paramName:@"destination" methodName:methodName];
}

#pragma mark Auth members

- (LiveConnectSession *) session 
{
    [self validateInit];
    return _liveClientCore.session;
}

- (void) login:(UIViewController *) currentViewController
      delegate:(id<LiveAuthDelegate>) delegate
{
    [self login:currentViewController delegate:delegate userState:nil];
}

- (void) login:(UIViewController *) currentViewController
      delegate:(id<LiveAuthDelegate>) delegate
     userState:(id) userState
{
    [self login:currentViewController scopes:nil delegate:delegate userState:userState];
}

- (void) login:(UIViewController *) currentViewController
        scopes:(NSArray *) scopes
      delegate:(id<LiveAuthDelegate>) delegate
{
    [self login:currentViewController scopes:scopes delegate:delegate userState:nil];
}

- (void) login:(UIViewController *) currentViewController
        scopes:(NSArray *) scopes
      delegate:(id<LiveAuthDelegate>) delegate
     userState:(id) userState
{
    [self validateInit];
    
    if (_liveClientCore.hasPendingUIRequest) 
    {
        [NSException raise:LIVE_EXCEPTION format:LIVE_ERROR_DESC_PENDING_LOGIN_EXIST];
    }    
    
    if (currentViewController == nil)
    {
        [NSException raise:NSInvalidArgumentException 
                    format:LIVE_ERROR_DESC_MISSING_PARAMETER, @"currentViewController", @"login:scopes:delegate:userState:"];
    }
    
    scopes = [LiveAuthHelper normalizeScopes:scopes];
    if (scopes.count == 0)
    {
        // scopes is not provided, then use the default scopes.
        scopes = _liveClientCore.scopes;
        if (scopes.count == 0) 
        {
            // Neither init nor login has scopes, raise error.
            [NSException raise:NSInvalidArgumentException 
                        format:LIVE_ERROR_DESC_MISSING_PARAMETER, @"scopes", @"login:scopes:delegate:userState:"];
        }
    }
    
    [_liveClientCore login:currentViewController 
                    scopes:scopes 
                  delegate:delegate 
                 userState:userState];
}

- (void) logout
{
    [self logoutWithDelegate:nil 
                   userState:nil];
}

- (void) logoutWithDelegate:(id<LiveAuthDelegate>)delegate
                  userState:(id)userState
{
    [self validateInit];
    
    [_liveClientCore logoutWithDelegate:delegate 
                              userState:userState];
}

#pragma mark API methods
- (LiveOperation *) getWithPath:(NSString *)path
                       delegate:(id <LiveOperationDelegate>)delegate
{
    return [self getWithPath:path delegate:delegate userState:nil];
}

- (LiveOperation *) getWithPath:(NSString *)path
                       delegate:(id <LiveOperationDelegate>)delegate
                      userState:(id) userState
{
    [self validateInit];
    [self validatePath:path
            methodName:@"getWithPath:delegate:userState:"
              relative:YES];
    
    return [_liveClientCore sendRequestWithMethod:@"GET" 
                                             path:path 
                                         jsonBody:nil 
                                         delegate:delegate 
                                        userState:userState];
}

- (LiveOperation *) deleteWithPath:(NSString *)path
                          delegate:(id <LiveOperationDelegate>)delegate
{
    return [self deleteWithPath:path delegate:delegate userState:nil];
}

- (LiveOperation *) deleteWithPath:(NSString *)path
                          delegate:(id <LiveOperationDelegate>)delegate
                         userState:(id) userState
{
    [self validateInit];
    [self validatePath:path
            methodName:@"deleteWithPath:delegate:userState:"
              relative:YES];
    
    return [_liveClientCore sendRequestWithMethod:@"DELETE" 
                                             path:path 
                                         jsonBody:nil 
                                         delegate:delegate 
                                        userState:userState];
}

- (LiveOperation *) putWithPath:(NSString *)path
                       jsonBody:(NSString *)jsonBody
                       delegate:(id <LiveOperationDelegate>)delegate
{
    return [self putWithPath:path jsonBody:jsonBody delegate:delegate userState:nil];
}

- (LiveOperation *) putWithPath:(NSString *)path
                       jsonBody:(NSString *)jsonBody
                       delegate:(id <LiveOperationDelegate>)delegate
                      userState:(id) userState
{
    [self validateInit];
    [self validatePath:path
            methodName:@"putWithPath:jsonBody:delegate:userState:"
              relative:YES];
   
    [self validateRequiredParam:jsonBody 
                      paramName:@"jsonBody" 
                     methodName:@"putWithPath:jsonBody:delegate:userState:"];
    
    return [_liveClientCore sendRequestWithMethod:@"PUT" 
                                             path:path 
                                         jsonBody:jsonBody 
                                         delegate:delegate 
                                        userState:userState];
}

- (LiveOperation *) putWithPath:(NSString *)path
                       dictBody:(NSDictionary *)dictBody
                       delegate:(id <LiveOperationDelegate>)delegate
{
    return [self putWithPath:path dictBody:dictBody delegate:delegate userState:nil];
}

- (LiveOperation *) putWithPath:(NSString *)path
                       dictBody:(NSDictionary *)dictBody
                       delegate:(id <LiveOperationDelegate>)delegate
                      userState:(id) userState
{
    [self validateInit];
    [self validatePath:path
            methodName:@"putWithPath:dictBody:delegate:userState:"
              relative:YES];

    [self validateRequiredDictionaryParam:dictBody
                                paramName:@"dictBody"
                               methodName:@"putWithPath:dictBody:delegate:userState:"];
    
    return [_liveClientCore sendRequestWithMethod:@"PUT" 
                                             path:path 
                                         jsonBody:[MSJSONWriter textForValue:dictBody] 
                                         delegate:delegate 
                                        userState:userState];
}

- (LiveOperation *) postWithPath:(NSString *)path
                        jsonBody:(NSString *)jsonBody
                        delegate:(id <LiveOperationDelegate>)delegate
{
    return [self postWithPath:path jsonBody:jsonBody delegate:delegate userState:nil];
}

- (LiveOperation *) postWithPath:(NSString *)path
                        jsonBody:(NSString *)jsonBody
                        delegate:(id <LiveOperationDelegate>)delegate
                       userState:(id) userState
{
    [self validateInit];
    [self validatePath:path
            methodName:@"postWithPath:jsonBody:delegate:userState:"
              relative:YES];
 
    [self validateRequiredParam:jsonBody 
                      paramName:@"jsonBody" 
                     methodName:@"postWithPath:jsonBody:delegate:userState:"];
    
    return [_liveClientCore sendRequestWithMethod:@"POST" 
                                             path:path 
                                         jsonBody:jsonBody 
                                         delegate:delegate 
                                        userState:userState];    
}

- (LiveOperation *) postWithPath:(NSString *)path
                        dictBody:(NSDictionary *)dictBody
                        delegate:(id <LiveOperationDelegate>)delegate
{
    return [self postWithPath:path dictBody:dictBody delegate:delegate userState:nil];
}

- (LiveOperation *) postWithPath:(NSString *)path
                        dictBody:(NSDictionary *)dictBody
                        delegate:(id <LiveOperationDelegate>)delegate
                       userState:(id) userState
{
    [self validateInit];
    [self validatePath:path
            methodName:@"postWithPath:dictBody:delegate:userState:"
              relative:YES];

    [self validateRequiredDictionaryParam:dictBody
                                paramName:@"dictBody"
                               methodName:@"postWithPath:dictBody:delegate:userState:"];
    
    return [_liveClientCore sendRequestWithMethod:@"POST" 
                                             path:path 
                                         jsonBody:[MSJSONWriter textForValue:dictBody] 
                                         delegate:delegate 
                                        userState:userState];    
}

- (LiveOperation *) moveFromPath:(NSString *)path
                   toDestination:(NSString *)destination
                        delegate:(id <LiveOperationDelegate>)delegate
{
    return [self moveFromPath:path toDestination:destination delegate:delegate userState:nil];
}

- (LiveOperation *) moveFromPath:(NSString *)path
                   toDestination:(NSString *)destination
                        delegate:(id <LiveOperationDelegate>)delegate
                       userState:(id) userState
{
    [self validateInit];
    [self validatePath:path
            methodName:@"moveFromPath:toDestination:delegate:userState:"
              relative:YES];
 
    [self validateCopyMoveDestination:destination 
                           methodName:@"moveFromPath:toDestination:delegate:userState:"];
    
    return [_liveClientCore sendRequestWithMethod:@"MOVE" 
                                             path:path 
                                         jsonBody:[LiveApiHelper buildCopyMoveBody:destination]
                                         delegate:delegate 
                                        userState:userState];
}

- (LiveOperation *) copyFromPath:(NSString *)path
                   toDestination:(NSString *)destination
                        delegate:(id <LiveOperationDelegate>)delegate
{
    return [self copyFromPath:path toDestination:destination delegate:delegate userState:nil];
}

- (LiveOperation *) copyFromPath:(NSString *)path
                   toDestination:(NSString *)destination
                        delegate:(id <LiveOperationDelegate>)delegate
                       userState:(id)userState
{
    [self validateInit];
    [self validatePath:path
            methodName:@"copyFromPath:toDestination:delegate:userState:"
              relative:YES];
  
    [self validateCopyMoveDestination:destination 
                           methodName:@"copyFromPath:toDestination:delegate:userState:"];
    
    return [_liveClientCore sendRequestWithMethod:@"COPY" 
                                             path:path 
                                         jsonBody:[LiveApiHelper buildCopyMoveBody:destination]
                                         delegate:delegate 
                                        userState:userState];
}

- (LiveDownloadOperation *) downloadFromPath:(NSString *)path
                                    delegate:(id <LiveDownloadOperationDelegate>)delegate
{
    return [self downloadFromPath:path delegate:delegate userState:nil];
}

- (LiveDownloadOperation *) downloadFromPath:(NSString *)path
                                    delegate:(id <LiveDownloadOperationDelegate>)delegate
                                   userState:(id)userState
{
    [self validateInit];
    [self validatePath:path
            methodName:@"downloadFromPath:delegate:userState:"
              relative:NO];

    
    return [_liveClientCore downloadFromPath:path 
                                    delegate:delegate 
                                   userState:userState];
}

- (LiveOperation *) uploadToPath:(NSString *)path
                        fileName:(NSString *)fileName
                            data:(NSData *)data
                        delegate:(id <LiveUploadOperationDelegate>)delegate
{
    return [self uploadToPath:path 
                     fileName:fileName 
                         data:data
                    overwrite:LiveUploadDoNotOverwrite 
                     delegate:delegate 
                    userState:nil];
}

- (LiveOperation *) uploadToPath:(NSString *)path
                        fileName:(NSString *)fileName
                            data:(NSData *)data
                       overwrite:(LiveUploadOverwriteOption)overwrite
                        delegate:(id <LiveUploadOperationDelegate>)delegate
                       userState:(id)userState
{
    [self validateInit];
    
    NSString *method = @"uploadToPath:fileName:data:overwrite:delegate:userState:";
    [self validatePath:path
            methodName:method
              relative:NO];
    
    [self validateStringParam:fileName 
                    paramName:@"fileName" 
                   methodName:method];
    
    [self validateRequiredParam:data
                      paramName:@"data" 
                     methodName:method];
    
    return [_liveClientCore uploadToPath:path 
                                fileName:fileName 
                                    data:data 
                               overwrite:overwrite 
                                delegate:delegate 
                               userState:userState];
}

- (LiveOperation *) uploadToPath:(NSString *)path
                        fileName:(NSString *)fileName
                     inputStream:(NSInputStream *)inputStream
                        delegate:(id <LiveUploadOperationDelegate>)delegate
{
    return [self uploadToPath:path 
                     fileName:fileName 
                  inputStream:inputStream 
                    overwrite:LiveUploadDoNotOverwrite
                     delegate:delegate 
                    userState:nil];
}

- (LiveOperation *) uploadToPath:(NSString *)path
                        fileName:(NSString *)fileName
                     inputStream:(NSInputStream *)inputStream
                       overwrite:(LiveUploadOverwriteOption)overwrite
                        delegate:(id <LiveUploadOperationDelegate>)delegate
                       userState:(id)userState
{
    [self validateInit];
    
    NSString *method = @"uploadToPath:fileName:inputStream:overwrite:delegate:userState:";
    [self validatePath:path
            methodName:method
              relative:NO];
    
    [self validateStringParam:fileName 
                    paramName:@"fileName" 
                   methodName:method];   
    
    [self validateRequiredParam:inputStream
                      paramName:@"inputStream" 
                     methodName:method];
    
    return [_liveClientCore uploadToPath:path 
                                fileName:fileName 
                             inputStream:inputStream 
                               overwrite:overwrite 
                                delegate:delegate 
                               userState:userState];
}

@end
