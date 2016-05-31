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

@class ADTokenCacheStoreKey;
@class ADTokenCacheStoreItem;
@class ADAuthenticationError;

/*! This protocol needs to be implemented by any token cache store.
 It is a key-based store, which stores 'AdTokenCacheStoreItem elements. */
@protocol ADTokenCacheStoring

/*! Return a copy of all items. The array will contain ADTokenCacheStoreItem objects,
 containing all of the cached information. Returns an empty array, if no items are found.
 Returns nil in case of error. */
-(NSArray*) allItemsWithError:(ADAuthenticationError* __autoreleasing*) error;

/*! May return nil, if no cache item corresponds to the requested key
 @param key: The key of the item.
 @param user: The specific user whose item is needed. May be nil, in which
 case the item for the first user in the cache will be returned. 
 @param error: Will be set only in case of ambiguity. E.g. if userId is nil
 and we have tokens from multiple users. If the cache item is not present,
 the error will not be set. */
-(ADTokenCacheStoreItem*) getItemWithKey: (ADTokenCacheStoreKey*)key
                                  userId: (NSString*) userId
                                   error: (ADAuthenticationError* __autoreleasing*) error;

/*! Returns all of the items for a given key. Multiple items may present,
 if the same resource was accessed by more than one user. The returned
 array should contain only ADTokenCacheStoreItem objects. Returns an empty array,
 if no items are found. Returns nil (and sets the error parameter) in case of error.*/
-(NSArray*) getItemsWithKey: (ADTokenCacheStoreKey*)key
                      error: (ADAuthenticationError* __autoreleasing*) error;

/*! Extracts the key from the item and uses it to set the cache details. If another item with the
 same key exists, it will be overriden by the new one. 'getItemWithKey' method can be used to determine
 if an item already exists for the same key.
 @param error: in case of an error, if this parameter is not nil, it will be filled with
 the error details. */
-(void) addOrUpdateItem: (ADTokenCacheStoreItem*) item
                  error: (ADAuthenticationError* __autoreleasing*) error;

/*! Clears token cache details for specific keys.
 @param key: the key of the cache item. Key can be extracted from the ADTokenCacheStoreItem using
 the method 'extractKeyWithError'
 @param userId: The user for which the item will be removed. Can be nil, in which case items for all users with
 the specified key will be removed.
 The method does not raise an error, if the item is not found.
*/
-(void) removeItemWithKey: (ADTokenCacheStoreKey*) key
                   userId: (NSString*) userId
                    error: (ADAuthenticationError* __autoreleasing*) error;

/*! Clears the whole cache store. The method does not raise an error if there are no items in the cache. */
-(void) removeAllWithError: (ADAuthenticationError* __autoreleasing*) error;

@end
