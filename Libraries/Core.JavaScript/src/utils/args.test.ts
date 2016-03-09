"use strict";

import { expect } from "chai";
import * as Args from "./args";

describe("Args", () => {

    it("stringIsNullOrEmpty operates as expected", () => {

        let caught = false;
        try {
            Args.stringIsNullOrEmpty("not empty", "arg");
        } catch (ex) {
            caught = true;
        }
        expect(caught).to.eq(false);

        caught = false;
        try {
            Args.stringIsNullOrEmpty("", "arg");
        } catch (ex) {
            caught = true;
        }
        expect(caught).to.eq(true);

        caught = false;
        try {
            Args.stringIsNullOrEmpty(null, "arg");
        } catch (ex) {
            caught = true;
        }
        expect(caught).to.eq(true);
    });

    it("objectIsNull operates as expected", () => {

        let caught = false;
        try {
            Args.objectIsNull({ value: "value" }, "arg");
        } catch (ex) {
            caught = true;
        }
        expect(caught).to.eq(false);

        caught = false;
        try {
            Args.objectIsNull(null, "arg");
        } catch (ex) {
            caught = true;
        }
        expect(caught).to.eq(true);
    });
});

