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

#import "ADALiOS.h"
#import "ADTokenCacheStoreItem.h"
#import "ADUserInformation.h"
#import "ADAuthenticationSettings.h"
#import "ADTokenCacheStoreKey.h"

@implementation ADTokenCacheStoreItem

@synthesize multiResourceRefreshToken;

//Multi-resource refresh tokens are stored separately, as they apply to all resources. As such,
//we create a special, "broad" cache item, with nil resource and access token:
-(BOOL) isMultiResourceRefreshToken
{
    return [NSString adIsStringNilOrBlank:self.resource]
        && [NSString adIsStringNilOrBlank:self.accessToken]
       && ![NSString adIsStringNilOrBlank:self.refreshToken];
}

-(id) copyWithZone:(NSZone*) zone
{
    ADTokenCacheStoreItem* item = [[self.class allocWithZone:zone] init];
    
    item.resource = [self.resource copyWithZone:zone];
    item.authority = [self.authority copyWithZone:zone];
    item.clientId = [self.clientId copyWithZone:zone];
    item.accessToken = [self.accessToken copyWithZone:zone];
    item.accessTokenType = [self.accessTokenType copyWithZone:zone];
    item.refreshToken = [self.refreshToken copyWithZone:zone];
    item.expiresOn = [self.expiresOn copyWithZone:zone];
    item.userInformation = [self.userInformation copyWithZone:zone];
    
    return item;
}

-(ADTokenCacheStoreKey*) extractKeyWithError: (ADAuthenticationError* __autoreleasing *) error
{
    return [ADTokenCacheStoreKey keyWithAuthority:self.authority
                                         resource:self.resource
                                         clientId:self.clientId
                                            error:error];
}

-(BOOL) isExpired
{
    if (nil == self.expiresOn)
    {
        return NO;//Assume opportunistically that it is not, as the expiration time is uknown.
    }
    //Check if it there is less than "expirationBuffer" time to the expiration:
    uint expirationBuffer = [[ADAuthenticationSettings sharedInstance] expirationBuffer];
    return [self.expiresOn compare:[NSDate dateWithTimeIntervalSinceNow:expirationBuffer]] == NSOrderedAscending;
}

-(BOOL) isEmptyUser
{
    //The userInformation object cannot be constructed with empty or blank string,
    //so its presence guarantees that the user is not empty:
    return !self.userInformation;
}

/*! Verifies if the user (as defined by userId) is the same between the two items. */
-(BOOL) isSameUser: (ADTokenCacheStoreItem*) other
{
    THROW_ON_NIL_ARGUMENT(other);
    
    if ([self isEmptyUser])
        return [other isEmptyUser];
    return (nil != other.userInformation && [self.userInformation.userId isEqualToString:other.userInformation.userId]);
}

+(BOOL) supportsSecureCoding
{
    return YES;
}

//Serializer:
-(void) encodeWithCoder:(NSCoder *)aCoder
{
    [aCoder encodeObject:self.resource forKey:@"resource"];
    [aCoder encodeObject:self.authority forKey:@"authority"];
    [aCoder encodeObject:self.clientId forKey:@"clientId"];
    [aCoder encodeObject:self.accessToken forKey:@"accessToken"];
    [aCoder encodeObject:self.accessTokenType forKey:@"accessTokenType"];
    [aCoder encodeObject:self.refreshToken forKey:@"refreshToken"];
    [aCoder encodeObject:self.expiresOn forKey:@"expiresOn"];
    [aCoder encodeObject:self.userInformation forKey:@"userInformation"];
}

//Deserializer:
-(id) initWithCoder:(NSCoder *)aDecoder
{
    self = [super init];
    if (self)
    {
        self.resource = [aDecoder decodeObjectOfClass:[NSString class] forKey:@"resource"];
        self.authority = [aDecoder decodeObjectOfClass:[NSString class] forKey:@"authority"];
        self.clientId = [aDecoder decodeObjectOfClass:[NSString class] forKey:@"clientId"];
        self.accessToken = [aDecoder decodeObjectOfClass:[NSString class] forKey:@"accessToken"];
        self.accessTokenType = [aDecoder decodeObjectOfClass:[NSString class] forKey:@"accessTokenType"];
        self.refreshToken = [aDecoder decodeObjectOfClass:[NSString class] forKey:@"refreshToken"];
        self.expiresOn = [aDecoder decodeObjectOfClass:[NSDate class] forKey:@"expiresOn"];
        self.userInformation = [aDecoder decodeObjectOfClass:[ADUserInformation class] forKey:@"userInformation"];
    }
    return self;
}

@end
