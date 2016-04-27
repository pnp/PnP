import * as Collections from "../collections/collections";
import * as providers from "./providers/providers";
/**
 * Set of pre-defined providers which are available from this library
 */
export declare let Providers: typeof providers;
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
export declare class Settings {
    /**
     * Creates a new instance of the settings class
     *
     * @constructor
     */
    constructor();
    /**
     * The settings currently stored in this instance
     */
    private _settings;
    /**
     * Adds a new single setting, or overwrites a previous setting with the same key
     *
     * @param key The key used to store this setting
     * @param value The setting value to store
     */
    add(key: string, value: string): void;
    /**
     * Adds a JSON value to the collection as a string, you must use getJSON to rehydrate the object when read
     *
     * @param key The key used to store this setting
     * @param value The setting value to store
     */
    addJSON(key: string, value: any): void;
    /**
     * Applies the supplied hash to the setting collection overwriting any existing value, or created new values
     *
     * @param hash The set of value to apply
     */
    apply(hash: Collections.ITypedHash<any>): void;
    /**
     * Loads configuration settings into the collection from the supplied provider and returns a Promise
     *
     * @param provider The provider from which we will load the settings
     */
    load(provider: IConfigurationProvider): Promise<void>;
    /**
     * Gets a value from the configuration
     *
     * @param key The key whose value we want to return. Returns null if the key does not exist
     */
    get(key: string): string;
    /**
     * Gets a JSON value, rehydrating the stored string to the original object
     *
     * @param key The key whose value we want to return. Returns null if the key does not exist
     */
    getJSON(key: string): any;
}
