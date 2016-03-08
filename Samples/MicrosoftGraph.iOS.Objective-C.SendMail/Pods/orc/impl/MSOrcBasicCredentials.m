/*******************************************************************************
 * Copyright (c) Microsoft Open Technologies, Inc.
 * All Rights Reserved
 * Licensed under the Apache License, Version 2.0.
 * See License.txt in the project root for license information.
 ******************************************************************************/

#import "MSOrcBasicCredentials.h"
#import "api/MSOrcRequest.h"

@implementation MSOrcBasicCredentials

@synthesize token = _token;

- (void)prepareRequest:(id<MSOrcRequest>)request {
    
    [request addHeaderWithName:@"Authorization" value:[NSString stringWithFormat:@"Basic %@", self.token]];
}

@end