"use strict";

/// <reference path="..\..\..\typings\main.d.ts" />

import { Queryable } from "../../Queryable/Queryable";

/**
 * TODO
 */
export class Items extends Queryable {
    constructor(url: string) {
        super(url, "/items");
    }

    public getById(itemId: number) {
        this._url.push(`(${itemId})`);
        return this;
    }
}
