(function (factory) {/* istanbul ignore next */
    if (typeof module === 'object' && typeof module.exports === 'object') {
        var v = factory(require, exports); if (v !== undefined) module.exports = v;
    }
    else if (typeof define === 'function' && define.amd) {
        define(["require", "exports", "../../utils/storage", "es6-promise"], factory);
    }
})(function (require, exports) {
    "use strict";
    var storage = require("../../utils/storage");
    var es6_promise_1 = require("es6-promise");
    var CachingConfigurationProvider = (function () {
        function CachingConfigurationProvider(wrappedProvider, cacheKey, cacheStore) {
            this.wrappedProvider = wrappedProvider;
            this.store = (cacheStore) ? cacheStore : this.selectPnPCache();
            this.cacheKey = "_configcache_" + cacheKey;
        }
        CachingConfigurationProvider.prototype.getWrappedProvider = function () {
            return this.wrappedProvider;
        };
        CachingConfigurationProvider.prototype.getConfiguration = function () {
            var _this = this;
            // Cache not available, pass control to  the wrapped provider
            if ((!this.store) || (!this.store.enabled)) {
                return this.wrappedProvider.getConfiguration();
            }
            // Value is found in cache, return it directly
            var cachedConfig = this.store.get(this.cacheKey);
            if (cachedConfig) {
                return new es6_promise_1.Promise(function (resolve, reject) {
                    resolve(cachedConfig);
                });
            }
            // Get and cache value from the wrapped provider
            var providerPromise = this.wrappedProvider.getConfiguration();
            providerPromise.then(function (providedConfig) {
                _this.store.put(_this.cacheKey, providedConfig);
            });
            return providerPromise;
        };
        CachingConfigurationProvider.prototype.selectPnPCache = function () {
            var pnpCache = new storage.PnPClientStorage();
            if ((pnpCache.local) && (pnpCache.local.enabled)) {
                return pnpCache.local;
            }
            if ((pnpCache.session) && (pnpCache.session.enabled)) {
                return pnpCache.session;
            }
            throw new Error("Cannot create a caching configuration provider since cache is not available.");
        };
        return CachingConfigurationProvider;
    }());
    Object.defineProperty(exports, "__esModule", { value: true });
    exports.default = CachingConfigurationProvider;
});
