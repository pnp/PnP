"use strict";

import * as Util from "./Util";

export class PnPClientStorageWrapper implements IPnPClientStore {

    public enabled: boolean;

    constructor(private store: Storage, public defaultTimeoutMinutes?: number) {
        this.defaultTimeoutMinutes = (defaultTimeoutMinutes === void 0) ? 5 : defaultTimeoutMinutes;
        this.enabled = this.test();
    }

    public get(key: string): any {

        if (!this.enabled) {
            return null;
        }

        let o = this.store.getItem(key);

        if (o == null) {
            return o;
        }

        let persistable = JSON.parse(o);

        if (new Date(persistable.expiration) <= new Date()) {

            this.delete(key);
            o = null;

        } else {

            o = persistable.value;
        }

        return o;
    }

    public put(key: string, o: any, expire?: Date): void {
        if (this.enabled) {
            this.store.setItem(key, this.createPersistable(o, expire));
        }
    }

    public delete(key: string): void {
        if (this.enabled) {
            this.store.removeItem(key);
        }
    }

    public getOrPut(key: string, getter: Function, expire?: Date): any {
        if (!this.enabled) {
            return getter();
        }

        if (!Util.isFunction(getter)) {
            throw "Function expected for parameter 'getter'.";
        }

        let o = this.get(key);

        if (o == null) {
            o = getter();
            this.put(key, o);
        }

        return o;
    }

    private test(): boolean {
        let str = "test";
        try {
            this.store.setItem(str, str);
            this.store.removeItem(str);
            return true;
        } catch (e) {
            return false;
        }
    }

    private createPersistable(o: any, expire?: Date): string {
        if (typeof expire === "undefined") {
            expire = Util.dateAdd(new Date(), "minute", this.defaultTimeoutMinutes);
        }

        return JSON.stringify({ expiration: expire, value: o });
    }
}

export interface IPnPClientStore {
    enabled: boolean;
    get(key: string): any;
    put(key: string, o: any, expire?: Date): void;
    delete(key: string): void;
    getOrPut(key: string, getter: Function, expire?: Date): any;
}

export class PnPClientStorage {
    constructor() {
        this.local = typeof localStorage !== "undefined" ? new PnPClientStorageWrapper(localStorage) : null;
        this.session = typeof sessionStorage !== "undefined" ? new PnPClientStorageWrapper(sessionStorage) : null;
    }

    public static $: PnPClientStorage = new PnPClientStorage();
    public local: IPnPClientStore;
    public session: IPnPClientStore;
}
