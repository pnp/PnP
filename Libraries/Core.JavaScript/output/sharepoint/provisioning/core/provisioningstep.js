// import { Promise } from "es6-promise";
(function (factory) {/* istanbul ignore next */
    if (typeof module === 'object' && typeof module.exports === 'object') {
        var v = factory(require, exports); if (v !== undefined) module.exports = v;
    }
    else if (typeof define === 'function' && define.amd) {
        define(["require", "exports"], factory);
    }
})(function (require, exports) {
    "use strict";
    var ProvisioningStep = (function () {
        function ProvisioningStep(name, index, objects, parameters, handler) {
            this.name = name;
            this.index = index;
            this.objects = objects;
            this.parameters = parameters;
            this.handler = handler;
        }
        ProvisioningStep.prototype.execute = function (dependentPromise) {
            var _this = this;
            var _handler = new this.handler();
            if (!dependentPromise) {
                return _handler.ProvisionObjects(this.objects, this.parameters);
            }
            return new Promise(function (resolve, reject) {
                dependentPromise.then(function () {
                    return _handler.ProvisionObjects(_this.objects, _this.parameters).then(resolve, resolve);
                });
            });
        };
        return ProvisioningStep;
    }());
    exports.ProvisioningStep = ProvisioningStep;
});
