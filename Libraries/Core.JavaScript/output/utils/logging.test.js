(function (factory) {/* istanbul ignore next */
    if (typeof module === 'object' && typeof module.exports === 'object') {
        var v = factory(require, exports); if (v !== undefined) module.exports = v;
    }
    else if (typeof define === 'function' && define.amd) {
        define(["require", "exports", "chai", "./logging"], factory);
    }
})(function (require, exports) {
    "use strict";
    var chai_1 = require("chai");
    var Logging = require("./logging");
    describe("Storage", function () {
        describe("PnPClientStorageWrapper", function () {
            var logger;
            beforeEach(function () {
                logger = new Logging.Logger(Logging.LogLevel.Verbose);
            });
            it("Can create an Logger instance and subscribe an ILogListener", function () {
                var message = "Test message";
                var message2 = "";
                logger.subscribe(new Logging.FunctionListener(function (e) {
                    message2 = e.message;
                }));
                logger.write(message);
                chai_1.expect(message2).to.eq(message);
            });
            it("Can create an Logger instance and log a simple object", function () {
                var message2 = "";
                var level2 = Logging.LogLevel.Verbose;
                logger.subscribe(new Logging.FunctionListener(function (e) {
                    level2 = e.level;
                    message2 = e.message;
                }));
                logger.log({ level: Logging.LogLevel.Error, message: "Test message" });
                chai_1.expect(message2).to.eq("Test message");
                chai_1.expect(level2).to.eql(Logging.LogLevel.Error);
            });
            it("Should return an accurate count of subscribers", function () {
                logger.subscribe(new Logging.FunctionListener(function (e) { return; }));
                logger.subscribe(new Logging.FunctionListener(function (e) { return; }));
                logger.subscribe(new Logging.FunctionListener(function (e) { return; }));
                chai_1.expect(logger.count()).to.eq(3);
            });
        });
    });
});
