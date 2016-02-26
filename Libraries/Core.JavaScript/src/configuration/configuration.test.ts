"use strict";

import { expect } from "chai";
import Collections = require("../Collections/Collections");
import Configuration = require("./Configuration");

describe("Configuration", () => {

    describe("Settings", () => {

        let settings: Configuration.Settings;

        beforeEach(() => {
            settings = new Configuration.Settings();
        });

        it("Add and get a setting", () => {
            settings.add("key1", "value1");
            let setting = settings.get("key1");
            expect(setting).to.eq("value1");
        });

        it("Add and get a JSON value", () => {
            let obj = { "prop1": "prop1value", "prop2": "prop2value" };
            settings.addJSON("obj1", obj);
            let setting = settings.getJSON("obj1");
            expect(setting).to.deep.equal(obj);
        });

        it("Apply a hash and retrieve one of the values", () => {

            let hash: Collections.ITypedHash<string> = {
                "key1": "value1",
                "key2": "value2",
            };

            settings.apply(hash);
            let setting = settings.get("key1");
            expect(setting).to.eq("value1");
        });

        it("Apply a hash, apply a second hard overwritting a value and get back the new value", () => {

            let hash1: Collections.ITypedHash<string> = {
                "key1": "value1",
                "key2": "value2",
            };

            let hash2: Collections.ITypedHash<string> = {
                "key1": "value3",
                "key2": "value4",
            };

            settings.apply(hash1);
            settings.apply(hash2);
            let setting = settings.get("key1");
            expect(setting).to.eq("value3");
        });

        it("Apply a hash containing a serialized JSON object and then retrieve that object using getJSON", () => {

            let obj = { "prop1": "prop1value", "prop2": "prop2value" };

            let hash: Collections.ITypedHash<string> = {
                "key1": "value1",
                "key2": "value2",
                "key3": JSON.stringify(obj),
            };

            settings.apply(hash);
            let setting = settings.getJSON("key3");
            expect(setting).to.deep.equal(obj);
        });
    });
});
