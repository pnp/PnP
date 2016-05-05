/**
 * Gets a callback function which will maintain context across async calls.
 * Allows for the calling pattern getCtxCallback(thisobj, method, methodarg1, methodarg2, ...)
 *
 * @param context The object that will be the 'this' value in the callback
 * @param method The method to which we will apply the context and parameters
 * @param params Optional, additional arguments to supply to the wrapped method when it is invoked
 */
export declare function getCtxCallback(context: any, method: Function, ...params: any[]): Function;
/**
 * Tests if a url param exists
 *
 * @param name The name of the url paramter to check
 */
export declare function urlParamExists(name: string): boolean;
/**
 * Gets a url param value by name
 *
 * @param name The name of the paramter for which we want the value
 */
export declare function getUrlParamByName(name: string): string;
/**
 * Gets a url param by name and attempts to parse a bool value
 *
 * @param name The name of the paramter for which we want the boolean value
 */
export declare function getUrlParamBoolByName(name: string): boolean;
/**
 * Inserts the string s into the string target as the index specified by index
 *
 * @param target The string into which we will insert s
 * @param index The location in target to insert s (zero based)
 * @param s The string to insert into target at position index
 */
export declare function stringInsert(target: string, index: number, s: string): string;
/**
 * Adds a value to a date
 *
 * @param date The date to which we will add units, done in local time
 * @param interval The name of the interval to add, one of: ['year', 'quarter', 'month', 'week', 'day', 'hour', 'minute', 'second']
 * @param units The amount to add to date of the given interval
 *
 * http://stackoverflow.com/questions/1197928/how-to-add-30-minutes-to-a-javascript-date-object
 */
export declare function dateAdd(date: Date, interval: string, units: number): Date;
/**
 * Loads a stylesheet into the current page
 *
 * @param path The url to the stylesheet
 * @param avoidCache If true a value will be appended as a query string to avoid browser caching issues
 */
export declare function loadStylesheet(path: string, avoidCache: boolean): void;
/**
 * Combines an arbitrary set of paths ensuring that the slashes are normalized
 *
 * @param paths 0 to n path parts to combine
 */
export declare function combinePaths(...paths: string[]): string;
/**
 * Gets a random string of chars length
 *
 * @param chars The length of the random string to generate
 */
export declare function getRandomString(chars: number): string;
/**
 * Gets a random GUID value
 *
 * http://stackoverflow.com/questions/105034/create-guid-uuid-in-javascript
 */
export declare function getGUID(): string;
/**
 * Determines if a given value is a function
 *
 * @param candidateFunction The thing to test for being a function
 */
export declare function isFunction(candidateFunction: any): boolean;
/**
 * Determines if a string is null or empty or undefined
 *
 * @param s The string to test
 */
export declare function stringIsNullOrEmpty(s: string): boolean;
