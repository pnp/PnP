"use strict";

import { Queryable } from "../Queryable";
import { Lists } from "./Lists/Lists";

export class Web extends Queryable {
    constructor(url: string) {
        super(url, "/web");
    }
    public static lists = new Lists();
}
