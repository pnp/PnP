
// Copyright (c) Microsoft Open Technologies, Inc.  All rights reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

var __extends = this.__extends || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
    function __() { this.constructor = d; }
    __.prototype = b.prototype;
    d.prototype = new __();
};

(function (Microsoft) {
    (function (Utility) {
        (function (EncodingHelpers) {
            function getKeyExpression(entityKeys) {
                var entityInstanceKey = '(';

                if (entityKeys.length == 1) {
                    entityInstanceKey += formatLiteral(entityKeys[0]);
                } else {
                    var addComma = false;
                    for (var i = 0; i < entityKeys.length; i++) {
                        if (addComma) {
                            entityInstanceKey += ',';
                        } else {
                            addComma = true;
                        }

                        entityInstanceKey += entityKeys[i].name + '=' + formatLiteral(entityKeys[i]);
                    }
                }

                entityInstanceKey += ')';

                return entityInstanceKey;
            }
            EncodingHelpers.getKeyExpression = getKeyExpression;

            function formatLiteral(literal) {
                /// <summary>Formats a value according to Uri literal format</summary>
                /// <param name="value">Value to be formatted.</param>
                /// <param name="type">Edm type of the value</param>
                /// <returns type="string">Value after formatting</returns>
                var result = "" + formatRowLiteral(literal.value, literal.type);

                result = encodeURIComponent(result.replace("'", "''"));

                switch ((literal.type)) {
                    case "Edm.Binary":
                        return "X'" + result + "'";
                    case "Edm.DateTime":
                        return "datetime" + "'" + result + "'";
                    case "Edm.DateTimeOffset":
                        return "datetimeoffset" + "'" + result + "'";
                    case "Edm.Decimal":
                        return result + "M";
                    case "Edm.Guid":
                        return "guid" + "'" + result + "'";
                    case "Edm.Int64":
                        return result + "L";
                    case "Edm.Float":
                        return result + "f";
                    case "Edm.Double":
                        return result + "D";
                    case "Edm.Geography":
                        return "geography" + "'" + result + "'";
                    case "Edm.Geometry":
                        return "geometry" + "'" + result + "'";
                    case "Edm.Time":
                        return "time" + "'" + result + "'";
                    case "Edm.String":
                        return "'" + result + "'";
                    default:
                        return result;
                }
            }
            EncodingHelpers.formatLiteral = formatLiteral;

            function formatRowLiteral(value, type) {
                switch (type) {
                    case "Edm.Binary":
                        return Microsoft.Utility.decodeBase64AsHexString(value);
                    default:
                        return value;
                }
            }
        })(Utility.EncodingHelpers || (Utility.EncodingHelpers = {}));
        var EncodingHelpers = Utility.EncodingHelpers;

        function findProperties(o) {
            var aPropertiesAndMethods = [];

            do {
                aPropertiesAndMethods = aPropertiesAndMethods.concat(Object.getOwnPropertyNames(o));
            } while(o = Object.getPrototypeOf(o));

            for (var a = 0; a < aPropertiesAndMethods.length; ++a) {
                for (var b = a + 1; b < aPropertiesAndMethods.length; ++b) {
                    if (aPropertiesAndMethods[a] === aPropertiesAndMethods[b]) {
                        aPropertiesAndMethods.splice(a--, 1);
                    }
                }
            }

            return aPropertiesAndMethods;
        }
        Utility.findProperties = findProperties;

        function decodeBase64AsHexString(base64) {
            var decoded = decodeBase64(base64), hexValue = "", hexValues = "0123456789ABCDEF";

            for (var j = 0; j < decoded.length; j++) {
                var byte = decoded[j];
                hexValue += hexValues[byte >> 4];
                hexValue += hexValues[byte & 0x0F];
            }

            return hexValue;
        }
        Utility.decodeBase64AsHexString = decodeBase64AsHexString;

        function decodeBase64(base64) {
            var decoded = [];

            if (window.atob !== undefined) {
                var binaryStr = window.atob(base64);
                for (var i = 0; i < binaryStr.length; i++) {
                    decoded.push(binaryStr.charCodeAt(i));
                }
                return decoded;
            }

            for (var index = 0; index < base64.length; index += 4) {
                var sextet1 = getBase64Sextet(base64[index]);
                var sextet2 = getBase64Sextet(base64[index + 1]);
                var sextet3 = (index + 2 < base64.length) ? getBase64Sextet(base64[index + 2]) : null;
                var sextet4 = (index + 3 < base64.length) ? getBase64Sextet(base64[index + 3]) : null;
                decoded.push((sextet1 << 2) | (sextet2 >> 4));
                if (sextet3)
                    decoded.push(((sextet2 & 0xF) << 4) | (sextet3 >> 2));
                if (sextet4)
                    decoded.push(((sextet3 & 0x3) << 6) | sextet4);
            }

            return decoded;
        }
        Utility.decodeBase64 = decodeBase64;

        function decodeBase64AsString(base64) {
            var decoded = decodeBase64(base64), decoded_string;

            decoded.forEach(function (value, index, decoded_access_token) {
                if (!decoded_string) {
                    decoded_string = String.fromCharCode(value);
                } else {
                    decoded_string += String.fromCharCode(value);
                }
            });

            return decoded_string;
        }
        Utility.decodeBase64AsString = decodeBase64AsString;

        function getBase64Sextet(character) {
            var code = character.charCodeAt(0);

            if (code >= 65 && code <= 90)
                return code - 65;

            if (code >= 97 && code <= 122)
                return code - 71;

            if (code >= 48 && code <= 57)
                return code + 4;

            if (character === "+")
                return 62;

            if (character === "/")
                return 63;

            return null;
        }

        var Exception = (function () {
            function Exception(message, innerException) {
                this._message = message;
                if (innerException) {
                    this._innerException = innerException;
                }
            }
            Object.defineProperty(Exception.prototype, "message", {
                get: function () {
                    return this._message;
                },
                enumerable: true,
                configurable: true
            });

            Object.defineProperty(Exception.prototype, "innerException", {
                get: function () {
                    return this._innerException;
                },
                enumerable: true,
                configurable: true
            });
            return Exception;
        })();
        Utility.Exception = Exception;

        var HttpException = (function (_super) {
            __extends(HttpException, _super);
            function HttpException(XHR, innerException) {
                _super.call(this, XHR.statusText, innerException);
                this.getHeaders = this.getHeadersFn(XHR);
            }
            HttpException.prototype.getHeadersFn = function (xhr) {
                return function (headerName) {
                    if (headerName && headerName.length > 0) {
                        return xhr.getResponseHeader(headerName);
                    } else {
                        return xhr.getAllResponseHeaders();
                    }
                    ;
                };
            };

            Object.defineProperty(HttpException.prototype, "xhr", {
                get: function () {
                    return this._xhr;
                },
                enumerable: true,
                configurable: true
            });
            return HttpException;
        })(Exception);
        Utility.HttpException = HttpException;

        var DeferredState;
        (function (DeferredState) {
            DeferredState[DeferredState["UNFULFILLED"] = 0] = "UNFULFILLED";
            DeferredState[DeferredState["RESOLVED"] = 1] = "RESOLVED";
            DeferredState[DeferredState["REJECTED"] = 2] = "REJECTED";
        })(DeferredState || (DeferredState = {}));

        var Deferred = (function () {
            function Deferred() {
                this._fulfilled = function () {
                };
                this._rejected = function () {
                };
                this._progress = function () {
                };
                this._state = 0 /* UNFULFILLED */;
            }
            Deferred.prototype.then = function (onFulfilled, onRejected, onProgress) {
                this._deferred = new Deferred();
                var that = this;

                if (onFulfilled && typeof onFulfilled === 'function') {
                    this._fulfilled = function (value) {
                        var result;
                        try {
                            result = onFulfilled(value);
                        } catch (err) {
                            that._deferred.reject(err);
                            return;
                        }

                        if (result instanceof Deferred) {
                            result.then(function (res) {
                                that._deferred.resolve(res);
                            }, function (err) {
                                that._deferred.reject(err);
                            });
                        } else {
                            that._deferred.resolve(result);
                        }
                    };
                }

                if (onRejected && typeof onRejected === 'function') {
                    this._rejected = function (reason) {
                        var result;
                        try {
                            result = onRejected(reason);
                        } catch (err) {
                            that._deferred.reject(err);
                            return;
                        }

                        if (result instanceof Deferred) {
                            result.then(function (res) {
                                that._deferred.resolve(res);
                            }, function (err) {
                                that._deferred.reject(err);
                            });
                        } else {
                            that._deferred.reject(result);
                        }
                    };
                }

                if (onProgress && typeof onProgress === 'function') {
                    this._progress = function (progress) {
                        var result;
                        try {
                            result = onProgress(progress);
                        } catch (err) {
                            that._deferred.reject(err);
                            return;
                        }

                        if (result instanceof Deferred) {
                            result.then(function (res) {
                                that._deferred.notify(res);
                            }, function (err) {
                                that._deferred.reject(err);
                            });
                        } else {
                            that._deferred.notify(result);
                        }
                    };
                }

                switch (this._state) {
                    case 0 /* UNFULFILLED */:
                        break;
                    case 1 /* RESOLVED */:
                        this._fulfilled(this._value);
                        break;
                    case 2 /* REJECTED */:
                        this._rejected(this._reason);
                        break;
                }

                return this._deferred;
            };

            Deferred.prototype.detach = function () {
                this._fulfilled = function () {
                };
                this._rejected = function () {
                };
                this._progress = function () {
                };
            };

            Deferred.prototype.resolve = function (value) {
                if (this._state !== 0 /* UNFULFILLED */) {
                    throw new Microsoft.Utility.Exception("Invalid deferred state = " + this._state);
                }
                this._value = value;
                var fulfilled = this._fulfilled;
                this.detach();
                this._state = 1 /* RESOLVED */;
                fulfilled(value);
            };

            Deferred.prototype.reject = function (reason) {
                if (this._state !== 0 /* UNFULFILLED */) {
                    throw new Microsoft.Utility.Exception("Invalid deferred state = " + this._state);
                }
                this._reason = reason;
                var rejected = this._rejected;
                this.detach();
                this._state = 2 /* REJECTED */;
                rejected(reason);
            };

            Deferred.prototype.notify = function (progress) {
                if (this._state !== 0 /* UNFULFILLED */) {
                    throw new Microsoft.Utility.Exception("Invalid deferred state = " + this._state);
                }
                this._progress(progress);
            };

            return Deferred;
        })();
        Utility.Deferred = Deferred;

        (function (HttpHelpers) {
            var Request = (function () {
                function Request(requestUri, method, data) {
                    this.requestUri = requestUri;
                    this.method = method;
                    this.data = data;
                    this.headers = {};
                    this.disableCache = false;
                }
                return Request;
            })();
            HttpHelpers.Request = Request;

            var AuthenticatedHttp = (function () {
                function AuthenticatedHttp(getAccessTokenFn) {
                    this._disableCache = false;
                    this._noCache = Date.now();
                    this._accept = 'application/json;q=0.9, */*;q=0.1';
                    this._contentType = 'application/json';
                    this._getAccessTokenFn = getAccessTokenFn;
                }
                Object.defineProperty(AuthenticatedHttp.prototype, "disableCache", {
                    get: function () {
                        return this._disableCache;
                    },
                    set: function (value) {
                        this._disableCache = value;
                    },
                    enumerable: true,
                    configurable: true
                });


                Object.defineProperty(AuthenticatedHttp.prototype, "accept", {
                    get: function () {
                        return this._accept;
                    },
                    set: function (value) {
                        this._accept = value;
                    },
                    enumerable: true,
                    configurable: true
                });


                Object.defineProperty(AuthenticatedHttp.prototype, "contentType", {
                    get: function () {
                        return this._contentType;
                    },
                    set: function (value) {
                        this._contentType = value;
                    },
                    enumerable: true,
                    configurable: true
                });


                AuthenticatedHttp.prototype.ajax = function (request) {
                    var deferred = new Microsoft.Utility.Deferred();

                    var xhr = new XMLHttpRequest();

                    if (!request.method) {
                        request.method = 'GET';
                    }

                    xhr.open(request.method.toUpperCase(), request.requestUri, true);

                    if (request.headers) {
                        for (name in request.headers) {
                            xhr.setRequestHeader(name, request.headers[name]);
                        }
                    }

                    xhr.onreadystatechange = function (e) {
                        if (xhr.readyState == 4) {
                            if (xhr.status >= 200 && xhr.status < 300 || xhr.status === 304) {
                                deferred.resolve(xhr.responseText);
                            } else {
                                deferred.reject(xhr);
                            }
                        } else {
                            deferred.notify(xhr.readyState);
                        }
                    };

                    if (request.data) {
                        if (typeof request.data === 'string') {
                            xhr.send(request.data);
                        } else {
                            xhr.send(JSON.stringify(request.data));
                        }
                    } else {
                        xhr.send();
                    }

                    return deferred;
                };

                AuthenticatedHttp.prototype.getUrl = function (url) {
                    return this.request(new Request(url));
                };

                AuthenticatedHttp.prototype.postUrl = function (url, data) {
                    return this.request(new Request(url, 'POST', data));
                };

                AuthenticatedHttp.prototype.deleteUrl = function (url) {
                    return this.request(new Request(url, 'DELETE'));
                };

                AuthenticatedHttp.prototype.patchUrl = function (url, data) {
                    return this.request(new Request(url, 'PATCH', data));
                };

                AuthenticatedHttp.prototype.request = function (request) {
                    var _this = this;
                    var deferred;

                    this.augmentRequest(request);

                    if (this._getAccessTokenFn) {
                        deferred = new Microsoft.Utility.Deferred();

                        this._getAccessTokenFn().then((function (token) {
                            request.headers["Authorization"] = 'Bearer ' + token;
                            _this.ajax(request).then(deferred.resolve, deferred.reject);
                        }).bind(this), deferred.reject);
                    } else {
                        deferred = this.ajax(request);
                    }

                    return deferred;
                };

                AuthenticatedHttp.prototype.augmentRequest = function (request) {
                    if (!request.headers) {
                        request.headers = {};
                    }

                    if (!request.headers['Accept']) {
                        request.headers['Accept'] = this._accept;
                    }

                    if (!request.headers['Content-Type']) {
                        request.headers['Content-Type'] = this._contentType;
                    }

                    if (request.disableCache || this._disableCache) {
                        request.requestUri += (request.requestUri.indexOf('?') >= 0 ? '&' : '?') + '_=' + this._noCache++;
                    }
                };
                return AuthenticatedHttp;
            })();
            HttpHelpers.AuthenticatedHttp = AuthenticatedHttp;
        })(Utility.HttpHelpers || (Utility.HttpHelpers = {}));
        var HttpHelpers = Utility.HttpHelpers;
    })(Microsoft.Utility || (Microsoft.Utility = {}));
    var Utility = Microsoft.Utility;
})(module.exports);

