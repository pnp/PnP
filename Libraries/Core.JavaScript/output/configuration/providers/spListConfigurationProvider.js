(function (factory) {/* istanbul ignore next */
    if (typeof module === 'object' && typeof module.exports === 'object') {
        var v = factory(require, exports); if (v !== undefined) module.exports = v;
    }
    else if (typeof define === 'function' && define.amd) {
        define(["require", "exports", "es6-promise", "./cachingConfigurationProvider", "../../Utils/Ajax"], factory);
    }
})(function (require, exports) {
    "use strict";
    var es6_promise_1 = require("es6-promise");
    var cachingConfigurationProvider_1 = require("./cachingConfigurationProvider");
    var ajax = require("../../Utils/Ajax");
    var SPListConfigurationProvider = (function () {
        function SPListConfigurationProvider(webUrl, listTitle) {
            if (listTitle === void 0) { listTitle = "config"; }
            this.webUrl = webUrl;
            this.listTitle = listTitle;
        }
        SPListConfigurationProvider.prototype.getWebUrl = function () {
            return this.webUrl;
        };
        SPListConfigurationProvider.prototype.getListTitle = function () {
            return this.listTitle;
        };
        SPListConfigurationProvider.prototype.getConfiguration = function () {
            var _this = this;
            return new es6_promise_1.Promise(function (resolve, reject) {
                var url = _this.webUrl + "/_api/web/lists/getByTitle('" + _this.listTitle + "')/items?$select=Title,Value";
                ajax.get(url).success(function (data) {
                    var results = (data.d.hasOwnProperty("results")) ? data.d.results : data.d;
                    var configuration = {};
                    results.forEach(function (i) {
                        configuration[i.Title] = i.Value;
                    });
                    resolve(configuration);
                });
            });
        };
        SPListConfigurationProvider.prototype.asCaching = function () {
            var cacheKey = "splist_" + this.webUrl + "+" + this.listTitle;
            return new cachingConfigurationProvider_1.default(this, cacheKey);
        };
        return SPListConfigurationProvider;
    }());
    Object.defineProperty(exports, "__esModule", { value: true });
    exports.default = SPListConfigurationProvider;
});
