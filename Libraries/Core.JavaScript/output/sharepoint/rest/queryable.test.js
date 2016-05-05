(function (factory) {/* istanbul ignore next */
    if (typeof module === 'object' && typeof module.exports === 'object') {
        var v = factory(require, exports); if (v !== undefined) module.exports = v;
    }
    else if (typeof define === 'function' && define.amd) {
        define(["require", "exports", "chai", "./Queryable"], factory);
    }
})(function (require, exports) {
    "use strict";
    var chai_1 = require("chai");
    var Queryable_1 = require("./Queryable");
    describe("Queryable", function () {
        it("Should be an object", function () {
            var queryable = new Queryable_1.Queryable([], "");
            chai_1.expect(queryable).to.be.a("object");
        });
        describe("select", function () {
            it("Should append a select query", function () {
                var queryable = new Queryable_1.Queryable(["_api/web"], "/lists");
                queryable.select(["Title", "Created"]);
                chai_1.expect(queryable.url()).to.include("$select=Title,Created");
            });
        });
        describe("filter", function () {
            it("Should append a filter query", function () {
                var queryable = new Queryable_1.Queryable(["_api/web"], "/lists");
                queryable.filter("Title eq 'Tasks'");
                chai_1.expect(queryable.url()).to.include("$filter=Title eq 'Tasks'");
            });
        });
    });
});
