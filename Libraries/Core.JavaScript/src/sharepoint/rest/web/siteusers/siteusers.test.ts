"use strict";

import { expect } from "chai";
import { SiteUsers } from "./SiteUsers";

describe("SiteUsers", () => {
    it("Should be an object", () => {
        let siteUsers = new SiteUsers(["_api/web"]);
        expect(siteUsers).to.be.a("object");
    });
    describe("url", () => {
        it("Should return _api/web/SiteUsers", () => {
            let siteUsers = new SiteUsers(["_api/web"]);
            expect(siteUsers.url()).to.equal("_api/web/SiteUsers");
        });
    });
});
