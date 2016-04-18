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
#import "ADKeyChainHelper.h"
#import "ADKeyChainHelper.h"
#import "ADWorkPlaceJoinUtil.h"

extern NSString* const sKeyChainlog;

@implementation ADKeyChainHelper
{
    id mValueDataKey;
}

-(id) init
{
    [super doesNotRecognizeSelector:_cmd];
    return nil;
}

-(id) initWithClass: (id) classValue
            generic: (NSData*) generic
        sharedGroup: (NSString *)sharedGroup
{
    THROW_ON_NIL_ARGUMENT(classValue);
    
    self = [super init];
    if (!self)
    {
        return nil;
    }
    
    mValueDataKey = (__bridge id)kSecValueData;
    _classValue = classValue;
    _genericValue = generic;
    if(sharedGroup){
        _sharedGroup = [NSString stringWithFormat:@"%@.%@", [[ADWorkPlaceJoinUtil WorkPlaceJoinUtilManager]  getApplicationIdentifierPrefix], sharedGroup];
    }
    return self;
}

//Adds the attributes which need to be set before each operation:
-(void) addStandardAttributes: (NSMutableDictionary*) attributes
{
    if (!attributes)
    {
        return;
    }
    
    [attributes setObject:_classValue forKey:(__bridge id)kSecClass];
    if (_genericValue)
    {
        [attributes setObject:_genericValue forKey:(__bridge id)kSecAttrGeneric];
    }
    if (![NSString adIsStringNilOrBlank:_sharedGroup])
    {
        //Apps are not signed on the simulator, so the shared group doesn't apply there.
#if !(TARGET_IPHONE_SIMULATOR)
        [attributes setObject:_sharedGroup forKey:(__bridge id)kSecAttrAccessGroup];
#endif
    }
}

//Given a set of attributes, deletes the matching keychain keys:
-(BOOL) deleteByAttributes: (NSDictionary*) attributes
                     error: (ADAuthenticationError* __autoreleasing*) error
{
    RETURN_NO_ON_NIL_ARGUMENT(attributes);

    NSMutableDictionary* query = [NSMutableDictionary dictionaryWithDictionary:attributes];
    [self addStandardAttributes:query];
    
    AD_LOG_VERBOSE_F(sKeyChainlog, @"Attempting to remove items that match attributes: %@", attributes);
    
    OSStatus res = SecItemDelete((__bridge CFDictionaryRef)query);
    switch (res)
    {
        case errSecSuccess:
            AD_LOG_VERBOSE_F(sKeyChainlog, @"Successfully removed any items that match: %@", attributes);
            return YES;
        case errSecItemNotFound:
            //It is expected: the item may be removed in parallel by another app, so no raising of error.
            AD_LOG_VERBOSE_F(sKeyChainlog, @"No items to remove. Searched for: %@", attributes);
            break;
        default:
        {
            //Couldn't extract the elements:
            NSString* errorDetails = [NSString stringWithFormat:@"Cannot the the items in the keychain. Error code: %ld. Items attempted: %@",
                                      (long)res, attributes];
            ADAuthenticationError* toReport = [ADAuthenticationError errorFromAuthenticationError:AD_ERROR_CACHE_PERSISTENCE
                                                                                     protocolCode:nil
                                                                                     errorDetails:errorDetails];
            if (error)
            {
                *error = toReport;
            }
        }
    }
    return NO;
}

-(BOOL) updateItemByAttributes: (NSDictionary*) attributes
                         value: (NSData*) value
                         error: (ADAuthenticationError* __autoreleasing*) error
{
    RETURN_NO_ON_NIL_ARGUMENT(attributes);
    RETURN_NO_ON_NIL_ARGUMENT(value);
    
    NSMutableDictionary* updatedAttributes = [NSMutableDictionary dictionaryWithDictionary:attributes];
    [self addStandardAttributes:updatedAttributes];
    
    OSStatus res = SecItemUpdate((__bridge CFMutableDictionaryRef)updatedAttributes,
                                 (__bridge CFDictionaryRef)@{ mValueDataKey:value });
    ADAuthenticationError* toReport = nil;
    switch(res)
    {
        case errSecSuccess:
            //All good
            return YES;
        case errSecItemNotFound:
        {
            NSString* errorDetails = [NSString stringWithFormat:@"Cannot update a keychain item, as it is not present anymore. Attributes: %@",
                                    attributes];
            toReport = [ADAuthenticationError errorFromAuthenticationError:AD_ERROR_CACHE_PERSISTENCE
                                                              protocolCode:nil
                                                              errorDetails:errorDetails];
            break;
        }
        default:
        {
            NSString* errorDetails = [NSString stringWithFormat:@"Cannot update the item in the keychain. Error code: %ld. Attributes: %@", (long)res,
                                      attributes];
            toReport = [ADAuthenticationError errorFromAuthenticationError:AD_ERROR_CACHE_PERSISTENCE
                                                              protocolCode:nil
                                                              errorDetails:errorDetails];
        }
    }
    
    if (error && toReport)
    {
        *error = toReport;
    }
    return NO;
}

