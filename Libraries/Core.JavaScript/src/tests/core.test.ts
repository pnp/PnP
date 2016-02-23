import { expect } from "chai";
import { pnp } from "../pnp";

describe("Core", () => {

    it("Should create context callback", () => {
        let thisVal = { ctx: 1 };
        let func = function(a) { return this.ctx + a; };
        let callback = pnp.core.getCtxCallback(thisVal, func, 7);
        expect(callback).to.not.eql(null);
        expect(callback()).eql(8);
    });

    it("Should produce a random string.", () => {
        let j = pnp.core.getRandomString(5);
        expect(j).to.be.a("string");
        expect(j).to.have.length(5);
    });

});
