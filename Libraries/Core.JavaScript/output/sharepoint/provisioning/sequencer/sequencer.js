// import { Promise } from "es6-promise";
(function (factory) {/* istanbul ignore next */
    if (typeof module === 'object' && typeof module.exports === 'object') {
        var v = factory(require, exports); if (v !== undefined) module.exports = v;
    }
    else if (typeof define === 'function' && define.amd) {
        define(["require", "exports"], factory);
    }
})(function (require, exports) {
    "use strict";
    var Sequencer = (function () {
        function Sequencer(__functions, __parameter, __scope) {
            this.parameter = __parameter;
            this.scope = __scope;
            this.functions = this.deferredArray(__functions);
        }
        Sequencer.prototype.execute = function () {
            var _this = this;
            return new Promise(function (resolve, reject) {
                var promises = [];
                promises.push(jQuery.Deferred());
                promises[0].resolve();
                promises[0].promise();
                var index = 1;
                while (_this.functions[index - 1] !== undefined) {
                    var i = promises.length - 1;
                    promises.push(_this.functions[index - 1].execute(promises[i]));
                    index++;
                }
                ;
                Promise.all(promises).then(resolve, resolve);
            });
        };
        Sequencer.prototype.deferredArray = function (__functions) {
            var _this = this;
            var functions = [];
            __functions.forEach(function (f) { return functions.push(new DeferredObject(f, _this.parameter, _this.scope)); });
            return functions;
        };
        return Sequencer;
    }());
    exports.Sequencer = Sequencer;
    var DeferredObject = (function () {
        function DeferredObject(func, parameter, scope) {
            this.func = func;
            this.parameter = parameter;
            this.scope = scope;
        }
        DeferredObject.prototype.execute = function (depFunc) {
            var _this = this;
            if (!depFunc) {
                return this.func.apply(this.scope, [this.parameter]);
            }
            return new Promise(function (resolve, reject) {
                depFunc.then(function () {
                    _this.func.apply(_this.scope, [_this.parameter]).then(resolve, resolve);
                });
            });
        };
        return DeferredObject;
    }());
});
