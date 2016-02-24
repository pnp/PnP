import { expect } from "chai";
import pnp = require("../pnp");
import MockLocation = require("./mocks/MockLocation");

describe("Util", () => {

    // describe("getCtxCallback", () => {
    //     it("Should create context callback", () => {
    //         let func = function(a: number): number { return this.ctx + a; };
    //         let callback = pnp.util.getCtxCallback({ ctx: 1 }, func, 7);
    //         expect(callback).to.exist;
    //         expect(callback).to.be.a("function");
    //         let ret = callback();
    //         expect(ret).to.be.a("number");
    //         expect(ret).eql(8);
    //     });
    // });

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

        it("Should not find a parameter called param1", () => {
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
                mock.search = "?param1=1&&param2=Hello%20World";
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

    describe("stringInsert", () => {
        it("Should insert the string cat into dog at index 2 resulting in docatg.", () => {
            expect(pnp.util.stringInsert("dog", 2, "cat")).to.eq("docatg");
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






});
