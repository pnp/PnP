"use strict";

import { Web } from "./Web/Web";

/**
 * Root of the SharePoint REST module
 */
export class Rest {
    public web = new Web([_spPageContextInfo.webAbsoluteUrl, "/_api"]);
}
