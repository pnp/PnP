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
#import "ADAuthenticationSettings.h"
#import "ADKeychainTokenCacheStore.h"

@implementation ADAuthenticationSettings


/*!
 An internal initializer used from the static creation function.
 */
-(id) initInternal
{
    self = [super init];
    if (self)
    {
        //Initialize the defaults here:
        self.credentialsType = AD_CREDENTIALS_AUTO;
        self.requestTimeOut = 300;//in seconds.
        self.expirationBuffer = 300;//in seconds, ensures catching of clock differences between the server and the device
        self.enableFullScreen = YES;
        
        //The current ADWebRequest implementation uses NSURLConnection, which calls its delegate on the same thread
        //that created the object. Unfortunately with Grand Central Dispatch, it is not guaranteed that the thread
        //exists. Hence for now, we create the connection on the main thread by default:
        self.dispatchQueue = dispatch_get_main_queue();
        self.defaultTokenCacheStore = [ADKeychainTokenCacheStore new];
    }
    return self;
}

+(ADAuthenticationSettings*)sharedInstance
{
    /* Below is a standard objective C singleton pattern*/
    static ADAuthenticationSettings* instance;
    static dispatch_once_t onceToken;
    @synchronized(self)
    {
        dispatch_once(&onceToken, ^{
            instance = [[ADAuthenticationSettings alloc] initInternal];
        });
    }
    return instance;
}

-(NSString*) getSharedCacheKeychainGroup
{
    id store = self.defaultTokenCacheStore;
    if ([store isKindOfClass:[ADKeychainTokenCacheStore class]])
    {
        return ((ADKeychainTokenCacheStore*)store).sharedGroup;
    }
    else
    {
        return nil;
    }
}

-(void) setSharedCacheKeychainGroup:(NSString *)sharedKeychainGroup
{
    id store = self.defaultTokenCacheStore;
    if ([store isKindOfClass:[ADKeychainTokenCacheStore class]])
    {
        ((ADKeychainTokenCacheStore*)store).sharedGroup = sharedKeychainGroup;
    }
}

@end

