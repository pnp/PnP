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

#import "ADTokenCacheStoring.h"

@interface ADKeychainTokenCacheStore : NSObject<ADTokenCacheStoring>

/* Initializes the token cache store with default shared group value.
 */
-(id) init;
/*! Initializes the token cache store.
 @param: sharedGroup: Optional. If the application needs to share the cached tokens
 with other applications from the same vendor, the app will need to specify the 
 shared group here and add the necessary entitlements to the application.
 See Apple's keychain services documentation for details. */
-(id) initWithGroup: (NSString*) sharedGroup;

/* The shared keychain group, where the ADAL library will keep the tokens.
 May be nil. The cache items from the previous keychain group are not transferred
 automatically. */
@property (getter = getSharedGroup, setter = setSharedGroup:) NSString* sharedGroup;

@end
