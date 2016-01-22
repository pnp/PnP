(function (window) {

    // this should have been created in the pnp-settingsjs file
    if (window.officepnp === 'undefined') {
        $.extend(window, {
            officepnp: {}
        });
    }

    // add our core functionality
    $.extend(window.officepnp, {

        // extend into our namespace for core static methods
        core: {

            _currentUserInfo: null,
            _currentUserInfoPromise: null,
            _currentUserInfoCacheKey: 'pnp-currentuserinfo',

            // allows for the calling pattern getCtxCallback(thisobj, method, methodarg1, methodarg2, ...)
            getCtxCallback: function (context, method) {
                var args = [].slice.call(arguments).slice(2);
                return function () {
                    method.apply(context, args);
                }
            },

            // loads a set of specificed files, returning a promise
            loadFiles: function (/*string[]*/ files) {

                // create a promise
                var promise = $.Deferred();

                // see if we have bad data supplied
                if (typeof files === 'undefined' || ($.isArray(files) && files.length < 1)) {
                    promise.resolve();
                    return promise.promise();
                }

                // if it isn't an array stick it in one, this allows the calling pattern loadFiles('filename.js') with a singular filename
                if (!$.isArray(files)) {
                    files = [files];
                }

                // this function will be used to recursively load all the files
                var engine = function () {

                    // maintain context
                    var self = this;

                    // get the next file to load
                    var file = self.files.shift();

                    // load the remote script file
                    $.getScript(file).done(function () {
                        if (self.files.length > 0) {
                            engine.call(self);
                        }
                        else {
                            self.promise.resolve();
                        }
                    }).fail(self.promise.reject);
                };

                // create our "this" we will apply to the engine function
                var ctx = {
                    files: files,
                    promise: promise
                };

                // call the engine with our context
                engine.call(ctx);

                // give back the promise
                return promise.promise();
            },

            // tests if a url param exists
            urlParamExists: function (name) {
                name = name.replace(/[\[]/, '\\[').replace(/[\]]/, '\\]');
                var regex = new RegExp('[\\?&]' + name + '=([^&#]*)');
                return regex.test(location.search);
            },

            // gets a url param value by name
            getUrlParamByName: function (name) {
                name = name.replace(/[\[]/, '\\[').replace(/[\]]/, '\\]');
                var regex = new RegExp('[\\?&]' + name + '=([^&#]*)');
                results = regex.exec(location.search);
                return results == null ? '' : decodeURIComponent(results[1].replace(/\+/g, ' '));
            },

            // gets a url param by name and attempts to parse a bool value
            getUrlParamBoolByName: function (name) {
                var p = this.getUrlParamByName(name);
                var isFalse = (p === '' || /[false|0]/i.test(p));
                return !isFalse;
            },

            // inserts the string s into the string target as the index specified by index
            stringInsert: function (target, index, s) {
                if (index > 0) {
                    return target.substring(0, index) + s + target.substring(index, target.length);
                }
                return s + target;
            },

            // attempts to extract the base username from the complex loginName
            getUserIdFromLogin: function (/*string*/ login, /*undefined*/ arr) {

                // test cases
                // login = "i:0#.w|domain\\1234567";
                // login = "domain\\1234567";
                // login = "1234567";
                // login = "i:0#.f|membership|1234567@domain.com"

                if (/^i:0#.w/i.test(login)) {
                    // we have a windows identity claim
                    arr = /\|(.*?)\\(.*)/i.exec(login);
                    return arr[arr.length - 1];
                } else if (/(.*?)\\(.*)/i.test(login)) {
                    // we have a domain account
                    arr = /(.*?)\\(.*)/i.exec(login);
                    return arr[arr.length - 1];
                } else if (/^i:0#.f/i.test(login)) {
                    // we have a SPO account
                    arr = /\|membership\|(.*)/i.exec(login);
                    return arr[arr.length - 1];
                }

                // we don't know what we have, so just return the original login
                return login;
            },

            // adds a value to a date
            // http://stackoverflow.com/questions/1197928/how-to-add-30-minutes-to-a-javascript-date-object
            dateAdd: function (date, interval, units) {
                var ret = new Date(date); //don't change original date
                switch (interval.toLowerCase()) {
                    case 'year': ret.setFullYear(ret.getFullYear() + units); break;
                    case 'quarter': ret.setMonth(ret.getMonth() + 3 * units); break;
                    case 'month': ret.setMonth(ret.getMonth() + units); break;
                    case 'week': ret.setDate(ret.getDate() + 7 * units); break;
                    case 'day': ret.setDate(ret.getDate() + units); break;
                    case 'hour': ret.setTime(ret.getTime() + units * 3600000); break;
                    case 'minute': ret.setTime(ret.getTime() + units * 60000); break;
                    case 'second': ret.setTime(ret.getTime() + units * 1000); break;
                    default: ret = undefined; break;
                }
                return ret;
            },

            // loads a stylesheet into the current page
            loadStylesheet: function (/*string*/ path, /*bool*/ avoidCache) {
                if (avoidCache) {
                    path += '?' + encodeURIComponent((new Date()).getTime());
                }
                $('<link />').appendTo('head').attr({ type: 'text/css', rel: 'stylesheet' }).attr({ 'href': path });
            },

            // combines an arbitrary set of paths ensureing that the slashes are normalized
            combinePaths: function () {
                var parts = [];
                for (var i = 0; i < arguments.length; i++) {
                    parts.push(arguments[i].replace(/^[\\|\/]/, '').replace(/[\\|\/]$/, ''));
                }
                return parts.join("/").replace(/\\/, '/');
            },

            // gets a random string of chars length
            getRandomString: function (/*int*/chars) {
                var text = "";
                var possible = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
                for (var i = 0; i < chars; i++) {
                    text += possible.charAt(Math.floor(Math.random() * possible.length));
                }
                return text;
            },

            // gets a random GUID value
            // http://stackoverflow.com/questions/105034/create-guid-uuid-in-javascript
            getGUID: function () {
                var d = new Date().getTime();
                var guid = 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
                    var r = (d + Math.random() * 16) % 16 | 0;
                    d = Math.floor(d / 16);
                    return (c == 'x' ? r : (r & 0x3 | 0x8)).toString(16);
                });
                return guid;
            },

            // loads the current user's information using the session cache for performance
            getCurrentUserInfo: function (ctx) {

                var self = this;

                if (self._currentUserInfoPromise == null) {

                    self._currentUserInfoPromise = $.Deferred(function (def) {

                        var cachingTest = $pnp.session !== 'undefined' && $pnp.session.enabled;

                        // if we have the caching module loaded
                        if (cachingTest) {
                            var userInfo = $pnp.session.get(self._currentUserInfoCacheKey);
                            if (userInfo !== null) {
                                self._currentUserInfo = userInfo;
                                def.resolveWith(ctx || self._currentUserInfo, [self._currentUserInfo]);
                                return;
                            }
                        }

                        // send the request and allow caching
                        $.ajax({
                            method: 'GET',
                            url: '/_api/SP.UserProfiles.PeopleManager/GetMyProperties?$select=AccountName,DisplayName,Title',
                            headers: { "Accept": "application/json; odata=verbose" },
                            cache: true
                        }).done(function (response) {

                            // we also parse and add some custom properties as an example
                            self._currentUserInfo = $.extend(response.d,
                                {
                                    ParsedLoginName: $pnp.core.getUserIdFromLogin(response.d.AccountName)
                                });

                            if (cachingTest) {
                                $pnp.session.add(self._currentUserInfoCacheKey, self._currentUserInfo);
                            }

                            def.resolveWith(ctx || self._currentUserInfo, [self._currentUserInfo]);

                        }).fail(function (jqXHR, textStatus, errorThrown) {

                            console.error('[PNP]=>[Fatal Error] Could not load current user data data from /_api/SP.UserProfiles.PeopleManager/GetMyProperties. status: ' + textStatus + ', error: ' + errorThrown);
                            def.rejectWith(ctx || null);
                        });
                    });
                }

                return this._currentUserInfoPromise.promise();
            }
        }
    });

    // now allow a shorthand calling pattern using $pnp
    window.$pnp = window.officepnp;

    // create a correlation id for this page load or if available use the global one
    $pnp.correlationId = g_correlationId || $pnp.core.getGUID();

})(window);