"use strict";

/// <reference path="../../typings/main.d.ts" />

import * as ajax from "../../Utils/Ajax";

export class Queryable {
    public _url: Array<string>;
    public _query: Array<string>;
    constructor(base: Array<string>, component: string) {
        this._url = base.concat([component]);
        this._query = [];
    }
    public select(select: Array<string>) {
        this._query.push(`$select=${select.join(",")}`);
        return this;
    }
    public filter(filter: string) {
        this._query.push(`$filter=${filter}`);
        return this;
    }
    public get() {
        return new Promise((resolve, reject) => {
            let url = this._url.join("");
            if (this._query.length > 0) {
                url += (`?${this._query.join("&")}`);
            }
            ajax.get(`${_spPageContextInfo.webAbsoluteUrl}/${url}`).success(data => {
                data.d.hasOwnProperty("results") ? resolve(data.d.results) : resolve(data.d);
            });
        });
    }
}
