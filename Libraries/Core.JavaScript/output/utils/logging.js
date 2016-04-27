(function (factory) {/* istanbul ignore next */
    if (typeof module === 'object' && typeof module.exports === 'object') {
        var v = factory(require, exports); if (v !== undefined) module.exports = v;
    }
    else if (typeof define === 'function' && define.amd) {
        define(["require", "exports", "./args"], factory);
    }
})(function (require, exports) {
    "use strict";
    var Args = require("./args");
    /**
     * A set of logging levels
     *
     */
    (function (LogLevel) {
        LogLevel[LogLevel["Verbose"] = 0] = "Verbose";
        LogLevel[LogLevel["Info"] = 1] = "Info";
        LogLevel[LogLevel["Warning"] = 2] = "Warning";
        LogLevel[LogLevel["Error"] = 3] = "Error";
        LogLevel[LogLevel["Off"] = 99] = "Off";
    })(exports.LogLevel || (exports.LogLevel = {}));
    var LogLevel = exports.LogLevel;
    /**
     * Class used to subscribe ILogListener and log messages throughout an application
     *
     */
    var Logger = (function () {
        /**
         * Creates a new instance of the Logger class
         *
         * @constructor
         * @param activeLogLevel the level used to filter messages (Default: LogLevel.Warning)
         * @param subscribers [Optional] if provided will initialize the array of subscribed listeners
         */
        function Logger(activeLogLevel, subscribers) {
            if (activeLogLevel === void 0) { activeLogLevel = LogLevel.Warning; }
            if (subscribers === void 0) { subscribers = []; }
            this.activeLogLevel = activeLogLevel;
            this.subscribers = subscribers;
        }
        /**
         * Adds an ILogListener instance to the set of subscribed listeners
         *
         */
        Logger.prototype.subscribe = function (listener) {
            Args.objectIsNull(listener, "listener");
            this.subscribers.push(listener);
        };
        /**
         * Gets the current subscriber count
         */
        Logger.prototype.count = function () {
            return this.subscribers.length;
        };
        /**
         * Writes the supplied string to the subscribed listeners
         *
         * @param message The message to write
         * @param level [Optional] if supplied will be used as the level of the entry (Default: LogLevel.Verbose)
         */
        Logger.prototype.write = function (message, level) {
            if (level === void 0) { level = LogLevel.Verbose; }
            this.log({ level: level, message: message });
        };
        /**
         * Logs the supplied entry to the subscribed listeners
         *
         * @param entry The message to log
         */
        Logger.prototype.log = function (entry) {
            Args.objectIsNull(entry, "entry");
            if (entry.level < this.activeLogLevel) {
                return;
            }
            for (var i = 0; i < this.subscribers.length; i++) {
                this.subscribers[i].log(entry);
            }
        };
        /**
         * Logs performance tracking data for the the execution duration of the supplied function using console.profile
         *
         * @param name The name of this profile boundary
         * @param f The function to execute and track within this performance boundary
         */
        Logger.prototype.measure = function (name, f) {
            console.profile(name);
            try {
                return f();
            }
            finally {
                console.profileEnd();
            }
        };
        return Logger;
    }());
    exports.Logger = Logger;
    /**
     * Implementation of ILogListener which logs to the browser console
     *
     */
    var ConsoleListener = (function () {
        function ConsoleListener() {
        }
        /**
         * Any associated data that a given logging listener may choose to log or ignore
         *
         * @param entry The information to be logged
         */
        ConsoleListener.prototype.log = function (entry) {
            var msg = this.format(entry);
            switch (entry.level) {
                case LogLevel.Verbose:
                case LogLevel.Info:
                    console.log(msg);
                    break;
                case LogLevel.Warning:
                    console.warn(msg);
                    break;
                case LogLevel.Error:
                    console.error(msg);
                    break;
            }
        };
        /**
         * Formats the message
         *
         * @param entry The information to format into a string
         */
        ConsoleListener.prototype.format = function (entry) {
            return "Message: " + entry.message + ". Data: " + JSON.stringify(entry.data);
        };
        return ConsoleListener;
    }());
    exports.ConsoleListener = ConsoleListener;
    /* tslint:disable */
    /**
     * Implementation of ILogListener which logs to Azure Insights
     *
     */
    var AzureInsightsListener = (function () {
        /**
         * Creats a new instance of the AzureInsightsListener class
         *
         * @constructor
         * @param azureInsightsInstrumentationKey The instrumentation key created when the Azure Insights instance was created
         */
        function AzureInsightsListener(azureInsightsInstrumentationKey) {
            this.azureInsightsInstrumentationKey = azureInsightsInstrumentationKey;
            Args.stringIsNullOrEmpty(azureInsightsInstrumentationKey, "azureInsightsInstrumentationKey");
            var appInsights = window["appInsights"] || function (config) {
                function r(config) {
                    t[config] = function () {
                        var i = arguments;
                        t.queue.push(function () { t[config].apply(t, i); });
                    };
                }
                var t = { config: config }, u = document, e = window, o = "script", s = u.createElement(o), i, f;
                for (s.src = config.url || "//az416426.vo.msecnd.net/scripts/a/ai.0.js", u.getElementsByTagName(o)[0].parentNode.appendChild(s), t.cookie = u.cookie, t.queue = [], i = ["Event", "Exception", "Metric", "PageView", "Trace"]; i.length;) {
                    r("track" + i.pop());
                }
                return r("setAuthenticatedUserContext"), r("clearAuthenticatedUserContext"), config.disableExceptionTracking || (i = "onerror", r("_" + i), f = e[i], e[i] = function (config, r, u, e, o) {
                    var s = f && f(config, r, u, e, o);
                    return s !== !0 && t["_" + i](config, r, u, e, o), s;
                }), t;
            }({
                instrumentationKey: this.azureInsightsInstrumentationKey
            });
            window["appInsights"] = appInsights;
        }
        /**
         * Any associated data that a given logging listener may choose to log or ignore
         *
         * @param entry The information to be logged
         */
        AzureInsightsListener.prototype.log = function (entry) {
            var ai = window["appInsights"];
            var msg = this.format(entry);
            if (entry.level === LogLevel.Error) {
                ai.trackException(msg);
            }
            else {
                ai.trackEvent(msg);
            }
        };
        /**
         * Formats the message
         *
         * @param entry The information to format into a string
         */
        AzureInsightsListener.prototype.format = function (entry) {
            return "Message: " + entry.message + ". Data: " + JSON.stringify(entry.data);
        };
        return AzureInsightsListener;
    }());
    exports.AzureInsightsListener = AzureInsightsListener;
    /* tslint:enable */
    /**
     * Implementation of ILogListener which logs to the supplied function
     *
     */
    var FunctionListener = (function () {
        /**
         * Creates a new instance of the FunctionListener class
         *
         * @constructor
         * @param  method The method to which any logging data will be passed
         */
        function FunctionListener(method) {
            this.method = method;
        }
        /**
         * Any associated data that a given logging listener may choose to log or ignore
         *
         * @param entry The information to be logged
         */
        FunctionListener.prototype.log = function (entry) {
            this.method(entry);
        };
        return FunctionListener;
    }());
    exports.FunctionListener = FunctionListener;
});
