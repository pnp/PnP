"use strict";

/// <reference path="..\..\..\typings\main.d.ts" />

export class Queryable {
    public _url: Array<string>;
    public _query: Array<string>;
    constructor(url: Array<string>) {
        this._url = url;
        this._query = [];
    }
    public select(select: string) {
        this._query.push(`$select='${select}'`)
        return this;
    }
    public filter(filter: string) {
        this._query.push(`$filter='${filter}'`)
        return this;
    }
    public get() {
        let url = this._url.join("");
        if (this._query.length > 0) {
            url += (`?${this._query.join("&")}`);
        }
        return url;
    }
}
