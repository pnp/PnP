(function (window) {

    $.extend(window.officepnp, {

        // this class loads configuration data from a SharePoint list having two columns, Title and Value.
        config: {

            // the key used to store the cache in local storage
            _cacheKey: 'pnp-configuration',

            // loading promise for configuration data
            _loadingPromise: null,

            // values loaded from config list
            _configArray: null,

            // initialize this configuration instance
            _init: function (arr) {

                if ($.isArray(arr)) {

                    this._configArray = arr;

                    // we populate this object with all the found configuration properties
                    for (var i = 0; i < this._configArray.length; i++) {
                        var propName = this._configArray[i].Title.replace(/\W/, '');
                        this[propName] = this._configArray[i].Value;
                    }
                }
            },

            // remove stored config from the cache
            clearCached: function () {

                $pnp.caching.remove(this._cacheKey);
            },

            // async loading of the config either from local cache or server
            ready: function (action) {

                var self = this;

                if (this._loadingPromise == null) {

                    this._loadingPromise = $.Deferred(function (def) {

                        var config = $pnp.caching.get(self._cacheKey);

                        if (config != null) {

                            // we have an item in the cache so we can just init and resolve
                            self._init(config);
                            def.resolveWith(self, [self]);

                        } else {

                            $.ajax({
                                method: 'GET',
                                url: $pnp.settings.configLoadUrl,
                                headers: { "Accept": "application/json; odata=verbose" },
                                cache: true
                            }).done(function (response) {

                                var configStore = [];

                                // TODO:: need to update this with config list field names if they are changed
                                for(var i = 0; i < response.d.results.length; i++) {
                                    configStore.push({ Title: response.d.results[i].Title, Value: response.d.results[i].Value });
                                }

                                $pnp.caching.add(self._cacheKey, configStore);
                                self._init(configStore);
                                def.resolveWith(self, [self]);

                            }).fail(function (jqXHR, textStatus, errorThrown) {

                                console.error('[Fatal Error] Could not load configuration data from ' + $pnp.settings.configLoadUrl + '. status: ' + textStatus + ', error: ' + errorThrown);
                                def.reject();
                            });
                        }

                    }).promise();
                }

                if ($.isFunction(action)) {
                    this._loadingPromise.done(action);
                }

                return this._loadingPromise;
            }
        }
    });
})(window);