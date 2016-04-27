(function (factory) {/* istanbul ignore next */
    if (typeof module === 'object' && typeof module.exports === 'object') {
        var v = factory(require, exports); if (v !== undefined) module.exports = v;
    }
    else if (typeof define === 'function' && define.amd) {
        define(["require", "exports"], factory);
    }
})(function (require, exports) {
    "use strict";
    var MockLocation = (function () {
        function MockLocation() {
        }
        MockLocation.prototype.assign = function (url) {
            return;
        };
        MockLocation.prototype.reload = function (forcedReload) {
            return;
        };
        MockLocation.prototype.replace = function (url) {
            return;
        };
        MockLocation.prototype.toString = function () {
            return "MockLocation.toString";
        };
        return MockLocation;
    }());
    return MockLocation;
});
