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
        define(["require", "exports", "../../Queryable", "./Items/Items"], factory);
    }
})(function (require, exports) {
    "use strict";
    /// <reference path="..\..\..\typings\main.d.ts" />
    var Queryable_1 = require("../../Queryable");
    var Items_1 = require("./Items/Items");
    var Lists = (function (_super) {
        __extends(Lists, _super);
        function Lists(url) {
            _super.call(this, url, "/lists");
        }
        Lists.prototype.getByTitle = function (title) {
            this._url.push("/getByTitle('" + title + "')");
            return jQuery.extend(this, { items: new Items_1.Items(this._url) });
        };
        Lists.prototype.getById = function (id) {
            this._url.push("('" + id + "')");
            return jQuery.extend(this, { items: new Items_1.Items(this._url) });
        };
        return Lists;
    }(Queryable_1.Queryable));
    exports.Lists = Lists;
});
