(function (factory) {/* istanbul ignore next */
    if (typeof module === 'object' && typeof module.exports === 'object') {
        var v = factory(require, exports); if (v !== undefined) module.exports = v;
    }
    else if (typeof define === 'function' && define.amd) {
        define(["require", "exports", "chai", "./util"], factory);
    }
})(function (require, exports) {
    "use strict";
    var chai_1 = require("chai");
    var Util = require("./util");
    // import MockLocation = require("../mocks/MockLocation");
    // let location: Location;
    describe("Util", function () {
        describe("getCtxCallback", function () {
            it("Should create contextual callback", function () {
                var func = function (a) { this.num = this.num + a; };
                var ctx = { num: 1 };
                var callback = Util.getCtxCallback(ctx, func, 7);
                chai_1.expect(callback).to.exist;
                chai_1.expect(callback).to.be.a("function");
                // this call will update ctx var inside the callback
                callback();
                chai_1.expect(ctx.num).to.eq(8);
            });
        });
        // TODO:: commented out pending resolution of the location mocking issue.
        //     describe("urlParamExists", () => {
        // 
        //         before(() => {
        //             let mock = new MockLocation();
        //             mock.search = "?param1=true&&param2=Hello%20World";
        //             location = mock;
        //         });
        // 
        //         after(() => {
        //             location = null;
        //         });
        // 
        //         it("Should find a parameter called param1", () => {
        //             expect(Util.urlParamExists("param1")).to.be.true;
        //         });
        // 
        //         it("Should not find a parameter called doesnotexist", () => {
        //             expect(Util.urlParamExists("doesnotexist")).to.be.false;
        //         });
        //     });
        // 
        //     describe("getUrlParamByName", () => {
        // 
        //         before(() => {
        //             let mock = new MockLocation();
        //             mock.search = "?param1=true&&param2=Hello%20World";
        //             location = mock;
        //         });
        // 
        //         after(() => {
        //             location = null;
        //         });
        // 
        //         it("Should find the value of param2 to be 'Hello World'", () => {
        //             expect(Util.getUrlParamByName("param2")).to.eq("Hello World");
        //         });
        // 
        //         it("Should find the value of doesnotexist to be empty string", () => {
        //             expect(Util.getUrlParamByName("doesnotexist")).to.eq("");
        //         });
        //     });
        // 
        //     describe("getUrlParamBoolByName", () => {
        // 
        //         before(() => {
        //             let mock = new MockLocation();
        //             mock.search = "?param1=true&&param2=Hello%20World";
        //             location = mock;
        //         });
        // 
        //         after(() => {
        //             location = null;
        //         });
        // 
        //         it("Should find the value of param1 to be true", () => {
        //             let val = Util.getUrlParamBoolByName("param1");
        //             expect(val).to.be.ok;
        //         });
        // 
        //         it("Should find the value of doesnotexist to be false", () => {
        //             expect(Util.getUrlParamBoolByName("doesnotexist")).to.be.not.ok;
        //         });
        //     });
        describe("dateAdd", function () {
            it("Should add 5 minutes to a date", function () {
                var testDate = new Date();
                var checkDate = new Date(testDate.toLocaleString());
                checkDate.setMinutes(testDate.getMinutes() + 5);
                chai_1.expect(Util.dateAdd(testDate, "minute", 5).getMinutes()).to.eq(checkDate.getMinutes());
            });
            it("Should add 2 years to a date", function () {
                var testDate = new Date();
                var checkDate = new Date(testDate.toLocaleString());
                checkDate.setFullYear(testDate.getFullYear() + 2);
                chai_1.expect(Util.dateAdd(testDate, "year", 2).getFullYear()).to.eq(checkDate.getFullYear());
            });
        });
        describe("stringInsert", function () {
            it("Should insert the string cat into dog at index 2 resulting in docatg", function () {
                chai_1.expect(Util.stringInsert("dog", 2, "cat")).to.eq("docatg");
            });
        });
        describe("combinePaths", function () {
            it("Should combine the paths '/path/', 'path2', 'path3' and '/path4' to be path/path2/path3/path4", function () {
                chai_1.expect(Util.combinePaths("/path/", "path2", "path3", "/path4")).to.eq("path/path2/path3/path4");
            });
            it("Should combine the paths 'http://site/path/' and '/path4/page.aspx' to be http://site/path/path4/page.aspx", function () {
                chai_1.expect(Util.combinePaths("http://site/path/", "/path4/page.aspx")).to.eq("http://site/path/path4/page.aspx");
            });
        });
        describe("getRandomString", function () {
            it("Should produce a random string of length 5", function () {
                var j = Util.getRandomString(5);
                chai_1.expect(j).to.exist;
                chai_1.expect(j).to.be.a("string");
                chai_1.expect(j).to.have.length(5);
            });
            it("Should produce a random string of length 27", function () {
                var j = Util.getRandomString(27);
                chai_1.expect(j).to.exist;
                chai_1.expect(j).to.be.a("string");
                chai_1.expect(j).to.have.length(27);
            });
        });
        describe("getGUID", function () {
            it("Should produce a GUID matching the expected pattern", function () {
                chai_1.expect(Util.getGUID()).to.match(/[a-f0-9]{8}(?:-[a-f0-9]{4}){3}-[a-f0-9]{12}/i);
            });
        });
        describe("isFunction", function () {
            it("Should find that a function is a function", function () {
                chai_1.expect(Util.isFunction(function () { return; })).to.be.true;
            });
            it("Should find that a non-function is not a function", function () {
                chai_1.expect(Util.isFunction({ val: 0 })).to.be.false;
            });
        });
    });
});
