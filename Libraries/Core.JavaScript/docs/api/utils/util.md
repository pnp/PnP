#PnP JavaScript Core#
##API Reference##

##Util##

The root object of the library and the object with which your interactions will likely begin.

**Source**: [util.ts](../../src/utils/util.ts)

###Properties###

Name | Description
---- | -----------
[util](utils/util.md) | Object, contains utility methods
sharepoint  | Object, contains methods for working with sharepoint
storage  | Object, contains methods for working with browser storage
configuration  | Object, contains methods for accessing configuration data
logging  | Object, contains methods for global application logging

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
