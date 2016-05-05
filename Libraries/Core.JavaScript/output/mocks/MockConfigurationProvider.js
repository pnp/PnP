(function (factory) {/* istanbul ignore next */
    if (typeof module === 'object' && typeof module.exports === 'object') {
        var v = factory(require, exports); if (v !== undefined) module.exports = v;
    }
    else if (typeof define === 'function' && define.amd) {
        define(["require", "exports", "es6-promise"], factory);
    }
})(function (require, exports) {
    "use strict";
    var es6_promise_1 = require("es6-promise");
    var MockConfigurationProvider = (function () {
        function MockConfigurationProvider(mockValues) {
            this.mockValues = mockValues;
            this.shouldThrow = false;
            this.shouldReject = false;
        }
        MockConfigurationProvider.prototype.getConfiguration = function () {
            var _this = this;
            if (this.shouldThrow) {
                throw new Error("Mocked error");
            }
            return new es6_promise_1.Promise(function (resolve, reject) {
                if (_this.shouldReject) {
                    reject("Mocked rejection");
                }
                else {
                    resolve(_this.mockValues);
                }
            });
        };
        return MockConfigurationProvider;
    }());
    Object.defineProperty(exports, "__esModule", { value: true });
    exports.default = MockConfigurationProvider;
});
