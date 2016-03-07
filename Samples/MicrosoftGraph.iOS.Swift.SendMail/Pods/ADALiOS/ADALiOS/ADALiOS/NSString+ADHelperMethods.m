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

typedef unsigned char byte;

static char base64UrlEncodeTable[64] =
{
    'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P',
    'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', 'a', 'b', 'c', 'd', 'e', 'f',
    'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v',
    'w', 'x', 'y', 'z', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '-', '_'
};

#define NA (255)

static byte rgbDecodeTable[128] = {                         // character code
    NA, NA, NA, NA, NA, NA, NA, NA, NA, NA, NA, NA, NA, NA, NA, NA,  // 0-15
    NA, NA, NA, NA, NA, NA, NA, NA, NA, NA, NA, NA, NA, NA, NA, NA,  // 16-31
    NA, NA, NA, NA, NA, NA, NA, NA, NA, NA, NA, NA, NA, 62, NA, NA,  // 32-47
    52, 53, 54, 55, 56, 57, 58, 59, 60, 61, NA, NA, NA,  0, NA, NA,  // 48-63
    NA,  0,  1,  2,  3,  4,  5,  6,  7,  8,  9, 10, 11, 12, 13, 14,  // 64-79
    15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, NA, NA, NA, NA, 63,  // 80-95
    NA, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40,  // 96-111
    41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 51, NA, NA, NA, NA, NA,  // 112-127
};

//Checks that all bytes inside the format are valid base64 characters:
BOOL validBase64Characters(const byte* data, const int size)    
{
    for (int i = 0; i < size; ++i)
    {
        if (data[i] >= sizeof(rgbDecodeTable) || rgbDecodeTable[data[i]] == NA)
        {
            return false;
        }
    }
    return true;
}

@implementation NSString (ADHelperMethods)

/// <summary>
/// Base64 URL decode a set of bytes.
/// </summary>
/// <remarks>
/// See RFC 4648, Section 5 plus switch characters 62 and 63 and no padding.
/// For a good overview of Base64 encoding, see http://en.wikipedia.org/wiki/Base64
/// </remarks>
+ (NSData *) Base64DecodeData:(NSString *)encodedString
{
    if ( nil == encodedString )
    {
        return nil;
    }
    
    NSData      *encodedBytes = [encodedString dataUsingEncoding:NSUTF8StringEncoding];
    const byte  *pbEncoded    = [encodedBytes bytes];
    const int    cbEncoded    = (int)[encodedBytes length];
    if (!validBase64Characters(pbEncoded, cbEncoded))
    {
        return nil;
    }
    
    int   cbDecodedSize;
    int   ich;
    int   ib;
    byte  b0, b1, b2, b3;
    
    // The input string lacks the usual '=' padding at the end, so the valid end sequences
    // are:
    //      ........XX           (cbEncodedSize % 4) == 2    (2 chars of virtual padding)
    //      ........XXX          (cbEncodedSize % 4) == 3    (1 char of virtual padding)
    //      ........XXXX         (cbEncodedSize % 4) == 0    (no virtual padding)
    // Invalid sequences are:
    //      ........X            (cbEncodedSize % 4) == 1
    
    // Input string is not sized correctly to be base64 URL encoded.
    if ( ( 0 == cbEncoded ) || ( 1 == ( cbEncoded % 4 ) ) )
    {
        return nil;
    }
    
    // 'virtual padding' is how many trailing '=' characters we would have
    // had under 'normal' base-64 encoding
    int virtualPadding = ( ( cbEncoded % 4 ) == 2 ) ? 2 : ( ( cbEncoded % 4 ) == 3 ) ? 1 : 0;
    
    // Calculate decoded buffer size.
    cbDecodedSize = (cbEncoded + virtualPadding + 3) / 4 * 3;
    cbDecodedSize -= virtualPadding;
    
    byte *pbDecoded = (byte *)calloc( cbDecodedSize, sizeof(byte) );
    
    if(!pbDecoded) {
        return nil;
    }
    
    // Decode each four-byte cluster into the corresponding three data bytes,
    // allowing for the fact that the last cluster may be less than four bytes
    // (virtual padding).
    ich = ib = 0;
    
    int end4 = (cbEncoded/4)*4;
    //Quick loop, no boundary checks:
    for(; ich < end4; )
    {
        b0 = rgbDecodeTable[pbEncoded[ich++]];
        b1 = rgbDecodeTable[pbEncoded[ich++]];
        b2 = rgbDecodeTable[pbEncoded[ich++]];
        b3 = rgbDecodeTable[pbEncoded[ich++]];
        
        pbDecoded[ib++] = (b0 << 2) | (b1 >> 4);
        pbDecoded[ib++] = (b1 << 4) | (b2 >> 2);
        pbDecoded[ib++] = (b2 << 6) | b3;
    }
    
    //Beyond the padding to 4. Requires boundary checks,
    //but the inner side shouldn't be executed more than 3 times:
    while ( ich < cbEncoded )
    {
        b0 = rgbDecodeTable[pbEncoded[ich++]];
        b1 = (ich < cbEncoded) ? rgbDecodeTable[pbEncoded[ich++]] : 0;
        b2 = (ich < cbEncoded) ? rgbDecodeTable[pbEncoded[ich++]] : 0;
        b3 = (ich < cbEncoded) ? rgbDecodeTable[pbEncoded[ich++]] : 0;
        
        pbDecoded[ib++] = (b0 << 2) | (b1 >> 4);
        
        if (ib < cbDecodedSize) {
            pbDecoded[ib++] = (b1 << 4) | (b2 >> 2);
            
            if (ib < cbDecodedSize) {
                pbDecoded[ib++] = (b2 << 6) | b3;
            }
        }
    }
    
    // Place the result in a NSData object and then free it.
    NSData *result = [NSData dataWithBytes:pbDecoded length:cbDecodedSize];
    
    free( pbDecoded );
    
    return result;
}

