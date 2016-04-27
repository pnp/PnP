(function (factory) {/* istanbul ignore next */
    if (typeof module === 'object' && typeof module.exports === 'object') {
        var v = factory(require, exports); if (v !== undefined) module.exports = v;
    }
    else if (typeof define === 'function' && define.amd) {
        define(["require", "exports", "./util"], factory);
    }
})(function (require, exports) {
    "use strict";
    var Util = require("./util");
    /**
     * Throws an exception if the supplied string value is null or emptry
     *
     * @param value The string to test
     * @param parameterName The name of the parameter, included in the thrown exception message
     */
    function stringIsNullOrEmpty(value, parameterName) {
        if (Util.stringIsNullOrEmpty(value)) {
            throw "Parameter '" + parameterName + "' cannot be null or empty.";
        }
    }
    exports.stringIsNullOrEmpty = stringIsNullOrEmpty;
    /**
     * Throws an exception if the supplied object is null
     *
     * @param value The object to test
     * @param parameterName The name of the parameter, included in the thrown exception message
     */
    function objectIsNull(value, parameterName) {
        if (typeof value === "undefined" || value === null) {
            throw "Parameter '" + parameterName + "' cannot be null.";
        }
    }
    exports.objectIsNull = objectIsNull;
});
