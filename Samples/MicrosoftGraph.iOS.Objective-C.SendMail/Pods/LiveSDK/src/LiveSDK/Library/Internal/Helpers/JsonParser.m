// 
//  JsonParser.m
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


#import "JsonParser.h"

// ------------------------------------------------------------------------
// External string contants
NSString * const MSJSONParserErrorDomain = @"MSJSONParserErrorDomain";
NSString * const MSJSONParserInternalException = @"MSJSONParserInternalException";
NSString * const MSJSONParserExceptionError = @"MSJSONParserExceptionError";
NSString * const MSJSONParserInternalExceptionKey = @"MSJSONParserInternalExceptionKey";


// ------------------------------------------------------------------------
// Parsing helper macros
#define VerifyNotAtEOS(ctx)  if ((_scanner == nil) || [_scanner isAtEnd]) [self raiseError:MSJSONErrorUnexpectedEndOfText reason:@"Unexpected end of text found " ctx]
#define GetNextCharacter()   [_scanner.string characterAtIndex:_scanner.scanLocation]
#define ScanCharacter(ch)    do { if (GetNextCharacter() == ch) _scanner.scanLocation = (_scanner.scanLocation + 1); } while(0)


// ------------------------------------------------------------------------
// Private method definitions
@interface MSJSONParser (MSJSONParser_Private)

- (NSUInteger) lineNumberForParseLocation:(NSUInteger)location column:(NSUInteger*)col;
- (void) raiseError:(MSJSONParseError)code reason:(NSString*)reason;
- (void) skipWhitespace;

- (id) parseValue;
- (NSString*) parseStringValue;
- (id) parseCollection;
- (id) parseObject;

@end




// ------------------------------------------------------------------------
// MSJSONParser class implementation
@implementation MSJSONParser

@synthesize error = _error;
@synthesize skipJavascriptComments = _skipJavascriptComments;
@synthesize supportJSONLight = _supportJSONLight;
@synthesize collectionClass = _collectionClass;
@synthesize objectClass = _objectClass;

static NSCharacterSet *s_CharacterSetNumericStartChars = nil;
static NSCharacterSet *s_CharacterSetIdentifierStartChars = nil;
static NSCharacterSet *s_CharacterSetIdentifierChars = nil;
static NSCharacterSet *s_CharacterSetBackslashAndQuote = nil;

+ (void) initialize
{
	static BOOL s_initialized = NO;
	if (!s_initialized)
	{
		s_initialized = YES;
		s_CharacterSetNumericStartChars =
			[[NSCharacterSet characterSetWithCharactersInString:@"-0123456789"] retain];
		s_CharacterSetIdentifierStartChars =
			[[NSCharacterSet characterSetWithCharactersInString:@"_abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ"] retain];
		s_CharacterSetIdentifierChars =
			[[NSCharacterSet characterSetWithCharactersInString:@"_.abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789"] retain];
		s_CharacterSetBackslashAndQuote =
			[[NSCharacterSet characterSetWithCharactersInString:@"\\\""] retain];
	}
}

+ (id) parseText:(NSString*)text error:(NSError**)error
{
	// Create a new parser
	MSJSONParser *parser = [[self alloc] initWithText:text];
	if (!parser)
	{
		if (error)
		{
			(*error) = [NSError errorWithDomain:MSJSONParserErrorDomain code:MSJSONErrorOutOfMemory userInfo:
				[NSDictionary dictionaryWithObject:@"Unable to create parser" forKey:NSLocalizedDescriptionKey]];
		}
		return nil;
	}

	// Parse the JSON text
	id result = [parser parse];

	// If there is no result, then get the error code (if the caller wants it)
	if (!result && error)
		(*error) = [[parser.error retain] autorelease];

	// Clean up
	[parser release];

	return result;
}

+ (id) parseJSONLightText:(NSString*)text error:(NSError**)error
{
	// Create a new parser
	MSJSONParser *parser = [[self alloc] initWithText:text];
	if (!parser)
	{
		if (error)
		{
			(*error) = [NSError errorWithDomain:MSJSONParserErrorDomain code:MSJSONErrorOutOfMemory userInfo:
				[NSDictionary dictionaryWithObject:@"Unable to create parser" forKey:NSLocalizedDescriptionKey]];
		}
		return nil;
	}

	// Parse the JSON text
	parser.supportJSONLight = YES;
	id result = [parser parse];

	// If there is no result, then get the error code (if the caller wants it)
	if (!result && error)
		(*error) = [[parser.error retain] autorelease];

	// Clean up
	[parser release];

	return result;
}

