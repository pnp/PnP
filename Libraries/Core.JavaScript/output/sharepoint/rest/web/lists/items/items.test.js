(function (factory) {/* istanbul ignore next */
    if (typeof module === 'object' && typeof module.exports === 'object') {
        var v = factory(require, exports); if (v !== undefined) module.exports = v;
    }
    else if (typeof define === 'function' && define.amd) {
        define(["require", "exports", "chai", "./Items"], factory);
    }
})(function (require, exports) {
    "use strict";
    var chai_1 = require("chai");
    var Items_1 = require("./Items");
    describe("Items", function () {
        it("Should be an object", function () {
            var items = new Items_1.Items(["_api/web"]);
            chai_1.expect(items).to.be.a("object");
        });
        describe("url", function () {
            it("Should return _api/web/lists/getByTitle('Tasks')/Items", function () {
                var items = new Items_1.Items(["_api/web/lists/getByTitle('Tasks')"]);
                chai_1.expect(items.url()).to.equal("_api/web/lists/getByTitle('Tasks')/Items");
            });
        });
        describe("getById", function () {
            it("Should return _api/web/lists/getByTitle('Tasks')/Items(1)", function () {
                var items = new Items_1.Items(["_api/web/lists/getByTitle('Tasks')"]);
                var item = items.getById(1);
                chai_1.expect(item.url()).to.equal("_api/web/lists/getByTitle('Tasks')/Items(1)");
            });
        });
    });
});
