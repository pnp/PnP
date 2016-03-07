//
//  LiveApiHelper.m
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


#import "LiveAuthHelper.h"
#import "LiveApiHelper.h"
#import "LiveConstants.h"
#import "JsonParser.h"
#import "JsonWriter.h"
#import "UrlHelper.h"

@implementation LiveApiHelper

+ (NSURL *) buildAPIUrl:(NSString *)path
{
    return [LiveApiHelper buildAPIUrl:path
                               params:nil];
}

+ (NSURL *) getApiServiceBaseUrl
{
    return [NSURL URLWithString: [NSString stringWithFormat: @"https://%@/v5.0/", LIVE_ENDPOINT_API_HOST]];
}

+ (NSURL *) buildAPIUrl:(NSString *)path
                 params:(NSDictionary *)params
{
    NSURL *baseUrl;
    if ([UrlHelper isFullUrl:path])
    {
        baseUrl = [NSURL URLWithString:path];
        
    }
    else
    {
        path = [path characterAtIndex:0] == '/'? [path substringFromIndex:1] : path;
        baseUrl = [NSURL URLWithString:path 
                         relativeToURL:[LiveApiHelper getApiServiceBaseUrl]];
    }
    
    return [UrlHelper constructUrl:baseUrl.absoluteString
                            params:params];
}

+ (NSError *) createAPIError:(NSDictionary *)info
{
    return [NSError errorWithDomain:LIVE_ERROR_DOMAIN 
                               code:LIVE_ERROR_CODE_API_FAILED 
                           userInfo:info];
}

+ (NSError *) createAPIError:(NSString *)code
                     message:(NSString *)message
                  innerError:(NSError *)error
{
    return [LiveApiHelper createAPIError:[NSDictionary dictionaryWithObjectsAndKeys:
                                              code, LIVE_ERROR_KEY_CODE,
                                           message, LIVE_ERROR_KEY_MESSAGE,
                                             error, LIVE_ERROR_KEY_INNER_ERROR,
                                          nil]];
}

+ (NSString *) getXHTTPLiveLibraryHeaderValue
{
    return [NSString stringWithFormat:
                @"iOS/%@%@_%@",
                [LiveAuthHelper isiPad]? @"iPad" : @"iPhone",
                [[UIDevice currentDevice] systemVersion], 
                LIVE_SDK_VERSION]; 
}

+ (BOOL) isFilePath: (NSString *)path
{
    NSString *lowerPath = [path lowercaseString];
    return [[lowerPath substringToIndex:5] isEqualToString:@"file."] || 
    [[lowerPath substringToIndex:6] isEqualToString:@"/file."];
}

+ (void) parseApiResponse:(NSData *)data
             textResponse:(NSString **)textResponse
                 response:(NSDictionary **)response
                    error:(NSError **)error
{
    if (data == nil)
    {
        *textResponse = @"";
        *response = [NSDictionary dictionary];
        *error = nil;
        return;
    }
    
    *textResponse = [[[NSString alloc] initWithData:data
                                           encoding:NSUTF8StringEncoding]
                     autorelease];
        
    *response = [MSJSONParser parseText:*textResponse 
                                  error:error ];
    
    NSDictionary *errorObj = [(*response) valueForKey:LIVE_ERROR_KEY_ERROR];
    if (errorObj != nil)
    {
        *error = [LiveApiHelper createAPIError:errorObj];
    }
}

+ (NSString *) buildCopyMoveBody:(NSString *)destination
{
    return [MSJSONWriter textForValue:
            [NSDictionary dictionaryWithObjectsAndKeys:destination, @"destination", nil]];
}

@end
