var __extends = (this && this.__extends) || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
    function __() { this.constructor = d; }
    d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
};
(function (factory) {/* istanbul ignore next */
    if (typeof module === 'object' && typeof module.exports === 'object') {
        var v = factory(require, exports); if (v !== undefined) module.exports = v;
    }
    else if (typeof define === 'function' && define.amd) {
        define(["require", "exports", "../Queryable", "./Lists/Lists"], factory);
    }
})(function (require, exports) {
    "use strict";
    var Queryable_1 = require("../Queryable");
    var Lists_1 = require("./Lists/Lists");
    var Web = (function (_super) {
        __extends(Web, _super);
        function Web(url) {
            _super.call(this, url, "/web");
            this.lists = new Lists_1.Lists(this._url);
        }
        return Web;
    }(Queryable_1.Queryable));
    exports.Web = Web;
});
