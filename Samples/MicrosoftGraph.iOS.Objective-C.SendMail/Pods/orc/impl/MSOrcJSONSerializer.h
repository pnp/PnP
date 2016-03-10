/*******************************************************************************
 * Copyright (c) Microsoft Open Technologies, Inc.
 * All Rights Reserved
 * Licensed under the Apache License, Version 2.0.
 * See License.txt in the project root for license information.
 ******************************************************************************/

#ifndef MSORCJSONSERIALIZER_H
#define MSORCJSONSERIALIZER_H

#import <Foundation/Foundation.h>
#import <api/MSOrcSerializer.h>

@interface MSOrcJSONSerializer : NSObject<MSOrcSerializer>

+ (NSString *) serialize:(id)objectToSerialize;
+ (id) deserializeString: (NSString *) string;

@end

#endif
