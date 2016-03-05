"use strict";

import { Provisioning } from "./Provisioning/Provisioning";

/**
 * Root class of the Patterns and Practices namespace, provides an entry point to the library
 */
export class SharePoint {
    public provisioning: Provisioning = new Provisioning();
}

