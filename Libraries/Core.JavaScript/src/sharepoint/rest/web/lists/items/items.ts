"use strict";

/// <reference path="..\..\..\..\typings\main.d.ts" />

import { Queryable } from "../../../Queryable";

export class Items extends Queryable {
    constructor(url: Array<string>) {
        super(url, "/Items");
    }
    public getById(id: number) {
        this._url.push(`(${id})`);
        return this;
    }
}
