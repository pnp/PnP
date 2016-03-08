// 
//  JsonWriter.m
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


#import "JsonWriter.h"

NSString * const MSJSONWriterCycleException = @"MSJSONWriterCycleException";


// ------------------------------------------------------------------------
@implementation MSJSONWriter  // including JFXJSONWriter_Overrides

@synthesize memberKeysSelector = _memberKeysSelector;

+ (NSString*) textForValue:(id)value
{
	MSJSONWriter *writer = [[self alloc] initWithValue:value];
	NSString *text = nil;
	@try
	{
		text = [writer JSONText];
	}
	@finally
	{
		[writer release];
	}
	return text;
}

+ (NSString*) escapeStringValue:(NSString*)value
{
	static NSCharacterSet *sInvalidStringCharacters = nil;
	if (!sInvalidStringCharacters)
	{
		@synchronized (self)
		{
			if (!sInvalidStringCharacters)
			{
				// NOTE: The JSON spec indicates that the slash ("/", or solidus) character can be escaped
				//       but doesn't really indicate whether or not it *should* be escaped. .NET doesn't seem
				//       to escape this character, so I will not either. The MSJSONParser works either way.
				//       If it is preferred to strictly follow the spec and escape this character, just remove
				//       it from the end of the 'validCharacters' text string, below, and it will be properly
				//       escaped.
				NSCharacterSet *validCharacters = [NSCharacterSet characterSetWithCharactersInString:
					@" !#$%&'()*+,-.0123456789:;<=>?@[]^_`{|}~ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz/"];
				sInvalidStringCharacters = [[validCharacters invertedSet] retain];
			}
		}
	}

	// Search the string for any occurrence of an invalid character. If none is found then just return value
	NSRange searchRange = [value rangeOfCharacterFromSet:sInvalidStringCharacters];
	if (searchRange.location == NSNotFound)
		return value;

	NSMutableString *result = [[[NSMutableString alloc] init] autorelease];
	if (!result) return nil;  // out of memory?

	NSUInteger strLen = [value length];
	searchRange.location = 0;
	searchRange.length = strLen;
	while (searchRange.location < strLen)
	{
		// Get the range of characters up to the first escaped character
		NSRange escapeRange = [value rangeOfCharacterFromSet:sInvalidStringCharacters options:NSLiteralSearch range:searchRange];
		if (escapeRange.location == NSNotFound)
		{
			// No more escape characters were found. Append the remainder of the value string and exit
			[result appendString:[value substringWithRange:searchRange]];
			break;
		}

		// Append any unescaped portion of the string
		if (escapeRange.location > searchRange.location)
			[result appendString:[value substringWithRange:NSMakeRange(searchRange.location, escapeRange.location - searchRange.location)]];

		// As long as we find characters that need to be escaped, process them
		unichar escChar = [value characterAtIndex:escapeRange.location];
		while ([sInvalidStringCharacters characterIsMember:escChar])
		{
			NSString *escapeString = nil;
			switch (escChar)
			{
				case '/': escapeString = @"\\/"; break;   // Not typically used... see note above
				case '\"': escapeString = @"\\\""; break;
				case '\\': escapeString = @"\\\\"; break;
				case '\b': escapeString = @"\\b"; break;
				case '\f': escapeString = @"\\f"; break;
				case '\n': escapeString = @"\\n"; break;
				case '\r': escapeString = @"\\r"; break;
				case '\t': escapeString = @"\\t"; break;
				default:
				{
					// We need to create a unicode escape sequence
					escapeString = [NSString stringWithFormat:@"\\u%04x", (unsigned int)escChar];
					break;
				}
			}

			// Append the escape sequence
			[result appendString:escapeString];

			// Advance to the next character (break if we are at the end of the value string)
			escapeRange.location += 1;
			if (escapeRange.location >= strLen)
				break;
			escChar = [value characterAtIndex:escapeRange.location];
		}

		// Set up the next search range
		searchRange.location = escapeRange.location;
		searchRange.length = strLen - searchRange.location;
	}

	return result;
}

- (id) initWithValue:(id)value
{
	return [self initWithValue:value memberInfo:nil];
}

- (id) initWithValue:(id)value memberInfo:(id)schema
{
	self = [super init];
	if (self)
	{
		_root = [value retain];
		_rootSchema = [schema retain];
		_objectStack = [[NSMutableArray alloc] init];
		_memberKeysSelector = NULL;
	}
	return self;
}

- (void) dealloc
{
	[_root release];
	[_rootSchema release];
	[_text release];
	[_objectStack release];
	[super dealloc];
}

- (NSString*) JSONText
{
	if (!_text)
	{
		_text = [[NSMutableString alloc] init];
		if (_rootSchema)
			[self appendObject:_root withMemberInfo:_rootSchema];
		else
			[self appendValue:_root];
		// MSDbgCheck([_objectStack count] == 0);
	}
	return [[_text retain] autorelease];
}

- (void) pushIndentLevel
{
	_indentLevel += 1;
}

- (void) popIndentLevel
{
	if (_indentLevel > 0)
		_indentLevel -= 1;
}

- (void) appendNewLine
{
	[self appendText:@"\n"];  // Should this be @"\r\n"?
	for (NSUInteger i = 0; i < _indentLevel; ++i)
		[self appendText:@"\t"];
}

- (void) appendText:(NSString*)text
{
	[_text appendString:text];
}