- (NSString *) adBase64UrlDecode
{
    NSData *decodedData = [self.class Base64DecodeData:self];
    
    return [[NSString alloc] initWithData:decodedData encoding:NSUTF8StringEncoding];
}

//Helper method to encode 3 bytes into a sequence of 4 bytes:
//"static inline" is the way declare inline methods in LLVM
static inline void Encode3bytesTo4bytes(char* output, int b0, int b1, int b2)
{
    output[0] = base64UrlEncodeTable[b0 >> 2];                                  // 6 MSB from byte 0
    output[1] = base64UrlEncodeTable[((b0 << 4) & 0x30) | ((b1 >> 4) & 0x0f)];  // 2 LSB from byte 0 and 4 MSB from byte 1
    output[2] = base64UrlEncodeTable[((b1 << 2) & 0x3c) | ((b2 >> 6) & 0x03)];  // 4 LSB from byte 1 and 2 MSB from byte 2
    output[3] = base64UrlEncodeTable[b2 & 0x3f];
}

/// <summary>
/// Base64 URL encode a set of bytes.
/// </summary>
/// <remarks>
/// See RFC 4648, Section 5 plus switch characters 62 and 63 and no padding.
/// For a good overview of Base64 encoding, see http://en.wikipedia.org/wiki/Base64
/// </remarks>
+ (NSString *) Base64EncodeData:(NSData *)data
{
    if ( nil == data )
        return nil;
    
    const byte *pbBytes = [data bytes];
    int         cbBytes = (int)[data length];
    
    // Calculate encoded string size including padding. This may be more than is actually
    // required since we will not pad and instead will terminate with null. The computation
    // is the number of byte triples times 4 radix64 characters plus 1 for null termination.
    int   encodedSize = 1 + ( cbBytes + 2 ) / 3 * 4;
    char *pbEncoded = (char *)calloc( encodedSize, sizeof(char) );
    
    if(!pbEncoded){
        return nil;
    }
    
    // Encode data byte triplets into four-byte clusters.
    int   iBytes;      // raw byte index
    int   iEncoded;    // encoded byte index
    byte  b0, b1, b2;  // individual bytes for triplet
    
    iBytes = iEncoded = 0;
    
    int end3 = (cbBytes/3)*3;
    //Fast loop, no bounderies check:
    for ( ; iBytes < end3; )
    {
        b0 = pbBytes[iBytes++];
        b1 = pbBytes[iBytes++];
        b2 = pbBytes[iBytes++];
        
        Encode3bytesTo4bytes(pbEncoded + iEncoded, b0, b1, b2);
        iEncoded += 4;
    }
    
    //Slower loop should execute no more than 3 times:
    while ( iBytes < cbBytes )
    {
        b0 = pbBytes[iBytes++];
        b1 = (iBytes < cbBytes) ? pbBytes[iBytes++] : 0;                                        // Add extra zero byte if needed
        b2 = (iBytes < cbBytes) ? pbBytes[iBytes++] : 0;                                        // Add extra zero byte if needed
        
        Encode3bytesTo4bytes(pbEncoded + iEncoded, b0, b1, b2);
        iEncoded += 4;
    }
    
    // Where we would have padded it, we instead truncate the string
    switch ( cbBytes % 3 )
    {
        case 0:
            // No left overs, nothing to pad
            break;
            
        case 1:
            // One left over, normally pad 2
            pbEncoded[iEncoded - 2] = '\0';
            // fall through
            
        case 2:
            pbEncoded[iEncoded - 1] = '\0';
            break;
    }
    
    // Null terminate, convert to NSString and free the buffer
    pbEncoded[iEncoded++] = '\0';
    
    NSString *result = [NSString stringWithCString:pbEncoded encoding:NSUTF8StringEncoding];
    
    free(pbEncoded);
    
    return result;
}

// Base64 URL encodes a string
- (NSString *) adBase64UrlEncode
{
    NSData *decodedData = [self dataUsingEncoding:NSUTF8StringEncoding];
    
    return [self.class Base64EncodeData:decodedData];
}

/* Caches statically the non-white characterset */
+(NSCharacterSet*) nonWhiteCharSet
{
    static NSCharacterSet* nonWhiteCharSet;//Cached instance
    static dispatch_once_t once;
    @synchronized(self)
    {
        dispatch_once(&once, ^{
            //Instance initialization (only once):
            nonWhiteCharSet = [[NSCharacterSet whitespaceAndNewlineCharacterSet] invertedSet];
        });
    }
    return nonWhiteCharSet;
}

