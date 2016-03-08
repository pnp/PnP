"use strict";

import { Provisioning } from "./Provisioning/Provisioning";
import { Rest } from "./Rest/Rest";

export class SharePoint {
    /**
     * The REST base class for SharePoint
     */
    public rest = new Rest();

    /**
    * The Provisioning base class for SharePoint
    */
    public provisioning: Provisioning = new Provisioning();
}