- (id) initWithText:(NSString*)text
{
	self = [super init];
	if (self)
	{
		_skipJavascriptComments = YES;

		_scanner = [[NSScanner alloc] initWithString:text];
		if (!_scanner)
		{
			// Out of memory?
			[self release];
			return nil;
		}

		// For some reason, NSScanner will not properly skip whitespace characters using 'scanCharactersFromSet:'
		// if those same characters are set at the 'charactersToBeSkipped' (some kind of optimization???). Since
		// I need for 'scanCharactersFromSet:' to actually advance the scanLocation (but don't need the automatic
		// character skipping functionality), I just set the automatic skipping to an empty character set.
		[_scanner setCharactersToBeSkipped:[NSCharacterSet characterSetWithCharactersInString:@""]];

		// Force the scanner locale to be en_US so that numeric parsing works correctly (the JSON spec requires
		// a dot "." as the decimal separator).
		NSLocale *localeUS = [[NSLocale alloc] initWithLocaleIdentifier:@"en_US"];
		[_scanner setLocale:localeUS];
		[localeUS release];
		
		_collectionClass = [[NSMutableArray class] retain];
		_objectClass = [[NSMutableDictionary class] retain];
	}
	return self;
}

- (void) dealloc
{
	[_collectionClass release];
	[_objectClass release];
	[_scanner release];
	[_error release];
	[super dealloc];
}

- (NSString*) memberNameForString:(NSString*)name
{
	return name;
}

- (id) valueForStringValue:(NSString*)value
{
	if ([value hasPrefix:@"/Date("] && [value hasSuffix:@")/"])
	{
		NSDate *date = [NSDate dateWithJSONStringValue:value];
		if (date != nil) return date;
	}
	
	return value;
}

- (id) parse
{
	self.error = nil;
	id value = nil;

	// Wrap this call in its own autorelease pool to clean up as much as possible
	NSAutoreleasePool *pool = [[NSAutoreleasePool alloc] init];
	@try
	{
		// Do the real work (retain the value temporarily so that it won't go away with the autorelease pool)
		value = [[self parseValue] retain];

		// Make sure that we have reached the end of the JSON text
		[self skipWhitespace];
		if (![_scanner isAtEnd])
			[self raiseError:MSJSONErrorTextAfterRootValue reason:@"Extra text found after root value"];
	}
	@catch (NSException *ex)
	{
		// An exception occurred, clear the parsed object
		[value release];
		value = nil;

		// Capture the exception information in the parse error object
		NSError *error = nil;
		if ([ex.name isEqualToString:MSJSONParserInternalException])
			error = [ex.userInfo objectForKey:MSJSONParserExceptionError];
		if (!error)
		{
			error = [NSError errorWithDomain:MSJSONParserErrorDomain code:MSJSONErrorUnknownException userInfo:
				[NSDictionary dictionaryWithObjectsAndKeys:
				@"Unknown exception occurred in JSON parser", NSLocalizedDescriptionKey,
				ex, MSJSONParserInternalExceptionKey,
				nil]];
		}
		
		// Get the last scanned location
		// NSUInteger column = 0;
		// NSUInteger line = [self lineNumberForParseLocation:[_scanner scanLocation] column:&column];
		// MSLogErr(@"MSJSONParser error:%d (%@) [%u/%u]", error.code, error.reason, line, column);
		self.error = error;
	}
	@finally
	{
		// Clean up the autorelease pool
		[pool drain];
	}

	return [value autorelease];
}

- (NSUInteger) lineNumberForParseLocation:(NSUInteger)location column:(NSUInteger*)col
{
	// Calculate a "friendly" string location based on the current scan location
	NSUInteger line = 0;
	NSUInteger searchEnd = location;
	NSUInteger lineStart = 0;
	NSRange eolRange = NSMakeRange(0, 0);
	do
	{
		line += 1;
		lineStart = eolRange.location + eolRange.length;
		NSRange searchRange = NSMakeRange(lineStart, searchEnd - lineStart);
		eolRange = [_scanner.string rangeOfString:@"\n" options:NSLiteralSearch range:searchRange];
	} while (eolRange.location != NSNotFound);
	if (col)
		(*col) = searchEnd - lineStart + 1;
	return line;
}

