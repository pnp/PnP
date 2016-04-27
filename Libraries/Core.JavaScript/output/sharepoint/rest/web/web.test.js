(function (factory) {/* istanbul ignore next */
    if (typeof module === 'object' && typeof module.exports === 'object') {
        var v = factory(require, exports); if (v !== undefined) module.exports = v;
    }
    else if (typeof define === 'function' && define.amd) {
        define(["require", "exports", "chai", "./Web"], factory);
    }
})(function (require, exports) {
    "use strict";
    var chai_1 = require("chai");
    var Web_1 = require("./Web");
    describe("Web", function () {
        it("Should be an object", function () {
            var web = new Web_1.Web(["_api"]);
            chai_1.expect(web).to.be.a("object");
        });
        describe("url", function () {
            it("Should return _api/web", function () {
                var web = new Web_1.Web(["_api"]);
                chai_1.expect(web.url()).to.equal("_api/web");
            });
        });
    });
});
