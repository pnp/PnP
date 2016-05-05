(function (factory) {/* istanbul ignore next */
    if (typeof module === 'object' && typeof module.exports === 'object') {
        var v = factory(require, exports); if (v !== undefined) module.exports = v;
    }
    else if (typeof define === 'function' && define.amd) {
        define(["require", "exports", "./ProvisioningStep", "../ObjectHandlers/ObjectNavigation/ObjectNavigation", "../ObjectHandlers/ObjectPropertyBagEntries/ObjectPropertyBagEntries", "../ObjectHandlers/ObjectFeatures/ObjectFeatures", "../ObjectHandlers/ObjectWebSettings/ObjectWebSettings", "../ObjectHandlers/ObjectComposedLook/ObjectComposedLook", "../ObjectHandlers/ObjectCustomActions/ObjectCustomActions", "../ObjectHandlers/ObjectFiles/ObjectFiles", "../ObjectHandlers/ObjectLists/ObjectLists", "../../Util", "../Resources/Resources", "../Provisioning"], factory);
    }
})(function (require, exports) {
    "use strict";
    /// <reference path="..\..\..\..\typings\main.d.ts" />
    /// <reference path="..\schema\schema.d.ts" />
    /// <reference path="iwaitmessageoptions.d.ts" />
    /// <reference path="options.d.ts" />
    // import { Promise } from "es6-promise";
    var ProvisioningStep_1 = require("./ProvisioningStep");
    var ObjectNavigation_1 = require("../ObjectHandlers/ObjectNavigation/ObjectNavigation");
    var ObjectPropertyBagEntries_1 = require("../ObjectHandlers/ObjectPropertyBagEntries/ObjectPropertyBagEntries");
    var ObjectFeatures_1 = require("../ObjectHandlers/ObjectFeatures/ObjectFeatures");
    var ObjectWebSettings_1 = require("../ObjectHandlers/ObjectWebSettings/ObjectWebSettings");
    var ObjectComposedLook_1 = require("../ObjectHandlers/ObjectComposedLook/ObjectComposedLook");
    var ObjectCustomActions_1 = require("../ObjectHandlers/ObjectCustomActions/ObjectCustomActions");
    var ObjectFiles_1 = require("../ObjectHandlers/ObjectFiles/ObjectFiles");
    var ObjectLists_1 = require("../ObjectHandlers/ObjectLists/ObjectLists");
    var Util_1 = require("../../Util");
    var Resources = require("../Resources/Resources");
    var Provisioning_1 = require("../Provisioning");
    var Core = (function () {
        function Core() {
            this.handlers = {
                "Navigation": ObjectNavigation_1.ObjectNavigation,
                "PropertyBagEntries": ObjectPropertyBagEntries_1.ObjectPropertyBagEntries,
                "Features": ObjectFeatures_1.ObjectFeatures,
                "WebSettings": ObjectWebSettings_1.ObjectWebSettings,
                "ComposedLook": ObjectComposedLook_1.ObjectComposedLook,
                "CustomActions": ObjectCustomActions_1.ObjectCustomActions,
                "Files": ObjectFiles_1.ObjectFiles,
                "Lists": ObjectLists_1.ObjectLists,
            };
        }
        Core.prototype.applyTemplate = function (path, _options) {
            var _this = this;
            var url = Util_1.replaceUrlTokens(path);
            this.options = _options || {};
            return new Promise(function (resolve, reject) {
                jQuery.getJSON(url, function (template) {
                    _this.start(template, Object.keys(template)).then(resolve, resolve);
                }).fail(function () {
                    Provisioning_1.Log.error("Provisioning", Resources.Template_invalid);
                });
            });
        };
        Core.prototype.start = function (json, queue) {
            var _this = this;
            Provisioning_1.Log.info("Provisioning", Resources.Code_execution_started);
            return new Promise(function (resolve, reject) {
                _this.startTime = new Date().getTime();
                _this.queueItems = [];
                queue.forEach(function (q, index) {
                    if (!_this.handlers[q]) {
                        return;
                    }
                    _this.queueItems.push(new ProvisioningStep_1.ProvisioningStep(q, index, json[q], json.Parameters, _this.handlers[q]));
                });
                var promises = [];
                promises.push(jQuery.Deferred());
                promises[0].resolve();
                promises[0].promise();
                var index = 1;
                while (_this.queueItems[index - 1] !== undefined) {
                    var i = promises.length - 1;
                    promises.push(_this.queueItems[index - 1].execute(promises[i]));
                    index++;
                }
                ;
                Promise.all(promises).then(function () {
                    Provisioning_1.Log.info("Provisioning", Resources.Code_execution_ended);
                }, function () {
                    Provisioning_1.Log.info("Provisioning", Resources.Code_execution_ended);
                });
            });
        };
        return Core;
    }());
    exports.Core = Core;
});
