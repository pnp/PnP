var Microsoft;
(function (Microsoft) {
    var ApplicationInsights;
    (function (ApplicationInsights) {
        (function (LoggingSeverity) {
            LoggingSeverity[LoggingSeverity["CRITICAL"] = 0] = "CRITICAL";
            LoggingSeverity[LoggingSeverity["WARNING"] = 1] = "WARNING";
        })(ApplicationInsights.LoggingSeverity || (ApplicationInsights.LoggingSeverity = {}));
        var LoggingSeverity = ApplicationInsights.LoggingSeverity;
        var _InternalLogging = (function () {
            function _InternalLogging() {
            }
            /**
             * This method will throw exceptions in debug mode or attempt to log the error as a console warning.
             */
            _InternalLogging.throwInternalNonUserActionable = function (severity, message) {
                if (_InternalLogging.enableDebugExceptions()) {
                    throw message;
                }
                else {
                    _InternalLogging.warn(message);
                    if (_InternalLogging.verboseLogging() || severity === 0 /* CRITICAL */) {
                        if (this.queue.length < this.MAX_QUEUE_SIZE) {
                            this.queue.push(_InternalLogging.AiNonUserActionable + message);
                        }
                    }
                }
            };
            /**
             * This method will throw exceptions in debug mode or attempt to log the error as a console warning.
             */
            _InternalLogging.throwInternalUserActionable = function (severity, message) {
                if (_InternalLogging.enableDebugExceptions()) {
                    throw message;
                }
                else {
                    _InternalLogging.warn(message);
                    if (_InternalLogging.verboseLogging() || severity === 0 /* CRITICAL */) {
                        if (this.queue.length < this.MAX_QUEUE_SIZE) {
                            this.queue.push(_InternalLogging.AiUserActionablePrefix + message);
                        }
                    }
                }
            };
            /**
             * This will write a warning to the console if possible
             */
            _InternalLogging.warn = function (message) {
                if (typeof console !== "undefined" && !!console) {
                    if (typeof console.warn === "function") {
                        console.warn(message);
                    }
                    else if (typeof console.log === "function") {
                        console.log(message);
                    }
                }
            };
            /**
             * Prefix of the traces in portal.
             */
            _InternalLogging.AiUserActionablePrefix = "AI: ";
            /**
             * For user non actionable traces use AI Internal prefix.
             */
            _InternalLogging.AiNonUserActionable = "AI (Internal): ";
            /**
             * Maximum queue size.
             */
            _InternalLogging.MAX_QUEUE_SIZE = 100;
            /**
             * When this is true the SDK will throw exceptions to aid in debugging.
             */
            _InternalLogging.enableDebugExceptions = function () { return false; };
            /**
             * When this is true the SDK will throw exceptions to aid in debugging.
             */
            _InternalLogging.verboseLogging = function () { return false; };
            _InternalLogging.queue = [];
            return _InternalLogging;
        })();
        ApplicationInsights._InternalLogging = _InternalLogging;
    })(ApplicationInsights = Microsoft.ApplicationInsights || (Microsoft.ApplicationInsights = {}));
})(Microsoft || (Microsoft = {}));
var Microsoft;
(function (Microsoft) {
    var ApplicationInsights;
    (function (ApplicationInsights) {
        var Util = (function () {
            function Util() {
            }
            /**
             * helper method to set userId and sessionId cookie
             */
            Util.setCookie = function (name, value) {
                Util.document.cookie = name + "=" + value + ";path=/";
            };
            Util.stringToBoolOrDefault = function (str) {
                if (!str) {
                    return false;
                }
                return str.toString().toLowerCase() === "true";
            };
            /**
             * helper method to access userId and sessionId cookie
             */
            Util.getCookie = function (name) {
                var value = "";
                if (name && name.length) {
                    var cookieName = name + "=";
                    var cookies = Util.document.cookie.split(";");
                    for (var i = 0; i < cookies.length; i++) {
                        var cookie = cookies[i];
                        cookie = Util.trim(cookie);
                        if (cookie && cookie.indexOf(cookieName) === 0) {
                            value = cookie.substring(cookieName.length, cookies[i].length);
                            break;
                        }
                    }
                }
                return value;
            };
            /**
             * helper method to trim strings (IE8 does not implement String.prototype.trim)
             */
            Util.trim = function (str) {
                if (typeof str !== "string")
                    return str;
                return str.replace(/^\s+|\s+$/g, "");
            };
            /**
             * generate GUID
             */
            Util.newGuid = function () {
                var hexValues = ["0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "A", "B", "C", "D", "E", "F"];
                // c.f. rfc4122 (UUID version 4 = xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx)
                var oct = "", tmp;
                for (var a = 0; a < 4; a++) {
                    tmp = (4294967296 * Math.random()) | 0;
                    oct += hexValues[tmp & 0xF] + hexValues[tmp >> 4 & 0xF] + hexValues[tmp >> 8 & 0xF] + hexValues[tmp >> 12 & 0xF] + hexValues[tmp >> 16 & 0xF] + hexValues[tmp >> 20 & 0xF] + hexValues[tmp >> 24 & 0xF] + hexValues[tmp >> 28 & 0xF];
                }
                // "Set the two most significant bits (bits 6 and 7) of the clock_seq_hi_and_reserved to zero and one, respectively"
                var clockSequenceHi = hexValues[8 + (Math.random() * 4) | 0];
                return oct.substr(0, 8) + "-" + oct.substr(9, 4) + "-4" + oct.substr(13, 3) + "-" + clockSequenceHi + oct.substr(16, 3) + "-" + oct.substr(19, 12);
            };
            /**
             * Check if an object is of type Array
             */
            Util.isArray = function (obj) {
                return Object.prototype.toString.call(obj) === "[object Array]";
            };
            /**
             * Check if an object is of type Error
             */
            Util.isError = function (obj) {
                return Object.prototype.toString.call(obj) === "[object Error]";
            };
            /**
             * Check if an object is of type Date
             */
            Util.isDate = function (obj) {
                return Object.prototype.toString.call(obj) === "[object Date]";
            };
            /**
             * Convert a date to I.S.O. format in IE8
             */
            Util.toISOStringForIE8 = function (date) {
                if (Util.isDate(date)) {
                    if (Date.prototype.toISOString) {
                        return date.toISOString();
                    }
                    else {
                        function pad(number) {
                            var r = String(number);
                            if (r.length === 1) {
                                r = "0" + r;
                            }
                            return r;
                        }
                        return date.getUTCFullYear() + "-" + pad(date.getUTCMonth() + 1) + "-" + pad(date.getUTCDate()) + "T" + pad(date.getUTCHours()) + ":" + pad(date.getUTCMinutes()) + ":" + pad(date.getUTCSeconds()) + "." + String((date.getUTCMilliseconds() / 1000).toFixed(3)).slice(2, 5) + "Z";
                    }
                }
            };
            /**
             * Convert ms to c# time span format
             */
            Util.msToTimeSpan = function (totalms) {
                if (isNaN(totalms) || totalms < 0) {
                    totalms = 0;
                }
                var ms = "" + totalms % 1000;
                var sec = "" + Math.floor(totalms / 1000) % 60;
                var min = "" + Math.floor(totalms / (1000 * 60)) % 60;
                var hour = "" + Math.floor(totalms / (1000 * 60 * 60)) % 24;
                ms = ms.length === 1 ? "00" + ms : ms.length === 2 ? "0" + ms : ms;
                sec = sec.length < 2 ? "0" + sec : sec;
                min = min.length < 2 ? "0" + min : min;
                hour = hour.length < 2 ? "0" + hour : hour;
                return hour + ":" + min + ":" + sec + "." + ms;
            };
            Util.document = typeof document !== "undefined" ? document : {};
            return Util;
        })();
        ApplicationInsights.Util = Util;
    })(ApplicationInsights = Microsoft.ApplicationInsights || (Microsoft.ApplicationInsights = {}));
})(Microsoft || (Microsoft = {}));
/// <reference path="logging.ts" />
/// <reference path="util.ts" />
var Microsoft;
(function (Microsoft) {
    var ApplicationInsights;
    (function (ApplicationInsights) {
        "use strict";
        var Serializer = (function () {
            function Serializer() {
            }
            /**
             * Serializes the current object to a JSON string.
             */
            Serializer.serialize = function (input) {
                var output = Serializer._serializeObject(input, "root");
                return JSON.stringify(output);
            };
            Serializer._serializeObject = function (source, name) {
                var circularReferenceCheck = "__aiCircularRefCheck";
                var output = {};
                if (!source) {
                    ApplicationInsights._InternalLogging.throwInternalUserActionable(0 /* CRITICAL */, "cannot serialize " + name + " because it is null or undefined");
                    return output;
                }
                if (source[circularReferenceCheck]) {
                    ApplicationInsights._InternalLogging.throwInternalUserActionable(1 /* WARNING */, "Circular reference detected while serializing: '" + name);
                    return output;
                }
                if (!source.aiDataContract) {
                    // special case for measurements/properties/tags
                    if (name === "measurements") {
                        output = Serializer._serializeStringMap(source, "number", name);
                    }
                    else if (name === "properties") {
                        output = Serializer._serializeStringMap(source, "string", name);
                    }
                    else if (name === "tags") {
                        output = Serializer._serializeStringMap(source, "string", name);
                    }
                    else if (ApplicationInsights.Util.isArray(source)) {
                        output = Serializer._serializeArray(source, name);
                    }
                    else {
                        ApplicationInsights._InternalLogging.throwInternalUserActionable(1 /* WARNING */, "Attempting to serialize an object which does not implement ISerializable: " + name);
                        try {
                            // verify that the object can be stringified
                            JSON.stringify(source);
                            output = source;
                        }
                        catch (e) {
                            // if serialization fails return an empty string
                            ApplicationInsights._InternalLogging.throwInternalUserActionable(0 /* CRITICAL */, e && typeof e.toString === 'function' ? e.toString() : "Error serializing object");
                        }
                    }
                    return output;
                }
                source[circularReferenceCheck] = true;
                for (var field in source.aiDataContract) {
                    var isRequired = source.aiDataContract[field];
                    var isArray = typeof isRequired !== "boolean";
                    var isPresent = source[field] !== undefined;
                    var isObject = typeof source[field] === "object" && source[field] !== null;
                    if (isRequired && !isPresent && !isArray) {
                        ApplicationInsights._InternalLogging.throwInternalUserActionable(0 /* CRITICAL */, "Missing required field specification: The field '" + field + "' is required but not present on source");
                        continue;
                    }
                    var value;
                    if (isObject) {
                        if (isArray) {
                            // special case; resurse on each object in the source array
                            value = Serializer._serializeArray(source[field], field);
                        }
                        else {
                            // recurse on the source object in this field
                            value = Serializer._serializeObject(source[field], field);
                        }
                    }
                    else {
                        // assign the source field to the output even if undefined or required
                        value = source[field];
                    }
                    // only emit this field if the value is defined
                    if (value !== undefined) {
                        output[field] = value;
                    }
                }
                delete source[circularReferenceCheck];
                return output;
            };
            Serializer._serializeArray = function (sources, name) {
                var output = undefined;
                if (!!sources) {
                    if (!ApplicationInsights.Util.isArray(sources)) {
                        ApplicationInsights._InternalLogging.throwInternalUserActionable(0 /* CRITICAL */, "This field was specified as an array in the contract but the item is not an array.\r\n" + name);
                    }
                    else {
                        output = [];
                        for (var i = 0; i < sources.length; i++) {
                            var source = sources[i];
                            var item = Serializer._serializeObject(source, name + "[" + i + "]");
                            output.push(item);
                        }
                    }
                }
                return output;
            };
            Serializer._serializeStringMap = function (map, expectedType, name) {
                var output = undefined;
                if (map) {
                    output = {};
                    for (var field in map) {
                        var value = map[field];
                        if (expectedType === "string") {
                            if (value === undefined) {
                                output[field] = "undefined";
                            }
                            else if (value === null) {
                                output[field] = "null";
                            }
                            else if (!value.toString) {
                                output[field] = "invalid field: toString() is not defined.";
                            }
                            else {
                                output[field] = value.toString();
                            }
                        }
                        else if (expectedType === "number") {
                            if (value === undefined) {
                                output[field] = "undefined";
                            }
                            else if (value === null) {
                                output[field] = "null";
                            }
                            else {
                                var num = parseFloat(value);
                                if (isNaN(num)) {
                                    output[field] = "NaN";
                                }
                                else {
                                    output[field] = num;
                                }
                            }
                        }
                        else {
                            output[field] = "invalid field: " + name + " is of unknown type.";
                            ApplicationInsights._InternalLogging.throwInternalUserActionable(0 /* CRITICAL */, output[field]);
                        }
                    }
                }
                return output;
            };
            return Serializer;
        })();
        ApplicationInsights.Serializer = Serializer;
    })(ApplicationInsights = Microsoft.ApplicationInsights || (Microsoft.ApplicationInsights = {}));
})(Microsoft || (Microsoft = {}));
// THIS TYPE WAS AUTOGENERATED
var Microsoft;
(function (Microsoft) {
    var Telemetry;
    (function (Telemetry) {
        "use strict";
        var Base = (function () {
            function Base() {
            }
            return Base;
        })();
        Telemetry.Base = Base;
    })(Telemetry = Microsoft.Telemetry || (Microsoft.Telemetry = {}));
})(Microsoft || (Microsoft = {}));
// THIS TYPE WAS AUTOGENERATED
/// <reference path="Base.ts" />
var Microsoft;
(function (Microsoft) {
    var Telemetry;
    (function (Telemetry) {
        "use strict";
        var Envelope = (function () {
            function Envelope() {
                this.ver = 1;
                this.sampleRate = 100.0;
                this.tags = {};
            }
            return Envelope;
        })();
        Telemetry.Envelope = Envelope;
    })(Telemetry = Microsoft.Telemetry || (Microsoft.Telemetry = {}));
})(Microsoft || (Microsoft = {}));
/// <reference path="../../Contracts/Generated/Envelope.ts" />
/// <reference path="../../Contracts/Generated/Base.ts" />
/// <reference path="../../Util.ts"/>
var __extends = this.__extends || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
    function __() { this.constructor = d; }
    __.prototype = b.prototype;
    d.prototype = new __();
};
var Microsoft;
(function (Microsoft) {
    var ApplicationInsights;
    (function (ApplicationInsights) {
        var Telemetry;
        (function (Telemetry) {
            var Common;
            (function (Common) {
                "use strict";
                var Envelope = (function (_super) {
                    __extends(Envelope, _super);
                    /**
                     * Constructs a new instance of telemetry data.
                     */
                    function Envelope(data, name) {
                        _super.call(this);
                        this.name = name;
                        this.data = data;
                        this.time = ApplicationInsights.Util.toISOStringForIE8(new Date());
                        this.aiDataContract = {
                            time: true,
                            iKey: true,
                            name: true,
                            tags: true,
                            data: true
                        };
                    }
                    return Envelope;
                })(Microsoft.Telemetry.Envelope);
                Common.Envelope = Envelope;
            })(Common = Telemetry.Common || (Telemetry.Common = {}));
        })(Telemetry = ApplicationInsights.Telemetry || (ApplicationInsights.Telemetry = {}));
    })(ApplicationInsights = Microsoft.ApplicationInsights || (Microsoft.ApplicationInsights = {}));
})(Microsoft || (Microsoft = {}));
/// <reference path="../../Contracts/Generated/Base.ts"/>
var Microsoft;
(function (Microsoft) {
    var ApplicationInsights;
    (function (ApplicationInsights) {
        var Telemetry;
        (function (Telemetry) {
            var Common;
            (function (Common) {
                "use strict";
                var Base = (function (_super) {
                    __extends(Base, _super);
                    function Base() {
                        _super.apply(this, arguments);
                        /**
                         * The data contract for serializing this object.
                         */
                        this.aiDataContract = {};
                    }
                    return Base;
                })(Microsoft.Telemetry.Base);
                Common.Base = Base;
            })(Common = Telemetry.Common || (Telemetry.Common = {}));
        })(Telemetry = ApplicationInsights.Telemetry || (ApplicationInsights.Telemetry = {}));
    })(ApplicationInsights = Microsoft.ApplicationInsights || (Microsoft.ApplicationInsights = {}));
})(Microsoft || (Microsoft = {}));
// THIS TYPE WAS AUTOGENERATED
var AI;
(function (AI) {
    "use strict";
    var ContextTagKeys = (function () {
        function ContextTagKeys() {
            this.applicationVersion = "ai.application.ver";
            this.applicationBuild = "ai.application.build";
            this.deviceId = "ai.device.id";
            this.deviceIp = "ai.device.ip";
            this.deviceLanguage = "ai.device.language";
            this.deviceLocale = "ai.device.locale";
            this.deviceModel = "ai.device.model";
            this.deviceNetwork = "ai.device.network";
            this.deviceOEMName = "ai.device.oemName";
            this.deviceOS = "ai.device.os";
            this.deviceOSVersion = "ai.device.osVersion";
            this.deviceRoleInstance = "ai.device.roleInstance";
            this.deviceRoleName = "ai.device.roleName";
            this.deviceScreenResolution = "ai.device.screenResolution";
            this.deviceType = "ai.device.type";
            this.deviceMachineName = "ai.device.machineName";
            this.locationIp = "ai.location.ip";
            this.operationId = "ai.operation.id";
            this.operationName = "ai.operation.name";
            this.operationParentId = "ai.operation.parentId";
            this.operationRootId = "ai.operation.rootId";
            this.operationSyntheticSource = "ai.operation.syntheticSource";
            this.operationIsSynthetic = "ai.operation.isSynthetic";
            this.sessionId = "ai.session.id";
            this.sessionIsFirst = "ai.session.isFirst";
            this.sessionIsNew = "ai.session.isNew";
            this.userAccountAcquisitionDate = "ai.user.accountAcquisitionDate";
            this.userAccountId = "ai.user.accountId";
            this.userAgent = "ai.user.userAgent";
            this.userId = "ai.user.id";
            this.userStoreRegion = "ai.user.storeRegion";
            this.sampleRate = "ai.sample.sampleRate";
            this.internalSdkVersion = "ai.internal.sdkVersion";
            this.internalAgentVersion = "ai.internal.agentVersion";
        }
        return ContextTagKeys;
    })();
    AI.ContextTagKeys = ContextTagKeys;
})(AI || (AI = {}));
var Microsoft;
(function (Microsoft) {
    var ApplicationInsights;
    (function (ApplicationInsights) {
        var Context;
        (function (Context) {
            "use strict";
            var Application = (function () {
                function Application() {
                }
                return Application;
            })();
            Context.Application = Application;
        })(Context = ApplicationInsights.Context || (ApplicationInsights.Context = {}));
    })(ApplicationInsights = Microsoft.ApplicationInsights || (Microsoft.ApplicationInsights = {}));
})(Microsoft || (Microsoft = {}));
var Microsoft;
(function (Microsoft) {
    var ApplicationInsights;
    (function (ApplicationInsights) {
        var Context;
        (function (Context) {
            "use strict";
            var Device = (function () {
                /**
                 * Constructs a new instance of the Device class
                 */
                function Device() {
                    // don't attempt to fingerprint browsers
                    this.id = "browser";
                    //get resolution
                    if (typeof screen !== "undefined" && screen.width && screen.height) {
                        this.resolution = screen.width + "X" + screen.height;
                    }
                    //get locale
                    this.locale = (typeof screen !== "undefined" && navigator.browserLanguage) ? navigator.browserLanguage : "unknown";
                }
                return Device;
            })();
            Context.Device = Device;
        })(Context = ApplicationInsights.Context || (ApplicationInsights.Context = {}));
    })(ApplicationInsights = Microsoft.ApplicationInsights || (Microsoft.ApplicationInsights = {}));
})(Microsoft || (Microsoft = {}));
var Microsoft;
(function (Microsoft) {
    var ApplicationInsights;
    (function (ApplicationInsights) {
        var Context;
        (function (Context) {
            "use strict";
            var Internal = (function () {
                /**
                * Constructs a new instance of the internal telemetry data class.
                */
                function Internal() {
                    this.sdkVersion = "JavaScript:" + ApplicationInsights.Version;
                }
                return Internal;
            })();
            Context.Internal = Internal;
        })(Context = ApplicationInsights.Context || (ApplicationInsights.Context = {}));
    })(ApplicationInsights = Microsoft.ApplicationInsights || (Microsoft.ApplicationInsights = {}));
})(Microsoft || (Microsoft = {}));
var Microsoft;
(function (Microsoft) {
    var ApplicationInsights;
    (function (ApplicationInsights) {
        var Context;
        (function (Context) {
            "use strict";
            var Location = (function () {
                function Location() {
                }
                return Location;
            })();
            Context.Location = Location;
        })(Context = ApplicationInsights.Context || (ApplicationInsights.Context = {}));
    })(ApplicationInsights = Microsoft.ApplicationInsights || (Microsoft.ApplicationInsights = {}));
})(Microsoft || (Microsoft = {}));
/// <reference path="../util.ts" />
var Microsoft;
(function (Microsoft) {
    var ApplicationInsights;
    (function (ApplicationInsights) {
        var Context;
        (function (Context) {
            "use strict";
            var Operation = (function () {
                function Operation() {
                    this.id = ApplicationInsights.Util.newGuid();
                }
                return Operation;
            })();
            Context.Operation = Operation;
        })(Context = ApplicationInsights.Context || (ApplicationInsights.Context = {}));
    })(ApplicationInsights = Microsoft.ApplicationInsights || (Microsoft.ApplicationInsights = {}));
})(Microsoft || (Microsoft = {}));
var Microsoft;
(function (Microsoft) {
    var ApplicationInsights;
    (function (ApplicationInsights) {
        var Context;
        (function (Context) {
            "use strict";
            var Sample = (function () {
                function Sample() {
                }
                return Sample;
            })();
            Context.Sample = Sample;
        })(Context = ApplicationInsights.Context || (ApplicationInsights.Context = {}));
    })(ApplicationInsights = Microsoft.ApplicationInsights || (Microsoft.ApplicationInsights = {}));
})(Microsoft || (Microsoft = {}));
// THIS TYPE WAS AUTOGENERATED
var AI;
(function (AI) {
    "use strict";
    (function (SessionState) {
        SessionState[SessionState["Start"] = 0] = "Start";
        SessionState[SessionState["End"] = 1] = "End";
    })(AI.SessionState || (AI.SessionState = {}));
    var SessionState = AI.SessionState;
})(AI || (AI = {}));
/// <reference path="../util.ts" />
/// <reference path="../Contracts/Generated/SessionState.ts"/>
var Microsoft;
(function (Microsoft) {
    var ApplicationInsights;
    (function (ApplicationInsights) {
        var Context;
        (function (Context) {
            "use strict";
            var Session = (function () {
                function Session() {
                }
                return Session;
            })();
            Context.Session = Session;
            var _SessionManager = (function () {
                function _SessionManager(config, sessionHandler) {
                    if (!config) {
                        config = {};
                    }
                    if (!(typeof config.sessionExpirationMs === "function")) {
                        config.sessionExpirationMs = function () { return _SessionManager.acquisitionSpan; };
                    }
                    if (!(typeof config.sessionRenewalMs === "function")) {
                        config.sessionRenewalMs = function () { return _SessionManager.renewalSpan; };
                    }
                    this.config = config;
                    this._sessionHandler = sessionHandler;
                    this.automaticSession = new Session();
                }
                _SessionManager.prototype.update = function () {
                    if (!this.automaticSession.id) {
                        this.initializeAutomaticSession();
                    }
                    var now = +new Date;
                    var acquisitionExpired = now - this.automaticSession.acquisitionDate > this.config.sessionExpirationMs();
                    var renewalExpired = now - this.automaticSession.renewalDate > this.config.sessionRenewalMs();
                    // renew if acquisitionSpan or renewalSpan has ellapsed
                    if (acquisitionExpired || renewalExpired) {
                        // first send session end than update automaticSession so session state has correct id
                        if (typeof this._sessionHandler === "function") {
                            this._sessionHandler(1 /* End */, this.automaticSession.renewalDate);
                        }
                        this.automaticSession.isFirst = undefined;
                        this.renew();
                    }
                    else {
                        this.automaticSession.renewalDate = +new Date;
                        this.setCookie(this.automaticSession.id, this.automaticSession.acquisitionDate, this.automaticSession.renewalDate);
                    }
                };
                _SessionManager.prototype.initializeAutomaticSession = function () {
                    var cookie = ApplicationInsights.Util.getCookie('ai_session');
                    if (cookie && typeof cookie.split === "function") {
                        var params = cookie.split("|");
                        if (params.length > 0) {
                            this.automaticSession.id = params[0];
                        }
                        if (params.length > 1) {
                            var acq = +params[1];
                            this.automaticSession.acquisitionDate = +new Date(acq);
                            this.automaticSession.acquisitionDate = this.automaticSession.acquisitionDate > 0 ? this.automaticSession.acquisitionDate : 0;
                        }
                        if (params.length > 2) {
                            var renewal = +params[2];
                            this.automaticSession.renewalDate = +new Date(renewal);
                            this.automaticSession.renewalDate = this.automaticSession.renewalDate > 0 ? this.automaticSession.renewalDate : 0;
                        }
                    }
                    if (!this.automaticSession.id) {
                        this.automaticSession.isFirst = true;
                        this.renew();
                    }
                };
                _SessionManager.prototype.renew = function () {
                    var now = +new Date;
                    this.automaticSession.id = ApplicationInsights.Util.newGuid();
                    this.automaticSession.acquisitionDate = now;
                    this.automaticSession.renewalDate = now;
                    this.setCookie(this.automaticSession.id, this.automaticSession.acquisitionDate, this.automaticSession.renewalDate);
                    // first we updated automaticSession than we send session start so it has correct id
                    if (typeof this._sessionHandler === "function") {
                        this._sessionHandler(0 /* Start */, now);
                    }
                };
                _SessionManager.prototype.setCookie = function (guid, acq, renewal) {
                    var date = new Date(acq);
                    var cookie = [guid, acq, renewal];
                    // Set cookie to never expire so we can set Session.IsFirst only when cookie is generated for the first time
                    // 365 * 24 * 60 * 60 * 1000 = 31536000000 
                    date.setTime(date.getTime() + 31536000000);
                    ApplicationInsights.Util.setCookie('ai_session', cookie.join('|') + ';expires=' + date.toUTCString());
                };
                _SessionManager.acquisitionSpan = 86400000; // 24 hours in ms
                _SessionManager.renewalSpan = 1800000; // 30 minutes in ms
                return _SessionManager;
            })();
            Context._SessionManager = _SessionManager;
        })(Context = ApplicationInsights.Context || (ApplicationInsights.Context = {}));
    })(ApplicationInsights = Microsoft.ApplicationInsights || (Microsoft.ApplicationInsights = {}));
})(Microsoft || (Microsoft = {}));
/// <reference path="../util.ts" />
var Microsoft;
(function (Microsoft) {
    var ApplicationInsights;
    (function (ApplicationInsights) {
        var Context;
        (function (Context) {
            "use strict";
            var User = (function () {
                function User(accountId) {
                    //get userId or create new one if none exists
                    var cookie = ApplicationInsights.Util.getCookie('ai_user');
                    if (cookie) {
                        var params = cookie.split("|");
                        if (params.length > 0) {
                            this.id = params[0];
                        }
                    }
                    if (!this.id) {
                        this.id = ApplicationInsights.Util.newGuid();
                        var date = new Date();
                        var acqStr = ApplicationInsights.Util.toISOStringForIE8(date);
                        this.accountAcquisitionDate = acqStr;
                        // without expiration, cookies expire at the end of the session
                        // set it to a year from now
                        // 365 * 24 * 60 * 60 * 1000 = 31536000000 
                        date.setTime(date.getTime() + 31536000000);
                        var newCookie = [this.id, acqStr];
                        ApplicationInsights.Util.setCookie('ai_user', newCookie.join('|') + ';expires=' + date.toUTCString());
                    }
                    this.accountId = accountId;
                }
                return User;
            })();
            Context.User = User;
        })(Context = ApplicationInsights.Context || (ApplicationInsights.Context = {}));
    })(ApplicationInsights = Microsoft.ApplicationInsights || (Microsoft.ApplicationInsights = {}));
})(Microsoft || (Microsoft = {}));
/// <reference path="serializer.ts" />
/// <reference path="Telemetry/Common/Envelope.ts"/>
/// <reference path="Telemetry/Common/Base.ts" />
/// <reference path="Contracts/Generated/ContextTagKeys.ts"/>
/// <reference path="Context/Application.ts"/>
/// <reference path="Context/Device.ts"/>
/// <reference path="Context/Internal.ts"/>
/// <reference path="Context/Location.ts"/>
/// <reference path="Context/Operation.ts"/>
/// <reference path="Context/Sample.ts"/>
/// <reference path="Context/Session.ts"/>
/// <reference path="Context/User.ts"/>
var Microsoft;
(function (Microsoft) {
    var ApplicationInsights;
    (function (ApplicationInsights) {
        "use strict";
        var Sender = (function () {
            /**
             * Constructs a new instance of the Sender class
             */
            function Sender(config) {
                this._buffer = [];
                this._lastSend = 0;
                this._config = config;
                this._sender = null;
                if (typeof XMLHttpRequest != "undefined") {
                    var testXhr = new XMLHttpRequest();
                    if ("withCredentials" in testXhr) {
                        this._sender = this._xhrSender;
                    }
                    else if (typeof XDomainRequest !== "undefined") {
                        this._sender = this._xdrSender; //IE 8 and 9
                    }
                }
            }
            /**
             * Add a telemetry item to the send buffer
             */
            Sender.prototype.send = function (envelope) {
                var _this = this;
                // if master off switch is set, don't send any data
                if (this._config.disableTelemetry()) {
                    // Do not send/save data
                    return;
                }
                // validate input
                if (!envelope) {
                    ApplicationInsights._InternalLogging.throwInternalUserActionable(1 /* WARNING */, "Cannot send empty telemetry");
                    return;
                }
                // ensure a sender was constructed
                if (!this._sender) {
                    ApplicationInsights._InternalLogging.warn("No sender could be constructed for this environment, payload will be added to buffer." + ApplicationInsights.Serializer.serialize(envelope));
                    return;
                }
                // check if the incoming payload is too large, truncate if necessary
                var payload = ApplicationInsights.Serializer.serialize(envelope);
                // flush if we would exceet the max-size limit by adding this item
                if (this._getSizeInBytes(this._buffer) + payload.length > this._config.maxBatchSizeInBytes()) {
                    this.triggerSend();
                }
                // enqueue the payload
                this._buffer.push(payload);
                // ensure an invocation timeout is set
                if (!this._timeoutHandle) {
                    this._timeoutHandle = setTimeout(function () {
                        _this._timeoutHandle = null;
                        _this.triggerSend();
                    }, this._config.maxBatchInterval());
                }
            };
            Sender.prototype._getSizeInBytes = function (list) {
                var size = 0;
                if (list && list.length) {
                    for (var i = 0; i < list.length; i++) {
                        var item = list[i];
                        if (item && item.length) {
                            size += item.length;
                        }
                    }
                }
                return size;
            };
            /**
             * Immediately sennd buffered data
             */
            Sender.prototype.triggerSend = function () {
                // Send data only if disableTelemetry is false
                if (!this._config.disableTelemetry()) {
                    if (this._buffer.length) {
                        // compose an array of payloads
                        var batch = this._config.emitLineDelimitedJson() ? this._buffer.join("\n") : "[" + this._buffer.join(",") + "]";
                        // invoke send
                        this._sender(batch);
                    }
                    // update lastSend time to enable throttling
                    this._lastSend = +new Date;
                }
                // clear buffer
                this._buffer.length = 0;
                clearTimeout(this._timeoutHandle);
                this._timeoutHandle = null;
            };
            /**
             * Send XMLHttpRequest
             */
            Sender.prototype._xhrSender = function (payload) {
                var xhr = new XMLHttpRequest();
                xhr.open("POST", this._config.endpointUrl(), true);
                xhr.setRequestHeader("Content-type", "application/json");
                xhr.onreadystatechange = function () { return Sender._xhrReadyStateChange(xhr, payload); };
                xhr.onerror = function (event) { return Sender._onError(payload, xhr.responseText || xhr.response || "", event); };
                xhr.send(payload);
            };
            /**
             * Send XDomainRequest
             */
            Sender.prototype._xdrSender = function (payload) {
                var xdr = new XDomainRequest();
                xdr.onload = function () { return Sender._xdrOnLoad(xdr, payload); };
                xdr.onerror = function (event) { return Sender._onError(payload, xdr.responseText || "", event); };
                xdr.open('POST', this._config.endpointUrl());
                xdr.send(payload);
            };
            /**
             * xhr state changes
             */
            Sender._xhrReadyStateChange = function (xhr, payload) {
                if (xhr.readyState === 4) {
                    if ((xhr.status < 200 || xhr.status >= 300) && xhr.status !== 0) {
                        Sender._onError(payload, xhr.responseText || xhr.response || "");
                    }
                    else {
                        Sender._onSuccess(payload);
                    }
                }
            };
            /**
             * xdr state changes
             */
            Sender._xdrOnLoad = function (xdr, payload) {
                if (xdr && (xdr.responseText + "" === "200" || xdr.responseText === "")) {
                    Sender._onSuccess(payload);
                }
                else {
                    Sender._onError(payload, xdr && xdr.responseText || "");
                }
            };
            /**
             * error handler
             */
            Sender._onError = function (payload, message, event) {
                ApplicationInsights._InternalLogging.throwInternalNonUserActionable(1 /* WARNING */, "Failed to send telemetry:\n" + message);
            };
            /**
             * success handler
             */
            Sender._onSuccess = function (payload) {
                // no-op, used in tests
            };
            return Sender;
        })();
        ApplicationInsights.Sender = Sender;
    })(ApplicationInsights = Microsoft.ApplicationInsights || (Microsoft.ApplicationInsights = {}));
})(Microsoft || (Microsoft = {}));
// THIS TYPE WAS AUTOGENERATED
var Microsoft;
(function (Microsoft) {
    var Telemetry;
    (function (Telemetry) {
        "use strict";
        var Domain = (function () {
            function Domain() {
            }
            return Domain;
        })();
        Telemetry.Domain = Domain;
    })(Telemetry = Microsoft.Telemetry || (Microsoft.Telemetry = {}));
})(Microsoft || (Microsoft = {}));
// THIS TYPE WAS AUTOGENERATED
var AI;
(function (AI) {
    "use strict";
    (function (SeverityLevel) {
        SeverityLevel[SeverityLevel["Verbose"] = 0] = "Verbose";
        SeverityLevel[SeverityLevel["Information"] = 1] = "Information";
        SeverityLevel[SeverityLevel["Warning"] = 2] = "Warning";
        SeverityLevel[SeverityLevel["Error"] = 3] = "Error";
        SeverityLevel[SeverityLevel["Critical"] = 4] = "Critical";
    })(AI.SeverityLevel || (AI.SeverityLevel = {}));
    var SeverityLevel = AI.SeverityLevel;
})(AI || (AI = {}));
// THIS TYPE WAS AUTOGENERATED
/// <reference path="Domain.ts" />
/// <reference path="SeverityLevel.ts" />
var AI;
(function (AI) {
    "use strict";
    var MessageData = (function (_super) {
        __extends(MessageData, _super);
        function MessageData() {
            this.ver = 2;
            this.properties = {};
            _super.call(this);
        }
        return MessageData;
    })(Microsoft.Telemetry.Domain);
    AI.MessageData = MessageData;
})(AI || (AI = {}));
/// <reference path="../../logging.ts" />
/// <reference path="../../Util.ts"/>
var Microsoft;
(function (Microsoft) {
    var ApplicationInsights;
    (function (ApplicationInsights) {
        var Telemetry;
        (function (Telemetry) {
            var Common;
            (function (Common) {
                "use strict";
                var DataSanitizer = (function () {
                    function DataSanitizer() {
                    }
                    DataSanitizer.sanitizeKeyAndAddUniqueness = function (key, map) {
                        var origLength = key.length;
                        var field = DataSanitizer.sanitizeKey(key);
                        // validation truncated the length.  We need to add uniqueness
                        if (field.length !== origLength) {
                            var i = 0;
                            var uniqueField = field;
                            while (map[uniqueField] !== undefined) {
                                i++;
                                uniqueField = field.substring(0, DataSanitizer.MAX_NAME_LENGTH - 3) + DataSanitizer.padNumber(i);
                            }
                            field = uniqueField;
                        }
                        return field;
                    };
                    DataSanitizer.sanitizeKey = function (name) {
                        if (name) {
                            // Remove any leading or trailing whitepace
                            name = ApplicationInsights.Util.trim(name.toString());
                            // Remove illegal chars
                            if (name.search(/[^0-9a-zA-Z-._()\/ ]/g) >= 0) {
                                name = name.replace(/[^0-9a-zA-Z-._()\/ ]/g, "_");
                                ApplicationInsights._InternalLogging.throwInternalUserActionable(1 /* WARNING */, "name contains illegal characters. Illgeal character have been replaced with '_'. new name: " + name);
                            }
                            // truncate the string to 150 chars
                            if (name.length > DataSanitizer.MAX_NAME_LENGTH) {
                                name = name.substring(0, DataSanitizer.MAX_NAME_LENGTH);
                                ApplicationInsights._InternalLogging.throwInternalUserActionable(1 /* WARNING */, "name is too long.  It has been truncated to " + DataSanitizer.MAX_NAME_LENGTH + " characters.  name: " + name);
                            }
                        }
                        return name;
                    };
                    DataSanitizer.sanitizeString = function (value) {
                        if (value) {
                            value = ApplicationInsights.Util.trim(value);
                            if (value.toString().length > DataSanitizer.MAX_STRING_LENGTH) {
                                value = value.substring(0, DataSanitizer.MAX_STRING_LENGTH);
                                ApplicationInsights._InternalLogging.throwInternalUserActionable(1 /* WARNING */, "string value is too long. It has been truncated to " + DataSanitizer.MAX_STRING_LENGTH + " characters. value: " + value);
                            }
                        }
                        return value;
                    };
                    DataSanitizer.sanitizeUrl = function (url) {
                        if (url) {
                            if (url.length > DataSanitizer.MAX_URL_LENGTH) {
                                url = url.substring(0, DataSanitizer.MAX_URL_LENGTH);
                                ApplicationInsights._InternalLogging.throwInternalUserActionable(1 /* WARNING */, "url is too long, it has been trucated to " + DataSanitizer.MAX_URL_LENGTH + " characters. url: " + url);
                            }
                        }
                        return url;
                    };
                    DataSanitizer.sanitizeMessage = function (message) {
                        if (message) {
                            if (message.length > DataSanitizer.MAX_MESSAGE_LENGTH) {
                                message = message.substring(0, DataSanitizer.MAX_MESSAGE_LENGTH);
                                ApplicationInsights._InternalLogging.throwInternalUserActionable(1 /* WARNING */, "message is too long, it has been trucated to " + DataSanitizer.MAX_MESSAGE_LENGTH + " characters.  message: " + message);
                            }
                        }
                        return message;
                    };
                    DataSanitizer.sanitizeException = function (exception) {
                        if (exception) {
                            if (exception.length > DataSanitizer.MAX_EXCEPTION_LENGTH) {
                                exception = exception.substring(0, DataSanitizer.MAX_EXCEPTION_LENGTH);
                                ApplicationInsights._InternalLogging.throwInternalUserActionable(1 /* WARNING */, "exception is too long, iit has been trucated to " + DataSanitizer.MAX_EXCEPTION_LENGTH + " characters.  exception: " + exception);
                            }
                        }
                        return exception;
                    };
                    DataSanitizer.sanitizeProperties = function (properties) {
                        if (properties) {
                            var tempProps = {};
                            for (var prop in properties) {
                                var value = DataSanitizer.sanitizeString(properties[prop]);
                                prop = DataSanitizer.sanitizeKeyAndAddUniqueness(prop, tempProps);
                                tempProps[prop] = value;
                            }
                            properties = tempProps;
                        }
                        return properties;
                    };
                    DataSanitizer.sanitizeMeasurements = function (measurements) {
                        if (measurements) {
                            var tempMeasurements = {};
                            for (var measure in measurements) {
                                var value = measurements[measure];
                                measure = DataSanitizer.sanitizeKeyAndAddUniqueness(measure, tempMeasurements);
                                tempMeasurements[measure] = value;
                            }
                            measurements = tempMeasurements;
                        }
                        return measurements;
                    };
                    DataSanitizer.padNumber = function (num) {
                        var s = "00" + num;
                        return s.substr(s.length - 3);
                    };
                    /**
                    * Max length allowed for custom names.
                    */
                    DataSanitizer.MAX_NAME_LENGTH = 150;
                    /**
                     * Max length allowed for custom values.
                     */
                    DataSanitizer.MAX_STRING_LENGTH = 1024;
                    /**
                     * Max length allowed for url.
                     */
                    DataSanitizer.MAX_URL_LENGTH = 2048;
                    /**
                     * Max length allowed for messages.
                     */
                    DataSanitizer.MAX_MESSAGE_LENGTH = 32768;
                    /**
                     * Max length allowed for exceptions.
                     */
                    DataSanitizer.MAX_EXCEPTION_LENGTH = 32768;
                    return DataSanitizer;
                })();
                Common.DataSanitizer = DataSanitizer;
            })(Common = Telemetry.Common || (Telemetry.Common = {}));
        })(Telemetry = ApplicationInsights.Telemetry || (ApplicationInsights.Telemetry = {}));
    })(ApplicationInsights = Microsoft.ApplicationInsights || (Microsoft.ApplicationInsights = {}));
})(Microsoft || (Microsoft = {}));
/// <reference path="../Contracts/Generated/MessageData.ts" />
/// <reference path="./Common/DataSanitizer.ts"/>
var Microsoft;
(function (Microsoft) {
    var ApplicationInsights;
    (function (ApplicationInsights) {
        var Telemetry;
        (function (Telemetry) {
            "use strict";
            var Trace = (function (_super) {
                __extends(Trace, _super);
                /**
                 * Constructs a new instance of the MetricTelemetry object
                 */
                function Trace(message, properties) {
                    _super.call(this);
                    this.aiDataContract = {
                        ver: true,
                        message: true,
                        severityLevel: false,
                        measurements: false,
                        properties: false
                    };
                    message = message || "";
                    this.message = Telemetry.Common.DataSanitizer.sanitizeMessage(message);
                    this.properties = Telemetry.Common.DataSanitizer.sanitizeProperties(properties);
                }
                Trace.envelopeType = "Microsoft.ApplicationInsights.Message";
                Trace.dataType = "MessageData";
                return Trace;
            })(AI.MessageData);
            Telemetry.Trace = Trace;
        })(Telemetry = ApplicationInsights.Telemetry || (ApplicationInsights.Telemetry = {}));
    })(ApplicationInsights = Microsoft.ApplicationInsights || (Microsoft.ApplicationInsights = {}));
})(Microsoft || (Microsoft = {}));
// THIS TYPE WAS AUTOGENERATED
/// <reference path="Domain.ts" />
var AI;
(function (AI) {
    "use strict";
    var EventData = (function (_super) {
        __extends(EventData, _super);
        function EventData() {
            this.ver = 2;
            this.properties = {};
            this.measurements = {};
            _super.call(this);
        }
        return EventData;
    })(Microsoft.Telemetry.Domain);
    AI.EventData = EventData;
})(AI || (AI = {}));
/// <reference path="../Contracts/Generated/EventData.ts" />
/// <reference path="./Common/DataSanitizer.ts"/>
var Microsoft;
(function (Microsoft) {
    var ApplicationInsights;
    (function (ApplicationInsights) {
        var Telemetry;
        (function (Telemetry) {
            "use strict";
            var Event = (function (_super) {
                __extends(Event, _super);
                /**
                 * Constructs a new instance of the EventTelemetry object
                 */
                function Event(name, properties, measurements) {
                    _super.call(this);
                    this.aiDataContract = {
                        ver: true,
                        name: true,
                        properties: false,
                        measurements: false
                    };
                    this.name = ApplicationInsights.Telemetry.Common.DataSanitizer.sanitizeString(name);
                    this.properties = ApplicationInsights.Telemetry.Common.DataSanitizer.sanitizeProperties(properties);
                    this.measurements = ApplicationInsights.Telemetry.Common.DataSanitizer.sanitizeMeasurements(measurements);
                }
                Event.envelopeType = "Microsoft.ApplicationInsights.Event";
                Event.dataType = "EventData";
                return Event;
            })(AI.EventData);
            Telemetry.Event = Event;
        })(Telemetry = ApplicationInsights.Telemetry || (ApplicationInsights.Telemetry = {}));
    })(ApplicationInsights = Microsoft.ApplicationInsights || (Microsoft.ApplicationInsights = {}));
})(Microsoft || (Microsoft = {}));
// THIS TYPE WAS AUTOGENERATED
var AI;
(function (AI) {
    "use strict";
    var ExceptionDetails = (function () {
        function ExceptionDetails() {
            this.hasFullStack = true;
            this.parsedStack = [];
        }
        return ExceptionDetails;
    })();
    AI.ExceptionDetails = ExceptionDetails;
})(AI || (AI = {}));
// THIS TYPE WAS AUTOGENERATED
/// <reference path="Domain.ts" />
/// <reference path="SeverityLevel.ts" />
/// <reference path="ExceptionDetails.ts"/>
var AI;
(function (AI) {
    "use strict";
    var ExceptionData = (function (_super) {
        __extends(ExceptionData, _super);
        function ExceptionData() {
            this.ver = 2;
            this.exceptions = [];
            this.properties = {};
            this.measurements = {};
            _super.call(this);
        }
        return ExceptionData;
    })(Microsoft.Telemetry.Domain);
    AI.ExceptionData = ExceptionData;
})(AI || (AI = {}));
// THIS TYPE WAS AUTOGENERATED
var AI;
(function (AI) {
    "use strict";
    var StackFrame = (function () {
        function StackFrame() {
        }
        return StackFrame;
    })();
    AI.StackFrame = StackFrame;
})(AI || (AI = {}));
/// <reference path="../Contracts/Generated/ExceptionData.ts" />
/// <reference path="../Contracts/Generated/StackFrame.ts" />
/// <reference path="./Common/DataSanitizer.ts"/>
var Microsoft;
(function (Microsoft) {
    var ApplicationInsights;
    (function (ApplicationInsights) {
        var Telemetry;
        (function (Telemetry) {
            "use strict";
            var Exception = (function (_super) {
                __extends(Exception, _super);
                /**
                * Constructs a new isntance of the ExceptionTelemetry object
                */
                function Exception(exception, handledAt, properties, measurements) {
                    _super.call(this);
                    this.aiDataContract = {
                        ver: true,
                        handledAt: true,
                        exceptions: true,
                        severityLevel: false,
                        properties: false,
                        measurements: false
                    };
                    this.properties = ApplicationInsights.Telemetry.Common.DataSanitizer.sanitizeProperties(properties);
                    this.measurements = ApplicationInsights.Telemetry.Common.DataSanitizer.sanitizeMeasurements(measurements);
                    this.handledAt = handledAt || "unhandled";
                    this.exceptions = [new _ExceptionDetails(exception)];
                }
                Exception.envelopeType = "Microsoft.ApplicationInsights.Exception";
                Exception.dataType = "ExceptionData";
                return Exception;
            })(AI.ExceptionData);
            Telemetry.Exception = Exception;
            var _ExceptionDetails = (function (_super) {
                __extends(_ExceptionDetails, _super);
                function _ExceptionDetails(exception) {
                    _super.call(this);
                    this.aiDataContract = {
                        id: false,
                        outerId: false,
                        typeName: true,
                        message: true,
                        hasFullStack: false,
                        stack: false,
                        parsedStack: []
                    };
                    this.typeName = Telemetry.Common.DataSanitizer.sanitizeString(exception.name);
                    this.message = Telemetry.Common.DataSanitizer.sanitizeMessage(exception.message);
                    var stack = exception["stack"];
                    this.parsedStack = this.parseStack(stack);
                    this.stack = Telemetry.Common.DataSanitizer.sanitizeException(stack);
                    this.hasFullStack = ApplicationInsights.Util.isArray(this.parsedStack) && this.parsedStack.length > 0;
                }
                _ExceptionDetails.prototype.parseStack = function (stack) {
                    var parsedStack = undefined;
                    if (typeof stack === "string") {
                        var frames = stack.split('\n');
                        parsedStack = [];
                        var level = 0;
                        var totalSizeInBytes = 0;
                        for (var i = 0; i <= frames.length; i++) {
                            var frame = frames[i];
                            if (_StackFrame.regex.test(frame)) {
                                var parsedFrame = new _StackFrame(frames[i], level++);
                                totalSizeInBytes += parsedFrame.sizeInBytes;
                                parsedStack.push(parsedFrame);
                            }
                        }
                        // DP Constraint - exception parsed stack must be < 32KB
                        // remove frames from the middle to meet the threshold
                        var exceptionParsedStackThreshold = 32 * 1024;
                        if (totalSizeInBytes > exceptionParsedStackThreshold) {
                            var left = 0;
                            var right = parsedStack.length - 1;
                            var size = 0;
                            var acceptedLeft = left;
                            var acceptedRight = right;
                            while (left < right) {
                                // check size
                                var lSize = parsedStack[left].sizeInBytes;
                                var rSize = parsedStack[right].sizeInBytes;
                                size += lSize + rSize;
                                if (size > exceptionParsedStackThreshold) {
                                    // remove extra frames from the middle
                                    var howMany = acceptedRight - acceptedLeft + 1;
                                    parsedStack.splice(acceptedLeft, howMany);
                                    break;
                                }
                                // update pointers
                                acceptedLeft = left;
                                acceptedRight = right;
                                left++;
                                right--;
                            }
                        }
                    }
                    return parsedStack;
                };
                return _ExceptionDetails;
            })(AI.ExceptionDetails);
            var _StackFrame = (function (_super) {
                __extends(_StackFrame, _super);
                function _StackFrame(frame, level) {
                    _super.call(this);
                    this.sizeInBytes = 0;
                    this.aiDataContract = {
                        level: true,
                        method: true,
                        assembly: false,
                        fileName: false,
                        line: false
                    };
                    this.level = level;
                    this.method = "unavailable";
                    this.assembly = ApplicationInsights.Util.trim(frame);
                    var matches = frame.match(_StackFrame.regex);
                    if (matches && matches.length >= 5) {
                        this.method = ApplicationInsights.Util.trim(matches[2]);
                        this.fileName = ApplicationInsights.Util.trim(matches[4]);
                        this.line = parseInt(matches[5]) || 0;
                    }
                    this.sizeInBytes += this.method.length;
                    this.sizeInBytes += this.fileName.length;
                    this.sizeInBytes += this.assembly.length;
                    // todo: these might need to be removed depending on how the back-end settles on their size calculation
                    this.sizeInBytes += _StackFrame.baseSize;
                    this.sizeInBytes += this.level.toString().length;
                    this.sizeInBytes += this.line.toString().length;
                }
                // regex to match stack frames from ie/chrome/ff
                // methodName=$2, fileName=$4, lineNo=$5, column=$6
                _StackFrame.regex = /^([\s]+at)?(.*?)(\@|\s\(|\s)([^\(\@\n]+):([0-9]+):([0-9]+)(\)?)$/;
                _StackFrame.baseSize = 58; //'{"method":"","level":,"assembly":"","fileName":"","line":}'.length
                return _StackFrame;
            })(AI.StackFrame);
        })(Telemetry = ApplicationInsights.Telemetry || (ApplicationInsights.Telemetry = {}));
    })(ApplicationInsights = Microsoft.ApplicationInsights || (Microsoft.ApplicationInsights = {}));
})(Microsoft || (Microsoft = {}));
// THIS TYPE WAS AUTOGENERATED
/// <reference path="Domain.ts" />
var AI;
(function (AI) {
    "use strict";
    var MetricData = (function (_super) {
        __extends(MetricData, _super);
        function MetricData() {
            this.ver = 2;
            this.metrics = [];
            this.properties = {};
            _super.call(this);
        }
        return MetricData;
    })(Microsoft.Telemetry.Domain);
    AI.MetricData = MetricData;
})(AI || (AI = {}));
// THIS TYPE WAS AUTOGENERATED
var AI;
(function (AI) {
    "use strict";
    (function (DataPointType) {
        DataPointType[DataPointType["Measurement"] = 0] = "Measurement";
        DataPointType[DataPointType["Aggregation"] = 1] = "Aggregation";
    })(AI.DataPointType || (AI.DataPointType = {}));
    var DataPointType = AI.DataPointType;
})(AI || (AI = {}));
// THIS TYPE WAS AUTOGENERATED
/// <reference path="DataPointType.ts" />
var AI;
(function (AI) {
    "use strict";
    var DataPoint = (function () {
        function DataPoint() {
            this.kind = 0 /* Measurement */;
        }
        return DataPoint;
    })();
    AI.DataPoint = DataPoint;
})(AI || (AI = {}));
/// <reference path="../../Contracts/Generated/DataPoint.ts"/>
var Microsoft;
(function (Microsoft) {
    var ApplicationInsights;
    (function (ApplicationInsights) {
        var Telemetry;
        (function (Telemetry) {
            var Common;
            (function (Common) {
                "use strict";
                var DataPoint = (function (_super) {
                    __extends(DataPoint, _super);
                    function DataPoint() {
                        _super.apply(this, arguments);
                        /**
                         * The data contract for serializing this object.
                         */
                        this.aiDataContract = {
                            name: true,
                            kind: false,
                            value: true,
                            count: false,
                            min: false,
                            max: false,
                            stdDev: false
                        };
                    }
                    return DataPoint;
                })(AI.DataPoint);
                Common.DataPoint = DataPoint;
            })(Common = Telemetry.Common || (Telemetry.Common = {}));
        })(Telemetry = ApplicationInsights.Telemetry || (ApplicationInsights.Telemetry = {}));
    })(ApplicationInsights = Microsoft.ApplicationInsights || (Microsoft.ApplicationInsights = {}));
})(Microsoft || (Microsoft = {}));
/// <reference path="../Contracts/Generated/MetricData.ts" />
/// <reference path="./Common/DataSanitizer.ts" />
/// <reference path="./Common/DataPoint.ts" />
var Microsoft;
(function (Microsoft) {
    var ApplicationInsights;
    (function (ApplicationInsights) {
        var Telemetry;
        (function (Telemetry) {
            "use strict";
            var Metric = (function (_super) {
                __extends(Metric, _super);
                /**
                 * Constructs a new instance of the MetricTelemetry object
                 */
                function Metric(name, value, count, min, max) {
                    _super.call(this);
                    this.aiDataContract = {
                        ver: true,
                        metrics: true,
                        properties: false
                    };
                    var dataPoint = new Microsoft.ApplicationInsights.Telemetry.Common.DataPoint();
                    dataPoint.count = count > 0 ? count : undefined;
                    dataPoint.max = isNaN(max) || max === null ? undefined : max;
                    dataPoint.min = isNaN(min) || min === null ? undefined : min;
                    dataPoint.name = Telemetry.Common.DataSanitizer.sanitizeString(name);
                    dataPoint.value = value;
                    this.metrics = [dataPoint];
                }
                Metric.envelopeType = "Microsoft.ApplicationInsights.Metric";
                Metric.dataType = "MetricData";
                return Metric;
            })(AI.MetricData);
            Telemetry.Metric = Metric;
        })(Telemetry = ApplicationInsights.Telemetry || (ApplicationInsights.Telemetry = {}));
    })(ApplicationInsights = Microsoft.ApplicationInsights || (Microsoft.ApplicationInsights = {}));
})(Microsoft || (Microsoft = {}));
// THIS TYPE WAS AUTOGENERATED
/// <reference path="EventData.ts" />
var AI;
(function (AI) {
    "use strict";
    var PageViewData = (function (_super) {
        __extends(PageViewData, _super);
        function PageViewData() {
            this.ver = 2;
            this.properties = {};
            this.measurements = {};
            _super.call(this);
        }
        return PageViewData;
    })(AI.EventData);
    AI.PageViewData = PageViewData;
})(AI || (AI = {}));
/// <reference path="../Contracts/Generated/PageViewData.ts" />
/// <reference path="./Common/DataSanitizer.ts"/>
var Microsoft;
(function (Microsoft) {
    var ApplicationInsights;
    (function (ApplicationInsights) {
        var Telemetry;
        (function (Telemetry) {
            "use strict";
            var PageView = (function (_super) {
                __extends(PageView, _super);
                /**
                 * Constructs a new instance of the PageEventTelemetry object
                 */
                function PageView(name, url, durationMs, properties, measurements) {
                    _super.call(this);
                    this.aiDataContract = {
                        ver: true,
                        name: false,
                        url: false,
                        duration: false,
                        properties: false,
                        measurement: false
                    };
                    this.url = Telemetry.Common.DataSanitizer.sanitizeUrl(url);
                    this.name = Telemetry.Common.DataSanitizer.sanitizeString(name);
                    if (!isNaN(durationMs)) {
                        this.duration = ApplicationInsights.Util.msToTimeSpan(durationMs);
                    }
                    this.properties = ApplicationInsights.Telemetry.Common.DataSanitizer.sanitizeProperties(properties);
                    this.measurements = ApplicationInsights.Telemetry.Common.DataSanitizer.sanitizeMeasurements(measurements);
                }
                PageView.envelopeType = "Microsoft.ApplicationInsights.Pageview";
                PageView.dataType = "PageviewData";
                return PageView;
            })(AI.PageViewData);
            Telemetry.PageView = PageView;
        })(Telemetry = ApplicationInsights.Telemetry || (ApplicationInsights.Telemetry = {}));
    })(ApplicationInsights = Microsoft.ApplicationInsights || (Microsoft.ApplicationInsights = {}));
})(Microsoft || (Microsoft = {}));
// THIS TYPE WAS AUTOGENERATED
/// <reference path="PageViewData.ts" />
var AI;
(function (AI) {
    "use strict";
    var PageViewPerfData = (function (_super) {
        __extends(PageViewPerfData, _super);
        function PageViewPerfData() {
            this.ver = 2;
            this.properties = {};
            this.measurements = {};
            _super.call(this);
        }
        return PageViewPerfData;
    })(AI.PageViewData);
    AI.PageViewPerfData = PageViewPerfData;
})(AI || (AI = {}));
/// <reference path="../Contracts/Generated/PageViewPerfData.ts"/>
/// <reference path="./Common/DataSanitizer.ts"/>
/// <reference path="../Util.ts"/>
var Microsoft;
(function (Microsoft) {
    var ApplicationInsights;
    (function (ApplicationInsights) {
        var Telemetry;
        (function (Telemetry) {
            "use strict";
            var PageViewPerformance = (function (_super) {
                __extends(PageViewPerformance, _super);
                /**
                 * Constructs a new instance of the PageEventTelemetry object
                 */
                function PageViewPerformance(name, url, durationMs, properties, measurements) {
                    _super.call(this);
                    this.aiDataContract = {
                        ver: true,
                        name: false,
                        url: false,
                        duration: false,
                        perfTotal: false,
                        networkConnect: false,
                        sentRequest: false,
                        receivedResponse: false,
                        domProcessing: false,
                        properties: false,
                        measurement: false
                    };
                    /*
                     * http://www.w3.org/TR/navigation-timing/#processing-model
                     *  |-navigationStart
                     *  |             |-connectEnd
                     *  |             ||-requestStart
                     *  |             ||             |-responseStart
                     *  |             ||             |              |-responseEnd
                     *  |             ||             |              ||-domLoading
                     *  |             ||             |              ||         |-loadEventEnd
                     *  |---network---||---request---|---response---||---dom---|
                     *  |--------------------------total-----------------------|
                     */
                    var timing = PageViewPerformance.getPerformanceTiming();
                    if (timing) {
                        var total = PageViewPerformance.getDuration(timing.navigationStart, timing.loadEventEnd);
                        var network = PageViewPerformance.getDuration(timing.navigationStart, timing.connectEnd);
                        var request = PageViewPerformance.getDuration(timing.requestStart, timing.responseStart);
                        var response = PageViewPerformance.getDuration(timing.responseStart, timing.responseEnd);
                        var dom = PageViewPerformance.getDuration(timing.domLoading, timing.loadEventEnd);
                        if (total < Math.floor(network) + Math.floor(request) + Math.floor(response) + Math.floor(dom)) {
                            // some browsers may report individual components incorrectly so that the sum of the parts will be bigger than total PLT
                            // in this case, don't report client performance from this page
                            ApplicationInsights._InternalLogging.throwInternalNonUserActionable(1 /* WARNING */, "client performance math error:" + total + " < " + network + " + " + request + " + " + response + " + " + dom);
                        }
                        else {
                            // use timing data for duration if possible
                            durationMs = total;
                            // convert to timespans
                            this.perfTotal = ApplicationInsights.Util.msToTimeSpan(total);
                            this.networkConnect = ApplicationInsights.Util.msToTimeSpan(network);
                            this.sentRequest = ApplicationInsights.Util.msToTimeSpan(request);
                            this.receivedResponse = ApplicationInsights.Util.msToTimeSpan(response);
                            this.domProcessing = ApplicationInsights.Util.msToTimeSpan(dom);
                        }
                    }
                    this.url = Telemetry.Common.DataSanitizer.sanitizeUrl(url);
                    this.name = Telemetry.Common.DataSanitizer.sanitizeString(name);
                    if (!isNaN(durationMs)) {
                        this.duration = ApplicationInsights.Util.msToTimeSpan(durationMs);
                    }
                    this.properties = ApplicationInsights.Telemetry.Common.DataSanitizer.sanitizeProperties(properties);
                    this.measurements = ApplicationInsights.Telemetry.Common.DataSanitizer.sanitizeMeasurements(measurements);
                }
                PageViewPerformance.getPerformanceTiming = function () {
                    if (typeof window != "undefined" && window.performance && window.performance.timing) {
                        return window.performance.timing;
                    }
                    return null;
                };
                /**
                 * Returns undefined if not available, true if ready, false otherwise
                 */
                PageViewPerformance.checkPageLoad = function () {
                    var status = undefined;
                    if (typeof window != "undefined" && window.performance && window.performance.timing) {
                        var timing = window.performance.timing;
                        status = timing.domainLookupStart > 0 && timing.navigationStart > 0 && timing.responseStart > 0 && timing.requestStart > 0 && timing.loadEventEnd > 0 && timing.responseEnd > 0 && timing.connectEnd > 0 && timing.domLoading > 0;
                    }
                    return status;
                };
                PageViewPerformance.getDuration = function (start, end) {
                    var duration = 0;
                    if (!(isNaN(start) || isNaN(end) || start === 0 || end === 0)) {
                        duration = Math.max(end - start, 0);
                    }
                    return duration;
                };
                PageViewPerformance.envelopeType = "Microsoft.ApplicationInsights.PageviewPerformance";
                PageViewPerformance.dataType = "PageviewPerformanceData";
                return PageViewPerformance;
            })(AI.PageViewPerfData);
            Telemetry.PageViewPerformance = PageViewPerformance;
        })(Telemetry = ApplicationInsights.Telemetry || (ApplicationInsights.Telemetry = {}));
    })(ApplicationInsights = Microsoft.ApplicationInsights || (Microsoft.ApplicationInsights = {}));
})(Microsoft || (Microsoft = {}));
// THIS TYPE WAS AUTOGENERATED
/// <reference path="Domain.ts" />
/// <reference path="SessionState.ts" />
var AI;
(function (AI) {
    "use strict";
    var SessionStateData = (function (_super) {
        __extends(SessionStateData, _super);
        function SessionStateData() {
            this.ver = 2;
            this.state = 0 /* Start */;
            _super.call(this);
        }
        return SessionStateData;
    })(Microsoft.Telemetry.Domain);
    AI.SessionStateData = SessionStateData;
})(AI || (AI = {}));
/// <reference path="../Contracts/Generated/SessionStateData.ts" />
/// <reference path="../Contracts/Generated/SessionState.ts"/>
/// <reference path="./Common/DataSanitizer.ts"/>
var Microsoft;
(function (Microsoft) {
    var ApplicationInsights;
    (function (ApplicationInsights) {
        var Telemetry;
        (function (Telemetry) {
            "use strict";
            var SessionTelemetry = (function (_super) {
                __extends(SessionTelemetry, _super);
                function SessionTelemetry(state) {
                    _super.call(this);
                    this.aiDataContract = {
                        ver: true,
                        state: true
                    };
                    this.state = state;
                }
                SessionTelemetry.envelopeType = "Microsoft.ApplicationInsights.SessionState";
                SessionTelemetry.dataType = "SessionStateData";
                return SessionTelemetry;
            })(AI.SessionStateData);
            Telemetry.SessionTelemetry = SessionTelemetry;
        })(Telemetry = ApplicationInsights.Telemetry || (ApplicationInsights.Telemetry = {}));
    })(ApplicationInsights = Microsoft.ApplicationInsights || (Microsoft.ApplicationInsights = {}));
})(Microsoft || (Microsoft = {}));
/// <reference path="sender.ts"/>
/// <reference path="telemetry/trace.ts" />
/// <reference path="telemetry/event.ts" />
/// <reference path="telemetry/exception.ts" />
/// <reference path="telemetry/metric.ts" />
/// <reference path="telemetry/pageview.ts" />
/// <reference path="telemetry/pageviewperformance.ts" />
/// <reference path="telemetry/SessionTelemetry.ts" />
/// <reference path="./Util.ts"/>
/// <reference path="./Contracts/Generated/SessionState.ts"/>
var Microsoft;
(function (Microsoft) {
    var ApplicationInsights;
    (function (ApplicationInsights) {
        "use strict";
        var TelemetryContext = (function () {
            function TelemetryContext(config) {
                var _this = this;
                this._config = config;
                this._sender = new ApplicationInsights.Sender(config);
                // window will be undefined in node.js where we do not want to initialize contexts
                if (typeof window !== 'undefined') {
                    this._sessionManager = new ApplicationInsights.Context._SessionManager(config, function (sessionState, timestamp) { return TelemetryContext._sessionHandler(_this, sessionState, timestamp); });
                    this.application = new ApplicationInsights.Context.Application();
                    this.device = new ApplicationInsights.Context.Device();
                    this.internal = new ApplicationInsights.Context.Internal();
                    this.location = new ApplicationInsights.Context.Location();
                    this.user = new ApplicationInsights.Context.User(config.accountId());
                    this.operation = new ApplicationInsights.Context.Operation();
                    this.session = new ApplicationInsights.Context.Session();
                    this.sample = new ApplicationInsights.Context.Sample();
                }
            }
            /**
             * Use Sender.ts to send telemetry object to the endpoint
             */
            TelemetryContext.prototype.track = function (envelope) {
                if (!envelope) {
                    ApplicationInsights._InternalLogging.throwInternalUserActionable(0 /* CRITICAL */, "cannot call .track() with a null or undefined argument");
                }
                else {
                    if (this.session) {
                        // If customer did not provide custom session id update sessionmanager
                        if (typeof this.session.id !== "string") {
                            this._sessionManager.update();
                        }
                    }
                    this._track(envelope);
                }
                return envelope;
            };
            TelemetryContext.prototype._track = function (envelope) {
                if (this.session) {
                    // If customer set id, apply his context; otherwise apply context generated from cookies 
                    if (typeof this.session.id === "string") {
                        this._applySessionContext(envelope, this.session);
                    }
                    else {
                        this._applySessionContext(envelope, this._sessionManager.automaticSession);
                    }
                }
                this._applyApplicationContext(envelope, this.application);
                this._applyDeviceContext(envelope, this.device);
                this._applyInternalContext(envelope, this.internal);
                this._applyLocationContext(envelope, this.location);
                this._applyOperationContext(envelope, this.operation);
                this._applySampleContext(envelope, this.sample);
                this._applyUserContext(envelope, this.user);
                envelope.iKey = this._config.instrumentationKey();
                this._sender.send(envelope);
            };
            TelemetryContext._sessionHandler = function (tc, sessionState, timestamp) {
                var sessionStateTelemetry = new ApplicationInsights.Telemetry.SessionTelemetry(sessionState);
                var sessionStateData = new ApplicationInsights.Telemetry.Common.Data(ApplicationInsights.Telemetry.SessionTelemetry.dataType, sessionStateTelemetry);
                var sessionStateEnvelope = new ApplicationInsights.Telemetry.Common.Envelope(sessionStateData, ApplicationInsights.Telemetry.SessionTelemetry.envelopeType);
                sessionStateEnvelope.time = ApplicationInsights.Util.toISOStringForIE8(new Date(timestamp));
                tc._track(sessionStateEnvelope);
            };
            TelemetryContext.prototype._applyApplicationContext = function (envelope, appContext) {
                if (appContext) {
                    var tagKeys = new AI.ContextTagKeys();
                    if (typeof appContext.ver === "string") {
                        envelope.tags[tagKeys.applicationVersion] = appContext.ver;
                    }
                    if (typeof appContext.build === "string") {
                        envelope.tags[tagKeys.applicationBuild] = appContext.build;
                    }
                }
            };
            TelemetryContext.prototype._applyDeviceContext = function (envelope, deviceContext) {
                var tagKeys = new AI.ContextTagKeys();
                if (deviceContext) {
                    if (typeof deviceContext.id === "string") {
                        envelope.tags[tagKeys.deviceId] = deviceContext.id;
                    }
                    if (typeof deviceContext.ip === "string") {
                        envelope.tags[tagKeys.deviceIp] = deviceContext.ip;
                    }
                    if (typeof deviceContext.language === "string") {
                        envelope.tags[tagKeys.deviceLanguage] = deviceContext.language;
                    }
                    if (typeof deviceContext.locale === "string") {
                        envelope.tags[tagKeys.deviceLocale] = deviceContext.locale;
                    }
                    if (typeof deviceContext.model === "string") {
                        envelope.tags[tagKeys.deviceModel] = deviceContext.model;
                    }
                    if (typeof deviceContext.network !== "undefined") {
                        envelope.tags[tagKeys.deviceNetwork] = deviceContext.network;
                    }
                    if (typeof deviceContext.oemName === "string") {
                        envelope.tags[tagKeys.deviceOEMName] = deviceContext.oemName;
                    }
                    if (typeof deviceContext.os === "string") {
                        envelope.tags[tagKeys.deviceOS] = deviceContext.os;
                    }
                    if (typeof deviceContext.osversion === "string") {
                        envelope.tags[tagKeys.deviceOSVersion] = deviceContext.osversion;
                    }
                    if (typeof deviceContext.resolution === "string") {
                        envelope.tags[tagKeys.deviceScreenResolution] = deviceContext.resolution;
                    }
                    if (typeof deviceContext.type === "string") {
                        envelope.tags[tagKeys.deviceType] = deviceContext.type;
                    }
                }
            };
            TelemetryContext.prototype._applyInternalContext = function (envelope, internalContext) {
                if (internalContext) {
                    var tagKeys = new AI.ContextTagKeys();
                    if (typeof internalContext.agentVersion === "string") {
                        envelope.tags[tagKeys.internalAgentVersion] = internalContext.agentVersion;
                    }
                    if (typeof internalContext.sdkVersion === "string") {
                        envelope.tags[tagKeys.internalSdkVersion] = internalContext.sdkVersion;
                    }
                }
            };
            TelemetryContext.prototype._applyLocationContext = function (envelope, locationContext) {
                if (locationContext) {
                    var tagKeys = new AI.ContextTagKeys();
                    if (typeof locationContext.ip === "string") {
                        envelope.tags[tagKeys.locationIp] = locationContext.ip;
                    }
                }
            };
            TelemetryContext.prototype._applyOperationContext = function (envelope, operationContext) {
                if (operationContext) {
                    var tagKeys = new AI.ContextTagKeys();
                    if (typeof operationContext.id === "string") {
                        envelope.tags[tagKeys.operationId] = operationContext.id;
                    }
                    if (typeof operationContext.name === "string") {
                        envelope.tags[tagKeys.operationName] = operationContext.name;
                    }
                    if (typeof operationContext.parentId === "string") {
                        envelope.tags[tagKeys.operationParentId] = operationContext.parentId;
                    }
                    if (typeof operationContext.rootId === "string") {
                        envelope.tags[tagKeys.operationRootId] = operationContext.rootId;
                    }
                    if (typeof operationContext.syntheticSource === "string") {
                        envelope.tags[tagKeys.operationSyntheticSource] = operationContext.syntheticSource;
                    }
                }
            };
            TelemetryContext.prototype._applySampleContext = function (envelope, sampleContext) {
                if (sampleContext) {
                    var tagKeys = new AI.ContextTagKeys();
                    if (typeof sampleContext.sampleRate === "string") {
                        envelope.tags[tagKeys.sampleRate] = sampleContext.sampleRate;
                    }
                }
            };
            TelemetryContext.prototype._applySessionContext = function (envelope, sessionContext) {
                if (sessionContext) {
                    var tagKeys = new AI.ContextTagKeys();
                    if (typeof sessionContext.id === "string") {
                        envelope.tags[tagKeys.sessionId] = sessionContext.id;
                    }
                    if (typeof sessionContext.isFirst !== "undefined") {
                        envelope.tags[tagKeys.sessionIsFirst] = sessionContext.isFirst;
                    }
                }
            };
            TelemetryContext.prototype._applyUserContext = function (envelope, userContext) {
                if (userContext) {
                    var tagKeys = new AI.ContextTagKeys();
                    if (typeof userContext.accountAcquisitionDate === "string") {
                        envelope.tags[tagKeys.userAccountAcquisitionDate] = userContext.accountAcquisitionDate;
                    }
                    if (typeof userContext.accountId === "string") {
                        envelope.tags[tagKeys.userAccountId] = userContext.accountId;
                    }
                    if (typeof userContext.agent === "string") {
                        envelope.tags[tagKeys.userAgent] = userContext.agent;
                    }
                    if (typeof userContext.id === "string") {
                        envelope.tags[tagKeys.userId] = userContext.id;
                    }
                    if (typeof userContext.storeRegion === "string") {
                        envelope.tags[tagKeys.userStoreRegion] = userContext.storeRegion;
                    }
                }
            };
            return TelemetryContext;
        })();
        ApplicationInsights.TelemetryContext = TelemetryContext;
    })(ApplicationInsights = Microsoft.ApplicationInsights || (Microsoft.ApplicationInsights = {}));
})(Microsoft || (Microsoft = {}));
// THIS TYPE WAS AUTOGENERATED
/// <reference path="Base.ts" />
var Microsoft;
(function (Microsoft) {
    var Telemetry;
    (function (Telemetry) {
        "use strict";
        var Data = (function (_super) {
            __extends(Data, _super);
            function Data() {
                _super.call(this);
            }
            return Data;
        })(Microsoft.Telemetry.Base);
        Telemetry.Data = Data;
    })(Telemetry = Microsoft.Telemetry || (Microsoft.Telemetry = {}));
})(Microsoft || (Microsoft = {}));
/// <reference path="../../Contracts/Generated/Data.ts"/>
var Microsoft;
(function (Microsoft) {
    var ApplicationInsights;
    (function (ApplicationInsights) {
        var Telemetry;
        (function (Telemetry) {
            var Common;
            (function (Common) {
                "use strict";
                var Data = (function (_super) {
                    __extends(Data, _super);
                    /**
                     * Constructs a new instance of telemetry data.
                     */
                    function Data(type, data) {
                        _super.call(this);
                        /**
                         * The data contract for serializing this object.
                         */
                        this.aiDataContract = {
                            baseType: true,
                            baseData: true
                        };
                        this.baseType = type;
                        this.baseData = data;
                    }
                    return Data;
                })(Microsoft.Telemetry.Data);
                Common.Data = Data;
            })(Common = Telemetry.Common || (Telemetry.Common = {}));
        })(Telemetry = ApplicationInsights.Telemetry || (ApplicationInsights.Telemetry = {}));
    })(ApplicationInsights = Microsoft.ApplicationInsights || (Microsoft.ApplicationInsights = {}));
})(Microsoft || (Microsoft = {}));
/// <reference path="telemetrycontext.ts" />
/// <reference path="./Telemetry/Common/Data.ts"/>
/// <reference path="./Util.ts"/>
/// <reference path="./Contracts/Generated/SessionState.ts"/>
var Microsoft;
(function (Microsoft) {
    var ApplicationInsights;
    (function (ApplicationInsights) {
        "use strict";
        ApplicationInsights.Version = "0.15.0.0";
        /**
         * The main API that sends telemetry to Application Insights.
         * Learn more: http://go.microsoft.com/fwlink/?LinkID=401493
         */
        var AppInsights = (function () {
            function AppInsights(config) {
                var _this = this;
                this.config = config || {};
                // load default values if specified
                var defaults = AppInsights.defaultConfig;
                if (defaults !== undefined) {
                    for (var field in defaults) {
                        // for each unspecified field, set the default value
                        if (this.config[field] === undefined) {
                            this.config[field] = defaults[field];
                        }
                    }
                }
                ApplicationInsights._InternalLogging.verboseLogging = function () { return _this.config.verboseLogging; };
                ApplicationInsights._InternalLogging.enableDebugExceptions = function () { return _this.config.enableDebug; };
                var configGetters = {
                    instrumentationKey: function () { return _this.config.instrumentationKey; },
                    accountId: function () { return _this.config.accountId; },
                    appUserId: function () { return _this.config.appUserId; },
                    sessionRenewalMs: function () { return _this.config.sessionRenewalMs; },
                    sessionExpirationMs: function () { return _this.config.sessionExpirationMs; },
                    endpointUrl: function () { return _this.config.endpointUrl; },
                    emitLineDelimitedJson: function () { return _this.config.emitLineDelimitedJson; },
                    maxBatchSizeInBytes: function () { return _this.config.maxBatchSizeInBytes; },
                    maxBatchInterval: function () { return _this.config.maxBatchInterval; },
                    disableTelemetry: function () { return _this.config.disableTelemetry; }
                };
                this.context = new ApplicationInsights.TelemetryContext(configGetters);
                // initialize event timing
                this._eventTracking = new Timing("trackEvent");
                this._eventTracking.action = function (name, url, duration, properties, measurements) {
                    var event = new ApplicationInsights.Telemetry.Event(name, properties, measurements);
                    var data = new ApplicationInsights.Telemetry.Common.Data(ApplicationInsights.Telemetry.Event.dataType, event);
                    var envelope = new ApplicationInsights.Telemetry.Common.Envelope(data, ApplicationInsights.Telemetry.Event.envelopeType);
                    _this.context.track(envelope);
                };
                // initialize page view timing
                this._pageTracking = new Timing("trackPageView");
                this._pageTracking.action = function (name, url, duration, properties, measurements) {
                    var pageView = new ApplicationInsights.Telemetry.PageView(name, url, duration, properties, measurements);
                    var data = new ApplicationInsights.Telemetry.Common.Data(ApplicationInsights.Telemetry.PageView.dataType, pageView);
                    var envelope = new ApplicationInsights.Telemetry.Common.Envelope(data, ApplicationInsights.Telemetry.PageView.envelopeType);
                    _this.context.track(envelope);
                };
            }
            /**
             * Starts timing how long the user views a page or other item. Call this when the page opens.
             * This method doesn't send any telemetry. Call {@link stopTrackTelemetry} to log the page when it closes.
             * @param   name  A string that idenfities this item, unique within this HTML document. Defaults to the document title.
             */
            AppInsights.prototype.startTrackPage = function (name) {
                if (typeof name !== "string") {
                    name = window.document && window.document.title || "";
                }
                this._pageTracking.start(name);
            };
            /**
             * Logs how long a page or other item was visible, after {@link startTrackPage}. Call this when the page closes.
             * @param   name  The string you used as the name in startTrackPage. Defaults to the document title.
             * @param   url   String - a relative or absolute URL that identifies the page or other item. Defaults to the window location.
             * @param   properties  map[string, string] - additional data used to filter pages and metrics in the portal. Defaults to empty.
             * @param   measurements    map[string, number] - metrics associated with this page, displayed in Metrics Explorer on the portal. Defaults to empty.
             */
            AppInsights.prototype.stopTrackPage = function (name, url, properties, measurements) {
                if (typeof name !== "string") {
                    name = window.document && window.document.title || "";
                }
                if (typeof url !== "string") {
                    url = window.location && window.location.href || "";
                }
                this._pageTracking.stop(name, url, properties, measurements);
            };
            /**
             * Logs that a page or other item was viewed.
             * @param   name  The string you used as the name in startTrackPage. Defaults to the document title.
             * @param   url   String - a relative or absolute URL that identifies the page or other item. Defaults to the window location.
             * @param   properties  map[string, string] - additional data used to filter pages and metrics in the portal. Defaults to empty.
             * @param   measurements    map[string, number] - metrics associated with this page, displayed in Metrics Explorer on the portal. Defaults to empty.
             */
            AppInsights.prototype.trackPageView = function (name, url, properties, measurements) {
                var _this = this;
                // ensure we have valid values for the required fields
                if (typeof name !== "string") {
                    name = window.document && window.document.title || "";
                }
                if (typeof url !== "string") {
                    url = window.location && window.location.href || "";
                }
                var durationMs = 0;
                // check if timing data is available
                if (ApplicationInsights.Telemetry.PageViewPerformance.checkPageLoad() !== undefined) {
                    // compute current duration (navigation start to now) for the pageViewTelemetry
                    var startTime = window.performance.timing.navigationStart;
                    durationMs = ApplicationInsights.Telemetry.PageViewPerformance.getDuration(startTime, +new Date);
                    // poll for page load completion and send page view performance data when ready
                    var handle = setInterval(function () {
                        // abort this check if we have not finished loading after 1 minute
                        durationMs = ApplicationInsights.Telemetry.PageViewPerformance.getDuration(startTime, +new Date);
                        var timingDataReady = ApplicationInsights.Telemetry.PageViewPerformance.checkPageLoad();
                        var timeoutReached = durationMs > 60000;
                        if (timeoutReached || timingDataReady) {
                            clearInterval(handle);
                            durationMs = ApplicationInsights.Telemetry.PageViewPerformance.getDuration(startTime, +new Date);
                            var pageViewPerformance = new ApplicationInsights.Telemetry.PageViewPerformance(name, url, durationMs, properties, measurements);
                            var pageViewPerformanceData = new ApplicationInsights.Telemetry.Common.Data(ApplicationInsights.Telemetry.PageViewPerformance.dataType, pageViewPerformance);
                            var pageViewPerformanceEnvelope = new ApplicationInsights.Telemetry.Common.Envelope(pageViewPerformanceData, ApplicationInsights.Telemetry.PageViewPerformance.envelopeType);
                            _this.context.track(pageViewPerformanceEnvelope);
                            _this.context._sender.triggerSend();
                        }
                    }, 100);
                }
                // track the initial page view
                var pageView = new ApplicationInsights.Telemetry.PageView(name, url, durationMs, properties, measurements);
                var pageViewData = new ApplicationInsights.Telemetry.Common.Data(ApplicationInsights.Telemetry.PageView.dataType, pageView);
                var pageViewEnvelope = new ApplicationInsights.Telemetry.Common.Envelope(pageViewData, ApplicationInsights.Telemetry.PageView.envelopeType);
                this.context.track(pageViewEnvelope);
                setTimeout(function () {
                    // fire this event as soon as initial code execution completes in case the user navigates away
                    _this.context._sender.triggerSend();
                }, 100);
            };
            /**
             * Start timing an extended event. Call {@link stopTrackEvent} to log the event when it ends.
             * @param   name    A string that identifies this event uniquely within the document.
             */
            AppInsights.prototype.startTrackEvent = function (name) {
                this._eventTracking.start(name);
            };
            /**
             * Log an extended event that you started timing with {@link startTrackEvent}.
             * @param   name    The string you used to identify this event in startTrackEvent.
             * @param   properties  map[string, string] - additional data used to filter events and metrics in the portal. Defaults to empty.
             * @param   measurements    map[string, number] - metrics associated with this event, displayed in Metrics Explorer on the portal. Defaults to empty.
             */
            AppInsights.prototype.stopTrackEvent = function (name, properties, measurements) {
                this._eventTracking.stop(name, undefined, properties, measurements);
            };
            /**
             * Log a user action or other occurrence.
             * @param   name    A string to identify this event in the portal.
             * @param   properties  map[string, string] - additional data used to filter events and metrics in the portal. Defaults to empty.
             * @param   measurements    map[string, number] - metrics associated with this event, displayed in Metrics Explorer on the portal. Defaults to empty.
             */
            AppInsights.prototype.trackEvent = function (name, properties, measurements) {
                var eventTelemetry = new ApplicationInsights.Telemetry.Event(name, properties, measurements);
                var data = new ApplicationInsights.Telemetry.Common.Data(ApplicationInsights.Telemetry.Event.dataType, eventTelemetry);
                var envelope = new ApplicationInsights.Telemetry.Common.Envelope(data, ApplicationInsights.Telemetry.Event.envelopeType);
                this.context.track(envelope);
            };
            /**
             * Log an exception you have caught.
             * @param   exception   An Error from a catch clause, or the string error message.
             * @param   properties  map[string, string] - additional data used to filter events and metrics in the portal. Defaults to empty.
             * @param   measurements    map[string, number] - metrics associated with this event, displayed in Metrics Explorer on the portal. Defaults to empty.
             */
            AppInsights.prototype.trackException = function (exception, handledAt, properties, measurements) {
                if (!ApplicationInsights.Util.isError(exception)) {
                    try {
                        throw new Error(exception);
                    }
                    catch (error) {
                        exception = error;
                    }
                }
                var exceptionTelemetry = new ApplicationInsights.Telemetry.Exception(exception, handledAt, properties, measurements);
                var data = new ApplicationInsights.Telemetry.Common.Data(ApplicationInsights.Telemetry.Exception.dataType, exceptionTelemetry);
                var envelope = new ApplicationInsights.Telemetry.Common.Envelope(data, ApplicationInsights.Telemetry.Exception.envelopeType);
                this.context.track(envelope);
            };
            /**
             * Log a numeric value that is not associated with a specific event. Typically used to send regular reports of performance indicators.
             * To send a single measurement, use just the first two parameters. If you take measurements very frequently, you can reduce the
             * telemetry bandwidth by aggregating multiple measurements and sending the resulting average at intervals.
             * @param   name    A string that identifies the metric.
             * @param   average Number representing either a single measurement, or the average of several measurements.
             * @param   sampleCount The number of measurements represented by the average. Defaults to 1.
             * @param   min The smallest measurement in the sample. Defaults to the average.
             * @param   max The largest measurement in the sample. Defaults to the average.
             */
            AppInsights.prototype.trackMetric = function (name, average, sampleCount, min, max) {
                var telemetry = new ApplicationInsights.Telemetry.Metric(name, average, sampleCount, min, max);
                var data = new ApplicationInsights.Telemetry.Common.Data(ApplicationInsights.Telemetry.Metric.dataType, telemetry);
                var envelope = new ApplicationInsights.Telemetry.Common.Envelope(data, ApplicationInsights.Telemetry.Metric.envelopeType);
                this.context.track(envelope);
            };
            AppInsights.prototype.trackTrace = function (message, properties) {
                var telemetry = new ApplicationInsights.Telemetry.Trace(message, properties);
                var data = new ApplicationInsights.Telemetry.Common.Data(ApplicationInsights.Telemetry.Trace.dataType, telemetry);
                var envelope = new ApplicationInsights.Telemetry.Common.Envelope(data, ApplicationInsights.Telemetry.Trace.envelopeType);
                this.context.track(envelope);
            };
            /**
             * Immediately send all queued telemetry.
             */
            AppInsights.prototype.flush = function () {
                this.context._sender.triggerSend();
            };
            AppInsights.prototype._onerror = function (message, url, lineNumber, columnNumber, error) {
                if (!ApplicationInsights.Util.isError(error)) {
                    try {
                        throw new Error(message);
                    }
                    catch (exception) {
                        error = exception;
                        if (!error["stack"]) {
                            error["stack"] = "@" + url + ":" + lineNumber + ":" + (columnNumber || 0);
                        }
                    }
                }
                this.trackException(error);
            };
            return AppInsights;
        })();
        ApplicationInsights.AppInsights = AppInsights;
        /**
         * Used to record timed events and page views.
         */
        var Timing = (function () {
            function Timing(name) {
                this._name = name;
                this._events = {};
            }
            Timing.prototype.start = function (name) {
                if (typeof this._events[name] !== "undefined") {
                    ApplicationInsights._InternalLogging.throwInternalUserActionable(1 /* WARNING */, "start" + this._name + " was called more than once for this event without calling stop" + this._name + ". key is '" + name + "'");
                }
                this._events[name] = +new Date;
            };
            Timing.prototype.stop = function (name, url, properties, measurements) {
                var start = this._events[name];
                if (start) {
                    var end = +new Date;
                    var duration = ApplicationInsights.Telemetry.PageViewPerformance.getDuration(start, end);
                    this.action(name, url, duration, properties, measurements);
                }
                else {
                    ApplicationInsights._InternalLogging.throwInternalUserActionable(1 /* WARNING */, "stop" + this._name + " was called without a corresponding start" + this._name + " . Event name is '" + name + "'");
                }
                delete this._events[name];
                this._events[name] = undefined;
            };
            return Timing;
        })();
    })(ApplicationInsights = Microsoft.ApplicationInsights || (Microsoft.ApplicationInsights = {}));
})(Microsoft || (Microsoft = {}));
// THIS TYPE WAS AUTOGENERATED
/// <reference path="PageViewData.ts" />
var AI;
(function (AI) {
    "use strict";
    var AjaxCallData = (function (_super) {
        __extends(AjaxCallData, _super);
        function AjaxCallData() {
            this.ver = 2;
            this.properties = {};
            this.measurements = {};
            _super.call(this);
        }
        return AjaxCallData;
    })(AI.PageViewData);
    AI.AjaxCallData = AjaxCallData;
})(AI || (AI = {}));
// THIS TYPE WAS AUTOGENERATED
var AI;
(function (AI) {
    "use strict";
    (function (DependencyKind) {
        DependencyKind[DependencyKind["SQL"] = 0] = "SQL";
        DependencyKind[DependencyKind["Http"] = 1] = "Http";
        DependencyKind[DependencyKind["Other"] = 2] = "Other";
    })(AI.DependencyKind || (AI.DependencyKind = {}));
    var DependencyKind = AI.DependencyKind;
})(AI || (AI = {}));
// THIS TYPE WAS AUTOGENERATED
var AI;
(function (AI) {
    "use strict";
    (function (DependencySourceType) {
        DependencySourceType[DependencySourceType["Undefined"] = 0] = "Undefined";
        DependencySourceType[DependencySourceType["Aic"] = 1] = "Aic";
        DependencySourceType[DependencySourceType["Apmc"] = 2] = "Apmc";
    })(AI.DependencySourceType || (AI.DependencySourceType = {}));
    var DependencySourceType = AI.DependencySourceType;
})(AI || (AI = {}));
// THIS TYPE WAS AUTOGENERATED
/// <reference path="Domain.ts" />
/// <reference path="DataPointType.ts" />
/// <reference path="DependencyKind.ts" />
/// <reference path="DependencySourceType.ts" />
var AI;
(function (AI) {
    "use strict";
    var RemoteDependencyData = (function (_super) {
        __extends(RemoteDependencyData, _super);
        function RemoteDependencyData() {
            this.ver = 2;
            this.kind = 0 /* Measurement */;
            this.dependencyKind = 2 /* Other */;
            this.success = true;
            this.dependencySource = 0 /* Undefined */;
            this.properties = {};
            _super.call(this);
        }
        return RemoteDependencyData;
    })(Microsoft.Telemetry.Domain);
    AI.RemoteDependencyData = RemoteDependencyData;
})(AI || (AI = {}));
// THIS TYPE WAS AUTOGENERATED
/// <reference path="Domain.ts" />
var AI;
(function (AI) {
    "use strict";
    var RequestData = (function (_super) {
        __extends(RequestData, _super);
        function RequestData() {
            this.ver = 2;
            this.properties = {};
            this.measurements = {};
            _super.call(this);
        }
        return RequestData;
    })(Microsoft.Telemetry.Domain);
    AI.RequestData = RequestData;
})(AI || (AI = {}));
/// <reference path="appinsights.ts" />
var Microsoft;
(function (Microsoft) {
    var ApplicationInsights;
    (function (ApplicationInsights) {
        "use strict";
        var Initialization = (function () {
            function Initialization(snippet) {
                // initialize the queue and config in case they are undefined
                snippet.queue = snippet.queue || [];
                var config = snippet.config || {};
                // ensure instrumentationKey is specified
                if (config && !config.instrumentationKey) {
                    config = snippet;
                    // check for legacy instrumentation key
                    if (config["iKey"]) {
                        Microsoft.ApplicationInsights.Version = "0.10.0.0";
                        config.instrumentationKey = config["iKey"];
                    }
                    else if (config["applicationInsightsId"]) {
                        Microsoft.ApplicationInsights.Version = "0.7.2.0";
                        config.instrumentationKey = config["applicationInsightsId"];
                    }
                    else {
                        throw new Error("Cannot load Application Insights SDK, no instrumentationKey was provided.");
                    }
                }
                // set default values
                config = Initialization.getDefaultConfig(config);
                this.snippet = snippet;
                this.config = config;
            }
            // note: these are split into methods to enable unit tests
            Initialization.prototype.loadAppInsights = function () {
                // initialize global instance of appInsights
                var appInsights = new Microsoft.ApplicationInsights.AppInsights(this.config);
                // implement legacy version of trackPageView for 0.10<
                if (this.config["iKey"]) {
                    var originalTrackPageView = appInsights.trackPageView;
                    appInsights.trackPageView = function (pagePath, properties, measurements) {
                        originalTrackPageView.apply(appInsights, [null, pagePath, properties, measurements]);
                    };
                }
                // implement legacy pageView interface if it is present in the snippet
                var legacyPageView = "logPageView";
                if (typeof this.snippet[legacyPageView] === "function") {
                    appInsights[legacyPageView] = function (pagePath, properties, measurements) {
                        appInsights.trackPageView(null, pagePath, properties, measurements);
                    };
                }
                // implement legacy event interface if it is present in the snippet
                var legacyEvent = "logEvent";
                if (typeof this.snippet[legacyEvent] === "function") {
                    appInsights[legacyEvent] = function (name, properties, measurements) {
                        appInsights.trackEvent(name, properties, measurements);
                    };
                }
                return appInsights;
            };
            Initialization.prototype.emptyQueue = function () {
                try {
                    if (Microsoft.ApplicationInsights.Util.isArray(this.snippet.queue)) {
                        // note: do not check length in the for-loop conditional in case something goes wrong and the stub methods are not overridden.
                        var length = this.snippet.queue.length;
                        for (var i = 0; i < length; i++) {
                            var call = this.snippet.queue[i];
                            call();
                        }
                        this.snippet.queue = undefined;
                        delete this.snippet.queue;
                    }
                }
                catch (exception) {
                    var message = "Failed to send queued telemetry";
                    if (exception && typeof exception.toString === "function") {
                        message += ": " + exception.toString();
                    }
                    Microsoft.ApplicationInsights._InternalLogging.throwInternalNonUserActionable(1 /* WARNING */, message);
                }
            };
            Initialization.prototype.pollInteralLogs = function (appInsightsInstance) {
                return setInterval(function () {
                    var queue = Microsoft.ApplicationInsights._InternalLogging["queue"];
                    var length = queue.length;
                    for (var i = 0; i < length; i++) {
                        appInsightsInstance.trackTrace(queue[i]);
                    }
                    queue.length = 0;
                }, this.config.diagnosticLogInterval);
            };
            Initialization.getDefaultConfig = function (config) {
                if (!config) {
                    config = {};
                }
                // set default values
                config.endpointUrl = config.endpointUrl || "//dc.services.visualstudio.com/v2/track";
                config.accountId = config.accountId;
                config.appUserId = config.appUserId;
                config.sessionRenewalMs = 30 * 60 * 1000;
                config.sessionExpirationMs = 24 * 60 * 60 * 1000;
                config.maxBatchSizeInBytes = config.maxBatchSizeInBytes > 0 ? config.maxBatchSizeInBytes : 1000000;
                config.maxBatchInterval = !isNaN(config.maxBatchInterval) ? config.maxBatchInterval : 15000;
                config.enableDebug = ApplicationInsights.Util.stringToBoolOrDefault(config.enableDebug);
                config.autoCollectErrors = (config.autoCollectErrors !== undefined && config.autoCollectErrors !== null) ? ApplicationInsights.Util.stringToBoolOrDefault(config.autoCollectErrors) : true;
                config.disableTelemetry = ApplicationInsights.Util.stringToBoolOrDefault(config.disableTelemetry);
                config.verboseLogging = ApplicationInsights.Util.stringToBoolOrDefault(config.verboseLogging);
                config.emitLineDelimitedJson = ApplicationInsights.Util.stringToBoolOrDefault(config.emitLineDelimitedJson);
                config.diagnosticLogInterval = config.diagnosticLogInterval || 10000;
                return config;
            };
            return Initialization;
        })();
        ApplicationInsights.Initialization = Initialization;
    })(ApplicationInsights = Microsoft.ApplicationInsights || (Microsoft.ApplicationInsights = {}));
})(Microsoft || (Microsoft = {}));
/// <reference path="initialization.ts" />
function initializeAppInsights() {
    // only initialize if we are running in a browser that supports JSON serialization (ie7<, node.js, cordova)
    if (typeof window !== "undefined" && typeof JSON !== "undefined") {
        // get snippet or initialize to an empty object
        var aiName = "appInsights";
        if (window[aiName] === undefined) {
            // if no snippet is present, initialize default values
            Microsoft.ApplicationInsights.AppInsights.defaultConfig = Microsoft.ApplicationInsights.Initialization.getDefaultConfig();
        }
        else {
            // this is the typical case for browser+snippet
            var snippet = window[aiName] || {};
            // overwrite snippet with full appInsights
            var init = new Microsoft.ApplicationInsights.Initialization(snippet);
            var appInsightsLocal = init.loadAppInsights();
            for (var field in appInsightsLocal) {
                snippet[field] = appInsightsLocal[field];
            }
            init.emptyQueue();
            init.pollInteralLogs(appInsightsLocal);
        }
    }
}
initializeAppInsights();
//# sourceMappingURL=ai.js.map