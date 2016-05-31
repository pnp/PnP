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

@class ADAuthenticationError;
@class ADClientMetrics;

/*! The completion block declaration. */
typedef void(^ADDiscoveryCallback)(BOOL validated, ADAuthenticationError* error);

/*! A singleton class, used to validate authorities with in-memory caching of the previously validated ones.
 The class is thread-safe. */
@interface ADInstanceDiscovery : NSObject
{
    NSMutableSet* mValidatedAuthorities;
}

@property (readonly, getter = getValidatedAuthorities) NSSet* validatedAuthorities;

/*! The shared instance of the class. Attempt to create the class with new or init will throw an exception.*/
+(ADInstanceDiscovery*) sharedInstance;

/*! Validates asynchronously the provided authority. Caches the validations in in-memory cache.
 @param authority: the authority to be validated. ADFS authority instances cannot be validated.
 @param correlationId: a special UUID sent out with the validation request. This UUID can be useful in case
 of calling support to track unexpected failures. This parameter may be null, in which case the method will generate a new UUID.
 @param completionBlock: the block to be called when the result is achieved.*/
-(void) validateAuthority: (NSString*) authority
            correlationId: (NSUUID*) correlationId
          completionBlock: (ADDiscoveryCallback) completionBlock;

/*! Takes the string and makes it canonical URL, e.g. lowercase with
 ending trailing "/". If the authority is not a valid URL, the method
 will return nil. */
+(NSString*) canonicalizeAuthority: (NSString*) authority;

@end
