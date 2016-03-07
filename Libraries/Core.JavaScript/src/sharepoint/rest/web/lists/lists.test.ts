"use strict";

import { expect } from "chai";
import { Lists } from "./Lists";

describe("Lists", () => {
    it("Should be an object", () => {
        let lists = new Lists(["_api/web"]);
        expect(lists).to.be.a("object");
    });
    describe("url", () => {
        it("Should return _api/web/lists", () => {
            let lists = new Lists(["_api/web"]);
            expect(lists.url()).to.equal("_api/web/lists");
        });
    });
    describe("getByTitle", () => {
        it("Should return _api/web/lists/getByTitle('Tasks')", () => {
            let lists = new Lists(["_api/web"]);
            let list = lists.getByTitle("Tasks");
            expect(list.url()).to.equal("_api/web/lists/getByTitle('Tasks')");
        });
    });
    describe("getById", () => {
        it("Should return _api/web/lists('4FC65058-FDDE-4FAD-AB21-2E881E1CF527')", () => {
            let lists = new Lists(["_api/web"]);
            let list = lists.getById("4FC65058-FDDE-4FAD-AB21-2E881E1CF527");
            expect(list.url()).to.equal("_api/web/lists('4FC65058-FDDE-4FAD-AB21-2E881E1CF527')");
        });
    });
});
