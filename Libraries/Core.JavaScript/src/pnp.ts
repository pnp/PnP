"use strict";

import * as Util from "./utils/Util";
import { SharePoint } from "./SharePoint/SharePoint";
import { PnPClientStorage } from "./utils/Storage";
import * as Configuration from "./configuration/configuration";
import { Logger } from "./utils/logging";

/**
 * Root class of the Patterns and Practices namespace, provides an entry point to the library
 */
class PnP {
    /**
     * Utility methods
     */
    public static util = Util;

    /**
     * SharePoint
     */
    public static sharepoint = new SharePoint();

    /**
     * Provides access to local and session storage through
     */
    public static storage: PnPClientStorage = new PnPClientStorage();

    /**
     * Configuration 
     */
    public static configuration = Configuration;

    /**
     * Global logging instance to which subscribers can be registered and messages written
     */
    public static logging = new Logger();
}

export = PnP;
