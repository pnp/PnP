/**
 * Interface defining an object with a known property type
 */
export interface ITypedHash<T> {
    [key: string]: T;
}
/**
 * Generic dictionary
 */
export declare class Dictionary<T> {
    /**
     * Creates a new instance of the Dictionary<T> class
     *
     * @constructor
     */
    constructor();
    /**
     * The array used to store all the keys
     */
    private keys;
    /**
     * The array used to store all the values
     */
    private values;
    /**
     * Gets a value from the collection using the specified key
     *
     * @param key The key whose value we want to return, returns null if the key does not exist
     */
    get(key: string): T;
    /**
     * Adds the supplied key and value to the dictionary
     *
     * @param key The key to add
     * @param o The value to add
     */
    add(key: string, o: T): void;
    /**
     * Merges the supplied typed hash into this dictionary instance. Existing values are updated and new ones are created as appropriate.
     */
    merge(source: ITypedHash<T>): void;
    /**
     * Removes a value from the dictionary
     *
     * @param key The key of the key/value pair to remove. Returns null if the key was not found.
     */
    remove(key: string): T;
    /**
     * Returns all the keys currently in the dictionary as an array
     */
    getKeys(): string[];
    /**
     * Returns all the values currently in the dictionary as an array
     */
    getValues(): T[];
    /**
     * Clears the current dictionary
     */
    clear(): void;
    /**
     * Gets a count of the items currently in the dictionary
     */
    count(): number;
}
