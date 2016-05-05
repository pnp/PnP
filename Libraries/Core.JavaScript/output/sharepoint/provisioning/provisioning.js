(function (factory) {/* istanbul ignore next */
    if (typeof module === 'object' && typeof module.exports === 'object') {
        var v = factory(require, exports); if (v !== undefined) module.exports = v;
    }
    else if (typeof define === 'function' && define.amd) {
        define(["require", "exports", "./Core/Core", "./Logger/Logger"], factory);
    }
})(function (require, exports) {
    "use strict";
    var Core_1 = require("./Core/Core");
    var Logger_1 = require("./Logger/Logger");
    var Provisioning = (function () {
        function Provisioning() {
            this.core = new Core_1.Core();
        }
        return Provisioning;
    }());
    exports.Provisioning = Provisioning;
    exports.Log = new Logger_1.Logger();
});
