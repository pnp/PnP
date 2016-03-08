/*******************************************************************************
 * Copyright (c) Microsoft Open Technologies, Inc.
 * All Rights Reserved
 * Licensed under the Apache License, Version 2.0.
 * See License.txt in the project root for license information.
 ******************************************************************************/

#ifndef MSORCINTEROPERABLEWITHDICTIOANRY
#define MSORCINTEROPERABLEWITHDICTIOANRY

#import <Foundation/Foundation.h>

/**
 * Implements
 */

@protocol MSOrcInteroperableWithDictionary <NSObject>

@required

- (instancetype) initWithDictionary: (NSDictionary *) dic;
- (NSDictionary *) toDictionary;
- (NSDictionary *) toUpdatedValuesDictionary;


@end


#endif