- (void) appendValue:(id)value
{
	if ([value respondsToSelector:@selector(JSONDescription)])
	{
		[self appendText:[value JSONDescription]];
	}
	else if (_memberKeysSelector && [value respondsToSelector:_memberKeysSelector])
	{
		id schema = [value performSelector:_memberKeysSelector];
		[self appendObject:value withMemberInfo:schema];
	}
	else if ([value respondsToSelector:@selector(JSONMemberKeys)])
	{
		id schema = [value JSONMemberKeys];
		[self appendObject:value withMemberInfo:schema];
	}
	else if ([value respondsToSelector:@selector(allKeys)])
	{
		NSArray *schema = [[value allKeys] sortedArrayUsingSelector:@selector(caseInsensitiveCompare:)];
		[self appendObject:value withMemberInfo:schema];
	}
	else if ([value conformsToProtocol:@protocol(NSFastEnumeration)])
	{
		[self appendCollection:value];
	}
	else
	{
		[self appendStringValue:[value description]];
	}
}

- (void) appendStringValue:(NSString*)value
{
	[self appendText:@"\""];
	[self appendText:[[self class] escapeStringValue:value]];
	[self appendText:@"\""];
}

- (void) appendObject:(id)value withMemberInfo:(id)schema
{
	// Check to see if this object is already on the stack. If so, then throw a cycle exception.
	if ([_objectStack indexOfObjectIdenticalTo:value] != NSNotFound)
		[NSException raise:MSJSONWriterCycleException format:@"Cycle found when generating JSON text: %08X", (unsigned int)value];

	// Add this object to the stack.
	[_objectStack addObject:value];

	// The member info schema should either be a dictionary (KVC key name -> JSON member name)
	// or an enumerable collection of strings (where KVC key name == JSON member name). If a
	// dictionary is passed, then the member key list will be the sorted list of dictionary keys.
	// Note that this means that the members will be sorted by *KVC key name*, not the JSON
	// member name, but at least the results should be deterministic.
	id memberKeyList = schema;
	NSDictionary *memberNameMap = nil;
	if ([schema isKindOfClass:[NSDictionary class]])
	{
		memberNameMap = schema;
		memberKeyList = [[memberNameMap allKeys] sortedArrayUsingSelector:@selector(caseInsensitiveCompare:)];
	}

	[self appendText:@"{"];
	[self pushIndentLevel];
	[self appendNewLine];
	BOOL firstPass = YES;
	for (id member in memberKeyList)
	{
		if (!firstPass)
		{
			[self appendText:@","];
			[self appendNewLine];
		}

		// Get the member name from the key map (if one is available)
		NSString *memberName = (memberNameMap) ? [memberNameMap objectForKey:member]: member;

		// Get the value for this property and write the JSON text results
		id propertyValue = nil;
		@try
		{
			propertyValue = [value valueForKey:member];
		}
		// Ignore exceptions and treat them like missing properties.
		@catch (...)
		{}

		// If we don't have a property value then skip to the next member.
		if (!propertyValue) continue;

		// Write the member name and property value.
		[self appendObjectProperty:propertyValue withMemberName:memberName];

		firstPass = NO;
	}
	[self popIndentLevel];
	[self appendNewLine];
	[self appendText:@"}"];

	// Pop this object from the stack.
	// MSDbgCheck([_objectStack lastObject] == value);
	[_objectStack removeLastObject];
}

- (void) appendObjectProperty:(id)value withMemberName:(NSString*)memberName
{
	[self appendStringValue:memberName];
	[self appendText:@": "];
	[self appendValue:value];
}

- (void) appendCollection:(id)value
{
	// Check to see if this object is already on the stack. If so, then throw a cycle exception.
	if ([_objectStack indexOfObjectIdenticalTo:value] != NSNotFound)
		[NSException raise:MSJSONWriterCycleException format:@"Cycle found when generating JSON text: %08X", (unsigned int)value];

	// Add this object to the stack.
	[_objectStack addObject:value];

	// Append the beginning of the collection and indent the contents.
	[self appendText:@"["];
	[self pushIndentLevel];
	[self appendNewLine];

	BOOL firstPass = YES;
	for (id item in value)
	{
		if (!firstPass)
		{
			[self appendText:@","];
			[self appendNewLine];
		}
		[self appendValue:item];
		firstPass = NO;
	}

	// Close the collection.
	[self popIndentLevel];
	[self appendNewLine];
	[self appendText:@"]"];

	// Pop this object from the stack.
	// MSDbgCheck([_objectStack lastObject] == value);
	[_objectStack removeLastObject];
}

@end



// ------------------------------------------------------------------------
// MSJSONWriter_Extensions

@implementation NSDate (MSJSONWriter_Extensions)
- (NSString*) JSONDescription
{
	// Generate the date text value (which is always represented as a string in JSON)
	NSString *dateString = [NSString stringWithFormat:@"\"\\/Date(%qi)\\/\"", (long long)([self timeIntervalSince1970] * 1000)];
	return dateString;
}
@end

@implementation NSNumber (MSJSONWriter_Extensions)
- (NSString*) JSONDescription
{
	// When an NSNumber value is initialized with +numberWithBool: it generates a
	// CFBooleanRef value (class == NSCFBoolean). This is the safest test to determine
	// if this NSNumber value is actually a boolean value
	if (CFGetTypeID(self) == CFBooleanGetTypeID())
		return [self boolValue] ? @"true" : @"false";

	// Force the locale for the text generation to en_US to match the JSON spec
	double d = [self doubleValue];
	if (fmod(d, 1.0) != 0.0)
	{
		NSLocale *localeUS = [[NSLocale alloc] initWithLocaleIdentifier:@"en_US"];
		NSString *numberString = [self descriptionWithLocale:localeUS];
		[localeUS release];
		return numberString;
	}
	else
	{
		long long ll = [self longLongValue];
		NSString *numberString = [NSString stringWithFormat:@"%lli", ll];
		return numberString;
	}
}
@end

@implementation NSNull (MSJSONWriter_Extensions)
- (NSString*) JSONDescription
{
	return @"null";
}
@end

