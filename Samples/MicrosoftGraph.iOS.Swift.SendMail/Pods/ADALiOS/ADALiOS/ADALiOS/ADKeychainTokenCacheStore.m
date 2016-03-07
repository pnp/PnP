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

#import <Security/Security.h>
#import "ADALiOS.h"
#import "ADKeyChainTokenCacheStore.h"
#import "ADTokenCacheStoreItem.h"
#import "NSString+ADHelperMethods.h"
#import "ADTokenCacheStoreKey.h"
#import "ADUserInformation.h"
#import "ADKeyChainHelper.h"

NSString* const sNilKey = @"CC3513A0-0E69-4B4D-97FC-DFB6C91EE132";//A special attribute to write, instead of nil/empty one.
NSString* const sDelimiter = @"|";
NSString* const sKeyChainlog = @"Keychain token cache store";
NSString* const sMultiUserError = @"The token cache store for this resource contain more than one user. Please set the 'userId' parameter to determine which one to be used.";
NSString* const sKeychainSharedGroup = @"com.microsoft.adalcache";

const long sKeychainVersion = 1;//will need to increase when we break the forward compatibility

@implementation ADKeychainTokenCacheStore
{
    //Cache store keys:
    id mItemKeyAttributeKey;
    id mUserIdKey;
    
    //Cache store values:
    id mClassValue;
    NSString* mLibraryString;
    NSData* mLibraryValue;//Data representation of the library string.

    ADKeyChainHelper* mHelper;
}

//Shouldn't be called.
-(id) init
{
    return [self initWithGroup:sKeychainSharedGroup];
}

-(id) initWithGroup: (NSString *)sharedGroup
{
    if (self = [super init])
    {
        //Full key:
        /* The keychain does allow searching for a limited set of attributes only,
         so we need to combine all of the ADTokenCacheStoreKey fields in a single string.*/
        mItemKeyAttributeKey   = (__bridge id)kSecAttrService;
        mUserIdKey             = (__bridge id)kSecAttrAccount;
        
        //Generic setup values:
        mClassValue     = (__bridge id)kSecClassGenericPassword;
        mLibraryString  = [NSString stringWithFormat:@"MSOpenTech.ADAL.%ld", sKeychainVersion];
        mLibraryValue   = [mLibraryString dataUsingEncoding:NSUTF8StringEncoding];
        
        mHelper = [[ADKeyChainHelper alloc] initWithClass:mClassValue
                                                  generic:mLibraryValue
                                              sharedGroup:sharedGroup];
    }
    return self;
}

//Extracts all of the key and user data fields into a single string.
//Used for comparison and verification that the item exists.
-(NSString*) fullKeychainKeyFromAttributes: (NSDictionary*)attributes
{
    THROW_ON_NIL_ARGUMENT(attributes);
    
    return [NSString stringWithFormat:@"%@%@%@",
            [attributes objectForKey:mItemKeyAttributeKey],
            sDelimiter,
            [attributes objectForKey:mUserIdKey]
            ];
}

//Given an item key, generates the string key used in the keychain:
-(NSString*) keychainKeyFromCacheKey: (ADTokenCacheStoreKey*) itemKey
{
    //The key contains all of the ADAL cache key elements plus the version of the
    //library. The latter is required to ensure that SecItemAdd won't break on collisions
    //with items left over from the previous versions of the library.
    return [NSString stringWithFormat:@"%@%@%@%@%@%@%@",
            mLibraryString, sDelimiter,
            [itemKey.authority adBase64UrlEncode], sDelimiter,
            [self.class getAttributeName:itemKey.resource], sDelimiter,
            [itemKey.clientId adBase64UrlEncode]
            ];
}


//Extracts the key text to be used to search explicitly for this item (without the user):
-(NSString*) keychainKeyFromCacheItem: (ADTokenCacheStoreItem*)item
                                error: (ADAuthenticationError* __autoreleasing*) error
{
    THROW_ON_NIL_ARGUMENT(item);
    ADTokenCacheStoreKey* key = [item extractKeyWithError:error];
    if (!key)
    {
        return nil;
    }
    
    return [self keychainKeyFromCacheKey:key];
}

//Same as extractKeychainKeyFromItem, but this time user is included
-(NSString*) fullKeychainKeyFromCacheItem: (ADTokenCacheStoreItem*)item
                                    error: (ADAuthenticationError* __autoreleasing*) error
{
    THROW_ON_NIL_ARGUMENT(item);
    
    NSString* keyText = [self keychainKeyFromCacheItem:item error:error];
    if (!keyText)
    {
        return nil;
    }

    return [NSString stringWithFormat:@"%@%@%@",
                       keyText, sDelimiter, [self.class getAttributeName:item.userInformation.userId]];
}

