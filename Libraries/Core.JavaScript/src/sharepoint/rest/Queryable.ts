"use strict";

/// <reference path="../../typings/main.d.ts" />

import * as ajax from "../../Utils/Ajax";

export class Queryable {
    public _url: Array<string>;
    public _query: Array<string>;
    constructor(base: string, component: string) {
        this._url = [base, component];
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
        return ajax.get(`${_spPageContextInfo.webAbsoluteUrl}/${url}`);
    }
}
