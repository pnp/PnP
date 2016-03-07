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

#import "ADWorkPlaceJoinUtil.h"
#import "ADRegistrationInformation.h"
#import "ADWorkPlaceJoinConstants.h"
#import "ADLogger.h"
#import "ADErrorCodes.h"

@implementation ADWorkPlaceJoinUtil

ADWorkPlaceJoinUtil* wpjUtilManager = nil;

+ (ADWorkPlaceJoinUtil*) WorkPlaceJoinUtilManager;
{
    if (!wpjUtilManager)
    {
        wpjUtilManager = [[self alloc] init];
    }
    
    return wpjUtilManager;
}

- (NSData *)getPrivateKeyForAccessGroup: (NSString*) sharedAccessGroup
                   privateKeyIdentifier: (NSString*) privateKey
                                  error: (NSError**) error
{
    AD_LOG_VERBOSE_F(@"Getting private key - ", @"%@ shared access Group", sharedAccessGroup);
    OSStatus status = noErr;
    CFDataRef item = NULL;
    NSData *keyData = nil;
    
    NSMutableDictionary *privateKeyAttr = [[NSMutableDictionary alloc] init];
    
    NSData *privateKeyTag = [NSData dataWithBytes:[privateKey UTF8String] length:privateKey.length];
    
    [privateKeyAttr setObject:privateKeyTag forKey:(__bridge id)kSecAttrApplicationTag];
    [privateKeyAttr setObject:(__bridge id)kSecClassKey forKey:(__bridge id)kSecClass];
    [privateKeyAttr setObject:(__bridge id)(kSecAttrKeyTypeRSA) forKey:(__bridge id<NSCopying>)(kSecAttrKeyType)];
    [privateKeyAttr setObject:(__bridge id)kCFBooleanTrue forKey:(__bridge id<NSCopying>)(kSecReturnData)];
#if !TARGET_IPHONE_SIMULATOR
    [privateKeyAttr setObject:sharedAccessGroup forKey:(__bridge id)kSecAttrAccessGroup];
#endif
    
    status = SecItemCopyMatching((__bridge CFDictionaryRef)privateKeyAttr, (CFTypeRef*)&item);
    
    if(item != NULL)
    {
        keyData = (__bridge_transfer NSData*)item;
    }
    else if (status != errSecSuccess)
    {
        if (*error != NULL)
        {
            *error = [self buildNSErrorForDomain:errorDomain
                                       errorCode:sharedKeychainPermission
                                    errorMessage: [NSString stringWithFormat:unabletoReadFromSharedKeychain, sharedAccessGroup]
                                 underlyingError:nil
                                     shouldRetry:false];
        }
    }
    
    return keyData;
}



