import { expect } from "chai";
import Storage = require("./Storage");
import MockStorage = require("../mocks/MockStorage");

describe("Storage", () => {

    describe("PnPClientStorageWrapper", () => {

        let wrapper: Storage.PnPClientStorageWrapper;

        beforeEach(() => {
            let store: Storage = (typeof localStorage === "undefined") ? new MockStorage() : localStorage;
            wrapper = new Storage.PnPClientStorageWrapper(store);
        });

        it("Add and Get a value", () => {
            wrapper.put("test", "value");
            let ret = wrapper.get("test");
            expect(ret).to.eq("value");
        });

        it("Add two values, remove one and still return the other", () => {
            wrapper.put("test1", "value1");
            wrapper.put("test2", "value2");
            wrapper.delete("test1");
            let ret = wrapper.get("test2");
            expect(ret).to.eq("value2");
        });

        it("Use getOrPut to add a value using a getter function and return it", () => {
            wrapper.getOrPut("test", function() { return "value"; });
            let ret = wrapper.get("test");
            expect(ret).to.eq("value");
        });
    });
});
