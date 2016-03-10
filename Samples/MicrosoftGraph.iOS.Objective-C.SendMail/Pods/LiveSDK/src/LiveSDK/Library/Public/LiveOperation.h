//
//  LiveOperation.h
//  Live SDK for iOS
//
//  Copyright 2015 Microsoft Corporation
//
//  Permission is hereby granted, free of charge, to any person obtaining a copy
//  of this software and associated documentation files (the "Software"), to deal
//  in the Software without restriction, including without limitation the rights
//  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//  copies of the Software, and to permit persons to whom the Software is
//  furnished to do so, subject to the following conditions:
//  
//  The above copyright notice and this permission notice shall be included in
//  all copies or substantial portions of the Software.
//  
//  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
//  THE SOFTWARE.
//


#import <Foundation/Foundation.h>
#import "LiveOperationDelegate.h"

// LiveOperation class represents an operation that sends a request to Live Service REST API.
@interface LiveOperation : NSObject 

// The path of the request.
@property (nonatomic, readonly) NSString *path;

// The method of the request.
@property (nonatomic, readonly) NSString *method;

// The text receieved from the Live Service REST API response.
@property (nonatomic, readonly) NSString *rawResult;

// The parsed result received from the Live Service REST API response
@property (nonatomic, readonly) NSDictionary *result;

// The userState object passed in when the original method was invoked on the LiveConnectClient instance.
@property (nonatomic, readonly) id userState; 

// The delegate instance to handle the operation callbacks.
@property (nonatomic, assign) id delegate;

// Cancel the current operation. 
- (void) cancel;

@end