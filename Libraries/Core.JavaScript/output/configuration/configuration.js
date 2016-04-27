(function (factory) {/* istanbul ignore next */
    if (typeof module === 'object' && typeof module.exports === 'object') {
        var v = factory(require, exports); if (v !== undefined) module.exports = v;
    }
    else if (typeof define === 'function' && define.amd) {
        define(["require", "exports", "../collections/collections", "es6-promise", "./providers/providers"], factory);
    }
})(function (require, exports) {
    "use strict";
    var Collections = require("../collections/collections");
    var es6_promise_1 = require("es6-promise");
    var providers = require("./providers/providers");
    /**
     * Set of pre-defined providers which are available from this library
     */
    exports.Providers = providers;
    /**
     * Class used to manage the current application settings
     *
     */
    var Settings = (function () {
        /**
         * Creates a new instance of the settings class
         *
         * @constructor
         */
        function Settings() {
            this._settings = new Collections.Dictionary();
        }
        /**
         * Adds a new single setting, or overwrites a previous setting with the same key
         *
         * @param key The key used to store this setting
         * @param value The setting value to store
         */
        Settings.prototype.add = function (key, value) {
            this._settings.add(key, value);
        };
        /**
         * Adds a JSON value to the collection as a string, you must use getJSON to rehydrate the object when read
         *
         * @param key The key used to store this setting
         * @param value The setting value to store
         */
        Settings.prototype.addJSON = function (key, value) {
            this._settings.add(key, JSON.stringify(value));
        };
        /**
         * Applies the supplied hash to the setting collection overwriting any existing value, or created new values
         *
         * @param hash The set of value to apply
         */
        Settings.prototype.apply = function (hash) {
            this._settings.merge(hash);
        };
        /**
         * Loads configuration settings into the collection from the supplied provider and returns a Promise
         *
         * @param provider The provider from which we will load the settings
         */
        Settings.prototype.load = function (provider) {
            var _this = this;
            return new es6_promise_1.Promise(function (resolve, reject) {
                provider.getConfiguration().then(function (value) {
                    _this._settings.merge(value);
                    resolve();
                }).catch(function (reason) {
                    reject(reason);
                });
            });
        };
        /**
         * Gets a value from the configuration
         *
         * @param key The key whose value we want to return. Returns null if the key does not exist
         */
        Settings.prototype.get = function (key) {
            return this._settings.get(key);
        };
        /**
         * Gets a JSON value, rehydrating the stored string to the original object
         *
         * @param key The key whose value we want to return. Returns null if the key does not exist
         */
        Settings.prototype.getJSON = function (key) {
            var o = this.get(key);
            if (typeof o === "undefined" || o === null) {
                return o;
            }
            return JSON.parse(o);
        };
        return Settings;
    }());
    exports.Settings = Settings;
});
