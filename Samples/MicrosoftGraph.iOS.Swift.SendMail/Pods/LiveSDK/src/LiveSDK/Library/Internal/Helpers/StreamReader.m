//
//  StreamReader.m
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


#import "StreamReader.h"

const NSUInteger BUFFERSIZE = 4096;

@implementation StreamReader

@synthesize data, 
            delegate = _delegate,
            stream = _stream;

- (id)initWithStream:(NSInputStream *)stream
            delegate:(id<StreamReaderDelegate>)delegate
{
    self = [super init];
    if (self) 
    {
        _stream = [stream retain];
        _delegate = delegate;
    }
    
    return self;
}

- (void)dealloc
{
    [_stream release];
    [data release];
    
    [super dealloc];
}

- (void)start
{
    _stream.delegate = self;
    [_stream scheduleInRunLoop:[NSRunLoop currentRunLoop]
                       forMode:NSDefaultRunLoopMode];
    [_stream open];
}

- (void)cleanup
{
    [self.stream close];
    [self.stream removeFromRunLoop:[NSRunLoop currentRunLoop]
                           forMode:NSDefaultRunLoopMode];
    self.stream.delegate = nil;
    self.stream = nil;
    self.data = nil;
    self.delegate = nil;    
}

- (void)stream:(NSStream *)aStream 
   handleEvent:(NSStreamEvent)eventCode
{
    switch (eventCode) 
    {
        case NSStreamEventHasBytesAvailable:
        {
            if(!data) 
            {
                self.data = [NSMutableData data];
            }
            
            uint8_t buf[BUFFERSIZE];
            unsigned int len = 0;
            len = (unsigned int)[_stream read:buf maxLength:BUFFERSIZE];
            if(len) 
            {
                [data appendBytes:(const void *)buf length:len];
            } 
            else 
            {
                NSLog(@"no buffer!");
            }
            
            break;
        }
        case NSStreamEventEndEncountered:
        {
            [_delegate streamReadingCompleted:data];
            [self cleanup];
            
            break;
        }
        case NSStreamEventErrorOccurred:
        {
            NSError *theError = [_stream streamError];            
            [_delegate streamReadingFailed:theError];
            [self cleanup];
            
            break;
        }          
        default:
        {
            break;
        }
    }
}

@end
