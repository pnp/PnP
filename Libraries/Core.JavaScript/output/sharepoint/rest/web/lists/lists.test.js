(function (factory) {/* istanbul ignore next */
    if (typeof module === 'object' && typeof module.exports === 'object') {
        var v = factory(require, exports); if (v !== undefined) module.exports = v;
    }
    else if (typeof define === 'function' && define.amd) {
        define(["require", "exports", "chai", "./Lists"], factory);
    }
})(function (require, exports) {
    "use strict";
    var chai_1 = require("chai");
    var Lists_1 = require("./Lists");
    describe("Lists", function () {
        it("Should be an object", function () {
            var lists = new Lists_1.Lists(["_api/web"]);
            chai_1.expect(lists).to.be.a("object");
        });
        describe("url", function () {
            it("Should return _api/web/lists", function () {
                var lists = new Lists_1.Lists(["_api/web"]);
                chai_1.expect(lists.url()).to.equal("_api/web/lists");
            });
        });
        describe("getByTitle", function () {
            it("Should return _api/web/lists/getByTitle('Tasks')", function () {
                var lists = new Lists_1.Lists(["_api/web"]);
                var list = lists.getByTitle("Tasks");
                chai_1.expect(list.url()).to.equal("_api/web/lists/getByTitle('Tasks')");
            });
        });
        describe("getById", function () {
            it("Should return _api/web/lists('4FC65058-FDDE-4FAD-AB21-2E881E1CF527')", function () {
                var lists = new Lists_1.Lists(["_api/web"]);
                var list = lists.getById("4FC65058-FDDE-4FAD-AB21-2E881E1CF527");
                chai_1.expect(list.url()).to.equal("_api/web/lists('4FC65058-FDDE-4FAD-AB21-2E881E1CF527')");
            });
        });
    });
});
