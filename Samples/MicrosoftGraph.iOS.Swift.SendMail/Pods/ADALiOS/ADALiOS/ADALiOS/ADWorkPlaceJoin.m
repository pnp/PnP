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

#import "ADWorkPlaceJoin.h"
#import "ADWorkPlaceJoinUtil.h"
#import "ADWorkPlaceJoinConstants.h"
#import "ADRegistrationInformation.h"
#import <UIKit/UIKit.h>
#import <MessageUI/MessageUI.h>


@implementation ADWorkPlaceJoin

NSArray *_upnParts;
NSString *_userPrincipalName;
UIViewController * _callingViewController;

static ADWorkPlaceJoin* wpjManager;

NSString* _oauthClientId;

#pragma mark - Public Methods

+ (ADWorkPlaceJoin*) WorkPlaceJoinManager
{
    if (!wpjManager)
    {
        wpjManager = [[self alloc] init];
    }
    
    return wpjManager;
}

- (id)init {
    self = [super init];
    if (self) {
        [ADWorkPlaceJoinUtil WorkPlaceJoinUtilManager].workplaceJoin = self;
        _sharedGroup = [NSString stringWithFormat:@"%@.%@", [[ADWorkPlaceJoinUtil WorkPlaceJoinUtilManager]  getApplicationIdentifierPrefix], _defaultSharedGroup];
    }
    return self;
}

- (BOOL)isWorkPlaceJoined
{
    ADRegistrationInformation *userRegInfo = [self getRegistrationInformation];
    BOOL certExists = [userRegInfo certificate] != NULL;
    [userRegInfo releaseData];
    userRegInfo = nil;
    return certExists;
}

- (ADRegistrationInformation*) getRegistrationInformation {
    return [[ADWorkPlaceJoinUtil WorkPlaceJoinUtilManager]  getRegistrationInformation:_sharedGroup error:nil];
}

@end
