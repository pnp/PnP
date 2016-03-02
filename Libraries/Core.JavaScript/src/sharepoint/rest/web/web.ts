"use strict";

import { Queryable } from "../Queryable";
import { Lists } from "./Lists/Lists";

export class Web extends Queryable {
    constructor(url: Array<string>) {
        super(url, "/web");
    }
    public lists() {
        return new Lists(this._url);
    }
}
