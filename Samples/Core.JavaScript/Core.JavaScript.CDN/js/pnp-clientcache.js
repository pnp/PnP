(function (window) {

    // after the modernizr test
    function storageTest() {
        var str = 'test';
        try {
            localStorage.setItem(str, str);
            localStorage.removeItem(str);
            return true;
        } catch (e) {
            return false;
        }
    }

    // after the modernizr test
    function sessionTest() {
        var str = 'test';
        try {
            sessionStorage.setItem(str, str);
            sessionStorage.removeItem(str);
            return true;
        } catch (e) {
            return false;
        }
    }

    $.extend(window.officepnp, {

        // adds the client cache capability
        caching: {

            // determine if we have local storage once
            enabled: storageTest(),

            add: function (/*string*/ key, /*object*/ value, /*datetime*/ expiration) {

                if (this.enabled) {
                    localStorage.setItem(key, this._createPersistable(value, expiration));
                }
            },

            // gets an item from the cache, checking the expiration and removing the object if it is expired
            get: function (/*string*/ key) {

                if (!this.enabled) {
                    return null;
                }

                var o = localStorage.getItem(key);

                if (o == null) {
                    return o;
                }

                var persistable = JSON.parse(o);

                if (new Date(persistable.expiration) <= new Date()) {

                    this.remove(key);
                    o = null;

                } else {

                    o = persistable.value;
                }

                return o;
            },

            // removes an item from local storage by key
            remove: function (/*string*/ key) {

                if (this.enabled) {
                    localStorage.removeItem(key);
                }
            },

            // gets an item from the cache or adds it using the supplied getter function
            getOrAdd: function (/*string*/ key, /*function*/ getter) {

                if (!this.enabled) {
                    return getter();
                }

                if (!$.isFunction(getter)) {
                    throw 'Function expected for parameter "getter".';
                }

                var o = this.get(key);

                if (o == null) {
                    o = getter();
                    this.add(key, o);
                }

                return o;
            },

            // creates the persisted object wrapper using the value and the expiration, setting the default expiration if none is applied
            _createPersistable: function (/*object*/ o, /*datetime*/ expiration) {

                if (typeof expiration === 'undefined') {
                    expiration = $pnp.core.dateAdd(new Date(), 'minute', $pnp.settings.localStorageDefaultTimeoutMinutes);
                }

                return JSON.stringify({
                    value: o,
                    expiration: expiration
                });
            }
        },

        // adds the client session capability
        session: {

            // determine if we have session storage once
            enabled: sessionTest(),

            add: function (/*string*/ key, /*object*/ value) {

                if (this.enabled) {
                    sessionStorage.setItem(key, this._createPersistable(value));
                }
            },

            // gets an item from the session storage if it exists
            get: function (/*string*/ key) {

                if (!this.enabled) {
                    return null;
                }

                var o = sessionStorage.getItem(key);

                if (o == null) {
                    return o;
                }

                var persistable = JSON.parse(o);

                return persistable.value;
            },

            // removes an item from session storage by key
            remove: function (/*string*/ key) {

                if (this.enabled) {
                    sessionStorage.removeItem(key);
                }
            },

            // gets an item from the cache or adds it using the supplied getter function
            getOrAdd: function (/*string*/ key, /*function*/ getter) {

                if (!this.enabled) {
                    return getter();
                }

                if (!$.isFunction(getter)) {
                    throw 'Function expected for parameter "getter".';
                }

                var o = this.get(key);

                if (o == null) {
                    o = getter();
                    this.add(key, o);
                }

                return o;
            },

            // creates the persisted object wrapper
            _createPersistable: function (/*object*/ o) {

                return JSON.stringify({
                    value: o
                });
            }
        }
    });
})(window);