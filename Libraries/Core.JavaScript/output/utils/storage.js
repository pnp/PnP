(function (factory) {/* istanbul ignore next */
    if (typeof module === 'object' && typeof module.exports === 'object') {
        var v = factory(require, exports); if (v !== undefined) module.exports = v;
    }
    else if (typeof define === 'function' && define.amd) {
        define(["require", "exports", "./Util"], factory);
    }
})(function (require, exports) {
    "use strict";
    var Util = require("./Util");
    /**
     * A wrapper class to provide a consistent interface to browser based storage
     *
     */
    var PnPClientStorageWrapper = (function () {
        /**
         * Creates a new instance of the PnPClientStorageWrapper class
         *
         * @constructor
         */
        function PnPClientStorageWrapper(store, defaultTimeoutMinutes) {
            this.store = store;
            this.defaultTimeoutMinutes = defaultTimeoutMinutes;
            this.defaultTimeoutMinutes = (defaultTimeoutMinutes === void 0) ? 5 : defaultTimeoutMinutes;
            this.enabled = this.test();
        }
        /**
         * Get a value from storage, or null if that value does not exist
         *
         * @param key The key whose value we want to retrieve
         */
        PnPClientStorageWrapper.prototype.get = function (key) {
            if (!this.enabled) {
                return null;
            }
            var o = this.store.getItem(key);
            if (o == null) {
                return o;
            }
            var persistable = JSON.parse(o);
            if (new Date(persistable.expiration) <= new Date()) {
                this.delete(key);
                o = null;
            }
            else {
                o = persistable.value;
            }
            return o;
        };
        /**
         * Adds a value to the underlying storage
         *
         * @param key The key to use when storing the provided value
         * @param o The value to store
         * @param expire Optional, if provided the expiration of the item, otherwise the default is used
         */
        PnPClientStorageWrapper.prototype.put = function (key, o, expire) {
            if (this.enabled) {
                this.store.setItem(key, this.createPersistable(o, expire));
            }
        };
        /**
         * Deletes a value from the underlying storage
         *
         * @param key The key of the pair we want to remove from storage
         */
        PnPClientStorageWrapper.prototype.delete = function (key) {
            if (this.enabled) {
                this.store.removeItem(key);
            }
        };
        /**
         * Gets an item from the underlying storage, or adds it if it does not exist using the supplied getter function
         *
         * @param key The key to use when storing the provided value
         * @param getter A function which will upon execution provide the desired value
         * @param expire Optional, if provided the expiration of the item, otherwise the default is used
         */
        PnPClientStorageWrapper.prototype.getOrPut = function (key, getter, expire) {
            if (!this.enabled) {
                return getter();
            }
            if (!Util.isFunction(getter)) {
                throw "Function expected for parameter 'getter'.";
            }
            var o = this.get(key);
            if (o == null) {
                o = getter();
                this.put(key, o);
            }
            return o;
        };
        /**
         * Used to determine if the wrapped storage is available currently
         */
        PnPClientStorageWrapper.prototype.test = function () {
            var str = "test";
            try {
                this.store.setItem(str, str);
                this.store.removeItem(str);
                return true;
            }
            catch (e) {
                return false;
            }
        };
        /**
         * Creates the persistable to store
         */
        PnPClientStorageWrapper.prototype.createPersistable = function (o, expire) {
            if (typeof expire === "undefined") {
                expire = Util.dateAdd(new Date(), "minute", this.defaultTimeoutMinutes);
            }
            return JSON.stringify({ expiration: expire, value: o });
        };
        return PnPClientStorageWrapper;
    }());
    exports.PnPClientStorageWrapper = PnPClientStorageWrapper;
    /**
     * A class that will establish wrappers for both local and session storage
     */
    var PnPClientStorage = (function () {
        /**
         * Creates a new instance of the PnPClientStorage class
         *
         * @constructor
         */
        function PnPClientStorage() {
            this.local = typeof localStorage !== "undefined" ? new PnPClientStorageWrapper(localStorage) : null;
            this.session = typeof sessionStorage !== "undefined" ? new PnPClientStorageWrapper(sessionStorage) : null;
        }
        return PnPClientStorage;
    }());
    exports.PnPClientStorage = PnPClientStorage;
});
