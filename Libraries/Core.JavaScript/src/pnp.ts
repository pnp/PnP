"use strict";

import * as Util from "./Utils/Util";
import { PnPClientStorage } from "./Utils/Storage";

/**
 * Root class of the Patterns and Practices namespace, provides an entry point to the library
 */
class PnP {
    /**
     * Utility methods
     */
    public static util = Util;

    /**
     * Provides access to local and session storage through
     */
    public static storage: PnPClientStorage = new PnPClientStorage();
}

export = PnP;
