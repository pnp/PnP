"use strict";

import { expect } from "chai";
import { Navigation } from "./Navigation";

describe("Navigation", () => {
    it("Should be an object", () => {
        let navigation = new Navigation(["_api/web"]);
        expect(navigation).to.be.a("object");
    });
    describe("url", () => {
        it("Should return _api/web/Navigation", () => {
            let navigation = new Navigation(["_api/web"]);
            expect(navigation.url()).to.equal("_api/web/Navigation");
        });
    });
});
