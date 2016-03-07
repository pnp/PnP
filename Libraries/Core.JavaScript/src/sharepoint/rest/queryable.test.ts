"use strict";

import { expect } from "chai";
import { Queryable } from "./Queryable";

describe("Queryable", () => {
    it("Should be an object", () => {
        let queryable = new Queryable([], "");
        expect(queryable).to.be.a("object");
    });
    describe("select", () => {
        it("Should append a select query", () => {
            let queryable = new Queryable(["_api/web"], "/lists");
            queryable.select(["Title", "Created"]);
            expect(queryable.url()).to.include("$select=Title,Created");
        });
    });
    describe("filter", () => {
        it("Should append a filter query", () => {
            let queryable = new Queryable(["_api/web"], "/lists");
            queryable.filter("Title eq 'Tasks'");
            expect(queryable.url()).to.include("$filter=Title eq 'Tasks'");
        });
    });
});
