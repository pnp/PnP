/*******************************************************************************
 * Copyright (c) Microsoft Open Technologies, Inc.
 * All Rights Reserved
 * Licensed under the Apache License, Version 2.0.
 * See License.txt in the project root for license information.
 ******************************************************************************/

#import "MSOrcResponseImpl.h"

@implementation MSOrcResponseImpl

@synthesize data = _data;
@synthesize stream = _stream;
@synthesize response = _response;

- (instancetype)initWithData:(NSData *)data response:(NSURLResponse *)response {
    
    if (self = [super init]) {
        
        _data = data;
        _response = response;
    }
    
    return self;
}

- (instancetype)initWithStream:(NSInputStream *)stream response:(NSURLResponse *)response {
    
    if (self = [super init]) {
    
        _stream = stream;
        _response = response;
    }
    
    return self;
}

- (int)status {
    return (int)[(NSHTTPURLResponse *)self.response statusCode];
}

@end