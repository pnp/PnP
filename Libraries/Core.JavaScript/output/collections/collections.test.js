(function (factory) {/* istanbul ignore next */
    if (typeof module === 'object' && typeof module.exports === 'object') {
        var v = factory(require, exports); if (v !== undefined) module.exports = v;
    }
    else if (typeof define === 'function' && define.amd) {
        define(["require", "exports", "chai", "./Collections"], factory);
    }
})(function (require, exports) {
    "use strict";
    var chai_1 = require("chai");
    var Collections = require("./Collections");
    describe("Collections", function () {
        describe("Dictionary<T>", function () {
            var dic;
            beforeEach(function () {
                dic = new Collections.Dictionary();
            });
            it("Should add and get the same value back", function () {
                dic.add("test", "value");
                var ret = dic.get("test");
                chai_1.expect(ret).to.eq("value");
            });
            it("Should add two values, remove one and result in a count() of 1", function () {
                dic.add("test1", "value");
                dic.add("test2", "value");
                chai_1.expect(dic.count()).to.eq(2);
                dic.remove("test2");
                chai_1.expect(dic.count()).to.eq(1);
            });
            it("Should return null for a non-existant value", function () {
                dic.add("test", "value");
                var ret = dic.get("test2");
                chai_1.expect(ret).to.be.null;
            });
            it("Should add four values, remove one and still contain the non-removed values", function () {
                dic.add("test1", "value1");
                dic.add("test2", "value2");
                dic.add("test3", "value3");
                dic.add("test4", "value4");
                chai_1.expect(dic.count()).to.eq(4);
                dic.remove("test3");
                chai_1.expect(dic.count()).to.eq(3);
                chai_1.expect(dic.get("test1")).to.eq("value1");
                chai_1.expect(dic.get("test2")).to.eq("value2");
                chai_1.expect(dic.get("test4")).to.eq("value4");
            });
            it("Should clear the collection and result in a count of 0", function () {
                dic.add("test1", "value1");
                dic.add("test2", "value2");
                dic.add("test3", "value3");
                dic.add("test4", "value4");
                chai_1.expect(dic.count()).to.eq(4);
                dic.clear();
                chai_1.expect(dic.count()).to.eq(0);
                chai_1.expect(dic.get("test1")).to.be.null;
            });
        });
    });
});