- (void) skipWhitespace
{
	// Skip any leading whitespace.
	[_scanner scanCharactersFromSet:[NSCharacterSet whitespaceAndNewlineCharacterSet] intoString:NULL];

	while (_skipJavascriptComments)
	{
		// If a single-line comment is found then consume text up to the next newline.
		if ([_scanner scanString:@"//" intoString:NULL])
		{
			[_scanner scanUpToCharactersFromSet:[NSCharacterSet newlineCharacterSet] intoString:NULL];
		}
		// Else, if a multi-line comment is found then consume text up to the end tag (*/)
		// Nested multi-line comments are not supported.
		else if ([_scanner scanString:@"/*" intoString:NULL])
		{
			// Skip over any commented text
			[_scanner scanUpToString:@"*/" intoString:NULL];

			// Consume the comment end (error if it is not found)
			if (![_scanner scanString:@"*/" intoString:NULL])
				[self raiseError:MSJSONErrorUnexpectedEndOfText reason:@"End of file encountered while parsing multi-line comment"];
		}
		else
		{
			// No comment was found, so we are done.
			break;
		}

		// Skip any additional whitespace after the comment.
		[_scanner scanCharactersFromSet:[NSCharacterSet whitespaceAndNewlineCharacterSet] intoString:NULL];
	}
}

- (void) raiseError:(MSJSONParseError)code reason:(NSString*)reason
{
	NSError *error = [NSError errorWithDomain:MSJSONParserErrorDomain code:code userInfo:
		[NSDictionary dictionaryWithObject:reason forKey:NSLocalizedDescriptionKey]];
	NSException *exception = [NSException exceptionWithName:MSJSONParserInternalException reason:reason userInfo:
		[NSDictionary dictionaryWithObject:error forKey:MSJSONParserExceptionError]];
	[exception raise];
}

- (id) parseValue
{
	// Skip whitespace and check for end
	[self skipWhitespace];
	VerifyNotAtEOS("looking for value");

	id value = nil;
	unichar nextChar = GetNextCharacter();
	switch (nextChar)
	{
		case '{':
		{
			// Object value found
			value = [self parseObject];
			break;
		}
		case '[':
		{
			// Collection value found
			value = [self parseCollection];
			break;
		}

		case '\"':
		{
			// String value found
			value = [self parseStringValue];
			break;
		}

		case 't':
		case 'f':
		{
			// Boolean value found
			if ([_scanner scanString:((nextChar == 't') ? @"true" : @"false") intoString:NULL])
				value = [NSNumber numberWithBool:((nextChar == 't') ? YES : NO)];
			break;
		}

		case 'n':
		{
			// Null value found
			if ([_scanner scanString:@"null" intoString:NULL])
				value = [NSNull null];
			break;
		}

		case '\'':
		{
			if (_supportJSONLight)
				value = [self parseStringValue];
			break;
		}
	}
	
	if (!value)
	{
		if ([s_CharacterSetNumericStartChars characterIsMember:nextChar])
		{
			// Numeric value found
			double val = 0.0;
			if ([_scanner scanDouble:&val])
				value = [NSNumber numberWithDouble:val];
		}
		else if (_supportJSONLight && [s_CharacterSetIdentifierStartChars characterIsMember:nextChar])
		{
			value = [self parseStringValue];
		}
	}

	// Error - no valid value vound
	if (!value)
		[self raiseError:MSJSONErrorValueExpected reason:@"Invalid value found"];

	return value;
}

