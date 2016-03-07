/*******************************************************************************
 * Copyright (c) Microsoft Open Technologies, Inc.
 * All Rights Reserved
 * Licensed under the Apache License, Version 2.0.
 * See License.txt in the project root for license information.
 ******************************************************************************/

#import "MSOrcCollectionFetcher.h"
#import "MSOrcBaseContainer.h"
#import "MSOrcObjectizer.h"
#import "api/MSOrcRequest.h"
#import "api/MSOrcResponse.h"
#import "api/MSOrcURL.h"

@interface MSOrcCollectionFetcher ()

@property (nonatomic) int top;
@property (nonatomic) int skip;
@property (copy, nonatomic, readonly) NSString *search;
@property (copy, nonatomic, readonly) NSString *filter;
@property (copy, nonatomic, readonly) NSString *select;
@property (copy, nonatomic, readonly) NSString *expand;
@property (copy, nonatomic, readonly) NSString *orderBy;
@property (copy, nonatomic, readonly) NSString *selectedId;
@property (copy, nonatomic, readonly) id operations;

@end

@implementation MSOrcCollectionFetcher

- (instancetype)initWithUrl:(NSString *)urlComponent parent:(id<MSOrcExecutable>)parent asClass:(Class)theClass {
    
    if (self = [super initWithUrl:urlComponent parent:parent asClass:theClass]) {
    
        [self reset];
    }

    return self;
}

- (MSOrcCollectionFetcher *)select:(NSString *)params {
    
    _select = params;
    
    return self;
}

- (MSOrcCollectionFetcher *)filter:(NSString *)params {
    
    _filter = params;
    
    return self;
}

- (MSOrcCollectionFetcher *)top:(int)value {
    
    _top = value;
    
    return self;
}

- (MSOrcCollectionFetcher *)expand:(NSString *)value {
    
    _expand = value;
    
    return self;
}

- (MSOrcCollectionFetcher *)skip:(int)value {
    
    _skip = value;
    
    return self;
}

- (MSOrcCollectionFetcher *)search:(NSString *)params {
    
    _search = params;
    
    return self;
}

- (MSOrcCollectionFetcher *)orderBy:(NSString*)params {
    
    _orderBy = params;
    
    return self;
}

- (void)orcExecuteRequest:(id<MSOrcRequest>)request callback:(void (^)(id<MSOrcResponse> response, MSOrcError *error))callback {
    
    [request.url appendPathComponent:self.urlComponent];
    
    [MSOrcEntityFetcher setPathForCollectionsWithUrl:request.url
                                                 top:self.top
                                                skip:self.skip
                                              select:self.select
                                              expand:self.expand
                                              filter:self.filter
                                             orderby:self.orderBy
                                              search:self.search];
    
    [MSOrcBaseContainer addCustomParametersToOrcURLWithRequest:request
                                                    parameters:self.customParameters
                                                       headers:self.customHeaders
                                            dependencyResolver:self.resolver];
    
    return [self.parent orcExecuteRequest:request callback:callback];
}

- (void)readRawWithCallback:(void (^)(NSString *result, MSOrcError *error))callback {
    
    id<MSOrcRequest> request = [self.parent.resolver createOrcRequest];

    return [self orcExecuteRequest:request callback:^(id<MSOrcResponse> response, MSOrcError *e) {
        
        callback([[NSString alloc] initWithData:response.data encoding:NSUTF8StringEncoding], e);
    }];
}

- (void)readWithCallback:(void (^)(id result, MSOrcError *error))callback {
    
    return [self readRawWithCallback:^(NSString *response, MSOrcError *e) {
        
        id unserialized = [[MSOrcObjectizer getCurrentSerializer] deserializeString: response];
        
        NSMutableArray *result =[[NSMutableArray alloc] init];
        
        for(id obj in unserialized[@"value"])
        {
            [result addObject: [MSOrcObjectizer objectize:obj toType: self.entityClass]];
        }
        
        callback(result, e);
    }];
}

- (void)count:(void (^)(NSInteger result, MSOrcError *error))callback {
    
    id<MSOrcRequest> request = [self.parent.resolver createOrcRequest];
    
    [[request url] appendPathComponent:@"$count"];
    [self reset];
    
    return [self orcExecuteRequest:request callback:^(id<MSOrcResponse> response, MSOrcError *e) {
        
        callback([[[NSString alloc] initWithData:response.data encoding:NSUTF8StringEncoding] integerValue], e);
    }];
}

- (void)addRaw:(NSString *)payload
      callback:(void (^)(NSString *result, MSOrcError *error))callback {

    id<MSOrcRequest> request = [self.parent.resolver createOrcRequest];
    
    [request setVerb:HTTP_VERB_POST];
    [request setContent:[NSMutableData dataWithData:[payload dataUsingEncoding:NSUTF8StringEncoding]]];
    
    return [self orcExecuteRequest:request callback:^(id<MSOrcResponse> response, MSOrcError *e) {
        
        callback([[NSString alloc] initWithData:response.data encoding:NSUTF8StringEncoding], e);
    }];
}

- (void)add:(id)entity callback:(void (^)(id entityAdded, MSOrcError *error))callback {
    
    NSString *payload = [MSOrcObjectizer deobjectizeToString: entity];
    
    __block MSOrcCollectionFetcher *_self = self;
    
    return [self addRaw:payload callback:^(NSString *r, MSOrcError *e) {
        
        id result = [MSOrcObjectizer objectizeFromString: r toType: _self.entityClass];
        
        callback(result, e);
    }];
}

- (MSOrcCollectionFetcher *)addCustomParametersWithName:(NSString *)name value:(id)value {
    [super addCustomParametersWithName:name value:value];
    
    return self;
}

- (MSOrcCollectionFetcher *)addCustomHeaderWithName:(NSString *)name value:(NSString *)value {
    [super addCustomHeaderWithName:name value:value];
    
    return self;
}

- (MSOrcEntityFetcher *)getById:(NSString *)theId {
    
    _selectedId = theId;
    
    MSOrcEntityFetcher *fetcher = [[MSOrcEntityFetcher alloc] initWithUrl:@""
                                                                   parent:self
                                                                  asClass:self.entityClass];
    
    return fetcher;
}

- (void)reset {
    
    _top = -1;
    _skip = -1;
    _selectedId = nil;
    _select = nil;
    _orderBy = nil;
    _expand = nil;
    _filter = nil;
    _search = nil;
}

@end