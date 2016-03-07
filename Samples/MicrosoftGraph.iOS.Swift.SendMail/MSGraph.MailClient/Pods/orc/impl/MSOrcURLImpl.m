/*******************************************************************************
 * Copyright (c) Microsoft Open Technologies, Inc.
 * All Rights Reserved
 * Licensed under the Apache License, Version 2.0.
 * See License.txt in the project root for license information.
 ******************************************************************************/

#import "MSOrcURLImpl.h"
#import "NSString+NSStringExtensions.h"

@implementation MSOrcURLImpl

@synthesize queryStringParameters = _queryStringParameters;
@synthesize pathComponents = _pathComponents;
@synthesize baseUrl = _baseUrl;

- (instancetype)init {
    
    if (self = [super init]) {
    
        _queryStringParameters = [[NSMutableDictionary alloc] init];
        _pathComponents = [[NSMutableArray alloc] init];
    }
    
    return self;
}

- (void)setBaseUrl:(NSString *)baseUrl {
    
    NSArray *urlParts = [baseUrl componentsSeparatedByString:@"?"];
    
    _baseUrl = [self removeTrailingSlash:(NSString *)[urlParts objectAtIndex:0]];
    
    if (urlParts.count > 1) {
        
        NSArray *parameters = [(NSString *)[urlParts objectAtIndex:1] componentsSeparatedByString:@"&"];
        
        for (NSString *kv in parameters)
        {
            NSArray *parameterParts = [kv componentsSeparatedByString:@"="];
            
            [self addQueryStringParameter:[parameterParts objectAtIndex:0]
                                    value:[parameterParts objectAtIndex:1]];
        }
    }
}

- (void)addQueryStringParameter:(NSString *)name value:(NSString *)value {
    
    NSMutableDictionary *dicc = [[NSMutableDictionary alloc] initWithObjectsAndKeys:value, name, nil];
    
    [dicc addEntriesFromDictionary: self.queryStringParameters];
    
    _queryStringParameters = dicc;
}

- (void)appendPathComponent:(NSString *)value {
    
    NSMutableArray *a = [[NSMutableArray alloc] initWithObjects:value, nil];
    
    [a addObjectsFromArray: self.pathComponents];
    
    _pathComponents = a;
}

- (NSString *)addTrailingSlash:(NSString *)s {
    
    NSMutableString *theString = [[NSMutableString alloc] initWithString:s];
    
    if (![theString hasSuffix:@"/"]) {
        
        [theString appendString:@"/"];
    }
    
    return theString;
}

- (NSString *)removeTrailingSlash:(NSString *)s {
    
    if ([s hasSuffix:@"/"]) {
        
        return [s substringWithRange:NSMakeRange(0, s.length-1)];
    }
    else if ([s hasSuffix:@"%2F"]) {
        
        return [s substringWithRange:NSMakeRange(0, s.length-3)];
    }
    
    return s;
}

- (NSString *)toString {
    
    NSMutableString *queryString = [[NSMutableString alloc] initWithFormat:@"%@/", self.baseUrl];
    
    for (NSString *value in self.pathComponents) {
        
        if ([value hasPrefix:@"('"] && [value hasSuffix:@"')"]) {
            
            queryString =[[NSMutableString alloc] initWithString : [self removeTrailingSlash:queryString]];
        }
        
        [queryString appendString:[self addTrailingSlash:value] ];
    }
    
    if (self.queryStringParameters.allKeys.count > 0) {
        
        [queryString appendString:@"?"];
    }
    
    for (NSString *key in self.queryStringParameters.allKeys) {
        
        [queryString appendFormat:@"%@=%@&",[key urlEncode], [[self.queryStringParameters objectForKey:key] urlEncode]];
    }
    
    if ([queryString hasSuffix:@"&"]) {
        
        queryString = (NSMutableString *)[queryString substringToIndex:[queryString length]-1];
    }
    
    return queryString;
}

@end