//Returns the keychain elements, specified in the query, or all cache keychain
//items if the query is nil. The keys in the returned dictionary are the full keychain key strings.
//The values are the attributes (as dictionaries) for the keychain items. These attributes
//can be used for further operations like deleting or retrieving contents.
-(NSMutableDictionary*) keychainAttributesWithQuery: (NSMutableDictionary*) query
                                              error: (ADAuthenticationError* __autoreleasing*)error
{
    NSArray* allAttributes = [mHelper getItemsAttributes:query error:error];
    if (!allAttributes)
    {
        return nil;
    }

    NSMutableDictionary* toReturn = [[NSMutableDictionary alloc] initWithCapacity:allAttributes.count];
    for(NSDictionary* dictionary in allAttributes)
    {
        NSString* key = [self fullKeychainKeyFromAttributes:dictionary];
        NSDictionary* first = [toReturn objectForKey:key];
        if (first != nil)
        {
            AD_LOG_INFO_F(sKeyChainlog, @"Duplicated keychain cache entry: %@. Picking the newest...", key);
            //Note that this can happen if the application has multiple keychain groups and the keychain group is
            //not specified explicitly:
            //Recover by picking the newest (based on modification date):
            NSDate* firstMod = [first objectForKey:(__bridge id)kSecAttrModificationDate];
            NSDate* secondMod = [dictionary objectForKey:(__bridge id)kSecAttrModificationDate];
            if (!firstMod || [secondMod compare:firstMod] == NSOrderedDescending)
            {
                [toReturn setObject:dictionary forKey:key];
            }
        }
        else
        {
            [toReturn setObject:dictionary forKey:key];
        }
    }
    return toReturn;
}

//Log operations that result in storing or reading cache item:
-(void) LogItem: (ADTokenCacheStoreItem*) item
        message: (NSString*) additionalMessage
{
    AD_LOG_VERBOSE_F(sKeyChainlog, @"%@. Resource: %@ Access token hash: %@; Refresh token hash: %@", item.resource,additionalMessage, [ADLogger getHash:item.accessToken], [ADLogger getHash:item.refreshToken]);
}

//Updates the keychain item. "attributes" parameter should ALWAYS come from previous
//SecItemCopyMatching else the function will fail.
-(void) updateKeychainItem: (ADTokenCacheStoreItem*) item
            withAttributes: (NSDictionary*) attributes /* The specific dictionary returned by previous SecItemCopyMatching call */
                     error: (ADAuthenticationError* __autoreleasing*) error
{
    RETURN_ON_NIL_ARGUMENT(item);
    RETURN_ON_NIL_ARGUMENT(attributes);

    [self LogItem:item message:@"Attempting to update an item"];
    if ([mHelper updateItemByAttributes:attributes
                                  value:[NSKeyedArchiver archivedDataWithRootObject:item]
                                  error:error])
    {
        [self LogItem:item message:@"Item successfully updated"];
    }
}

-(void) addKeychainItem: (ADTokenCacheStoreItem*) item
                  error: (ADAuthenticationError* __autoreleasing*) error
{
    RETURN_ON_NIL_ARGUMENT(item);

    [self LogItem:item message:@"Attempting to add an item"];

    NSString* keyText = [self keychainKeyFromCacheItem:item error:error];
    if (!keyText)
    {
        return;
    }
    
    NSMutableDictionary* keychainItem = [NSMutableDictionary dictionaryWithDictionary:@{
        mItemKeyAttributeKey: keyText,//Item key
        mUserIdKey:[self.class getAttributeName:item.userInformation.userId],
        }];
    if ([mHelper addItemWithAttributes:keychainItem
                                 value:[NSKeyedArchiver archivedDataWithRootObject:item]
                                 error:error])
    {
        [self LogItem:item message:@"Item successfully added"];
    }
}

