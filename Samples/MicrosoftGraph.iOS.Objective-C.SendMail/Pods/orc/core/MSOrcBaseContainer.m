/*******************************************************************************
 * Copyright (c) Microsoft Open Technologies, Inc.
 * All Rights Reserved
 * Licensed under the Apache License, Version 2.0.
 * See License.txt in the project root for license information.
 ******************************************************************************/

#import "MSOCalendarSerializer.h"
#import "MSOrcBaseContainer.h"
#import "api/MSOrcHttpTransport.h"
#import "api/MSOrcCredentials.h"
#import "api/MSOrcRequest.h"
#import "api/MSOrcURL.h"

@implementation MSOrcBaseContainer

@synthesize parent = _parent;
@synthesize urlComponent = _urlComponent;
@synthesize resolver = _resolver;

- (instancetype)initWithUrl:(NSString *)url dependencyResolver:(id<MSOrcDependencyResolver>)resolver {
    
    if (self = [super init]) {

        _urlComponent = url;
        _resolver = resolver;
    }
    
    return self;
}

- (void)orcExecuteRequest:(id<MSOrcRequest>)request
                                 callback:(void (^)(id<MSOrcResponse> response, MSOrcError *error))callback {
    
    request.url.baseUrl = self.urlComponent;
    
    id<MSOrcHttpTransport> httpTransport = self.resolver.httpTransport;

    if([request.headers objectForKey:@"Content-Type"] == nil) {
        
        [request addHeaderWithName:@"Content-Type" value:@"application/json"];
    }
    
    [request addHeaderWithName:@"User-Agent"
                         value:[self.resolver getPlatformUserAgent:NSStringFromClass([self class])]];
     
    NSString *productName = [[[NSBundle mainBundle] infoDictionary] objectForKey:(NSString *)kCFBundleNameKey];
    
    [request addHeaderWithName:@"X-ClientService-ClientTag" value:[self.resolver getPlatformUserAgent:productName]];
    [request addHeaderWithName:@"OData-Version" value:@"4.0"];
    [request addHeaderWithName:@"OData-MaxVersion" value:@"4.0"];
    
    // resolver.credentials results in call to authenticationContext.acquireTokenSilent
    // which may fail if all tokens are expired, or if they were never set properly
    // TODO: Plan for handling errors.
    [self.resolver.credentials prepareRequest:request];
    
    return [httpTransport executeRequest:request callback:^(id<MSOrcResponse> r, MSOrcError *e) {
        
        callback(r,e);
    }];
}


+ (NSString *)generatePayloadWithParameters:(NSArray *)parameters
                         dependencyResolver:(id<MSOrcDependencyResolver>)resolver {
    
    NSMutableString *result = [[NSMutableString alloc] initWithString:@"{"];
    
    NSArray *reversedParameters = [parameters reverseObjectEnumerator].allObjects;
    
    for (NSDictionary *item in reversedParameters) {
        
        for (NSString *key in item.allKeys) {
            
            [result appendFormat:@"\"%@\":%@,", key, [item objectForKey:key]];
        }
    }
    
    return [NSString stringWithFormat:@"%@}", [result substringWithRange:NSMakeRange(0, [result length] -1)]];
}

+ (void)addCustomParametersToOrcURLWithRequest:(id<MSOrcRequest>)request
                                    parameters:(NSDictionary *)parameters
                                       headers:(NSDictionary *)headers
                            dependencyResolver:(id<MSOrcDependencyResolver>)resolver {
    
    for (NSString *key in parameters.allKeys) {
        
        id object = [parameters objectForKey:key];
        
        NSString *value = [object isKindOfClass:[NSDate class]] ? [MSOCalendarSerializer serialize:object]
        : [[NSString alloc] initWithFormat: @"\"%@\"", object];
        
        [request.url addQueryStringParameter:key value:value];
    }
    
    for (NSString *header in headers.allKeys) {
        
        [request addHeaderWithName:header value:[headers objectForKey:header]];
    }
}

+ (NSString *)getFunctionParameters:(NSDictionary *)parameters {
    
    NSMutableString *theString = [[NSMutableString alloc] init];
    
    for (NSString *key in parameters.allKeys) {
        
        if (theString.length > 0) {
            
            [theString appendString:@","];
        }
        
        [theString appendFormat:@"%@=%@", key, [self toOrcURLValue:[parameters objectForKey:key]]];
    }
    
    return theString;
}

+ (NSString *)toOrcURLValue:(id)o {
    
    return [NSString stringWithFormat:@"'%@'", o];
}

@end