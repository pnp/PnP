"use strict";

import { Web } from "./Web/Web";
import { Site } from "./Site/Site";

/**
 * Root of the SharePoint REST module
 */
export class Rest {
    /**
     * Web
     */
    public static web = new Web();
    
    /**
     * Web
     */
    public static site = new Site();
}