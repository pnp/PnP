(function (factory) {/* istanbul ignore next */
    if (typeof module === 'object' && typeof module.exports === 'object') {
        var v = factory(require, exports); if (v !== undefined) module.exports = v;
    }
    else if (typeof define === 'function' && define.amd) {
        define(["require", "exports", "chai", "./pnp"], factory);
    }
})(function (require, exports) {
    "use strict";
    var chai_1 = require("chai");
    var pnp = require("./pnp");
    describe("PnP", function () {
        it("util should not be null", function () {
            chai_1.expect(pnp.util).to.not.be.null;
        });
        it("sharepoint should not be null", function () {
            chai_1.expect(pnp.sharepoint).to.not.be.null;
        });
        it("storage should not be null", function () {
            chai_1.expect(pnp.storage).to.not.be.null;
        });
        it("configuration should not be null", function () {
            chai_1.expect(pnp.configuration).to.not.be.null;
        });
        it("logging should not be null", function () {
            chai_1.expect(pnp.logging).to.not.be.null;
        });
    });
});
