(function (factory) {/* istanbul ignore next */
    if (typeof module === 'object' && typeof module.exports === 'object') {
        var v = factory(require, exports); if (v !== undefined) module.exports = v;
    }
    else if (typeof define === 'function' && define.amd) {
        define(["require", "exports", "./Provisioning/Provisioning", "./Rest/Rest"], factory);
    }
})(function (require, exports) {
    "use strict";
    var Provisioning_1 = require("./Provisioning/Provisioning");
    var Rest_1 = require("./Rest/Rest");
    var SharePoint = (function () {
        function SharePoint() {
            /**
             * The REST base class for SharePoint
             */
            this.rest = new Rest_1.Rest();
            /**
            * The Provisioning base class for SharePoint
            */
            this.provisioning = new Provisioning_1.Provisioning();
        }
        return SharePoint;
    }());
    exports.SharePoint = SharePoint;
});
