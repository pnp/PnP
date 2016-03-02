"use strict";

import { Queryable } from "../Queryable/Queryable";
import { Lists } from "./Lists/Lists";

export class Web extends Queryable {
    constructor(url: Array<string>) {
        super(url.concat(["/Web"]));
    }
    public static lists = new Lists();
}
