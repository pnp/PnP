(function (factory) {/* istanbul ignore next */
    if (typeof module === 'object' && typeof module.exports === 'object') {
        var v = factory(require, exports); if (v !== undefined) module.exports = v;
    }
    else if (typeof define === 'function' && define.amd) {
        define(["require", "exports", "../../Utils/Ajax"], factory);
    }
})(function (require, exports) {
    "use strict";
    /// <reference path="../../typings/main.d.ts" />
    var ajax = require("../../Utils/Ajax");
    /**
     * Queryable Base Class
     *
     */
    var Queryable = (function () {
        function Queryable(base, component) {
            this._url = base.concat([component]);
            this._query = [];
        }
        Queryable.prototype.select = function (select) {
            this._query.push("$select=" + select.join(","));
            return this;
        };
        Queryable.prototype.filter = function (filter) {
            this._query.push("$filter=" + filter);
            return this;
        };
        Queryable.prototype.url = function () {
            var url = this._url.join("");
            if (this._query.length > 0) {
                url += ("?" + this._query.join("&"));
            }
            return url;
        };
        Queryable.prototype.get = function () {
            var _this = this;
            return new Promise(function (resolve, reject) {
                ajax.get(_spPageContextInfo.webAbsoluteUrl + "/" + _this.url()).success(function (data) {
                    data.d.hasOwnProperty("results") ? resolve(data.d.results) : resolve(data.d);
                });
            });
        };
        return Queryable;
    }());
    exports.Queryable = Queryable;
});