/**
 * Pads a string at the right to specified length with specified string
 *
 * @param  {String} str Input string to be padded
 *
 * @param  {Number} n   Resulting length
 *
 * @param  {String} pad String to pad with
 *
 * @return {String}     Right-padded string
 */
function padRight(str, n, pad) {
    var temp = str;

    if (n > str.length) {
        for (var i = 0; i < n - str.length; i++) {
            temp += pad;
        }
    }

    return temp;
}

/**
 * Converts Base64URL to Base64 encoded string
 *
 * @param  {String} jwt Base64URL encoded string
 *
 * @return {String}     Base64 encoded string with applied '=' right padding
 */
function base64UrlToBase64(b64Url) {
    b64Url = padRight(b64Url, b64Url.length + (4 - b64Url.length % 4) % 4, '=');
    return b64Url.replace(/-/g, '+').replace(/_/g, '/');
}

/**
 * Parses a valid JWT token into JSON representation.
 * This method doesn't validate/encode token.
 *
 * @param  {String} jwt Raw JWT token string
 *
 * @return {Object}     Raw object that contains data from token
 */
function parseJWT (jwt) {

    var jwtParseError = new Error("Error parsing JWT token.");

    var jwtParts = jwt.split('.');
    if (jwtParts.length !== 3) {
        throw jwtParseError;
    }

    var jwtBody = jwtParts[1];
    jwtBody = base64UrlToBase64(jwtBody);

    try {
        return JSON.parse(window.atob(jwtBody));
    } catch (e) {
        throw jwtParseError;
    }
}

module.exports.extends = __extends;

module.exports.parseJWT = parseJWT;
