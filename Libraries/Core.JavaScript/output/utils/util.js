(function (factory) {/* istanbul ignore next */
    if (typeof module === 'object' && typeof module.exports === 'object') {
        var v = factory(require, exports); if (v !== undefined) module.exports = v;
    }
    else if (typeof define === 'function' && define.amd) {
        define(["require", "exports"], factory);
    }
})(function (require, exports) {
    "use strict";
    /**
     * Gets a callback function which will maintain context across async calls.
     * Allows for the calling pattern getCtxCallback(thisobj, method, methodarg1, methodarg2, ...)
     *
     * @param context The object that will be the 'this' value in the callback
     * @param method The method to which we will apply the context and parameters
     * @param params Optional, additional arguments to supply to the wrapped method when it is invoked
     */
    function getCtxCallback(context, method) {
        var params = [];
        for (var _i = 2; _i < arguments.length; _i++) {
            params[_i - 2] = arguments[_i];
        }
        return function () {
            method.apply(context, params);
        };
    }
    exports.getCtxCallback = getCtxCallback;
    /**
     * Tests if a url param exists
     *
     * @param name The name of the url paramter to check
     */
    function urlParamExists(name) {
        name = name.replace(/[\[]/, "\\[").replace(/[\]]/, "\\]");
        var regex = new RegExp("[\\?&]" + name + "=([^&#]*)");
        return regex.test(location.search);
    }
    exports.urlParamExists = urlParamExists;
    /**
     * Gets a url param value by name
     *
     * @param name The name of the paramter for which we want the value
     */
    function getUrlParamByName(name) {
        name = name.replace(/[\[]/, "\\[").replace(/[\]]/, "\\]");
        var regex = new RegExp("[\\?&]" + name + "=([^&#]*)");
        var results = regex.exec(location.search);
        return results == null ? "" : decodeURIComponent(results[1].replace(/\+/g, " "));
    }
    exports.getUrlParamByName = getUrlParamByName;
    /**
     * Gets a url param by name and attempts to parse a bool value
     *
     * @param name The name of the paramter for which we want the boolean value
     */
    function getUrlParamBoolByName(name) {
        var p = getUrlParamByName(name);
        var isFalse = (p === "" || /false|0/i.test(p));
        return !isFalse;
    }
    exports.getUrlParamBoolByName = getUrlParamBoolByName;
    /**
     * Inserts the string s into the string target as the index specified by index
     *
     * @param target The string into which we will insert s
     * @param index The location in target to insert s (zero based)
     * @param s The string to insert into target at position index
     */
    function stringInsert(target, index, s) {
        if (index > 0) {
            return target.substring(0, index) + s + target.substring(index, target.length);
        }
        return s + target;
    }
    exports.stringInsert = stringInsert;
    /**
     * Adds a value to a date
     *
     * @param date The date to which we will add units, done in local time
     * @param interval The name of the interval to add, one of: ['year', 'quarter', 'month', 'week', 'day', 'hour', 'minute', 'second']
     * @param units The amount to add to date of the given interval
     *
     * http://stackoverflow.com/questions/1197928/how-to-add-30-minutes-to-a-javascript-date-object
     */
    function dateAdd(date, interval, units) {
        var ret = new Date(date.toLocaleString()); // don't change original date
        switch (interval.toLowerCase()) {
            case "year":
                ret.setFullYear(ret.getFullYear() + units);
                break;
            case "quarter":
                ret.setMonth(ret.getMonth() + 3 * units);
                break;
            case "month":
                ret.setMonth(ret.getMonth() + units);
                break;
            case "week":
                ret.setDate(ret.getDate() + 7 * units);
                break;
            case "day":
                ret.setDate(ret.getDate() + units);
                break;
            case "hour":
                ret.setTime(ret.getTime() + units * 3600000);
                break;
            case "minute":
                ret.setTime(ret.getTime() + units * 60000);
                break;
            case "second":
                ret.setTime(ret.getTime() + units * 1000);
                break;
            default:
                ret = undefined;
                break;
        }
        return ret;
    }
    exports.dateAdd = dateAdd;
    /**
     * Loads a stylesheet into the current page
     *
     * @param path The url to the stylesheet
     * @param avoidCache If true a value will be appended as a query string to avoid browser caching issues
     */
    function loadStylesheet(path, avoidCache) {
        if (avoidCache) {
            path += "?" + encodeURIComponent((new Date()).getTime().toString());
        }
        var head = document.getElementsByTagName("head");
        if (head.length > 1) {
            var e = document.createElement("link");
            head[0].appendChild(e);
            e.setAttribute("type", "text/css");
            e.setAttribute("rel", "stylesheet");
            e.setAttribute("href", path);
        }
    }
    exports.loadStylesheet = loadStylesheet;
    /**
     * Combines an arbitrary set of paths ensuring that the slashes are normalized
     *
     * @param paths 0 to n path parts to combine
     */
    function combinePaths() {
        var paths = [];
        for (var _i = 0; _i < arguments.length; _i++) {
            paths[_i - 0] = arguments[_i];
        }
        var parts = [];
        for (var i = 0; i < paths.length; i++) {
            parts.push(arguments[i].replace(/^[\\|\/]/, "").replace(/[\\|\/]$/, ""));
        }
        return parts.join("/").replace(/\\/, "/");
    }
    exports.combinePaths = combinePaths;
    /**
     * Gets a random string of chars length
     *
     * @param chars The length of the random string to generate
     */
    function getRandomString(chars) {
        var text = "";
        var possible = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        for (var i = 0; i < chars; i++) {
            text += possible.charAt(Math.floor(Math.random() * possible.length));
        }
        return text;
    }
    exports.getRandomString = getRandomString;
    /**
     * Gets a random GUID value
     *
     * http://stackoverflow.com/questions/105034/create-guid-uuid-in-javascript
     */
    function getGUID() {
        var d = new Date().getTime();
        var guid = "xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx".replace(/[xy]/g, function (c) {
            var r = (d + Math.random() * 16) % 16 | 0;
            d = Math.floor(d / 16);
            return (c === "x" ? r : (r & 0x3 | 0x8)).toString(16);
        });
        return guid;
    }
    exports.getGUID = getGUID;
    /**
     * Determines if a given value is a function
     *
     * @param candidateFunction The thing to test for being a function
     */
    function isFunction(candidateFunction) {
        return typeof candidateFunction === "function";
    }
    exports.isFunction = isFunction;
    /**
     * Determines if a string is null or empty or undefined
     *
     * @param s The string to test
     */
    function stringIsNullOrEmpty(s) {
        return typeof s === "undefined" || s === null || s === "";
    }
    exports.stringIsNullOrEmpty = stringIsNullOrEmpty;
});
