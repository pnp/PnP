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

#import "ADUserInformation.h"
#import "ADALiOS.h"
#import "ADOAuth2Constants.h"
#import "NSString+ADHelperMethods.h"

NSString* const ID_TOKEN_SUBJECT = @"sub";
NSString* const ID_TOKEN_TENANTID = @"tid";
NSString* const ID_TOKEN_UPN = @"upn";
NSString* const ID_TOKEN_GIVEN_NAME = @"given_name";
NSString* const ID_TOKEN_FAMILY_NAME = @"family_name";
NSString* const ID_TOKEN_UNIQUE_NAME = @"unique_name";
NSString* const ID_TOKEN_EMAIL = @"email";
NSString* const ID_TOKEN_IDENTITY_PROVIDER = @"idp";
NSString* const ID_TOKEN_TYPE = @"typ";
NSString* const ID_TOKEN_JWT_TYPE = @"JWT";
NSString* const ID_TOKEN_OBJECT_ID = @"oid";
NSString* const ID_TOKEN_GUEST_ID = @"altsecid";

@implementation ADUserInformation

-(id) init
{
    //Throws, as this init function should not be used
    [super doesNotRecognizeSelector:_cmd];
    return nil;
}

+(NSString*) normalizeUserId: (NSString*) userId
{
    if (!userId)
    {
        return nil;//Quick exit;
    }
    NSString* normalized = [userId adTrimmedString].lowercaseString;
        
    return normalized.length ? normalized : nil;
}

-(id) initWithUserId: (NSString*) userId
{
    THROW_ON_NIL_EMPTY_ARGUMENT(userId);//Shouldn't be called with nil.
    self = [super init];
    if (self)
    {
        //Minor canonicalization of the userId:
        _userId = [self.class normalizeUserId:userId];
    }
    return self;
}

#define RETURN_ID_TOKEN_ERROR(text) \
{ \
    ADAuthenticationError* idTokenError = [self errorFromIdToken:text]; \
    if (error) \
    { \
        *error = idTokenError; \
    } \
    return nil; \
}


-(ADAuthenticationError*) errorFromIdToken: (NSString*) idTokenText
{
    THROW_ON_NIL_ARGUMENT(idTokenText);
    return [ADAuthenticationError errorFromAuthenticationError:AD_ERROR_AUTHENTICATION protocolCode:nil errorDetails:[NSString stringWithFormat: @"The id_token contents cannot be parsed: %@", idTokenText]];
}

-(id) initWithIdToken: (NSString*) idToken
                error: (ADAuthenticationError* __autoreleasing*) error
{
    THROW_ON_NIL_ARGUMENT(idToken);
    self = [super init];
    if (!self)
    {
        return nil;
    }

    if ([NSString adIsStringNilOrBlank:idToken])
    {
        RETURN_ID_TOKEN_ERROR(idToken);
    }
    
    _rawIdToken = idToken;
    NSMutableDictionary* allClaims = [NSMutableDictionary new];
    
    NSArray* parts = [idToken componentsSeparatedByCharactersInSet:[NSCharacterSet characterSetWithCharactersInString:@"."]];
    if (parts.count < 1)
    {
        RETURN_ID_TOKEN_ERROR(idToken);
    }
    
    NSString* type = nil;
    for (NSString* part in parts)
    {
        AD_LOG_VERBOSE(@"Id_token part", part);
        NSString* decoded = [part adBase64UrlDecode];
        if (![NSString adIsStringNilOrBlank:decoded])
        {
            NSError* jsonError  = nil;
            id jsonObject = [NSJSONSerialization JSONObjectWithData:[decoded dataUsingEncoding:NSUTF8StringEncoding]
                                                            options:0
                                                              error:&jsonError];
                if (jsonError)
                {
                    ADAuthenticationError* adError = [ADAuthenticationError errorFromNSError:jsonError
                                                                                errorDetails:[NSString stringWithFormat:@"Failed to deserialize the id_token contents: %@", part]];
                    if (error)
                    {
                        *error = adError;
                    }
                    return nil;
                }
            
            if (![jsonObject isKindOfClass:[NSDictionary class]])
            {
                RETURN_ID_TOKEN_ERROR(part);
            }
            
            NSDictionary* contents = (NSDictionary*)jsonObject;
            if (!type)
            {
                type = [contents objectForKey:ID_TOKEN_TYPE];
                if (type)
                {
                    //Type argument is passed, check if it is the expected one
                    if (![ID_TOKEN_JWT_TYPE isEqualToString:type])
                    {
                        //Log it, but still try to use it as if it was a JWT token
                        AD_LOG_WARN(@"Incompatible id_token type.", type);
                    }
                }
            }

            [allClaims addEntriesFromDictionary:contents];
        }
    }
    if (!type)
    {
        AD_LOG_WARN(@"The id_token type is missing.", @"Assuming JWT type.");
    }
    
    //Create a read-only dictionary object. Note that the properties checked below are calculated off this dictionary:
    _allClaims = [NSDictionary dictionaryWithDictionary:allClaims];
    
    //Now attempt to extract an unique user id:
    if (![NSString adIsStringNilOrBlank:self.upn])
    {
        _userId = self.upn;
        _userIdDisplayable = YES;
    }
    else if (![NSString adIsStringNilOrBlank:self.eMail])
    {
        _userId = self.eMail;
        _userIdDisplayable = YES;
    }
    else if (![NSString adIsStringNilOrBlank:self.subject])
    {
        _userId = self.subject;
    }
    else if (![NSString adIsStringNilOrBlank:self.userObjectId])
    {
        _userId = self.userObjectId;
    }
    else if (![NSString adIsStringNilOrBlank:self.uniqueName])
    {
        _userId = self.uniqueName;
        _userIdDisplayable = YES;//This is what the server provided
    }
    else if (![NSString adIsStringNilOrBlank:self.guestId])
    {
        _userId = self.guestId;
    }
    else
    {
        RETURN_ID_TOKEN_ERROR(idToken);
    }
    _userId = [self.class normalizeUserId:_userId];
    
    return self;
}

