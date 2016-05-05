(function (factory) {/* istanbul ignore next */
    if (typeof module === 'object' && typeof module.exports === 'object') {
        var v = factory(require, exports); if (v !== undefined) module.exports = v;
    }
    else if (typeof define === 'function' && define.amd) {
        define(["require", "exports", "./utils/Util", "./SharePoint/SharePoint", "./utils/Storage", "./configuration/configuration", "./utils/logging"], factory);
    }
})(function (require, exports) {
    "use strict";
    var Util = require("./utils/Util");
    var SharePoint_1 = require("./SharePoint/SharePoint");
    var Storage_1 = require("./utils/Storage");
    var Configuration = require("./configuration/configuration");
    var logging_1 = require("./utils/logging");
    /**
     * Root class of the Patterns and Practices namespace, provides an entry point to the library
     */
    var PnP = (function () {
        function PnP() {
        }
        /**
         * Utility methods
         */
        PnP.util = Util;
        /**
         * SharePoint
         */
        PnP.sharepoint = new SharePoint_1.SharePoint();
        /**
         * Provides access to local and session storage through
         */
        PnP.storage = new Storage_1.PnPClientStorage();
        /**
         * Configuration
         */
        PnP.configuration = Configuration;
        /**
         * Global logging instance to which subscribers can be registered and messages written
         */
        PnP.logging = new logging_1.Logger();
        return PnP;
    }());
    return PnP;
});
