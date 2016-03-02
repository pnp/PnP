"use strict";

import { Web } from "./Web/Web";

/**
 * Root of the SharePoint REST module
 */
export class Rest {
    /**
     * Web
     */
    public static web = new Web();
}