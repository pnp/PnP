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
        define(["require", "exports", "../../../Queryable"], factory);
    }
})(function (require, exports) {
    "use strict";
    /// <reference path="..\..\..\..\typings\main.d.ts" />
    var Queryable_1 = require("../../../Queryable");
    var Items = (function (_super) {
        __extends(Items, _super);
        function Items(url) {
            _super.call(this, url, "/Items");
        }
        Items.prototype.getById = function (id) {
            this._url.push("(" + id + ")");
            return this;
        };
        return Items;
    }(Queryable_1.Queryable));
    exports.Items = Items;
});
