(function (factory) {/* istanbul ignore next */
    if (typeof module === 'object' && typeof module.exports === 'object') {
        var v = factory(require, exports); if (v !== undefined) module.exports = v;
    }
    else if (typeof define === 'function' && define.amd) {
        define(["require", "exports"], factory);
    }
})(function (require, exports) {
    "use strict";
    exports.Code_execution_started = "Code execution scope started";
    exports.Code_execution_ended = "Code execution scope ended";
    exports.Template_invalid = "The provided template is invalid";
});
