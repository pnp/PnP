/*******************************************************************************
 * Copyright (c) Microsoft Open Technologies, Inc.
 * All Rights Reserved
 * Licensed under the Apache License, Version 2.0.
 * See License.txt in the project root for license information.
 ******************************************************************************/

#import "MSOrcMultiPartElement.h"

@implementation MSOrcMultiPartElement

@synthesize name = _name;
@synthesize content = _content;
@synthesize contentType = _contentType;

- (instancetype)initWithName:(NSString *)name andContentString:(NSString *)content {
    
    return [self initWithName:name andContentType:@"text/html" andContent:[content dataUsingEncoding:NSUTF8StringEncoding]];
}

- (instancetype)initWithName:(NSString *)name andContentType:(NSString *)contentType andContent:(NSData *)content {
    
    if (self = [super init]) {
        
        _name = name;
        _contentType = contentType;
        _content = content;
    }
    
    return self;
}

@end