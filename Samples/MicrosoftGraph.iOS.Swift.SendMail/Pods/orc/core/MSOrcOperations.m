/*******************************************************************************
 * Copyright (c) Microsoft Open Technologies, Inc.
 * All Rights Reserved
 * Licensed under the Apache License, Version 2.0.
 * See License.txt in the project root for license information.
 ******************************************************************************/

#import "MSOrcBaseContainer.h"
#import "MSOrcOperations.h"
#import "api/MSOrcRequest.h"
#import "api/MSOrcURL.h"

@implementation MSOrcOperations

@synthesize urlComponent = _urlComponent;
@synthesize parent = _parent;

- (instancetype)initOperationWithUrl:(NSString *)urlComponent parent:(id<MSOrcExecutable>)parent {
    
    if (self = [super initWithUrl:urlComponent parent:parent asClass:nil]) {
      
        _urlComponent = urlComponent;
        _parent = parent;
    }
    
    return self;
}

- (void)orcExecuteRequest:(id<MSOrcRequest>)request
                 callback:(void (^)(id<MSOrcResponse> response, MSOrcError *error))callback {
    
    [request.url appendPathComponent:_urlComponent];
    
    [MSOrcBaseContainer addCustomParametersToOrcURLWithRequest:request
                                                    parameters:self.customParameters
                                                       headers:self.customHeaders
                                            dependencyResolver:self.resolver];
    
    return [self.parent orcExecuteRequest:request callback:callback];
}

@end