- (NSString*) parseStringValue
{
	// Skip whitespace
	[self skipWhitespace];
	if (![_scanner scanString:@"\"" intoString:NULL])
	{
		// If we support JSON light parsing, then check to see if this is a "light"
		// string (a string enclosed in single quotes with no escape character support)
		unichar nextChar = GetNextCharacter();
		if (_supportJSONLight && nextChar == (unichar)'\'')
		{
			NSString *lightStringValue = nil;
			[_scanner scanString:@"'" intoString:NULL];
			[_scanner scanUpToString:@"'" intoString:&lightStringValue];
			VerifyNotAtEOS("parsing string value");
			[_scanner scanString:@"'" intoString:NULL];
			return [self valueForStringValue:lightStringValue];
		}
		// If we support JSON light parsing, then also check to see if the next
		// character is a legal identifier start character (_a-zA-Z) and if so
		// parse the identifier as the string value
		else if (_supportJSONLight && [s_CharacterSetIdentifierStartChars characterIsMember:nextChar])
		{
			NSString *identifierString = nil;
			(void)[_scanner scanCharactersFromSet:s_CharacterSetIdentifierChars intoString:&identifierString];
			return identifierString;
		}
		else
		{
			[self raiseError:MSJSONErrorStringValueExpected reason:@"String value is expected"];
		}		
	}

	// Create the mutable string value
	NSMutableString *value = [[[NSMutableString alloc] init] autorelease];
	if (!value) [self raiseError:MSJSONErrorOutOfMemory reason:@"Error creating mutable string - out of memory?"];

	while (true)
	{
		// Get the next partial string value
		NSString *partial = nil;
		if ([_scanner scanUpToCharactersFromSet:s_CharacterSetBackslashAndQuote intoString:&partial] && [partial length] > 0)
			[value appendString:partial];

		// Check to see what character stopped the scan (or if it stopped at end of text)
		VerifyNotAtEOS("parsing string value");
		unichar nextChar = GetNextCharacter();
		if (nextChar == '\"')
		{
			// We have found the end of the string - consume the closing quote
			ScanCharacter('\"');
			break;
		}
		else if (nextChar == '\\')
		{
			// We are in an escape sequence - consume the escape character
			ScanCharacter('\\');

			// Make sure that we haven't hit the end of the text and get the escape value
			VerifyNotAtEOS("parsing string escape code");

			NSString *escapeString = nil;
			unichar escapeType = GetNextCharacter();
			switch (escapeType)
			{
				case '\"': escapeString = @"\""; break;
				case '\\': escapeString = @"\\"; break;
				case '/': escapeString = @"/"; break;
				case 'b': escapeString = @"\b"; break;
				case 'f': escapeString = @"\f"; break;
				case 'n': escapeString = @"\n"; break;
				case 'r': escapeString = @"\r"; break;
				case 't': escapeString = @"\t"; break;

				case 'u':
				{
					// Scan the four-hexidecimal digit unicode character value
					if (_scanner.scanLocation + 5 > [_scanner.string length])
						[self raiseError:MSJSONErrorIllegalStringUnicodeEscape reason:@"End of text while parsing string unicode escape code"];
					NSString *characterCode = [_scanner.string substringWithRange:NSMakeRange(_scanner.scanLocation + 1, 4)];
					NSScanner *unicodeScanner = [NSScanner scannerWithString:characterCode];
					unsigned int intValue = 0;
					if (![unicodeScanner scanHexInt:&intValue] || ![unicodeScanner isAtEnd] || intValue > USHRT_MAX)
						[self raiseError:MSJSONErrorIllegalStringUnicodeEscape reason:[NSString stringWithFormat:@"Illegal unicode escape code: %@", characterCode]];
					unichar unicodeCharacter = (unichar)intValue;
					escapeString = [NSString stringWithCharacters:&unicodeCharacter length:1];
					break;
				}

				default:
					[self raiseError:MSJSONErrorIllegalStringEscape reason:[NSString stringWithFormat:@"Illegal string escape found: %C", escapeType]];
					break;
			}

			// Append the escape string value
			[value appendString:escapeString];

			// Consume the escape value (unicode escape sequences include the four hex digits)
			[_scanner setScanLocation:_scanner.scanLocation + ((escapeType == 'u') ? 5 : 1)];
		}
		else
		{
			// We shouldn't be able to hit this case
			// MSLogDbg(@"Error parsing partial string");
		}
	}

	// Return the result value object for this string value
	id result = [self valueForStringValue:value];
	return result;
}

