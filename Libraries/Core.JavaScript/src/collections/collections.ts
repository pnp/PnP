"use strict";

export interface ITypedHash<T> {
    [key: string]: T;
}

export class Dictionary<T> {

    constructor() {
        this.keys = [];
        this.values = [];
    }

    private keys: string[];
    private values: T[];

    public get(key: string): T {
        let index = this.keys.indexOf(key);
        if (index < 0) {
            return null;
        }
        return this.values[index];
    }

    public add(key: string, o: T): void {
        let index = this.keys.indexOf(key);
        if (index > -1) {
            this.values[index] = o;
        } else {
            this.keys.push(key);
            this.values.push(o);
        }
    }

    public merge(source: ITypedHash<T>): void {
        for (let key in source) {
            if (typeof key === "string") {
                this.add(key, source[key]);
            }
        }
    }

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

    public getKeys(): string[] {
        return this.keys;
    }

    public getValues(): T[] {
        return this.values;
    }

    public clear(): void {
        this.keys = [];
        this.values = [];
    }

    public count(): number {
        return this.keys.length;
    }
}
