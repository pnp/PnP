(function (factory) {/* istanbul ignore next */
    if (typeof module === 'object' && typeof module.exports === 'object') {
        var v = factory(require, exports); if (v !== undefined) module.exports = v;
    }
    else if (typeof define === 'function' && define.amd) {
        define(["require", "exports", "chai", "./cachingConfigurationProvider", "../configuration", "../../mocks/mockConfigurationProvider", "../../mocks/MockStorage", "../../utils/storage"], factory);
    }
})(function (require, exports) {
    "use strict";
    var chai_1 = require("chai");
    var cachingConfigurationProvider_1 = require("./cachingConfigurationProvider");
    var Configuration = require("../configuration");
    var mockConfigurationProvider_1 = require("../../mocks/mockConfigurationProvider");
    var MockStorage = require("../../mocks/MockStorage");
    var storage = require("../../utils/storage");
    describe("Configuration", function () {
        describe("CachingConfigurationProvider", function () {
            var wrapped;
            var store;
            var settings;
            beforeEach(function () {
                var mockValues = {
                    "key1": "value1",
                    "key2": "value2",
                };
                wrapped = new mockConfigurationProvider_1.default();
                wrapped.mockValues = mockValues;
                store = new storage.PnPClientStorageWrapper(new MockStorage());
                settings = new Configuration.Settings();
            });
            it("Loads the config from the wrapped provider", function () {
                var provider = new cachingConfigurationProvider_1.default(wrapped, "cacheKey", store);
                return settings.load(provider).then(function () {
                    chai_1.expect(settings.get("key1")).to.eq("value1");
                    chai_1.expect(settings.get("key2")).to.eq("value2");
                });
            });
            it("Returns cached values", function () {
                var provider = new cachingConfigurationProvider_1.default(wrapped, "cacheKey", store);
                return settings.load(provider).then(function () {
                    var updatedValues = {
                        "key1": "update1",
                        "key2": "update2",
                    };
                    wrapped.mockValues = updatedValues;
                    return settings.load(provider);
                }).then(function () {
                    chai_1.expect(settings.get("key1")).to.eq("value1");
                    chai_1.expect(settings.get("key2")).to.eq("value2");
                });
            });
            it("Bypasses a disabled cache", function () {
                store.enabled = false;
                var provider = new cachingConfigurationProvider_1.default(wrapped, "cacheKey", store);
                return settings.load(provider).then(function () {
                    var updatedValues = {
                        "key1": "update1",
                        "key2": "update2",
                    };
                    wrapped.mockValues = updatedValues;
                    return settings.load(provider);
                }).then(function () {
                    chai_1.expect(settings.get("key1")).to.eq("update1");
                    chai_1.expect(settings.get("key2")).to.eq("update2");
                });
            });
            it("Uses provided cachekey with a '_configcache_' prefix", function () {
                var provider = new cachingConfigurationProvider_1.default(wrapped, "cacheKey", store);
                return settings.load(provider).then(function () {
                    chai_1.expect(store.get("_configcache_cacheKey")).not.to.be.null;
                });
            });
        });
    });
});
