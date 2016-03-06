"use strict";

import { expect } from "chai";
import Collections = require("../collections/collections");
import * as Configuration from "./configuration";
import {default as MockConfigurationProvider} from "../mocks/mockConfigurationProvider";

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

        it("loads settings from a configuration provider", () => {
            let mockValues: Collections.ITypedHash<string> = {
                "key2": "value_from_provider_2",
                "key3": "value_from_provider_3",
            };
            let mockProvider = new MockConfigurationProvider();
            mockProvider.mockValues = mockValues;

            settings.add("key1", "value1");
            let p = settings.load(mockProvider);

            return p.then(() => {
                expect(settings.get("key1")).to.eq("value1");
                expect(settings.get("key2")).to.eq("value_from_provider_2");
                expect(settings.get("key3")).to.eq("value_from_provider_3");
            });
        });

        it("rejects a promise if configuration provider throws", () => {
            let mockProvider = new MockConfigurationProvider();
            mockProvider.shouldThrow = true;
            let p = settings.load(mockProvider);
            return p.then(
                () => { expect.fail(null, null, "Should not resolve when provider throws!"); },
                (reason) => { expect(reason).not.to.be.null; }
            );
        });

        it("rejects a promise if configuration provider rejects the promise", () => {
            let mockProvider = new MockConfigurationProvider();
            mockProvider.shouldReject = true;
            let p = settings.load(mockProvider);
            return p.then(
                () => { expect.fail(null, null, "Should not resolve when provider rejects!"); },
                (reason) => { expect(reason).not.to.be.null; }
            );
        });
    });
});
