//
//  ADTokenCacheValue.h
//  ADALiOS
//
//  Created by Boris Vidolov on 10/15/13.
//  Copyright (c) 2013 MS Open Tech. All rights reserved.
//

#import <Foundation/Foundation.h>

/*! Encapsulates the values of the token cache */
@interface ADTokenCacheValue : NSObject

/*! An access token that can be used directly for the resource if it
 has not expired. See the expirationTime for the latter. */
@property NSString* accessToken;

/*! In case of expiration the supplied refreshToken can be used to obtain
 an access token. This property can be null if the server did not supply
 a refresh token*/
@property NSString* refreshToken;

/*! Defines until when the access token is valid */
@property NSDate* expirationTime;

/* The user whose credentials have been used to extract the token.
 This property is used for sign off scenarios, where the cache needs to be
 cleaned for a specific user */
@property NSDate* user;

@end

