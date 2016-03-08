/*******************************************************************************
 * Copyright (c) Microsoft Open Technologies, Inc.
 * All Rights Reserved
 * Licensed under the Apache License, Version 2.0.
 * See License.txt in the project root for license information.
 ******************************************************************************/

#import "NSString+NSStringExtensions.h"

@implementation NSString (NSStringExtensions)

const NSString *exceptions = @"!$&'()*+,;=:@";

- (NSString *)urlEncode {
    
    NSMutableString *output = [NSMutableString string];
    
    const unsigned char *source = (const unsigned char *)[self UTF8String];
    
    long sourceLen = strlen((const char *)source);
    
    for (int i = 0; i < sourceLen; ++i) {
        
        const unsigned char thisChar = source[i];
        
        if (thisChar == ' '){
            
            [output appendString:@"%20"];
        }
        else if ([self evaluateChar:thisChar] || ([exceptions rangeOfString:[[NSString alloc] initWithFormat:@"%c",thisChar]].length > 0)){
            
            [output appendFormat:@"%c", thisChar];
        }
        else {

            [output appendFormat:@"%%%02X", thisChar];
        }
    }
    return output;
}

- (BOOL)evaluateChar:(char)c {
    return (c == '.' || c == '-' || c == '_' || c == '~' ||
           (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') ||
           (c >= '0' && c <= '9'));
}

@end