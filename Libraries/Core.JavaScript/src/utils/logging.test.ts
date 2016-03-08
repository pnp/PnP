"use strict";

import { expect } from "chai";
import * as Logging from "./logging";

describe("Storage", () => {

    describe("PnPClientStorageWrapper", () => {

        let logger: Logging.Logger;

        beforeEach(() => {
            logger = new Logging.Logger(Logging.LogLevel.Verbose);
        });

        it("Can create an Logger instance and subscribe an ILogListener", () => {
            let message = "Test message";
            let message2 = "";
            logger.subscribe(new Logging.FunctionListener((e) => {
                message2 = e.message;
            }));
            logger.write(message);
            expect(message2).to.eq(message);
        });

        it("Can create an Logger instance and log a simple object", () => {
            let message2 = "";
            let level2 = Logging.LogLevel.Verbose;
            logger.subscribe(new Logging.FunctionListener((e) => {
                level2 = e.level;
                message2 = e.message;
            }));
            logger.log({ level: Logging.LogLevel.Error, message: "Test message" });
            expect(message2).to.eq("Test message");
            expect(level2).to.eql(Logging.LogLevel.Error);
        });

        it("Should return an accurate count of subscribers", () => {
            logger.subscribe(new Logging.FunctionListener((e) => { return; }));
            logger.subscribe(new Logging.FunctionListener((e) => { return; }));
            logger.subscribe(new Logging.FunctionListener((e) => { return; }));
            expect(logger.count()).to.eq(3);
        });
    });
});
