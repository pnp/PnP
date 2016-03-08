// Copyright © Microsoft Open Technologies, Inc.
//
// All Rights Reserved
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// THIS CODE IS PROVIDED *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS
// OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION
// ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A
// PARTICULAR PURPOSE, MERCHANTABILITY OR NON-INFRINGEMENT.
//
// See the Apache License, Version 2.0 for the specific language
// governing permissions and limitations under the License.

#import <Foundation/Foundation.h>
#import "ADRegistrationInformation.h"

@class ADWorkPlaceJoin;

@interface ADWorkPlaceJoin : NSObject

/// Returns a static instance of the WorkPlaceJoin class which can then be used
/// to perform a join, leave, verify if the device is joined and get the
/// registered UPN in the event the device is joined.
+ (ADWorkPlaceJoin*) WorkPlaceJoinManager;

/*! Represents the shared access group used by this api. */
@property (readwrite) NSString* sharedGroup;

/// Will look at the shared application keychain in search for a certificate
/// Certificate found returns true
/// Certificate not found returns false
- (BOOL)isWorkPlaceJoined;

- (ADRegistrationInformation*) getRegistrationInformation;

@end

