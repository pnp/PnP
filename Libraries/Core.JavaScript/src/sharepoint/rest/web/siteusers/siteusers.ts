"use strict";

import { Queryable } from "../../Queryable";

export class SiteUsers extends Queryable {
    constructor(url: Array<string>) {
        super(url, "/SiteUsers");
    }
}
