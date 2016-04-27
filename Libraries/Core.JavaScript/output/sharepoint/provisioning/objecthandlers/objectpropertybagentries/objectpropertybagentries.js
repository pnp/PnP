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
        define(["require", "exports", "../../../Util", "../ObjectHandlerBase/ObjectHandlerBase"], factory);
    }
})(function (require, exports) {
    "use strict";
    // "use strict";
    // 
    // /// <reference path="..\schema\ipropertybagentry.d.ts" />
    // import { Promise } from "es6-promise";
    var Util_1 = require("../../../Util");
    var ObjectHandlerBase_1 = require("../ObjectHandlerBase/ObjectHandlerBase");
    var ObjectPropertyBagEntries = (function (_super) {
        __extends(ObjectPropertyBagEntries, _super);
        function ObjectPropertyBagEntries() {
            _super.call(this, "PropertyBagEntries");
        }
        ObjectPropertyBagEntries.prototype.ProvisionObjects = function (entries) {
            var _this = this;
            _super.prototype.scope_started.call(this);
            return new Promise(function (resolve, reject) {
                if (!entries || entries.length === 0) {
                    resolve();
                }
                else {
                    var clientContext_1 = SP.ClientContext.get_current();
                    var web_1 = clientContext_1.get_web();
                    var propBag_1 = web_1.get_allProperties();
                    var indexedProperties_1 = [];
                    for (var i = 0; i < entries.length; i++) {
                        var entry = entries[i];
                        propBag_1.set_item(entry.Key, entry.Value);
                        if (entry.Indexed) {
                            indexedProperties_1.push(Util_1.encodePropertyKey(entry.Key));
                        }
                        ;
                    }
                    ;
                    web_1.update();
                    clientContext_1.load(propBag_1);
                    clientContext_1.executeQueryAsync(function () {
                        if (indexedProperties_1.length > 0) {
                            propBag_1.set_item("vti_indexedpropertykeys", indexedProperties_1.join("|"));
                            web_1.update();
                            clientContext_1.executeQueryAsync(function () {
                                _super.prototype.scope_ended.call(_this);
                                resolve();
                            }, function () {
                                _super.prototype.scope_ended.call(_this);
                                resolve();
                            });
                        }
                        else {
                            _super.prototype.scope_ended.call(_this);
                            resolve();
                        }
                    }, function () {
                        _super.prototype.scope_ended.call(_this);
                        resolve();
                    });
                }
            });
        };
        return ObjectPropertyBagEntries;
    }(ObjectHandlerBase_1.ObjectHandlerBase));
    exports.ObjectPropertyBagEntries = ObjectPropertyBagEntries;
});
