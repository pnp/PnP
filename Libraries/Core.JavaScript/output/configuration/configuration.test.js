(function (factory) {/* istanbul ignore next */
    if (typeof module === 'object' && typeof module.exports === 'object') {
        var v = factory(require, exports); if (v !== undefined) module.exports = v;
    }
    else if (typeof define === 'function' && define.amd) {
        define(["require", "exports", "chai", "./configuration", "../mocks/mockConfigurationProvider"], factory);
    }
})(function (require, exports) {
    "use strict";
    var chai_1 = require("chai");
    var Configuration = require("./configuration");
    var mockConfigurationProvider_1 = require("../mocks/mockConfigurationProvider");
    describe("Configuration", function () {
        describe("Settings", function () {
            var settings;
            beforeEach(function () {
                settings = new Configuration.Settings();
            });
            it("Add and get a setting", function () {
                settings.add("key1", "value1");
                var setting = settings.get("key1");
                chai_1.expect(setting).to.eq("value1");
            });
            it("Add and get a JSON value", function () {
                var obj = { "prop1": "prop1value", "prop2": "prop2value" };
                settings.addJSON("obj1", obj);
                var setting = settings.getJSON("obj1");
                chai_1.expect(setting).to.deep.equal(obj);
            });
            it("Apply a hash and retrieve one of the values", function () {
                var hash = {
                    "key1": "value1",
                    "key2": "value2",
                };
                settings.apply(hash);
                var setting = settings.get("key1");
                chai_1.expect(setting).to.eq("value1");
            });
            it("Apply a hash, apply a second hash overwritting a value and get back the new value", function () {
                var hash1 = {
                    "key1": "value1",
                    "key2": "value2",
                };
                var hash2 = {
                    "key1": "value3",
                    "key2": "value4",
                };
                settings.apply(hash1);
                settings.apply(hash2);
                var setting = settings.get("key1");
                chai_1.expect(setting).to.eq("value3");
            });
            it("Apply a hash containing a serialized JSON object and then retrieve that object using getJSON", function () {
                var obj = { "prop1": "prop1value", "prop2": "prop2value" };
                var hash = {
                    "key1": "value1",
                    "key2": "value2",
                    "key3": JSON.stringify(obj),
                };
                settings.apply(hash);
                var setting = settings.getJSON("key3");
                chai_1.expect(setting).to.deep.equal(obj);
            });
            it("loads settings from a configuration provider", function () {
                var mockValues = {
                    "key2": "value_from_provider_2",
                    "key3": "value_from_provider_3",
                };
                var mockProvider = new mockConfigurationProvider_1.default();
                mockProvider.mockValues = mockValues;
                settings.add("key1", "value1");
                var p = settings.load(mockProvider);
                return p.then(function () {
                    chai_1.expect(settings.get("key1")).to.eq("value1");
                    chai_1.expect(settings.get("key2")).to.eq("value_from_provider_2");
                    chai_1.expect(settings.get("key3")).to.eq("value_from_provider_3");
                });
            });
            it("rejects a promise if configuration provider throws", function () {
                var mockProvider = new mockConfigurationProvider_1.default();
                mockProvider.shouldThrow = true;
                var p = settings.load(mockProvider);
                return p.then(function () { chai_1.expect.fail(null, null, "Should not resolve when provider throws!"); }, function (reason) { chai_1.expect(reason).not.to.be.null; });
            });
            it("rejects a promise if configuration provider rejects the promise", function () {
                var mockProvider = new mockConfigurationProvider_1.default();
                mockProvider.shouldReject = true;
                var p = settings.load(mockProvider);
                return p.then(function () { chai_1.expect.fail(null, null, "Should not resolve when provider rejects!"); }, function (reason) { chai_1.expect(reason).not.to.be.null; });
            });
        });
    });
});
