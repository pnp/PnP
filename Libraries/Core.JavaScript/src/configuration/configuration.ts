"use strict";

import * as Collections from "../collections/collections";
import {Promise} from "es6-promise";

export interface IConfigurationProvider {
    getConfiguration(): Promise<Collections.ITypedHash<string>>;
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

    public load(provider: IConfigurationProvider): Promise<void> {
        return new Promise<void>((resolve, reject) => {
            provider.getConfiguration().then((value) => {
                this._settings.merge(value);
                resolve();
            }).catch((reason) => {
               reject(reason);
            });
        });
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
