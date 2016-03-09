#PnP JavaScript Core#
##API Reference - Util##

A utility library to provide global methods to support common actions.

**Source**: [util.ts](../../src/utils/util.ts)
**Tests**: [util.ts](../../src/utils/util.test.ts)

###Properties###

none

###Methods###

Name | Description
---- | -----------
getCtxCallback | Gets a callback function which will maintain context across async calls. Allows for the calling pattern getCtxCallback(thisobj, method, methodarg1, methodarg2, ...)
urlParamExists  | Determines if a given URL query parameter exists by name
getUrlParamByName  | Gets a URL query parameter's value by name
getUrlParamBoolByName  | Gets a URL query parameter's value by name, converting it to a boolean value
stringInsert  | Inserts a string into another string
dateAdd  | Adds (or subtracts) an amount to a date
loadStylesheet  | Loads a stylesheet into the current document
combinePaths  | Combines an arbitrary set of paths ensuring that the slashes are normalized
getRandomString  | Gets a random string of the specified length
getGUID  | Gets a random GUID
isFunction  | Determines if the supplied argument is a function
stringIsNullOrEmpty  | Determines if the supplied argument is null or empty