//Declares a propperty getter, which extracts the property from the claims dictionary
#define ID_TOKEN_PROPERTY_GETTER(property, claimName) \
-(NSString*) get##property \
{ \
    return [self.allClaims objectForKey:claimName]; \
}

ID_TOKEN_PROPERTY_GETTER(GivenName, ID_TOKEN_GIVEN_NAME);
ID_TOKEN_PROPERTY_GETTER(FamilyName, ID_TOKEN_FAMILY_NAME);
ID_TOKEN_PROPERTY_GETTER(Subject, ID_TOKEN_SUBJECT);
ID_TOKEN_PROPERTY_GETTER(TenantId, ID_TOKEN_TENANTID);
ID_TOKEN_PROPERTY_GETTER(Upn, ID_TOKEN_UPN);
ID_TOKEN_PROPERTY_GETTER(UniqueName, ID_TOKEN_UNIQUE_NAME);
ID_TOKEN_PROPERTY_GETTER(EMail, ID_TOKEN_EMAIL);
ID_TOKEN_PROPERTY_GETTER(IdentityProvider, ID_TOKEN_IDENTITY_PROVIDER);
ID_TOKEN_PROPERTY_GETTER(UserObjectId, ID_TOKEN_OBJECT_ID);
ID_TOKEN_PROPERTY_GETTER(GuestId, ID_TOKEN_GUEST_ID);

+(ADUserInformation*) userInformationWithUserId: (NSString*) userId
                                          error: (ADAuthenticationError* __autoreleasing*) error
{
    RETURN_NIL_ON_NIL_EMPTY_ARGUMENT(userId);
    ADUserInformation* userInfo = [[ADUserInformation alloc] initWithUserId:userId];
    return userInfo;
}

+(ADUserInformation*) userInformationWithIdToken: (NSString*) idToken
                                           error: (ADAuthenticationError* __autoreleasing*) error
{
    RETURN_NIL_ON_NIL_ARGUMENT(idToken);
    
    return [[ADUserInformation alloc] initWithIdToken:idToken error:error];
}

-(id) copyWithZone:(NSZone*) zone
{
    //Deep copy. Note that the user may have passed NSMutableString objects, so all of the objects should be copied:
    ADUserInformation* info = [[ADUserInformation allocWithZone:zone] initWithUserId:self.userId];
    info->_userIdDisplayable  = self.userIdDisplayable;
    info->_rawIdToken       = [self.rawIdToken copyWithZone:zone];
    info->_allClaims        = [self.allClaims copyWithZone:zone];
    
    return info;
}

+(BOOL) supportsSecureCoding
{
    return YES;
}

//Serialize:
-(void) encodeWithCoder:(NSCoder *)aCoder
{
    [aCoder encodeObject:self.userId forKey:@"userId"];
    [aCoder encodeBool:self.userIdDisplayable forKey:@"userIdDisplayable"];
    [aCoder encodeObject:self.rawIdToken forKey:@"rawIdToken"];
    [aCoder encodeObject:self.allClaims forKey:@"allClaims"];
}

//Deserialize:
-(id) initWithCoder:(NSCoder *) aDecoder
{
    NSString* storedUserId      = [aDecoder decodeObjectOfClass:[NSString class] forKey:@"userId"];
    if ([NSString adIsStringNilOrBlank:storedUserId])
    {
        //The userId should be valid:
        AD_LOG_ERROR_F(@"Invalid user information", AD_ERROR_BAD_CACHE_FORMAT, @"Invalid userId: %@", storedUserId);
        
        return nil;
    }
    self = [self initWithUserId:storedUserId];
    if (self)
    {
        _userIdDisplayable  = [aDecoder decodeBoolForKey:@"userIdDisplayable"];
        _rawIdToken             = [aDecoder decodeObjectOfClass:[NSString class] forKey:@"rawIdToken"];
        _allClaims              = [aDecoder decodeObjectOfClass:[NSDictionary class] forKey:@"allClaims"];
    }
    
    return self;
}

@end
