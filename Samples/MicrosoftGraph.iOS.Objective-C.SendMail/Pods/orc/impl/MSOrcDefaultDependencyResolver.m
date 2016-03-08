/*******************************************************************************
 * Copyright (c) Microsoft Open Technologies, Inc.
 * All Rights Reserved
 * Licensed under the Apache License, Version 2.0.
 * See License.txt in the project root for license information.
 ******************************************************************************/

#import "MSOrcDefaultDependencyResolver.h"
#import "MSOrcHttpConnection.h"
#import "MSOrcRequestImpl.h"
#import "MSOrcLoggerImpl.h"

#import <UIKit/UIKit.h>
#include <sys/types.h>
#include <sys/sysctl.h>
#include <mach/machine.h>

@implementation MSOrcDefaultDependencyResolver

@synthesize httpTransport = _httpTransport;
@synthesize logger = _logger;
@synthesize credentials = _credentials;

- (id<MSOrcHttpTransport>)httpTransport {
    
    if (!_httpTransport) {
        
        _httpTransport = [[MSOrcHttpConnection alloc] init];
    }
    
    return _httpTransport;
}

- (id<MSOrcLogger>)logger {
    
    if (!_logger) {
        
        _logger = [[MSOrcLoggerImpl alloc] init];
    }
    return _logger;
}


- (id<MSOrcRequest>)createOrcRequest {
    
    return [[MSOrcRequestImpl alloc] init];
}

- (NSString *)getPlatformUserAgent:(NSString *)productName {
    
    NSMutableString *cpu = [[NSMutableString alloc] init];
    
    cpu_type_t type;
    size_t size = sizeof(type);
    sysctlbyname("hw.cputype", &type, &size, NULL, 0);
    
    if (type == CPU_TYPE_X86) {
        
        [cpu appendString:@"x86 "];
        
    }else if (type == CPU_TYPE_ARM) {
        
        [cpu appendString:@"ARM"];
    }
    else if (type == CPU_TYPE_X86_64){
        
        [cpu appendString:@"X86_64"];
    }
    else {
        
        [cpu appendString:@"Other"];
    }
    
    return [[NSString alloc] initWithFormat:@"%@/1.0 (lang=%@; os=%@; os_version=%@; arch=%@; version=%d)", productName, @"Objective-C", [[UIDevice currentDevice] systemName], [[UIDevice currentDevice] systemVersion], cpu ,6];
}

@end