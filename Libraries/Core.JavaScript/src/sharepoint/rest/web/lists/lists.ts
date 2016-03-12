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
        return jQuery.extend(this, { items: new Items(this._url) });
    }

    public getById(id: string) {
        this._url.push(`('${id}')`);
        return jQuery.extend(this, { items: new Items(this._url) });
    }
}
