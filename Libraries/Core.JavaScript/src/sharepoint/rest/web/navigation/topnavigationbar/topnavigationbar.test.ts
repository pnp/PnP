"use strict";

import { expect } from "chai";
import { TopNavigationBar } from "./TopNavigationBar";

describe("TopNavigationBar", () => {
    it("Should be an object", () => {
        let topNavigationBar = new TopNavigationBar(["_api/web/Navigation"]);
        expect(topNavigationBar).to.be.a("object");
    });
    describe("url", () => {
        it("Should return _api/web/Navigation/TopNavigationBar", () => {
            let topNavigationBar = new TopNavigationBar(["_api/web/Navigation"]);
            expect(topNavigationBar.url()).to.equal("_api/web/Navigation/TopNavigationBar");
        });
    });
});
