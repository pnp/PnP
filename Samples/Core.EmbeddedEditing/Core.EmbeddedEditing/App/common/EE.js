(function ($, EE, window, undefined) {
    "use strict";

    /* Generic Helpers */
    EE.Utilities = EE.Utilities || {};

    EE.Utilities.convertDictionaryToListOfKeyValuePairs = function (o) {
        var list = Object.keys(o).map(function (key) {
            return {
                key: key,
                value: o[key]
            };
        });

        return list;
    };

    EE.Utilities.getAppWebResource = function (uri) {
        var promise = $.ajax({
            url: uri,
            method: "GET",
            headers: { "Accept": "application/json; odata=verbose" },
        })
            .then(function (odata) {
                return odata.d && odata.d.hasOwnProperty('results') && odata.d.results;
            })
        ;

        return promise;
    };

    EE.Utilities.parseList = function (list) {
        return {
            id: list.Id,
            title: list.Title,
            templateId: list.BaseTemplate
        };
    };

    EE.Utilities.parseListItem = function (item) {
        return {
            id: item.Id,
            etag: item.__metadata.etag,
            appPartId: item.AppPartId,
            type: item.Type,
            key: item.Key,
            value: item.String
        };
    };

    EE.Utilities.parseHostListItem = function (item) {
        return {
            id: item.Id,
            title: item.Title
        };
    };

    /**
     * Creates an object with the keys provided if they are found within the list of items.
     * 
     * @function getRelaventPropertiesFromListItems
     * @keys {Array} List of keys to look.
     * @items {Array} List of `config` items
     * @returns {Object} Object with key/value pairs found within both the items and keys list.
     */
    EE.Utilities.getRelaventPropertiesFromListItems = function (keys) {
        return function (items) {
            var properties = {};

            // For each item in the config list, 
            // if an item has a key in the list of keys provided, store it on the 
            // properties object.
            items.forEach(function (item) {
                if (keys.indexOf(item.key) >= 0) {
                    properties[item.key] = {
                        id: item.id
                        , etag: item.etag
                        , value: item.value
                    };
                }
            });

            return properties;
        };
    };

    /**
     * Coerces properties from strings to the expected data type.
     * Currently this only coerces strings to numbers based on the property type of the default value
     * however it could be extended to do whatever you need.
     * 
     * @function coercePropertyValues
     * @param defaultProperties {Object} App Part Default Properties
     * @param configProperties {Object} Properties found in config list, originally all strings
     * @returns {Object} which excepts properties from config and coerces them based on {defaultProperties} types.
     */
    EE.Utilities.coercePropertyValues = function (defaultProperties) {
        return function (configProperties) {
            // Recursive extend becuse properties object now contains pointers to nested objects
            var coercedProperties = $.extend(true, {}, configProperties);

            // At this point we have a copy of the config properties
            // Go through all the values and if one of them is supposed to be a number type
            // change it
            Object.keys(configProperties).forEach(function (key) {
                if (defaultProperties.hasOwnProperty(key)) {
                    if (typeof defaultProperties[key].value === "number") {
                        coercedProperties[key].value = parseInt(coercedProperties[key].value, 10);
                    }
                }
            });

            return coercedProperties;
        };
    };

    /**
     * Sets the value at path on object.
     * 
     * @param o {Object} Target object to set properties on.
     * @param path {String} string path with dot notation
     * @param value {Object} Value to set at the path
     */
    EE.Utilities.setProperty = function (o, path, value) {
        var keys = path.split('.')
            , lastKey = keys.pop()
            , keysLength = keys.length
            , i
            , currentObj = o
            , currentKey
            , currentType
        ;

        for (i = 0; i < keysLength; i++) {
            currentKey = keys[i];
            currentType = typeof currentObj[currentKey];

            if ((currentType === "undefined") || (currentObj[currentKey] === null)) {
                currentObj[currentKey] = {};
            }
            else if (typeof currentObj[currentKey] !== "object") {
                throw new Error('When traversing the object the path encountered a non-object value. Please ensure the object your are attempting to set and the path.');
            }

            currentObj = currentObj[currentKey];
        }

        currentObj[lastKey] = value;
        return currentObj[lastKey];
    };

    EE.Validate = EE.Validate || {};

    /**
     * Given a map which associates validation functions with propertynames/keys, go through
     * all the keys in an object and run the corresponding validate function if it's found in the
     * map.  If an error is returned, add it to an array and reject the current promise.
     * 
     * @function propertiesByMap
     * @param keyValidatorMapping {Object} Object with propties which map to properties on the objects being validated.
     * @param o {Object} Any object to validate with the given `keyValidatorMapping`
     * @return {Object} Returns original object if it passes validation otherwise, throws error and rejects with errors {Array}
     */
    EE.Validate.propertiesByMap = function (keyValidatorMapping) {
        return function (o) {
            var errors = []
                , result
                , e
            ;

            Object.keys(o).forEach(function (key) {
                if (keyValidatorMapping.hasOwnProperty(key)) {
                    result = keyValidatorMapping[key](o[key].value);
                    if (result !== true) {
                        e = {
                            key: key,
                            value: result
                        };
                        errors.push(e);
                    }
                }
            });

            if (errors.length > 0) {
                // This is just used to reject the current promise
                // TODO: Possibly refactor to juse RSVP.Promise.reject(errors)
                throw {
                    name: 'Error',
                    message: "One or more properties is not valid!",
                    errors: errors
                };
            }

            // If we've fallen through to here, disregard errors array since it's empty
            // and let the current promise chain continue uninterrupted
            return o;
        };
    };

    /**
     * Angular specific error handling. Given a function which adds values to the scope
     * If the error object returned has the expected errors array, it will call the 
     * function with the errors array.
     * Otherwise, just pass the error through and remain rejected for another error handler
     * to handle or to bubble to the window.
     * 
     *  * Current this function is overly abstract becuase I wanted to re-use the applyScopeFunc
     *    which is preconfigurd to add errors to $scope.errors, but all the passing of information
     *    could be confusing / too much dependency on functions that should have been refactored
     *    to begin with
     * 
     * @function handleValidationErrors
     * @applyScopeFunc {Function} Funtion which takes an argument and applies it the scope
     * @error {Object} Error object with expected errors array added
     */
    EE.Validate.handleValidationErrors = function (applyScopeFunc) {
        return function (error) {
            if (error.hasOwnProperty('errors')) {
                applyScopeFunc(error.errors);
            }

            return error;
        };
    };


    /* Proimse Helpers */

    EE.Promise = EE.Promise || {};

    EE.Promise.map = function (f) {
        return function (xs) {
            return xs.map(f);
        };
    };

    EE.Promise.logResponse = function (msg) {
        return function (items) {
            console && console.log(msg, items);
            return items;
        };
    };

})(jQuery, window.EE = window.EE || {}, this);