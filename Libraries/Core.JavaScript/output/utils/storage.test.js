(function (factory) {/* istanbul ignore next */
    if (typeof module === 'object' && typeof module.exports === 'object') {
        var v = factory(require, exports); if (v !== undefined) module.exports = v;
    }
    else if (typeof define === 'function' && define.amd) {
        define(["require", "exports", "chai", "./Storage", "../mocks/MockStorage"], factory);
    }
})(function (require, exports) {
    "use strict";
    var chai_1 = require("chai");
    var Storage = require("./Storage");
    var MockStorage = require("../mocks/MockStorage");
    describe("Storage", function () {
        describe("PnPClientStorageWrapper", function () {
            var wrapper;
            beforeEach(function () {
                var store = (typeof localStorage === "undefined") ? new MockStorage() : localStorage;
                wrapper = new Storage.PnPClientStorageWrapper(store);
            });
            it("Add and Get a value", function () {
                wrapper.put("test", "value");
                var ret = wrapper.get("test");
                chai_1.expect(ret).to.eq("value");
            });
            it("Add two values, remove one and still return the other", function () {
                wrapper.put("test1", "value1");
                wrapper.put("test2", "value2");
                wrapper.delete("test1");
                var ret = wrapper.get("test2");
                chai_1.expect(ret).to.eq("value2");
            });
            it("Use getOrPut to add a value using a getter function and return it", function () {
                wrapper.getOrPut("test", function () { return "value"; });
                var ret = wrapper.get("test");
                chai_1.expect(ret).to.eq("value");
            });
        });
    });
});
