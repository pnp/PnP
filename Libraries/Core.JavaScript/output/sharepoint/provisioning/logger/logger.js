(function (factory) {/* istanbul ignore next */
    if (typeof module === 'object' && typeof module.exports === 'object') {
        var v = factory(require, exports); if (v !== undefined) module.exports = v;
    }
    else if (typeof define === 'function' && define.amd) {
        define(["require", "exports"], factory);
    }
})(function (require, exports) {
    "use strict";
    /// <reference path="..\..\..\..\typings\main.d.ts" />
    var Logger = (function () {
        function Logger() {
            this.isLoggerDefined = false;
            if (console && console.log) {
                this.isLoggerDefined = true;
            }
            this.spacing = "\t\t";
            this.template = "{0} " + this.spacing + " [{1}] " + this.spacing + " [{2}] " + this.spacing + " {3}";
        }
        Logger.prototype.info = function (object, message) {
            this.print(String.format(this.template, new Date(), object, "Information", message));
        };
        Logger.prototype.debug = function (object, message) {
            this.print(String.format(this.template, new Date(), object, "Debug", message));
        };
        Logger.prototype.error = function (object, message) {
            this.print(String.format(this.template, new Date(), object, "Error", message));
        };
        Logger.prototype.print = function (msg) {
            if (this.isLoggerDefined) {
                console.log(msg);
            }
        };
        return Logger;
    }());
    exports.Logger = Logger;
});