//Extracts the item data from the keychain, based on the "attributes".
//Attributes can either be the result of another bulk get call or set to
//contain the full key of the item.
-(ADTokenCacheStoreItem*) readCacheItemWithAttributes: (NSDictionary*)attributes
                                                error: (ADAuthenticationError* __autoreleasing*)error
{
    RETURN_NIL_ON_NIL_ARGUMENT(attributes);
    
    NSData* data = [mHelper getItemDataWithAttributes:attributes error:error];
    if (!data)
    {
        return nil;
    }

    ADTokenCacheStoreItem* item = [NSKeyedUnarchiver unarchiveObjectWithData:data];
    if ([item isKindOfClass:[ADTokenCacheStoreItem class]])
    {
        //Verify that the item is valid:
        ADTokenCacheStoreKey* key = [item extractKeyWithError:error];
        if (!key)
        {
            return nil;
        }
        
        [self LogItem:item message:@"Item successfully read"];
        return item;
    }
    else
    {
        NSString* errorDetails = [NSString stringWithFormat:@"The key chain item data does not contain cache item. Attributes: %@",
                        attributes];
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

//We should not put nil keys in the keychain. The method substitutes nil with a special GUID:
+(NSString*) getAttributeName: (NSString*)original
{
    return ([NSString adIsStringNilOrBlank:original]) ? sNilKey : [original adBase64UrlEncode];
}

//Stores the passed items in the group. Does not explicitly remove items that are not there.
//The method is efficient only for bulk operations, as it pulls all items in the dictionary
//first to determine which ones are new and which ones are existing.
-(BOOL) persistWithItems: (NSArray*) flatItemsList
                   error: (ADAuthenticationError *__autoreleasing *) error
{
    ADAuthenticationError* toReport;
    //Get all items which are already in the cache:
    NSMutableDictionary* stored = [self keychainAttributesWithQuery:nil error:&toReport];
    if (stored)
    {
        //Add or update all passed items:
        for(ADTokenCacheStoreItem* item in flatItemsList)
        {
            NSString* fullKey = [self fullKeychainKeyFromCacheItem:item error:&toReport];
            if (!fullKey)
            {
                continue;
            }
            NSDictionary* storedAttributes = [stored objectForKey:fullKey];
            if (storedAttributes)
            {
                [stored removeObjectForKey:fullKey];//Clear, as it will be updated.
                //Update item:
                [self updateKeychainItem:item withAttributes:storedAttributes error:&toReport];
            }
            else
            {
                //Add the new item:
                [self addKeychainItem:item error:&toReport];
            }
        }
    }
  
    if (error && toReport)
    {
        *error = toReport;
    }
    return !toReport;//No error
}

//Internal method: returns a dictionary with all items that match the criteria.
//The keys are the keychain fullkey of the items; the values are the
//keychain attributes as extracted by SecItemCopyMatching. The attributes
//(represented as dictionaries) can be used to obtain the actual token cache item.
//May return nil in case of error.
//The method is not thread-safe.
-(NSDictionary*) keychainAttributesWithKey: (ADTokenCacheStoreKey*) key
                                    userId: (NSString*) userId
                                     error: (ADAuthenticationError* __autoreleasing*) error
{
    NSMutableDictionary* query = [NSMutableDictionary dictionaryWithDictionary:
                                  @{
                                    mItemKeyAttributeKey:[self keychainKeyFromCacheKey:key],
                                    }];
    
    if (![NSString adIsStringNilOrBlank:userId])
    {
        [query setObject:[userId adBase64UrlEncode] forKey:mUserIdKey];
    }
    
    return [self keychainAttributesWithQuery:query error:error];
}



//Internal method, used by getItemWithKey and getItemsWithKey public methods.
//The method is thread-safe.
-(NSArray*) readCacheItemsWithKey: (ADTokenCacheStoreKey*) key
                           userId: (NSString*) userId
                        allowMany: (BOOL) allowMany
                            error: (ADAuthenticationError *__autoreleasing *)error
{
    ADAuthenticationError* adError = nil;
    NSArray* toReturn = nil;
    if (key)
    {
        @synchronized(self)
        {
            NSDictionary* keyItemsAttributes = [self keychainAttributesWithKey:key userId:userId error:&adError];
            if (keyItemsAttributes)
            {
                if (!allowMany && keyItemsAttributes.count > 1)
                {
                    adError = [ADAuthenticationError errorFromAuthenticationError:AD_ERROR_MULTIPLE_USERS
                                                                     protocolCode:nil
                                                                     errorDetails:sMultiUserError];
                }
                else
                {
                    //Note that we may have an empty dictionary too:
                    NSMutableArray* array = [[NSMutableArray alloc] initWithCapacity:keyItemsAttributes.count];
                    for(NSDictionary* attributes in keyItemsAttributes.allValues)
                    {
                        ADTokenCacheStoreItem* item = [self readCacheItemWithAttributes:attributes
                                                                                  error:&adError];
                        if (item)
                        {
                            [array addObject:item];
                        }
                        else if (adError)//It is ok for the item to be nil, if it is not found
                        {
                            //Break on the first data issue:
                            array = nil;
                            break;
                        }
                    }
                    toReturn = array;
                }
            }
        }
    }
    else
    {
        adError = [ADAuthenticationError errorFromArgument:key argumentName:@"key"];
    }
    
    if (error && adError)
    {
        *error = adError;
    }
    return toReturn;
}

//Removes all items, specified by the passed dictionary.
//The dictionary contains item keychain keys (strings) as keys
//and metadata attributes as values (dictionaries). The latter are provided
//exactly as returned by SecItemCopyMatching function.
//keysAndAttributes can be nil.
-(void) removeWithAttributesDictionaries: (NSDictionary*) keysAndAttributes
                                   error: (ADAuthenticationError* __autoreleasing*) error
{
    if (!keysAndAttributes.count)
    {
        return;
    }
    for(NSDictionary* attributes in keysAndAttributes.allValues)
    {
        [mHelper deleteByAttributes:attributes error:error];
    }
}


//From ADTokenCacheStoring protocol
-(NSArray*) allItemsWithError:(ADAuthenticationError *__autoreleasing *)error
{
    API_ENTRY;
    
    NSMutableArray* toReturn = nil;
    ADAuthenticationError* adError;
    
    @synchronized(self)
    {
        //Read all stored keys, then extract the data (full cache item) for each key:
        NSMutableDictionary* all = [self keychainAttributesWithQuery:nil error:&adError];
        if (all)
        {
            toReturn = [[NSMutableArray alloc] initWithCapacity:all.count];
            for(NSDictionary* attributes in all.allValues)
            {
                ADTokenCacheStoreItem* item = [self readCacheItemWithAttributes:attributes error:&adError];//The error is always logged internally.
                if (item)
                {
                    [toReturn addObject:item];
                }
                else if (adError)
                {
                    toReturn = nil;//Break on error, but ignore item being null if no error is raised
                    break;
                }
            }
        }
    }
    
    if (error && adError)
    {
        *error = adError;
    }
    return toReturn;
}

//From ADTokenCacheStoring protocol
-(ADTokenCacheStoreItem*) getItemWithKey: (ADTokenCacheStoreKey*)key
                                  userId: (NSString*) userId
                                   error: (ADAuthenticationError *__autoreleasing *)error
{
    API_ENTRY;

    userId = [ADUserInformation normalizeUserId:userId];
    NSArray* items = [self readCacheItemsWithKey:key userId:userId allowMany:NO error:error];
    
    return items.count ? items.firstObject : nil;
}

//From ADTokenCacheStoring protocol
-(NSArray*) getItemsWithKey: (ADTokenCacheStoreKey*)key
                      error: (ADAuthenticationError* __autoreleasing*) error
{
    API_ENTRY;
    
    return [self readCacheItemsWithKey:key userId:nil allowMany:YES error:error];
}

/*! Extracts the key from the item and uses it to set the cache details. If another item with the
 same key exists, it will be overriden by the new one. 'getItemWithKey' method can be used to determine
 if an item already exists for the same key.
 @param error: in case of an error, if this parameter is not nil, it will be filled with
 the error details. */
-(void) addOrUpdateItem: (ADTokenCacheStoreItem*) item
                  error: (ADAuthenticationError* __autoreleasing*) error
{
    API_ENTRY;
    @synchronized(self)
    {
        ADTokenCacheStoreKey* key = [item extractKeyWithError:error];
        if (!key)
        {
            return;
        }
        NSDictionary* allAttributes = [self keychainAttributesWithKey:key userId:item.userInformation.userId error:error];
        NSString* keychainKey = [self fullKeychainKeyFromCacheItem:item error:error];
        if (!keychainKey)
        {
            return;
        }
        NSDictionary* attributes = [allAttributes objectForKey:keychainKey];
        if (attributes)
        {
            [self updateKeychainItem:item withAttributes:attributes error:error];
        }
        else
        {
            [self addKeychainItem:item error:error];
        }
    }
}

//From ADTokenCacheStoring protocol
-(void) removeItemWithKey: (ADTokenCacheStoreKey*) key
                   userId: (NSString*) userId
                    error: (ADAuthenticationError* __autoreleasing* ) error
{
    API_ENTRY;
    
    if (!key)
    {
        return;
    }
    
    userId = [ADUserInformation normalizeUserId:userId];
    
    @synchronized(self)
    {
        NSDictionary* allAttributes = [self keychainAttributesWithKey:key userId:userId error:error];
        if (allAttributes)
        {
            [self removeWithAttributesDictionaries:allAttributes error:error];
        }
    }
}

-(void) removeAllWithError:(ADAuthenticationError *__autoreleasing *)error
{
    API_ENTRY;
    @synchronized(self)
    {
        NSDictionary* allAttributes = [self keychainAttributesWithQuery:nil error:error];
        if (allAttributes)
        {
            [self removeWithAttributesDictionaries:allAttributes error:error];
        }
    }
}

-(NSString*) getSharedGroup
{
    return mHelper.sharedGroup;
}

-(void) setSharedGroup:(NSString *)sharedGroup
{
    API_ENTRY;
    @synchronized(self)
    {
        if (![NSString adSame:mHelper.sharedGroup toString:sharedGroup])
        {
            mHelper.sharedGroup = sharedGroup;
        }
    }
}



@end