+(BOOL) adIsStringNilOrBlank: (NSString*)string
{
    if (!string || !string.length)
        return YES;
    else
    {
        long nonWhite = [string adFindNonWhiteCharacterAfter:0];
        return nonWhite >= string.length;
    }
}

-(BOOL) adContainsString: (NSString*) contained
{
    THROW_ON_NIL_ARGUMENT(contained);
    if (!contained.length)
        return YES;
    return [self rangeOfString:contained].location != NSNotFound;
}

-(long) adFindCharactersFromSet: (NSCharacterSet*) set
                        start: (long) startIndex
{
    THROW_ON_NIL_ARGUMENT(set);
    long end = self.length;
    if (startIndex >= end)
        return end;
    
    NSRange toSearch = {.location  = startIndex, .length = (end - startIndex)};
    long found = [self rangeOfCharacterFromSet:set options:NSLiteralSearch range:toSearch].location;
    return (found == NSNotFound) ? end : found;
}

-(long) adFindNonWhiteCharacterAfter: (long) startIndex
{
    return [self adFindCharactersFromSet:[NSString nonWhiteCharSet] start:startIndex];
}

-(long) adFindCharacter:(unichar)toFind start: (long) startIndex
{
    NSRange chars = {.location = toFind, .length = 1};
    NSCharacterSet* set = [NSCharacterSet characterSetWithRange:chars];
    return [self adFindCharactersFromSet:set start:startIndex];
}

-(NSString*) adTrimmedString
{
    //The white characters set is cached by the system:
    NSCharacterSet* set = [NSCharacterSet whitespaceAndNewlineCharacterSet];
    return [self stringByTrimmingCharactersInSet:set];
}

-(BOOL) adRangeHasPrefixWord: (NSString*) prefixWord range: (NSRange) range
{
    THROW_ON_NIL_ARGUMENT(prefixWord);
    if (!prefixWord.length)
        return YES;
    if (range.location >= self.length)
        return NO;//The range is beyond the string.
    if (range.location + range.length >= self.length)
        range.length = self.length - range.location;//Cut to the end of the string to avoid throwing below
    
    //Anchored search ensures that the search happens only at the start:
    NSRange found =  [self rangeOfString:prefixWord options:NSAnchoredSearch range:range];//Can throw.
    if (found.location == NSNotFound)
        return NO;
    long after = found.location + prefixWord.length;
    if (after >= self.length || after >= (range.location + range.length))
        return YES;//Full containment
    
    //The next character should be white space to complete the word:
    return ([[NSCharacterSet whitespaceAndNewlineCharacterSet] characterIsMember:[self characterAtIndex:after]]);
}

-(BOOL) adSubstringHasPrefixWord: (NSString*) prefixWord start: (long) substringStart
{
    NSRange range = {.location = substringStart, .length = (self.length - substringStart)};
    return [self adRangeHasPrefixWord:prefixWord range:range];
}

- (NSString *)adUrlFormDecode
{
    // Two step decode: first replace + with a space, then percent unescape
    CFMutableStringRef decodedString = CFStringCreateMutableCopy( NULL, 0, (__bridge CFStringRef)self );
    CFStringFindAndReplace( decodedString, CFSTR("+"), CFSTR(" "), CFRangeMake( 0, CFStringGetLength( decodedString ) ), kCFCompareCaseInsensitive );
    
    CFStringRef unescapedString = CFURLCreateStringByReplacingPercentEscapesUsingEncoding( NULL,                    // Allocator
                                                                                          decodedString,           // Original string
                                                                                          CFSTR(""),               // Characters to leave escaped
                                                                                          kCFStringEncodingUTF8 ); // Encoding
    CFRelease( decodedString );
    
    return CFBridgingRelease(unescapedString);
}

- (NSString *)adUrlFormEncode
{
    // Two step encode: first percent escape everything except spaces, then convert spaces to +
    CFStringRef escapedString = CFURLCreateStringByAddingPercentEscapes( NULL,                         // Allocator
                                                                        (__bridge CFStringRef)self,            // Original string
                                                                        CFSTR(" "),                   // Characters to leave unescaped
                                                                        CFSTR("!#$&'()*+,/:;=?@[]%"), // Legal Characters to be escaped
                                                                        kCFStringEncodingUTF8 );      // Encoding
    
    // Replace spaces with +
    CFMutableStringRef encodedString = CFStringCreateMutableCopy( NULL, 0, escapedString );
    CFStringFindAndReplace( encodedString, CFSTR(" "), CFSTR("+"), CFRangeMake( 0, CFStringGetLength( encodedString ) ), kCFCompareCaseInsensitive );
    
    CFRelease( escapedString );
    
    return CFBridgingRelease( encodedString );
}

+ (BOOL) adSame: (NSString*) string1
       toString: (NSString*) string2
{
    if (!string1)
        return !string2; //if both are nil, they are equal
    else
        return [string1 isEqualToString:string2];
}

@end
