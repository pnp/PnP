"use strict";

import * as Collections from "../collections/collections";

export interface IConfigurationProvider {
    getConfiguration(): Collections.ITypedHash<string>;
}

export class Settings {
    constructor() {
        this._settings = new Collections.Dictionary<string>();
    }

    private _settings: Collections.Dictionary<string>;

    public add(key: string, value: string) {
        this._settings.add(key, value);
    }

    public addJSON(key: string, value: any) {
        this._settings.add(key, JSON.stringify(value));
    }

    public apply(hash: Collections.ITypedHash<any>): void {
        this._settings.merge(hash);
    }

    public load(provider: IConfigurationProvider): void {
        this._settings.merge(provider.getConfiguration());
    }

    public get(key: string): string {
        return this._settings.get(key);
    }

    public getJSON(key: string): any {
        let o = this.get(key);
        if (typeof o === "undefined" || o === null) {
            return o;
        }

        return JSON.parse(o);
    }
}
