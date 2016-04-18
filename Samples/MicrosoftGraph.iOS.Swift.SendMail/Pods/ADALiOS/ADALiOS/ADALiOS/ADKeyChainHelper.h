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
#import "ADAuthenticationError.h"

/*! Internal helper class for keychain operations. 
 The class is not thread-safe. */
@interface ADKeyChainHelper : NSObject

/*! Initializes the object. The default initializer is not supported. 
 Only classValue is required as it identifies the item type. */
-(id) initWithClass: (id) classValue
            generic: (NSData*) generic
        sharedGroup: (NSString*) sharedGroup;

//The type of the keychain item. Should not be nil:
@property (readonly) id   classValue;

//Some generic data to identify the items. Can be nil.
@property NSData*         genericValue;

//Shared keychain group. Can be nil.
@property NSString*       sharedGroup;


/*! Returns the attributes (as dictionary values in the array) of the items that match the query.
 If query is nil or empty dictionary, all items in the keychain of the specifed type, with the
 expected generic value and key chain group are returned. */
-(NSArray*) getItemsAttributes: (NSDictionary*) query
                         error: (ADAuthenticationError* __autoreleasing*) error;

/*! Extracts the data stored for the item, using the passed attributes to identify the item. */
-(NSData*) getItemDataWithAttributes: (NSDictionary*) attributes
                               error: (ADAuthenticationError* __autoreleasing*) error;

/*! Extracts an item as a reference, specified by the "attributes" parameter. */
-(CFTypeRef) getItemTypeRefWithAttributes: (NSDictionary*) attributes
                                    error: (ADAuthenticationError* __autoreleasing*) error;

/*! Deletes an item, specified by the passed attributes. Returns YES, if a real
 deletion occurred. Does not raise an error if the keychain item is not present anymore. 
 @param attributes: The attributes, as returned by SecItemCopyMatching (wrapped by getItemsWithAttributes). */
-(BOOL) deleteByAttributes: (NSDictionary*) attributes
                     error: (ADAuthenticationError* __autoreleasing*) error;

-(BOOL) updateItemByAttributes: (NSDictionary*) attributes
                         value: (NSData*) value
                         error: (ADAuthenticationError* __autoreleasing*) error;


-(BOOL) addItemWithAttributes: (NSDictionary*) attributes
                        value: (NSData*) value
                        error: (ADAuthenticationError* __autoreleasing*) error;

@end
