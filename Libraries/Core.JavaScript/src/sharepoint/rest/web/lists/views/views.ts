"use strict";

/// <reference path="..\..\..\typings\main.d.ts" />

import { Queryable } from "../../Queryable/Queryable";

/**
 * TODO
 */
export class Views extends Queryable {
    constructor(url: Array<string>) {
        super(url.concat(["/Views"]));
    }
}