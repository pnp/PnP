"use strict";

import { expect } from "chai";
import { QuickLaunch } from "./QuickLaunch";

describe("QuickLaunch", () => {
    it("Should be an object", () => {
        let quickLaunch = new QuickLaunch(["_api/web/Navigation"]);
        expect(quickLaunch).to.be.a("object");
    });
    describe("url", () => {
        it("Should return _api/web/Navigation/QuickLaunch", () => {
            let quickLaunch = new QuickLaunch(["_api/web/Navigation"]);
            expect(quickLaunch.url()).to.equal("_api/web/Navigation/QuickLaunch");
        });
    });
});
