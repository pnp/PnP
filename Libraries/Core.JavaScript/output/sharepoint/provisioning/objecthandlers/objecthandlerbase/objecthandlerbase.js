(function (factory) {/* istanbul ignore next */
    if (typeof module === 'object' && typeof module.exports === 'object') {
        var v = factory(require, exports); if (v !== undefined) module.exports = v;
    }
    else if (typeof define === 'function' && define.amd) {
        define(["require", "exports", "../../Resources/Resources", "../../Provisioning"], factory);
    }
})(function (require, exports) {
    "use strict";
    // import { Promise } from "es6-promise";
    var Resources = require("../../Resources/Resources");
    var Provisioning_1 = require("../../Provisioning");
    var ObjectHandlerBase = (function () {
        function ObjectHandlerBase(name) {
            this.name = name;
        }
        ObjectHandlerBase.prototype.ProvisionObjects = function (objects, parameters) {
            return new Promise(function (resolve, reject) { resolve("Not implemented."); });
        };
        ObjectHandlerBase.prototype.scope_started = function () {
            Provisioning_1.Log.info(this.name, Resources.Code_execution_started);
        };
        ObjectHandlerBase.prototype.scope_ended = function () {
            Provisioning_1.Log.info(this.name, Resources.Code_execution_ended);
        };
        return ObjectHandlerBase;
    }());
    exports.ObjectHandlerBase = ObjectHandlerBase;
});
