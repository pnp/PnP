var __extends = (this && this.__extends) || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
    function __() { this.constructor = d; }
    d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
};
(function (factory) {/* istanbul ignore next */
    if (typeof module === 'object' && typeof module.exports === 'object') {
        var v = factory(require, exports); if (v !== undefined) module.exports = v;
    }
    else if (typeof define === 'function' && define.amd) {
        define(["require", "exports", "../ObjectHandlerBase/ObjectHandlerBase"], factory);
    }
})(function (require, exports) {
    "use strict";
    /// <reference path="..\schema\ifeature.d.ts" />
    // import { Promise } from "es6-promise";
    var ObjectHandlerBase_1 = require("../ObjectHandlerBase/ObjectHandlerBase");
    var ObjectFeatures = (function (_super) {
        __extends(ObjectFeatures, _super);
        function ObjectFeatures() {
            _super.call(this, "Features");
        }
        ObjectFeatures.prototype.ProvisionObjects = function (features) {
            var _this = this;
            _super.prototype.scope_started.call(this);
            return new Promise(function (resolve, reject) {
                var clientContext = SP.ClientContext.get_current();
                var web = clientContext.get_web();
                var webFeatures = web.get_features();
                features.forEach(function (f) {
                    if (f.Deactivate === true) {
                        webFeatures.remove(new SP.Guid(f.ID), true);
                    }
                    else {
                        webFeatures.add(new SP.Guid(f.ID), true, SP.FeatureDefinitionScope.none);
                    }
                });
                web.update();
                clientContext.load(webFeatures);
                clientContext.executeQueryAsync(function () {
                    _super.prototype.scope_ended.call(_this);
                    resolve();
                }, function () {
                    _super.prototype.scope_ended.call(_this);
                    resolve();
                });
            });
        };
        return ObjectFeatures;
    }(ObjectHandlerBase_1.ObjectHandlerBase));
    exports.ObjectFeatures = ObjectFeatures;
});
