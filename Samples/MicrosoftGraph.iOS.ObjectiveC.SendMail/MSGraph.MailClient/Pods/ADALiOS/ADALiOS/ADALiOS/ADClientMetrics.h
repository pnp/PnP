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

@interface ADClientMetrics : NSObject
{
@private
    NSString* _endpoint;
    NSString* _responseTime;
    NSString* _correlationId;
    NSString* _errorToReport;
    NSDate* _startTime;
    bool _isPending;
}

@property (readonly) NSString* endpoint;
@property (readonly) NSString* responseTime;
@property (readonly) NSString* correlationId;
@property (readonly) NSString* errorToReport;
@property (readonly) NSDate* startTime;
@property bool isPending;

+ (ADClientMetrics*) getInstance;

- (void) beginClientMetricsRecordForEndpoint: (NSString*) endPoint
                               correlationId: (NSString*) correlationId
                               requestHeader: (NSMutableDictionary*) requestHeader;

-(void) endClientMetricsRecord: (NSString*) error;

@end