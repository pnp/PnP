"use strict";

import * as Util from "./util";

/**
 * Throws an exception if the supplied string value is null or emptry
 * 
 * @param value The string to test
 * @param parameterName The name of the parameter, included in the thrown exception message
 */
export function stringIsNullOrEmpty(value: string, parameterName: string): void {
    if (Util.stringIsNullOrEmpty(value)) {
        throw "Parameter '" + parameterName + "' cannot be null or empty.";
    }
}

/**
 * Throws an exception if the supplied object is null
 * 
 * @param value The object to test
 * @param parameterName The name of the parameter, included in the thrown exception message
 */
export function objectIsNull(value: Object, parameterName: string): void {
    if (typeof value === "undefined" || value === null) {
        throw "Parameter '" + parameterName + "' cannot be null.";
    }
}
