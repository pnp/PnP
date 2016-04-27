(function (factory) {/* istanbul ignore next */
    if (typeof module === 'object' && typeof module.exports === 'object') {
        var v = factory(require, exports); if (v !== undefined) module.exports = v;
    }
    else if (typeof define === 'function' && define.amd) {
        define(["require", "exports"], factory);
    }
})(function (require, exports) {
    "use strict";
    /**
     * Generic dictionary
     */
    var Dictionary = (function () {
        /**
         * Creates a new instance of the Dictionary<T> class
         *
         * @constructor
         */
        function Dictionary() {
            this.keys = [];
            this.values = [];
        }
        /**
         * Gets a value from the collection using the specified key
         *
         * @param key The key whose value we want to return, returns null if the key does not exist
         */
        Dictionary.prototype.get = function (key) {
            var index = this.keys.indexOf(key);
            if (index < 0) {
                return null;
            }
            return this.values[index];
        };
        /**
         * Adds the supplied key and value to the dictionary
         *
         * @param key The key to add
         * @param o The value to add
         */
        Dictionary.prototype.add = function (key, o) {
            var index = this.keys.indexOf(key);
            if (index > -1) {
                this.values[index] = o;
            }
            else {
                this.keys.push(key);
                this.values.push(o);
            }
        };
        /**
         * Merges the supplied typed hash into this dictionary instance. Existing values are updated and new ones are created as appropriate.
         */
        Dictionary.prototype.merge = function (source) {
            for (var key in source) {
                if (typeof key === "string") {
                    this.add(key, source[key]);
                }
            }
        };
        /**
         * Removes a value from the dictionary
         *
         * @param key The key of the key/value pair to remove. Returns null if the key was not found.
         */
        Dictionary.prototype.remove = function (key) {
            var index = this.keys.indexOf(key);
            if (index < 0) {
                // could throw an exception here
                return null;
            }
            var val = this.values[index];
            this.keys.splice(index, 1);
            this.values.splice(index, 1);
            return val;
        };
        /**
         * Returns all the keys currently in the dictionary as an array
         */
        Dictionary.prototype.getKeys = function () {
            return this.keys;
        };
        /**
         * Returns all the values currently in the dictionary as an array
         */
        Dictionary.prototype.getValues = function () {
            return this.values;
        };
        /**
         * Clears the current dictionary
         */
        Dictionary.prototype.clear = function () {
            this.keys = [];
            this.values = [];
        };
        /**
         * Gets a count of the items currently in the dictionary
         */
        Dictionary.prototype.count = function () {
            return this.keys.length;
        };
        return Dictionary;
    }());
    exports.Dictionary = Dictionary;
});
