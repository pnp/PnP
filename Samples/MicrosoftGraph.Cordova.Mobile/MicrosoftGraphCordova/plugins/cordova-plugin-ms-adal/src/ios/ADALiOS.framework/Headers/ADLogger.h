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

/*! Levels of logging. Defines the priority of the logged message */
#import <Foundation/Foundation.h>

typedef enum
{
    ADAL_LOG_LEVEL_NO_LOG,//Available to fully disable logging
    ADAL_LOG_LEVEL_ERROR,//Default
    ADAL_LOG_LEVEL_WARN,
    ADAL_LOG_LEVEL_INFO,
    ADAL_LOG_LEVEL_VERBOSE,
    ADAL_LOG_LAST = ADAL_LOG_LEVEL_VERBOSE,
} ADAL_LOG_LEVEL;

@interface ADLogger : NSObject

/*! Sets the logging level for the internal logging messages. Messages with
 priority lower than the specified level will be ignored. 
 @param logLevel: desired logging level. The higher the number, the more logging information is included. */
+(void) setLevel: (ADAL_LOG_LEVEL)logLevel;

/*! Returns the current log level. See setLevel for details */
+(ADAL_LOG_LEVEL) getLevel;

/*! Main logging function. Macros like ADAL_LOG_ERROR are provided on top for convenience
 @param logLevel: The applicable priority of the logged message. Use AD_LOG_LEVEL_NO_LOG to disable all logging.
 @param message: Short text defining the operation/condition.
 @param additionalInformation: Full details. May contain parameter names, stack traces, etc. May be nil.
 @param errorCode: if an explicit error has occurred, this code will contain its code.
 */
+(void) log: (ADAL_LOG_LEVEL)logLevel
    message: (NSString*) message
  errorCode: (NSInteger) errorCode
additionalInformation: (NSString*) additionalInformation;

/*! Logs obtaining of a token. The method does not log the actual token, only its hash.
 @param token: the token to log.
 @param tokenType: "access token", "refresh token", "multi-resource refresh token"
 @param expiresOn: the time when an access token will stop to be valid. Nil for refresh token types.
 @param correlationId: In case the token was just obtained from the server, the correlation id of the call.
 This parameter can be nil.
*/
+(void) logToken: (NSString*) token
       tokenType: (NSString*) tokenType
       expiresOn: (NSDate*) expiresOn
   correlationId: (NSUUID*) correlationId;


//The block declaration. Needs to be weak to ensure that the pointer does not hold static reference
//to the parent class of the callback.
typedef void (^LogCallback)(ADAL_LOG_LEVEL logLevel,
                            NSString* message,
                            NSString* additionalInformation,
                            NSInteger errorCode);

/*! Provided block will be called when the logged messages meet the priority threshold
 @param callback: The block to be executed when suitable messages are logged. By default, when
 callback is set, messages will contingue to be logged through NSLog. Such logging can be disabled
 through setNSLogging. */
+(void) setLogCallBack: (LogCallback) callback;

/*! Returns previously set callback call or nil, if the user has not set such callback. */
+(LogCallback) getLogCallBack;

/*! By default, logging sends messages through standard NSLog. This function allows to disable this
 behavior. Disabling is useful if faster logging is implemented through the callback. */
+(void) setNSLogging: (BOOL) nslogging;

/*! YES if the messages are logged through NSLog.*/
+(BOOL) getNSLogging;

/*! Returns diagnostic trace data to be sent to the Auzure Active Directory servers. */
+(NSDictionary*) adalId;

/*! Calculates a hash of the passed string. Useful for logging tokens, where we do not log
 the actual contents, but still want to log something that can be correlated. */
+(NSString*) getHash: (NSString*) input;

/*! Sets correlation id to be used in the requests sent to server. */
+(void) setCorrelationId: (NSUUID*) correlationId;

/*! Gets correlation Id. */
+(NSUUID*) getCorrelationId;

+(NSString*) getAdalVersion;

@end

//A simple macro for single-line logging:
#define AD_LOG(level, msg, code, info) \
{ \
            [ADLogger log: level \
                  message: msg \
                errorCode: code \
    additionalInformation: info]; \
}

#define FIRST_ARG(ARG,...) ARG

//Allows formatting, e.g. AD_LOG_FORMAT(ADAL_LOG_LEVEL_INFO, "Something", "Check this: %@ and this: %@", this1, this2)
//If we make this a method, we will lose the warning when the string formatting parameters do not match the actual parameters.
#define AD_LOG_FORMAT(level, msg, code, info...) \
{ \
    if (FIRST_ARG(info))/*Avoid crash in logging*/ \
    { \
        NSString* logInfo = [NSString stringWithFormat:info]; \
        [ADLogger log: level \
              message: msg \
            errorCode: code \
additionalInformation: logInfo]; \
    } \
    else \
    { \
        [ADLogger log: level \
              message: msg \
            errorCode: code \
additionalInformation: @"Bad logging: nil info specified."]; \
    } \
}

#define AD_LOG_ERROR(message, code, info) AD_LOG(ADAL_LOG_LEVEL_ERROR, message, code, info)
#define AD_LOG_WARN(message, info) AD_LOG(ADAL_LOG_LEVEL_WARN, message, AD_ERROR_SUCCEEDED, info)
#define AD_LOG_INFO(message, info) AD_LOG(ADAL_LOG_LEVEL_INFO, message, AD_ERROR_SUCCEEDED, info)
#define AD_LOG_VERBOSE(message, info) AD_LOG(ADAL_LOG_LEVEL_VERBOSE, message, AD_ERROR_SUCCEEDED, info)

#define AD_LOG_ERROR_F(message, code, info...) AD_LOG_FORMAT(ADAL_LOG_LEVEL_ERROR, message, code, info)
#define AD_LOG_WARN_F(message, info...) AD_LOG_FORMAT(ADAL_LOG_LEVEL_WARN, message, AD_ERROR_SUCCEEDED, info)
#define AD_LOG_INFO_F(message, info...) AD_LOG_FORMAT(ADAL_LOG_LEVEL_INFO, message, AD_ERROR_SUCCEEDED, info)
#define AD_LOG_VERBOSE_F(message, info...) AD_LOG_FORMAT(ADAL_LOG_LEVEL_VERBOSE, message, AD_ERROR_SUCCEEDED, info)

#ifndef DebugLog
#ifdef DEBUG
#   define DebugLog(fmt, ...) NSLog((@"%s[%d][%@] " fmt), __PRETTY_FUNCTION__, __LINE__, [[NSThread currentThread] isEqual:[NSThread mainThread]] ? @"main" : @"work", ##__VA_ARGS__);
#else
#   define DebugLog(...)
#endif
#endif

