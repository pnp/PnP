(function (factory) {/* istanbul ignore next */
    if (typeof module === 'object' && typeof module.exports === 'object') {
        var v = factory(require, exports); if (v !== undefined) module.exports = v;
    }
    else if (typeof define === 'function' && define.amd) {
        define(["require", "exports", "../collections/collections"], factory);
    }
})(function (require, exports) {
    "use strict";
    var Collections = require("../collections/collections");
    var MockStorage = (function () {
        function MockStorage() {
            this._store = new Collections.Dictionary();
        }
        Object.defineProperty(MockStorage.prototype, "length", {
            get: function () {
                return this._store.count();
            },
            set: function (i) {
                return;
            },
            enumerable: true,
            configurable: true
        });
        MockStorage.prototype.clear = function () {
            this._store.clear();
        };
        MockStorage.prototype.getItem = function (key) {
            return this._store.get(key);
        };
        MockStorage.prototype.key = function (index) {
            return this._store.getKeys()[index];
        };
        MockStorage.prototype.removeItem = function (key) {
            this._store.remove(key);
        };
        MockStorage.prototype.setItem = function (key, data) {
            this._store.add(key, data);
        };
        return MockStorage;
    }());
    return MockStorage;
});
