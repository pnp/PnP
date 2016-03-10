/*******************************************************************************
 * Copyright (c) Microsoft Open Technologies, Inc.
 * All Rights Reserved
 * Licensed under the Apache License, Version 2.0.
 * See License.txt in the project root for license information.
 ******************************************************************************/

#import <UIKit/UIKit.h>

FOUNDATION_EXPORT double implVersionNumber;
FOUNDATION_EXPORT const unsigned char implVersionString[];

#import <api/api.h>
#import <core/core.h>

#import <impl/MSOrcDefaultDependencyResolver.h>
#import <impl/NSString+NSStringExtensions.h>
#import <impl/MSOrcURLImpl.h>
#import <impl/MSOrcBasicCredentials.h>
#import <impl/MSOrcHttpConnection.h>
#import <impl/MSOrcLoggerImpl.h>
#import <impl/MSOrcRequestImpl.h>
#import <impl/MSOrcResponseImpl.h>
#import <impl/MSOrcOAuthCredentials.h>
#import <impl/ADALDependencyResolver.h>
#import <impl/LiveDependencyResolver.h>
#import <impl/MSOrcJSONSerializer.h>