- (ADRegistrationInformation*)getRegistrationInformation: (NSString*) sharedAccessGroup
                                                   error: (NSError**) error
{
    AD_LOG_VERBOSE_F(@"Attempting to get registration information - ", @"%@ shared access Group", sharedAccessGroup);
    
    SecIdentityRef identity = NULL;
    SecCertificateRef certificate = NULL;
    SecKeyRef privateKey = NULL;
    NSString *certificateSubject = nil;
    NSData *certificateData = nil;
    NSData *privateKeyData = nil;
    NSString *certificateIssuer = nil;
    NSString *userPrincipalName = nil;
    error = nil;
    
    NSMutableDictionary *identityAttr = [[NSMutableDictionary alloc] init];
    [identityAttr setObject:(__bridge id)kSecClassIdentity forKey:(__bridge id)kSecClass];
    [identityAttr setObject:(__bridge id)kCFBooleanTrue forKey:(__bridge id<NSCopying>)(kSecReturnRef)];
    [identityAttr setObject:(__bridge id) kSecAttrKeyClassPrivate forKey:(__bridge id)kSecAttrKeyClass];
    [identityAttr setObject:(__bridge id)kCFBooleanTrue forKey:(__bridge id<NSCopying>)(kSecReturnAttributes)];
    
#if !TARGET_IPHONE_SIMULATOR
    [identityAttr setObject:sharedAccessGroup forKey:(__bridge id)kSecAttrAccessGroup];
#endif
    
    CFDictionaryRef  result;
    OSStatus status = noErr;
    //get the issuer information
    status = SecItemCopyMatching((__bridge CFDictionaryRef)identityAttr, (CFTypeRef *) &result);
    
    if (status == noErr) {
        NSDictionary *  cerDict = (__bridge NSDictionary *) result;
        assert([cerDict isKindOfClass:[NSDictionary class]]);
        NSData* issuer = [cerDict objectForKey:(__bridge id)kSecAttrIssuer];
        certificateIssuer = [[NSString alloc] initWithData:issuer encoding:NSISOLatin1StringEncoding];
        CFRelease(result);
    } else {
        NSLog(@"error %d", (int) status);
    }
    
    // now get the identity out and use it.
    [identityAttr removeObjectForKey:(__bridge id<NSCopying>)(kSecReturnAttributes)];
    status = SecItemCopyMatching((__bridge CFDictionaryRef)identityAttr, (CFTypeRef*)&identity);
    
    //Get the identity
    if(status == errSecSuccess && identity)
    {
        AD_LOG_VERBOSE(@"Found identity in keychain", nil);
        //Get the certificate and data
        SecIdentityCopyCertificate(identity, &certificate);
        if(certificate)
        {
            AD_LOG_VERBOSE(@"Found certificate in keychain", nil);
            certificateSubject = (NSString *)CFBridgingRelease(SecCertificateCopySubjectSummary(certificate));
            certificateData = (NSData *)CFBridgingRelease(SecCertificateCopyData(certificate));
        }
        
        //Get the private key and data
        status = SecIdentityCopyPrivateKey(identity, &privateKey);
        if (status != errSecSuccess)
        {
            return nil;
        }
        
    }
    
    if(identity && certificate && certificateSubject && certificateData && privateKey && certificateIssuer)
    {
        ADRegistrationInformation *info = [[ADRegistrationInformation alloc] initWithSecurityIdentity:identity
                                                                                    userPrincipalName:userPrincipalName
                                                                                    certificateIssuer:certificateIssuer
                                                                                          certificate:certificate
                                                                                   certificateSubject:certificateSubject
                                                                                      certificateData:certificateData
                                                                                           privateKey:privateKey
                                                                                       privateKeyData:privateKeyData];
        return info;
    }
    else
    {
        AD_LOG_VERBOSE_F(@"Unable to extract a workplace join identity for", @"%@ shared access keychain",
                         sharedAccessGroup);        
        return nil;
    }
}

- (NSError*)getCertificateForAccessGroup: (NSString*)sharedAccessGroup
                                identity: (SecIdentityRef*) identity
                             certificate: (SecCertificateRef*) clientCertificate
{
    NSMutableDictionary *identityAttr = [[NSMutableDictionary alloc] init];
    [identityAttr setObject:(__bridge id)kSecClassIdentity forKey:(__bridge id)kSecClass];
    [identityAttr setObject:(__bridge id)kCFBooleanTrue forKey:(__bridge id<NSCopying>)(kSecReturnRef)];
    [identityAttr setObject:(__bridge id) kSecAttrKeyClassPrivate forKey:(__bridge id)kSecAttrKeyClass];
    
    
#if !TARGET_IPHONE_SIMULATOR
    [identityAttr setObject:sharedAccessGroup forKey:(__bridge id)kSecAttrAccessGroup];
#endif
    
    SecItemCopyMatching((__bridge CFDictionaryRef)identityAttr, (CFTypeRef*)identity);
    
    OSStatus status = SecIdentityCopyCertificate(*identity, clientCertificate );
    
    if (status == errSecSuccess)
    {
        return nil;
    }
    else
    {
        return [self buildNSErrorForDomain:errorDomain
                                 errorCode:sharedKeychainPermission
                              errorMessage: [NSString stringWithFormat:unabletoReadFromSharedKeychain, sharedAccessGroup]
                           underlyingError:nil
                               shouldRetry:false];
    }
    
    
}


- (NSError*) buildNSErrorForDomain:(NSString*)domain
                         errorCode:(NSInteger) errorCode
                      errorMessage:(NSString*) message
                   underlyingError:(NSError*) underlyingError
                       shouldRetry:(BOOL) retry
{
    NSMutableDictionary* details = [NSMutableDictionary dictionary];
    [details setValue:message forKey:NSLocalizedDescriptionKey];
    
    if (underlyingError != nil)
    {
        [details setValue:underlyingError forKey:NSUnderlyingErrorKey];
    }
    
    if (retry)
    {
        [details setValue:@"retry" forKey:NSLocalizedRecoverySuggestionErrorKey];
    }
    
    
    NSError *error = [NSError errorWithDomain:domain code:errorCode userInfo:details];
    return error;
}

