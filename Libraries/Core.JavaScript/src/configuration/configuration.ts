"use strict";

import * as Collections from "../collections/collections";
import { Promise } from "es6-promise";
import * as providers from "./providers/providers";

/**
 * Set of pre-defined providers which are available from this library
 */
export let Providers = providers;

/**
 * Interface for configuration providers
 * 
 */
export interface IConfigurationProvider {

    /**
     * Gets the configuration from the provider
     */
    getConfiguration(): Promise<Collections.ITypedHash<string>>;
}

/** 
 * Class used to manage the current application settings
 * 
 */
export class Settings {

    /**
     * Creates a new instance of the settings class
     * 
     * @constructor
     */
    constructor() {
        this._settings = new Collections.Dictionary<string>();
    }

    /** 
     * The settings currently stored in this instance
     */
    private _settings: Collections.Dictionary<string>;

    /**
     * Adds a new single setting, or overwrites a previous setting with the same key
     * 
     * @param key The key used to store this setting
     * @param value The setting value to store
     */
    public add(key: string, value: string) {
        this._settings.add(key, value);
    }

    /**
     * Adds a JSON value to the collection as a string, you must use getJSON to rehydrate the object when read
     * 
     * @param key The key used to store this setting
     * @param value The setting value to store
     */
    public addJSON(key: string, value: any) {
        this._settings.add(key, JSON.stringify(value));
    }

    /**
     * Applies the supplied hash to the setting collection overwriting any existing value, or created new values
     * 
     * @param hash The set of value to apply
     */
    public apply(hash: Collections.ITypedHash<any>): void {
        this._settings.merge(hash);
    }

    /**
     * Loads configuration settings into the collection from the supplied provider and returns a Promise
     * 
     * @param provider The provider from which we will load the settings
     */
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

    /** 
     * Gets a value from the configuration
     * 
     * @param key The key whose value we want to return. Returns null if the key does not exist
     */
    public get(key: string): string {
        return this._settings.get(key);
    }

    /**
     * Gets a JSON value, rehydrating the stored string to the original object
     * 
     * @param key The key whose value we want to return. Returns null if the key does not exist
     */
    public getJSON(key: string): any {
        let o = this.get(key);
        if (typeof o === "undefined" || o === null) {
            return o;
        }

        return JSON.parse(o);
    }
}
