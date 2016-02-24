"use strict";

import { expect } from "chai";
import Collections = require("../Collections");

describe("Collections", () => {

    describe("Dictionary<T>", () => {

        let dic: Collections.Dictionary<string>;

        beforeEach(() => {
            dic = new Collections.Dictionary<string>();
        });

        it("Should add and get the same value back", () => {
            dic.add("test", "value");
            let ret = dic.get("test");
            expect(ret).to.eq("value");
        });

        it("Should add two values, remove one and result in a count() of 1", () => {
            dic.add("test1", "value");
            dic.add("test2", "value");
            expect(dic.count()).to.eq(2);
            dic.remove("test2");
            expect(dic.count()).to.eq(1);
        });

        it("Should add four values, remove one and still contain the non-removed values", () => {
            dic.add("test1", "value1");
            dic.add("test2", "value2");
            dic.add("test3", "value3");
            dic.add("test4", "value4");
            expect(dic.count()).to.eq(4);
            dic.remove("test3");
            expect(dic.count()).to.eq(3);
            expect(dic.get("test1")).to.eq("value1");
            expect(dic.get("test2")).to.eq("value2");
            expect(dic.get("test4")).to.eq("value4");
        });

        it("Should clear the collection and result in a count of 0", () => {
            dic.add("test1", "value1");
            dic.add("test2", "value2");
            dic.add("test3", "value3");
            dic.add("test4", "value4");
            expect(dic.count()).to.eq(4);
            dic.clear();
            expect(dic.count()).to.eq(0);
            expect(dic.get("test1")).to.be.null;
        });
    });
});
