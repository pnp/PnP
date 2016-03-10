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
#import "ADAuthenticationParameters.h"
#import "ADAuthenticationParameters+Internal.h"

NSString* const OAuth2_Bearer  = @"Bearer";
NSString* const OAuth2_Authenticate_Header = @"WWW-Authenticate";
NSString* const OAuth2_Authorization_Uri  = @"authorization_uri";
NSString* const OAuth2_Resource_Id = @"resource_id";

NSString* const MissingHeader = @"The authentication header '%@' is missing in the Unauthorized (401) response. Make sure that the resouce server supports OAuth2 protocol.";
NSString* const MissingOrInvalidAuthority = @"The authentication header '%@' in the Unauthorized (401) response does not contain valid '%@' parameter. Make sure that the resouce server supports OAuth2 protocol.";
NSString* const InvalidHeader = @"The authentication header '%@' for the Unauthorized (401) response does cannot be parsed. Header value: %@";
NSString* const ConnectionError = @"Connection error: %@";
NSString* const InvalidResponse = @"Missing or invalid Url response.";
NSString* const UnauthorizedHTTStatusExpected = @"Expected Unauthorized (401) HTTP status code. Actual status code %d";
const unichar Quote = '\"';
//The regular expression that matches the Bearer contents:
NSString* const RegularExpression = @"^Bearer\\s+([^,\\s=\"]+?)=\"([^\"]*?)\"\\s*(?:,\\s*([^,\\s=\"]+?)=\"([^\"]*?)\"\\s*)*$";
NSString* const ExtractionExpression = @"\\s*([^,\\s=\"]+?)=\"([^\"]*?)\"";

@implementation ADAuthenticationParameters (Internal)


-(id) initInternalWithParameters: (NSDictionary *) extractedParameters
                           error: (ADAuthenticationError* __autoreleasing*) error;

{
    THROW_ON_NIL_ARGUMENT(extractedParameters);
    
    self = [super init];
    if (self)
    {
        self->_extractedParameters = extractedParameters;
        self->_authority = [_extractedParameters objectForKey:OAuth2_Authorization_Uri];
        NSURL* testUrl = [NSURL URLWithString:_authority];//Nil argument returns nil
        if (!testUrl)
        {
            NSString* errorDetails = [NSString stringWithFormat:MissingOrInvalidAuthority,
                                      OAuth2_Authenticate_Header, OAuth2_Authorization_Uri];
            ADAuthenticationError* adError = [ADAuthenticationError errorFromUnauthorizedResponse:AD_ERROR_AUTHENTICATE_HEADER_BAD_FORMAT
                                                              errorDetails:errorDetails];
            if (error)
            {
                *error = adError;
            }
            return nil;
        }
        
        self->_resource = [_extractedParameters objectForKey:OAuth2_Resource_Id];
    }
    return self;
}

//Generates and returns an error
+(ADAuthenticationError*) invalidHeader:(NSString*) headerContents
{
    NSString* errorDetails = [NSString stringWithFormat:InvalidHeader,
     OAuth2_Authenticate_Header, headerContents];
    return [ADAuthenticationError errorFromUnauthorizedResponse:AD_ERROR_AUTHENTICATE_HEADER_BAD_FORMAT
                                                   errorDetails:errorDetails];
}

+ (NSDictionary*) extractChallengeParameters: (NSString*) headerContents
                                       error: (ADAuthenticationError* __autoreleasing*) error;
{
    NSError* rgError;
    __block ADAuthenticationError* adError;
    
    if ([NSString adIsStringNilOrBlank:headerContents])
    {
        adError = [self invalidHeader:headerContents];
    }
    else
    {
        //First check that the header conforms to the protocol:
        NSRegularExpression* overAllRegEx = [NSRegularExpression regularExpressionWithPattern:RegularExpression
                                                                                      options:0
                                                                                        error:&rgError];
        if (overAllRegEx)
        {
            long matched = [overAllRegEx numberOfMatchesInString:headerContents options:0 range:NSMakeRange(0, headerContents.length)];
            if (!matched)
            {
                adError = [self invalidHeader:headerContents];
            }
            else
            {
                //Once we know that the header is in the right format, the regex below will extract individual
                //name-value pairs. This regex is not as exclusive, so it relies on the previous check
                //to guarantee correctness:
                NSRegularExpression* extractionRegEx = [NSRegularExpression regularExpressionWithPattern:ExtractionExpression
                                                                                                 options:0
                                                                                                   error:&rgError];
                if (extractionRegEx)
                {
                    NSMutableDictionary* parameters = [NSMutableDictionary new];
                    [extractionRegEx enumerateMatchesInString:headerContents
                                                      options:0
                                                        range:NSMakeRange(0, headerContents.length)
                                                   usingBlock:^(NSTextCheckingResult *result, NSMatchingFlags flags, BOOL *stop)
                     {//Block executed for each name-value match:
                         if (result.numberOfRanges != 3)//0: whole match, 1 - name group, 2 - value group
                         {
                             //Shouldn't happen given the explicit expressions and matches, but just in case:
                             adError = [self invalidHeader:headerContents];
                         }
                         else
                         {
                             NSRange key = [result rangeAtIndex:1];
                             NSRange value = [result rangeAtIndex:2];
                             if (key.length && value.length)
                             {
                                 [parameters setObject:[headerContents substringWithRange:value]
                                                forKey:[headerContents substringWithRange:key]];
                             }
                         }
                     }];
                    return parameters;
                }
            }
        }
    }
    
    if (rgError)
    {
        //The method below will log internally the error:
        adError =[ADAuthenticationError errorFromNSError:rgError errorDetails:rgError.description];
    }
    
    if (error)
    {
        *error = adError;
    }
    return nil;
}

@end