-(NSArray*) getItemsAttributes: (NSDictionary*) query
                         error: (ADAuthenticationError* __autoreleasing*) error
{
    NSMutableDictionary* updatedQuery = [NSMutableDictionary new];
    if (query.count)//Query can be nil or empty
    {
        [updatedQuery addEntriesFromDictionary:query];
    }
    
    [self addStandardAttributes:updatedQuery];
    //Add the standard library values:
    [updatedQuery addEntriesFromDictionary:
     @{
       (__bridge id)kSecMatchLimit:(__bridge id)kSecMatchLimitAll,
       (__bridge id)kSecReturnAttributes:(__bridge id)kCFBooleanTrue,
       }];
    
    CFArrayRef all;
    OSStatus res = SecItemCopyMatching((__bridge CFMutableDictionaryRef)updatedQuery, (CFTypeRef*)&all);
    switch(res)
    {
        case errSecSuccess:
            //Success:
            return (__bridge_transfer NSArray*)all;
        case errSecItemNotFound:
            AD_LOG_VERBOSE_F(sKeyChainlog, @"No cache items found.");
            return [NSArray new];//Empty one
        default:
        {
            //Couldn't extract the elements:
            NSString* errorDetails = [NSString stringWithFormat:@"Cannot read the items in the keychain. Error code: %ld. Query: %@", (long)res, query];
            ADAuthenticationError* toReport = [ADAuthenticationError errorFromAuthenticationError:AD_ERROR_CACHE_PERSISTENCE
                                                                                     protocolCode:nil
                                                                                     errorDetails:errorDetails];
            if (error)
            {
                *error = toReport;
            }
            return nil;
        }
    }
}

-(BOOL) addItemWithAttributes: (NSDictionary*) attributes
                        value: (NSData*) value
                        error: (ADAuthenticationError* __autoreleasing*) error
{
    RETURN_NO_ON_NIL_ARGUMENT(attributes);
    RETURN_NO_ON_NIL_ARGUMENT(value);
    
    NSMutableDictionary* updatedAttributes = [NSMutableDictionary dictionaryWithDictionary:attributes];
    [self addStandardAttributes:updatedAttributes];
    
    [updatedAttributes addEntriesFromDictionary:
     @{
        (__bridge id)kSecAttrIsInvisible:(__bridge id)kCFBooleanTrue, // do not show in the keychain UI
        (__bridge id)kSecAttrAccessible:(__bridge id)kSecAttrAccessibleAfterFirstUnlockThisDeviceOnly, // do not roam or migrate to other devices
        mValueDataKey:value,//Item data
    }];
    
    OSStatus res = SecItemAdd((__bridge CFMutableDictionaryRef)updatedAttributes, NULL);
    if (errSecSuccess != res)
    {
        NSString* errorDetails = [NSString stringWithFormat:@"Cannot add a new item in the keychain. Error code: %ld. Attributes: %@", (long)res, attributes];
        ADAuthenticationError* toReport = [ADAuthenticationError errorFromAuthenticationError:AD_ERROR_CACHE_PERSISTENCE
                                                                                 protocolCode:nil
                                                                                 errorDetails:errorDetails];
        if (error)
        {
            *error = toReport;
        }
        return NO;
    }
    
    return YES;
}

-(BOOL) getItemWithAttributes: (NSDictionary*) attributes
                   returnData: (BOOL) returnData
                         item: (CFTypeRef*) item
                        error: (ADAuthenticationError* __autoreleasing*) error
{
    RETURN_NO_ON_NIL_ARGUMENT(attributes);
    
    //Set up the extraction query:
    NSMutableDictionary* updatedAttributes = [NSMutableDictionary dictionaryWithDictionary:attributes];
    [self addStandardAttributes:updatedAttributes];
    [updatedAttributes setObject:(__bridge id)kSecMatchLimitOne forKey:(__bridge id)kSecMatchLimit];
    if (returnData)
    {
        [updatedAttributes setObject:(__bridge id)kCFBooleanTrue forKey:(__bridge id<NSCopying>)kSecReturnData];
    }
    else
    {
        [updatedAttributes setObject:(__bridge id)kCFBooleanTrue forKey:(__bridge id<NSCopying>)kSecReturnRef];
    }
    
    OSStatus res = SecItemCopyMatching((__bridge CFMutableDictionaryRef)updatedAttributes, item);
    switch (res)
    {
        case errSecSuccess:
            return YES;
        case errSecItemNotFound:
            //This can happen in the case of shared keychain groups, where the item can be deleted by another app
            //while this application is working on accessing it:
            AD_LOG_WARN_F(sKeyChainlog, @"Cannot find item with attributes: %@", attributes);
            return NO;
        default:
        {
            NSString* errorDetails = [NSString stringWithFormat:@"Cannot read the data from the keychain. Error code: %ld. Attributes: %@",
                                      (long)res, attributes];
            ADAuthenticationError* toReport = [ADAuthenticationError errorFromAuthenticationError:AD_ERROR_CACHE_PERSISTENCE
                                                                                     protocolCode:nil
                                                                                     errorDetails:errorDetails];
            if (error)
            {
                *error = toReport;
            }
            return NO;
        }
    }
}

-(NSData*) getItemDataWithAttributes: (NSDictionary*) attributes
                               error: (ADAuthenticationError* __autoreleasing*) error
{
    RETURN_NIL_ON_NIL_ARGUMENT(attributes);
    
    CFTypeRef data;
    if (![self getItemWithAttributes:attributes returnData:YES item:&data error:error])
    {
        return nil;
    }
    
    return (__bridge_transfer NSData*)data;
}

-(CFTypeRef) getItemTypeRefWithAttributes: (NSDictionary*) attributes
                                    error: (ADAuthenticationError* __autoreleasing*) error
{
    RETURN_NIL_ON_NIL_ARGUMENT(attributes);
    CFTypeRef result;
    if (![self getItemWithAttributes:attributes returnData:NO item:&result error:error])
    {
        return NULL;
    }
    return result;
}

@end
