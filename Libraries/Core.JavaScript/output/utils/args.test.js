(function (factory) {/* istanbul ignore next */
    if (typeof module === 'object' && typeof module.exports === 'object') {
        var v = factory(require, exports); if (v !== undefined) module.exports = v;
    }
    else if (typeof define === 'function' && define.amd) {
        define(["require", "exports", "chai", "./args"], factory);
    }
})(function (require, exports) {
    "use strict";
    var chai_1 = require("chai");
    var Args = require("./args");
    describe("Args", function () {
        it("stringIsNullOrEmpty operates as expected", function () {
            var caught = false;
            try {
                Args.stringIsNullOrEmpty("not empty", "arg");
            }
            catch (ex) {
                caught = true;
            }
            chai_1.expect(caught).to.eq(false);
            caught = false;
            try {
                Args.stringIsNullOrEmpty("", "arg");
            }
            catch (ex) {
                caught = true;
            }
            chai_1.expect(caught).to.eq(true);
            caught = false;
            try {
                Args.stringIsNullOrEmpty(null, "arg");
            }
            catch (ex) {
                caught = true;
            }
            chai_1.expect(caught).to.eq(true);
        });
        it("objectIsNull operates as expected", function () {
            var caught = false;
            try {
                Args.objectIsNull({ value: "value" }, "arg");
            }
            catch (ex) {
                caught = true;
            }
            chai_1.expect(caught).to.eq(false);
            caught = false;
            try {
                Args.objectIsNull(null, "arg");
            }
            catch (ex) {
                caught = true;
            }
            chai_1.expect(caught).to.eq(true);
        });
    });
});
