"use strict";

/// <reference path="..\..\..\typings\main.d.ts" />

import { Queryable } from "../../Queryable/Queryable";
import { Items } from "./Items/Items";

export class Lists extends Queryable {
    constructor(url: string) {
        super(url, "/lists");
    }
    public getByTitle(title: string) {
        this._url.push(`/getByTitle('${title}')`);
        return this;
    }
    public items() {
        return new Items(this._url);
    }
}
