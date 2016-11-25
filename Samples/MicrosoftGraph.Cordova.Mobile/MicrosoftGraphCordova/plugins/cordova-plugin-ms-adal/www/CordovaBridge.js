// Copyright (c) Microsoft Open Technologies, Inc.  All rights reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

/*global module, require*/

var exec = require('cordova/exec');
var Deferred = require('./utility').Utility.Deferred;

var GENERIC_ERR_MESSAGE = "Error occured while executing native method.";

/**
 * Implements proxy between Cordova JavaScript and Native functionality
 */
var cordovaBridge = {
    /**
     * Helper method to execute Cordova native method
     *
     * @param   {String}  nativeMethodName Method to execute.
     * @param   {Array}   args             Execution arguments.
     *
     * @returns {Promise} Promise which wraps method success/error callbacks.
     */
    executeNativeMethod : function (nativeMethodName, args) {
        var deferred = new Deferred();

        var win = function(res) {
            deferred.resolve(res);
        };

        var fail = function(err){

            if (typeof err === "string") {
                err = { errorDescription: err };
            }

            var error = new Error(err.errorDescription || err.message || err.Message || GENERIC_ERR_MESSAGE);
            error.details = err;

            deferred.reject(error);
        };

        exec(win, fail, "ADALProxy", nativeMethodName, args);

        return deferred;
    }
};

module.exports = cordovaBridge;
