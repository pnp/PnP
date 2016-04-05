"use strict";

/**
 * Interface defining an object with a known property type
 */
export interface ITypedHash<T> {
    [key: string]: T;
}

/**
 * Generic dictionary
 */
export class Dictionary<T> {

    /**
     * Creates a new instance of the Dictionary<T> class
     * 
     * @constructor
     */
    constructor() {
        this.keys = [];
        this.values = [];
    }

    /**
     * The array used to store all the keys
     */
    private keys: string[];

    /**
     * The array used to store all the values
     */
    private values: T[];

    /**
     * Gets a value from the collection using the specified key
     * 
     * @param key The key whose value we want to return, returns null if the key does not exist
     */
    public get(key: string): T {
        let index = this.keys.indexOf(key);
        if (index < 0) {
            return null;
        }
        return this.values[index];
    }

    /**
     * Adds the supplied key and value to the dictionary
     * 
     * @param key The key to add
     * @param o The value to add
     */
    public add(key: string, o: T): void {
        let index = this.keys.indexOf(key);
        if (index > -1) {
            this.values[index] = o;
        } else {
            this.keys.push(key);
            this.values.push(o);
        }
    }

    /**
     * Merges the supplied typed hash into this dictionary instance. Existing values are updated and new ones are created as appropriate.
     */
    public merge(source: ITypedHash<T>): void {
        for (let key in source) {
            if (typeof key === "string") {
                this.add(key, source[key]);
            }
        }
    }

    /**
     * Removes a value from the dictionary
     * 
     * @param key The key of the key/value pair to remove. Returns null if the key was not found.
     */
    public remove(key: string): T {
        let index = this.keys.indexOf(key);
        if (index < 0) {
            // could throw an exception here
            return null;
        }
        let val = this.values[index];
        this.keys.splice(index, 1);
        this.values.splice(index, 1);
        return val;
    }

    /**
     * Returns all the keys currently in the dictionary as an array
     */
    public getKeys(): string[] {
        return this.keys;
    }

    /**
     * Returns all the values currently in the dictionary as an array
     */
    public getValues(): T[] {
        return this.values;
    }

    /**
     * Clears the current dictionary
     */
    public clear(): void {
        this.keys = [];
        this.values = [];
    }

    /**
     * Gets a count of the items currently in the dictionary
     */
    public count(): number {
        return this.keys.length;
    }
}
