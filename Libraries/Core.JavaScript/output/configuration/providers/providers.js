(function (factory) {/* istanbul ignore next */
    if (typeof module === 'object' && typeof module.exports === 'object') {
        var v = factory(require, exports); if (v !== undefined) module.exports = v;
    }
    else if (typeof define === 'function' && define.amd) {
        define(["require", "exports", "./cachingConfigurationProvider", "./spListConfigurationProvider"], factory);
    }
})(function (require, exports) {
    "use strict";
    var cachingConfigurationProvider_1 = require("./cachingConfigurationProvider");
    var spListConfigurationProvider_1 = require("./spListConfigurationProvider");
    exports.CachingConfigurationProvider = cachingConfigurationProvider_1.default;
    exports.SPListConfigurationProvider = spListConfigurationProvider_1.default;
});
