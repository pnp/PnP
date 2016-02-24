"use strict";

import { expect } from "chai";
import pnp = require("../pnp");
import MockLocation = require("./mocks/MockLocation");

describe("Util", () => {

    describe("getCtxCallback", () => {
        it("Should create contextual callback", () => {
            let func = function(a) { this.num = this.num + a; };
            let ctx = { num: 1 };
            let callback = pnp.util.getCtxCallback(ctx, func, 7);
            expect(callback).to.exist;
            expect(callback).to.be.a("function");
            // this call will update ctx var inside the callback
            callback();
            expect(ctx.num).to.eq(8);
        });
    });

    describe("urlParamExists", () => {

        let origMethod: () => Location;

        before(function() {
            origMethod = pnp.util.getBrowserLocation;
            pnp.util.getBrowserLocation = function(): Location {
                let mock = new MockLocation();
                mock.search = "?param1=true&&param2=Hello%20World";
                return mock;
            };
        });

        after(function() {
            pnp.util.getBrowserLocation = origMethod;
        });

        it("Should find a parameter called param1", () => {
            expect(pnp.util.urlParamExists("param1")).to.be.true;
        });

        it("Should not find a parameter called doesnotexist", () => {
            expect(pnp.util.urlParamExists("doesnotexist")).to.be.false;
        });
    });

    describe("getUrlParamByName", () => {

        let origMethod: () => Location;

        before(function() {
            origMethod = pnp.util.getBrowserLocation;
            pnp.util.getBrowserLocation = function(): Location {
                let mock = new MockLocation();
                mock.search = "?param1=true&&param2=Hello%20World";
                return mock;
            };
        });

        after(function() {
            pnp.util.getBrowserLocation = origMethod;
        });

        it("Should find the value of param2 to be 'Hello World'", () => {
            expect(pnp.util.getUrlParamByName("param2")).to.eq("Hello World");
        });

        it("Should find the value of doesnotexist to be empty string", () => {
            expect(pnp.util.getUrlParamByName("doesnotexist")).to.eq("");
        });
    });

    describe("getUrlParamBoolByName", () => {

        let origMethod: () => Location;

        before(function() {
            origMethod = pnp.util.getBrowserLocation;
            pnp.util.getBrowserLocation = function(): Location {
                let mock = new MockLocation();
                mock.search = "?param1=true&&param2=Hello%20World";
                return mock;
            };
        });

        after(function() {
            pnp.util.getBrowserLocation = origMethod;
        });

        it("Should find the value of param1 to be true", () => {
            let val = pnp.util.getUrlParamBoolByName("param1");
            expect(val).to.be.ok;
        });

        it("Should find the value of doesnotexist to be false", () => {
            expect(pnp.util.getUrlParamBoolByName("doesnotexist")).to.be.not.ok;
        });
    });

    describe("dateAdd", () => {
        it("Should add 5 minutes to a date", () => {
            let testDate = new Date();
            let checkDate = new Date(testDate.toLocaleString());
            checkDate.setMinutes(testDate.getMinutes() + 5);
            expect(pnp.util.dateAdd(testDate, "minute", 5).getMinutes()).to.eq(checkDate.getMinutes());
        });

        it("Should add 2 years to a date", () => {
            let testDate = new Date();
            let checkDate = new Date(testDate.toLocaleString());
            checkDate.setFullYear(testDate.getFullYear() + 2);
            expect(pnp.util.dateAdd(testDate, "year", 2).getFullYear()).to.eq(checkDate.getFullYear());
        });
    });

    describe("stringInsert", () => {
        it("Should insert the string cat into dog at index 2 resulting in docatg", () => {
            expect(pnp.util.stringInsert("dog", 2, "cat")).to.eq("docatg");
        });
    });

    describe("combinePaths", () => {
        it("Should combine the paths '/path/', 'path2', 'path3' and '/path4' to be path/path2/path3/path4", () => {
            expect(pnp.util.combinePaths("/path/", "path2", "path3", "/path4")).to.eq("path/path2/path3/path4");
        });

        it("Should combine the paths 'http://site/path/' and '/path4/page.aspx' to be http://site/path/path4/page.aspx", () => {
            expect(pnp.util.combinePaths("http://site/path/", "/path4/page.aspx")).to.eq("http://site/path/path4/page.aspx");
        });
    });

    describe("getRandomString", () => {
        it("Should produce a random string of length 5", () => {
            let j = pnp.util.getRandomString(5);
            expect(j).to.exist;
            expect(j).to.be.a("string");
            expect(j).to.have.length(5);
        });

        it("Should produce a random string of length 27", () => {
            let j = pnp.util.getRandomString(27);
            expect(j).to.exist;
            expect(j).to.be.a("string");
            expect(j).to.have.length(27);
        });
    });

    describe("getGUID", () => {
        it("Should produce a GUID matching the expected pattern", () => {
            expect(pnp.util.getGUID()).to.match(/[a-f0-9]{8}(?:-[a-f0-9]{4}){3}-[a-f0-9]{12}/i);
        });
    });

    describe("isFunction", () => {
        it("Should find that a function is a function", () => {
            expect(pnp.util.isFunction(function() { return; })).to.be.true;
        });

        it("Should find that a non-function is not a function", () => {
            expect(pnp.util.isFunction({ val: 0 })).to.be.false;
        });
    });
});
