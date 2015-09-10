$pnpcore = {

    onStartPromise: null,
    getContextPromise: null,
    spContext: null,
    scriptbasepath: '/_layouts/15/',

    getHostWebUrl: function () {
        return $pnpcore.getUrlParamByName('SPHostUrl');
    },

    getAddInWebUrl: function () {
        return $pnpcore.getUrlParamByName('SPAppWebUrl');
    },

    onStart: function (/*function()*/ onStartFunc) {

        var hostUrl = $pnpcore.getHostWebUrl();
        var scriptbase = hostUrl + $pnpcore.scriptbasepath;

        if ($pnpcore.onStartPromise == null) {

            // these scripts will be loaded ahead of other files and scripts
            var fileUrls = ['SP.Runtime.js', 'SP.js', 'SP.RequestExecutor.js'].select(function (s) {
                return scriptbase + s;
            });

            $pnpcore.onStartPromise = $.when($pnpcore.loadFiles(fileUrls), $.Deferred(function (d) { $(d.resolve); }).then(function () { $pnpcore.ensureContextQueryString(); })).promise();

            // our default post-onstart actions.
            $pnpcore.onStartPromise.then(function () {
                $pnpcore.extendSPClientContext();
            });
        }

        if ($.isFunction(onStartFunc)) {
            $pnpcore.onStartPromise.done(onStartFunc);
        }

        return $pnpcore.onStartPromise;
    },

    withSPContext: function (/*function(context)*/ action) {

        if ($pnpcore.getContextPromise == null) {

            $pnpcore.getContextPromise = $.Deferred(function (def) {

                // use custom onstart to ensure we have the files loaded and are ready to do stuff
                $pnpcore.onStart(function () {

                    try {

                        var addinWebUrl = $pnpcore.getAddInWebUrl();
                        $pnpcore.spContext = new SP.ClientContext(addinWebUrl);
                        $pnpcore.spContext.set_webRequestExecutorFactory(new SP.ProxyWebRequestExecutorFactory(addinWebUrl));

                        def.resolveWith($pnpcore.spContext, [$pnpcore.spContext]);
                    }
                    catch (e) {

                        // if we have a problem just reject with the associated error
                        def.rejectWith(e, [e]);
                    }
                });

            }).promise();
        }

        // if a function was passed in helpfully register it
        if ($.isFunction(action)) {
            $pnpcore.getContextPromise.done(action);
        }

        // return the promise to support withSPContext().done().fail() syntax
        return $pnpcore.getContextPromise;
    },

    // there are lots of ways to extend the SP.ClientContext object
    // it could be done in a file (see arrayextensions.js for an example of this)
    // if could be done by passing a context to a method
    // here we show using an explicit call and the jQuery $.extend method
    // the benefit of this approach is that you can call it at any time to ensure the extended functionality is present
    // another option would be to pass a ClientContext instance to this method instead of extending prototype directly
    extendSPClientContext: function () {

        $.extend(SP.ClientContext.prototype, {

            ext_executeQueryPromise: function () {

                // maintain self-awareness
                var self = this;

                // create a deferred
                var d = $.Deferred();

                // resolve either success or failure and call back with self and whatever arguments were passed back from executeQueryAsync
                self.executeQueryAsync(function () { d.resolveWith(self, arguments); }, function () { d.rejectWith(self, arguments); });

                // return a promise
                return d.promise();
            },

            // execute the query with retry
            ext_executeQueryRetry: function (i) {

                // maintain self-awareness
                var self = this;

                // create a deferred
                var def = $.Deferred();

                // create a context to track our async retries
                var ctx = {
                    // used to track how many times we have tried
                    retryAttempts: 0,
                    // the max number of times to retry the operation
                    retryCount: 5,
                    // the starting delay in ms, increased with each retry
                    delay: 100,
                    // hold a ref to the deferred object
                    deferred: def,
                    // track all the errors as we retry
                    errors: []
                };

                // call the implementation method to start the cycle
                $pnpcore._$_executeQueryRetryImpl.apply(self, [ctx]);

                // return a promise
                return def.promise();
            }
        });
    },

    // implements the retry logic
    _$_executeQueryRetryImpl: function (ctx) {

        // maintain self-awareness
        var self = this;

        // call our extension method to execute query with a promise
        self.ext_executeQueryPromise().done(function () { ctx.deferred.resolveWith(self, arguments); }).fail(function (sender, error) {

            // record our error
            ctx.errors.push(error);

            var retry = false;

            // see if we should retry
            if (error && error.get_message) {
                retry = /unable to connect to the target server/i.test(error.get_message());
            }

            if (!retry) {
                // if we can't retry, reject
                ctx.deferred.rejectWith(sender, [sender, $pnpcore.getNoRetryAvailableException(ctx)]);
            }

            // grab our current delay value
            var delay = ctx.delay;

            // increment our counters
            ctx.delay *= 2;
            ctx.retryAttempts++;

            // if we have exceeded the retry count, reject
            if (ctx.retryCount <= ctx.retryAttempts) {
                ctx.deferred.rejectWith(sender, [sender, $pnpcore.getRetryLimitReachedException(ctx)]);
            }

            // retry in {delay} milliseconds
            setTimeout($pnpcore.getCtxCallback(self, $pnpcore._$_executeQueryRetryImpl, ctx), delay);
        });
    },

    getNoRetryAvailableException: function (ctx) {
        return {
            message: 'Retry not available.',
            retryCount: ctx.retryAttempts,
            errors: ctx.errors,
            get_message: function () { return this.message; }
        };
    },

    getRetryLimitReachedException: function (ctx) {
        return {
            message: 'Exceeded retry limit.',
            retryCount: ctx.retryAttempts,
            errors: ctx.errors,
            get_message: function () { return this.message; }
        };
    },

    getCtxCallback: function (context, method) {

        var args = [].slice.call(arguments).slice(2);

        return function () {
            method.apply(context, args);
        }
    },

    loadFiles: function (/*string[]*/ files) {

        var promise = $.Deferred();

        if (typeof (files) === undefined || ($.isArray(files) && files.length < 1)) {
            promise.resolve();
            return promise.promise();
        }

        if (!$.isArray(files)) {
            files = [files];
        }

        var engine = function () {

            var self = this;
            var file = self.files.shift();

            $.getScript(file).done(function () {
                if (self.files.length > 0) {
                    engine.call(self);
                }
                else {
                    self.promise.resolve();
                }
            }).fail(function () {
                self.promise.reject();
            });
        };

        // create our "this" we will apply to the engine function
        var ctx = {
            files: files,
            promise: promise
        };

        engine.call(ctx);

        return promise.promise();
    },

    urlParamExists: function (name) {
        name = name.replace(/[\[]/, '\\[').replace(/[\]]/, '\\]');
        var regex = new RegExp('[\\?&]' + name + '=([^&#]*)');
        return regex.test(location.search);
    },

    getUrlParamByName: function (name) {
        name = name.replace(/[\[]/, '\\[').replace(/[\]]/, '\\]');
        var regex = new RegExp('[\\?&]' + name + '=([^&#]*)');
        results = regex.exec(location.search);
        return results == null ? '' : decodeURIComponent(results[1].replace(/\+/g, ' '));
    },

    getUrlParamBoolByName: function (name) {
        var p = $app.getUrlParamByName(name);
        var isFalse = (p == '' || p == '0' || p.toLowerCase() == 'false');
        return !isFalse;
    },

    stringInsert: function (target, index, s) {
        if (index > 0) {
            return target.substring(0, index) + s + target.substring(index, target.length);
        }
        return s + target;
    },

    appendSPQueryToUrl: function (/*string*/ url) {

        // we already have the SPHostUrl param from somewhere else, just give back the url
        if (url.indexOf('SPHostUrl=') > -1) {
            return url;
        }

        // add the required parameters
        url += url.indexOf('?') > -1 ? '&' : '?';
        url += 'SPHostUrl=' + encodeURIComponent($pnpcore.getUrlParamByName('SPHostUrl'));
        if ($pnpcore.urlParamExists('SPAppWebUrl')) {
            url += '&SPAppWebUrl=' + encodeURIComponent($pnpcore.getUrlParamByName('SPAppWebUrl'));
        }
        url += '&SPLanguage=' + encodeURIComponent($pnpcore.getUrlParamByName('SPLanguage'));
        url += '&SPClientTag=' + encodeURIComponent($pnpcore.getUrlParamByName('SPClientTag'));
        url += '&SPProductNumber=' + encodeURIComponent($pnpcore.getUrlParamByName('SPProductNumber'));

        return url;
    },

    getAuthorityFromUrl: function (/*string*/ url) {
        if (url) {
            var match = /^(?:https:\/\/|http:\/\/|\/\/)([^\/\?#]+)(?:\/|#|$|\?)/i.exec(url);
            if (match) {
                return match[1].toUpperCase();
            }
        }
        return null;
    },

    ensureContextQueryString: function () {

        // remove the redirect flag
        var SPHasRedirectedToSharePointParam = "&SPHasRedirectedToSharePoint=1";
        var queryString = window.location.search;
        if (queryString.indexOf(SPHasRedirectedToSharePointParam) >= 0) {
            window.location.search = queryString.replace(SPHasRedirectedToSharePointParam, "");
        }

        $pnpcore.ensureSPHostUrlInLinks($('a'));
    },

    ensureSPHostUrlInLinks: function (/*jquery*/ parentNode) {

        var currentAuthority = $pnpcore.getAuthorityFromUrl(window.location.href);

        parentNode.filter(function () {
            var authority = $pnpcore.getAuthorityFromUrl(this.href);
            if (!authority && /^#|:/.test(this.href)) {
                // Filters out anchors and urls with other unsupported protocols.
                return false;
            }
            return authority != null && authority.toUpperCase() == currentAuthority;
        })
        .each(function () {
            this.href = $pnpcore.appendSPQueryToUrl(this.href);
        });
    }
}