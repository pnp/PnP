import * as Util from "./utils/Util";
import { SharePoint } from "./SharePoint/SharePoint";
import { PnPClientStorage } from "./utils/Storage";
import * as Configuration from "./configuration/configuration";
import { Logger } from "./utils/logging";
/**
 * Root class of the Patterns and Practices namespace, provides an entry point to the library
 */
declare class PnP {
    /**
     * Utility methods
     */
    static util: typeof Util;
    /**
     * SharePoint
     */
    static sharepoint: SharePoint;
    /**
     * Provides access to local and session storage through
     */
    static storage: PnPClientStorage;
    /**
     * Configuration
     */
    static configuration: typeof Configuration;
    /**
     * Global logging instance to which subscribers can be registered and messages written
     */
    static logging: Logger;
}
export = PnP;
