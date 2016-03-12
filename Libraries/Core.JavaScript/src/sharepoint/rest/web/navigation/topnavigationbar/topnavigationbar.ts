"use strict";

import { Queryable } from "../../../Queryable";

export class TopNavigationBar extends Queryable {
    constructor(url: Array<string>) {
        super(url, "/TopNavigationBar");
    }
}
