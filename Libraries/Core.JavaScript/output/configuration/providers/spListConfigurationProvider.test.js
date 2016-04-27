(function (factory) {/* istanbul ignore next */
    if (typeof module === 'object' && typeof module.exports === 'object') {
        var v = factory(require, exports); if (v !== undefined) module.exports = v;
    }
    else if (typeof define === 'function' && define.amd) {
        define(["require", "exports", "chai", "./spListConfigurationProvider", "../../mocks/MockStorage"], factory);
    }
})(function (require, exports) {
    "use strict";
    var chai_1 = require("chai");
    var spListConfigurationProvider_1 = require("./spListConfigurationProvider");
    var MockStorage = require("../../mocks/MockStorage");
    describe("Configuration", function () {
        describe("SPListConfigurationProvider", function () {
            var webUrl;
            var mockData;
            var calledUrl;
            beforeEach(function () {
                webUrl = "https://fake.sharepoint.com/sites/test/subsite";
                mockData = { "key1": "value1", "key2": "value2" };
                calledUrl = "";
            });
            function mockJQuery() {
                // Create a mock JQuery.ajax method, which will always return our testdata.
                var mock = {};
                mock.ajax = function (options) {
                    calledUrl = options.url;
                    var wrappedMockData = new Array();
                    for (var key in mockData) {
                        if (typeof key === "string") {
                            wrappedMockData.push({ "Title": key, "Value": mockData[key] });
                        }
                    }
                    return {
                        "success": function (callback) {
                            callback({ d: { results: wrappedMockData } });
                        },
                    };
                };
                return mock;
            }
            it("Returns the webUrl passed in to the constructor", function () {
                var provider = new spListConfigurationProvider_1.default(webUrl);
                chai_1.expect(provider.getWebUrl()).to.equal(webUrl);
            });
            it("Uses 'config' as the default title for the list", function () {
                var provider = new spListConfigurationProvider_1.default(webUrl);
                chai_1.expect(provider.getListTitle()).to.equal("config");
            });
            it("Allows user to overwrite the default list title", function () {
                var listTitle = "testTitle";
                var provider = new spListConfigurationProvider_1.default(webUrl, listTitle);
                chai_1.expect(provider.getListTitle()).to.equal(listTitle);
            });
            it("Fetches configuration data from SharePoint using ajax", function () {
                // Mock JQuery
                global.jQuery = mockJQuery();
                var listTitle = "testTitle";
                var provider = new spListConfigurationProvider_1.default(webUrl, listTitle);
                return provider.getConfiguration().then(function (values) {
                    // Verify url
                    chai_1.expect(calledUrl).to.equal(webUrl + "/_api/web/lists/getByTitle('" + listTitle + "')/items?$select=Title,Value");
                    // Verify returned values
                    for (var key in mockData) {
                        if (typeof key === "string") {
                            chai_1.expect(values[key]).to.equal(mockData[key]);
                        }
                    }
                    // Remove JQuery mock
                    delete global.jQuery;
                });
            });
            it("Can wrap itself inside a caching configuration provider", function () {
                // Mock localStorage
                global.localStorage = new MockStorage();
                var provider = new spListConfigurationProvider_1.default(webUrl);
                var cached = provider.asCaching();
                var wrappedProvider = cached.getWrappedProvider();
                chai_1.expect(wrappedProvider).to.equal(provider);
                // Remove localStorage mock
                delete global.localStorage;
            });
        });
    });
});
