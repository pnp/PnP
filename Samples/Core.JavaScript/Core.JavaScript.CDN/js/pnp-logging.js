(function (window) {

    $.extend(window.officepnp, {

        // create our logging class, which allows any number of subscribers to a custom event
        logging: {

            levels: {
                Verbose: 0,
                Info: 1,
                Warning: 2,
                Error: 3
            },

            _eventName: 'pnpLogWrite',

            subscribe: function (handler) {

                if (!$.isFunction(handler)) {
                    return;
                }

                $(window).on(this._eventName, handler);
            },

            write: function (/*string*/ message, /*level*/ level, /*string*/ component, /*string*/ origin) {

                // validate our params and set defaults, if we have no message just return
                if (typeof message === 'undefined') { return; }
                if (typeof level === 'undefined') { level = 1; }
                if (typeof component === 'undefined') { component = 'not specified'; }
                if (typeof origin === 'undefined') { origin = 'not specified'; }

                // if we are below the active level don't do anything
                if (level < $pnp.settings.activeLoggingLevel) {
                    return;
                }

                // ensure we have the current user's loging name as part of the logging data
                // pass in our this so we maintain context in the callback
                $pnp.core.getCurrentUserInfo(this).done(function (currentUserInfo) {

                    var eventArgs = {
                        correlationId: $pnp.correlationId,
                        currentUserLogin: currentUserInfo.AccountName,
                        timestamp: (new Date()).toISOString(),
                        message: message,
                        level: level,
                        origin: origin,
                        component: component
                    };

                    // fire all the subscribed handlers
                    $(window).trigger(this._eventName, [eventArgs]);
                });
            }
        }
    });

    // add console logging (a simple subscription example)
    $pnp.logging.subscribe(function (e, args) {
        switch (args.level) {
            case 0: console.log(args.message); break;
            case 1: console.info(args.message); break;
            case 2: console.warn(args.message); break;
            case 3: console.error(args.message); break;
        }
    });

    // add azure telemetry logging, a more complex example
    // https://azure.microsoft.com/en-us/documentation/articles/app-insights-javascript/
    // https://github.com/Microsoft/ApplicationInsights-JS/tree/master/JavaScript
    (function (window) {

        // try and avoid errors if we haven't set a key in the settings file, in practice you can comment out or remove this section
        if ($pnp.settings.azureInsightsInstrumentationKey === 'undefined' || $pnp.settings.azureInsightsInstrumentationKey === '') {
            return;
        }

        // add the base logging, get this code from your application insights instance in the Azure portal
        var appInsights = window.appInsights || function (config) {
            function r(config) {
                t[config] = function () {
                    var i = arguments;
                    t.queue.push(function () { t[config].apply(t, i) });
                }
            }
            var t = { config: config }, u = document, e = window, o = "script", s = u.createElement(o), i, f;
            for (s.src = config.url || "//az416426.vo.msecnd.net/scripts/a/ai.0.js", u.getElementsByTagName(o)[0].parentNode.appendChild(s), t.cookie = u.cookie, t.queue = [], i = ["Event", "Exception", "Metric", "PageView", "Trace"]; i.length;) {
                r("track" + i.pop());
            }
            return r("setAuthenticatedUserContext"), r("clearAuthenticatedUserContext"), config.disableExceptionTracking || (i = "onerror", r("_" + i), f = e[i], e[i] = function (config, r, u, e, o) {
                var s = f && f(config, r, u, e, o);
                return s !== !0 && t["_" + i](config, r, u, e, o), s
            }), t
        }({
            instrumentationKey: $pnp.settings.azureInsightsInstrumentationKey
        });

        window.appInsights = appInsights;

        // track the page view (this will be part of the code from the Azure portal by default)
        appInsights.trackPageView();

        // now we subscribe our per-message listener
        $pnp.logging.subscribe(function (e, eventArgs) {
            var ai = window.appInsights;
            // you can extend this to use the other features of the application insights telemetry client
            if (eventArgs.level == $pnp.logging.levels.Error) {
                ai.trackException(eventArgs.message);
            }
            else {
                ai.trackEvent(eventArgs.message);
            }
        });

    })(window);

})(window);