- (id) parseCollection
{
	// Skip whitespace and consume collection start character
	[self skipWhitespace];
	ScanCharacter('[');

	// Create the collection object and verify that it supports the 'addObject:' selector
	id collection = [[[_collectionClass alloc] init] autorelease];
	if (!collection) [self raiseError:MSJSONErrorOutOfMemory reason:@"Error creating collection - out of memory?"];
	// MSDbgCheck([collection respondsToSelector:@selector(addObject:)] || [collection respondsToSelector:@selector(addJSONObject:)]);

	// Check to see if this object supports the custom setter
	BOOL supportsJSONSetter = [collection respondsToSelector:@selector(addJSONObject:)];

	// Parse the elements in the collection
	BOOL elementFound = NO;
	while (true)
	{
		// Check to see if we are at the end of the collection
		[self skipWhitespace];
		VerifyNotAtEOS("parsing collection");
		unichar nextChar = GetNextCharacter();
		if (nextChar == ']')
			break;

		// If we have found an element then check for the separator and consume it
		if (elementFound)
		{
			if (nextChar != ',')
				[self raiseError:MSJSONErrorSeparatorExpected reason:@"Collection separator expected"];
			ScanCharacter(',');
		}

		// Parse the next element within the collection
		id item = [self parseValue];

		// Add the value to the collection
		if (supportsJSONSetter)
			[collection addJSONObject:item];
		else
			[collection addObject:item];
		elementFound = YES;
	}

	// Consume the collection end character
	ScanCharacter(']');
	return collection;
}

- (id) parseObject
{
	// Skip whitespace and consume collection start character
	[self skipWhitespace];
	ScanCharacter('{');

	// Create and initialize the object
	id object = [[[_objectClass alloc] init] autorelease];
	if (!object) [self raiseError:MSJSONErrorOutOfMemory reason:@"Error creating object - out of memory?"];

	// Check to see if this object supports the custom setter
	BOOL supportsJSONSetter = [object respondsToSelector:@selector(setJSONValue:forMemberName:)];

	// Parse the members (attributes) of the object
	BOOL memberFound = NO;
	while (true)
	{
		// Check to see if we are at the end of the collection
		[self skipWhitespace];
		VerifyNotAtEOS("parsing object");
		unichar nextChar = GetNextCharacter();
		if (nextChar == '}')
			break;

		// If we have parsed a member then check for the item separator and consume it
		if (memberFound)
		{
			if (nextChar != ',')
				[self raiseError:MSJSONErrorSeparatorExpected reason:@"Object member separator expected"];
			ScanCharacter(',');
		}

		// Parse the member name
		[self skipWhitespace];
		VerifyNotAtEOS("parsing object member name");
		NSString *memberName = [self memberNameForString:[self parseStringValue]];

		// Verify the member name separator
		[self skipWhitespace];
		VerifyNotAtEOS("parsing object");
		nextChar = GetNextCharacter();
		if (nextChar != ':')
			[self raiseError:MSJSONErrorNameSeparatorExpected reason:[NSString stringWithFormat:@"Object name separator expected for member %@", memberName]];
		ScanCharacter(':');

		// Parse the object member value
		id value = [self parseValue];

		// Store the new member value (using custom setter or KVC)
		// NOTE: KVC validation semantics are not supported here
		if (supportsJSONSetter)
			[object setJSONValue:value forMemberName:memberName];
		else
			[object setValue:value forKey:memberName];

		memberFound = YES;
	}

	// Consume the object end character
	ScanCharacter('}');

	return object;
}

@end



// ------------------------------------------------------------------------
@implementation NSDate (MSJSON_Extensions)

+ (id) dateWithJSONStringValue:(NSString*)value
{	
	long long timeValue = 0L;
	NSDate *date = nil;

	// Scan the string prefix to make sure that this is a correct JSON date representation
	NSScanner *scanner = [NSScanner scannerWithString:value];
	if ([scanner scanString:@"/Date(" intoString:NULL] || [scanner scanString:@"\\/Date(" intoString:NULL])
	{
		// Scan the number of 'ticks' (milliseconds since the epoch)
		if (![scanner scanLongLong:&timeValue] || timeValue == LLONG_MAX || timeValue == LLONG_MIN)
			return nil;

		// Make sure that the date string ends properly
		if (![scanner scanString:@")/" intoString:NULL] && ![scanner scanString:@")\\/" intoString:NULL])
			return nil;
		if (![scanner isAtEnd])
			return nil;

		// Get the date for this time value (adjusted to seconds)
		[self release];
		date = [NSDate dateWithTimeIntervalSince1970:(timeValue / 1000.0)];
		return date;
	}
    
    return nil;
}

@end
