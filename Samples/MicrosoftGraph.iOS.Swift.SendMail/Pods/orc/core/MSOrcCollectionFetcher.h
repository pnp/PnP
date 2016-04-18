/*******************************************************************************
 * Copyright (c) Microsoft Open Technologies, Inc.
 * All Rights Reserved
 * Licensed under the Apache License, Version 2.0.
 * See License.txt in the project root for license information.
 ******************************************************************************/

#import "MSOrcExecutable.h"
#import "MSOrcEntityFetcher.h"

#define __message_msg(message)

@interface MSOrcCollectionFetcher : MSOrcExecutable

- (MSOrcCollectionFetcher *)select:(NSString *)params;
- (MSOrcCollectionFetcher *)filter:(NSString *)params;
- (MSOrcCollectionFetcher *)top:(int)value;
- (MSOrcCollectionFetcher *)skip:(int)value;
- (MSOrcCollectionFetcher *)expand:(NSString *)value;
- (MSOrcCollectionFetcher *)orderBy:(NSString *)params;
- (MSOrcCollectionFetcher *)search:(NSString *)params;

- (void)add:(id)entity callback:(void (^)(id entityAdded, MSOrcError *error))callback;
- (MSOrcEntityFetcher *)getById:(NSString *)theId;
- (void)count:(void (^)(NSInteger result, MSOrcError *error))callback; __message_msg("This method will override all the orc operators -> select, top, filter, orderby, skip, expand.");
- (void)addRaw:(NSString *)payload callback:(void (^)(NSString *result, MSOrcError *error))callback;
- (MSOrcCollectionFetcher *)addCustomParametersWithName:(NSString *)name value:(id)value;
- (MSOrcCollectionFetcher *)addCustomHeaderWithName:(NSString *)name value:(NSString *)value;

@end