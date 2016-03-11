"use strict";

import { Queryable } from "../../../Queryable";

export class QuickLaunch extends Queryable {
    constructor(url: Array<string>) {
        super(url, "/QuickLaunch");
    }
}