- (NSData *)base64DataFromString: (NSString *)string
{
    unsigned char ch, accumulated[BASE64QUANTUMREP], outbuf[BASE64QUANTUM];
    const unsigned char *charString;
    NSMutableData *theData;
    const int OUTOFRANGE = 64;
    const unsigned char LASTCHARACTER = '=';
    
    if (string == nil)
    {
        return [NSData data];
    }
    
    for (int i = 0; i < BASE64QUANTUMREP; i++) {
        accumulated[i] = 0;
    }
    
    charString = (const unsigned char *)[string UTF8String];
    
    theData = [NSMutableData dataWithCapacity: [string length]];
    
    short accumulateIndex = 0;
    for (int index = 0; index < [string length]; index++) {
        
        ch = decodeBase64[charString [index]];
        
        if (ch < OUTOFRANGE)
        {
            short ctcharsinbuf = BASE64QUANTUM;
            
            if (charString [index] == LASTCHARACTER)
            {
                if (accumulateIndex == 0)
                {
                    break;
                }
                else if (accumulateIndex <= 2)
                {
                    ctcharsinbuf = 1;
                }
                else
                {
                    ctcharsinbuf = 2;
                }
                
                accumulateIndex = BASE64QUANTUM;
            }
            //
            // Accumulate 4 valid characters (ignore everything else)
            //
            accumulated [accumulateIndex++] = ch;
            
            //
            // Store the 6 bits from each of the 4 characters as 3 bytes
            //
            if (accumulateIndex == BASE64QUANTUMREP)
            {
                accumulateIndex = 0;
                
                outbuf[0] = (accumulated[0] << 2) | ((accumulated[1] & 0x30) >> 4);
                outbuf[1] = ((accumulated[1] & 0x0F) << 4) | ((accumulated[2] & 0x3C) >> 2);
                outbuf[2] = ((accumulated[2] & 0x03) << 6) | (accumulated[3] & 0x3F);
                
                for (int i = 0; i < ctcharsinbuf; i++)
                {
                    [theData appendBytes: &outbuf[i] length: 1];
                }
            }
            
        }
        
    }
    
    return theData;
}

- (NSString*)getApplicationIdentifierPrefix{
    
    AD_LOG_VERBOSE(@"Looking for application identifier prefix in app data", nil);
    NSUserDefaults* c = [NSUserDefaults standardUserDefaults];
    NSString* appIdentifierPrefix = [c objectForKey:applicationIdentifierPrefix];
    
    if (!appIdentifierPrefix)
    {
        appIdentifierPrefix = [self bundleSeedID];
        
        AD_LOG_VERBOSE(@"Storing application identifier prefix in app data", nil);
        NSUserDefaults* c = [NSUserDefaults standardUserDefaults];
        [c setObject:appIdentifierPrefix forKey:applicationIdentifierPrefix];
        [c synchronize];
    }
    
    return appIdentifierPrefix;
}

- (NSString*)bundleSeedID {
    NSDictionary *query = [NSDictionary dictionaryWithObjectsAndKeys:
                           (__bridge id)(kSecClassGenericPassword), kSecClass,
                           @"bundleSeedID", kSecAttrAccount,
                           @"", kSecAttrService,
                           (id)kCFBooleanTrue, kSecReturnAttributes,
                           nil];
    CFDictionaryRef result = nil;
    OSStatus status = SecItemCopyMatching((__bridge CFDictionaryRef)query, (CFTypeRef *)&result);
    if (status == errSecItemNotFound)
        status = SecItemAdd((__bridge CFDictionaryRef)query, (CFTypeRef *)&result);
    if (status != errSecSuccess)
        return nil;
    NSString *accessGroup = [(__bridge NSDictionary *)result objectForKey:(__bridge id)(kSecAttrAccessGroup)];
    NSArray *components = [accessGroup componentsSeparatedByString:@"."];
    NSString *bundleSeedID = [[components objectEnumerator] nextObject];
    SecItemDelete((__bridge CFDictionaryRef)(query));
    
    CFRelease(result);
    return bundleSeedID;
}

@end

