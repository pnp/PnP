"use strict";

import { expect } from "chai";
import { Items } from "./Items";

describe("Items", () => {
    it("Should be an object", () => {
        let items = new Items(["_api/web"]);
        expect(items).to.be.a("object");
    });
    describe("url", () => {
        it("Should return _api/web/lists/getByTitle('Tasks')/Items", () => {
            let items = new Items(["_api/web/lists/getByTitle('Tasks')"]);
            expect(items.url()).to.equal("_api/web/lists/getByTitle('Tasks')/Items");
        });
    });
    describe("getById", () => {
        it("Should return _api/web/lists/getByTitle('Tasks')/Items(1)", () => {
            let items = new Items(["_api/web/lists/getByTitle('Tasks')"]);
            let item = items.getById(1);
            expect(item.url()).to.equal("_api/web/lists/getByTitle('Tasks')/Items(1)");
        });
    });
});
