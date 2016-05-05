(function (factory) {/* istanbul ignore next */
    if (typeof module === 'object' && typeof module.exports === 'object') {
        var v = factory(require, exports); if (v !== undefined) module.exports = v;
    }
    else if (typeof define === 'function' && define.amd) {
        define(["require", "exports", "./Web/Web"], factory);
    }
})(function (require, exports) {
    "use strict";
    var Web_1 = require("./Web/Web");
    /**
     * Root of the SharePoint REST module
     */
    var Rest = (function () {
        function Rest() {
            this.web = new Web_1.Web(["/_api"]);
        }
        return Rest;
    }());
    exports.Rest = Rest;
});
