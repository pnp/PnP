"use strict";

/// <reference path="..\..\..\typings\main.d.ts" />

import { Queryable } from "../../Queryable";
import { Items } from "./Items/Items";

export class Lists extends Queryable {
    constructor(url: Array<string>) {
        super(url, "/lists");
    }

    public getByTitle(title: string) {
        this._url.push(`/getByTitle('${title}')`);
        return this;
    }

    public getById(id: string) {
        this._url.push(`('${id}')`);
        return this;
    }
}
