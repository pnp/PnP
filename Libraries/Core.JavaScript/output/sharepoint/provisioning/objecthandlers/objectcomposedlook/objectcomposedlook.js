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
    /// <reference path="..\schema\icomposedlook.d.ts" />
    var Util_1 = require("../../../Util");
    var ObjectHandlerBase_1 = require("../ObjectHandlerBase/ObjectHandlerBase");
    var ObjectComposedLook = (function (_super) {
        __extends(ObjectComposedLook, _super);
        function ObjectComposedLook() {
            _super.call(this, "ComposedLook");
        }
        ObjectComposedLook.prototype.ProvisionObjects = function (object) {
            var _this = this;
            _super.prototype.scope_started.call(this);
            return new Promise(function (resolve, reject) {
                var clientContext = SP.ClientContext.get_current();
                var web = clientContext.get_web();
                var colorPaletteUrl = object.ColorPaletteUrl ? Util_1.replaceUrlTokens(object.ColorPaletteUrl) : "";
                var fontSchemeUrl = object.FontSchemeUrl ? Util_1.replaceUrlTokens(object.FontSchemeUrl) : "";
                var backgroundImageUrl = object.BackgroundImageUrl ? Util_1.replaceUrlTokens(object.BackgroundImageUrl) : null;
                web.applyTheme(Util_1.getRelativeUrl(colorPaletteUrl), Util_1.getRelativeUrl(fontSchemeUrl), backgroundImageUrl, true);
                web.update();
                clientContext.executeQueryAsync(function () {
                    _super.prototype.scope_ended.call(_this);
                    resolve();
                }, function () {
                    _super.prototype.scope_ended.call(_this);
                    resolve();
                });
            });
        };
        return ObjectComposedLook;
    }(ObjectHandlerBase_1.ObjectHandlerBase));
    exports.ObjectComposedLook = ObjectComposedLook;
});
