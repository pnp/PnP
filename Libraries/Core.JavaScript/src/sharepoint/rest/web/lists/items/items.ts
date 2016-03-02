"use strict";

/// <reference path="..\..\..\typings\main.d.ts" />

import { Queryable } from "../../Queryable/Queryable";

/**
 * TODO
 */
export class Items extends Queryable {
    constructor(url: Array<string>) {
        super(url.concat(["/Items"]));
    }

    public getById(itemId: number) {
        this._url.push(`(${itemId})`);
        return this;
    }
}
