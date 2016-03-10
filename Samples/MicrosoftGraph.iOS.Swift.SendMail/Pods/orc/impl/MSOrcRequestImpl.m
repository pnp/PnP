/*******************************************************************************
 * Copyright (c) Microsoft Open Technologies, Inc.
 * All Rights Reserved
 * Licensed under the Apache License, Version 2.0.
 * See License.txt in the project root for license information.
 ******************************************************************************/

#import "MSOrcRequestImpl.h"
#import "MSOrcURLImpl.h"

@implementation MSOrcRequestImpl

@synthesize streamContent = _streamContent;
@synthesize options = _options;
@synthesize content = _content;
@synthesize headers = _headers;
@synthesize size = _size;
@synthesize url = _url;
@synthesize verb = _verb;

-(instancetype)init {
    
    if (self = [super init]) {
        
        _verb = HTTP_VERB_GET;
        _headers = [NSMutableDictionary dictionary];
        _options = [NSMutableArray array];
        _content = nil;
        _streamContent = nil;
    }
    
    return self;
}

- (id<MSOrcURL>)url {
    
    if (_url == nil) {
        _url = [[MSOrcURLImpl alloc] init];
    }
    
    return _url;
}

- (NSMutableArray *)options {
    
    if (_options == nil) {
        _options = [NSMutableArray array];
    }
    
    return _options;
}

- (void)addOptionWithName:(NSString *)name value:(NSString *)value {
    
    NSDictionary *dicc = [[NSMutableDictionary alloc] initWithObjectsAndKeys:value, name, nil];
    [self.options addObject:dicc];
}

- (void)addHeaderWithName:(NSString *)name value:(NSString *)value {
    
    NSDictionary *dicc = [[NSMutableDictionary alloc] initWithObjectsAndKeys: value, name, nil];
    [self.headers addEntriesFromDictionary:dicc];
}

- (NSString *)verbToString {
    
    NSString *verbToString;
    
    switch (_verb) {
        case HTTP_VERB_GET:
            verbToString = @"GET";
            break;
        case HTTP_VERB_POST:
            verbToString = @"POST";
            break;
        case HTTP_VERB_DELETE:
            verbToString = @"DELETE";
            break;
        case HTTP_VERB_PATCH:
            verbToString = @"PATCH";
            break;
        case HTTP_VERB_PUT:
            verbToString = @"PUT";
            break;
        case HTTP_VERB_HEAD:
            verbToString = @"HEAD";
            break;
        case HTTP_VERB_OPTIONS:
            verbToString = @"OPTIONS";
            break;
        default:
            break;
    }
    
    return verbToString;
}

@end