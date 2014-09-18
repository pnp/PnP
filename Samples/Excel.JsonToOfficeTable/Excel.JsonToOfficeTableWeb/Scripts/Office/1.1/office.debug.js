/* Office JavaScript API library */
/* Version: 16.0.2420.1000 */
/*
	Copyright (c) Microsoft Corporation.  All rights reserved.
*/

/*
	Your use of this file is governed by the Microsoft Services Agreement http://go.microsoft.com/fwlink/?LinkId=266419.
*/

var OSF = OSF || {
};
OSF.HostSpecificFileVersion = "16.00";
OSF.ConstantNames = {
    HostSpecificFallbackVersion: OSF.HostSpecificFileVersion,
    OfficeJS: "office.js",
    OfficeDebugJS: "office.debug.js",
    DefaultLocale: "en-us",
    LocaleStringLoadingTimeout: 2000,
    OfficeStringJS: "office_strings.debug.js",
    O15InitHelper: "o15apptofilemappingtable.debug.js"
};
OSF.InitializationHelper = function OSF_InitializationHelper(hostInfo, webAppState, context, settings, hostFacade) {
    this._hostInfo = hostInfo;
    this._webAppState = webAppState;
    this._context = context;
    this._settings = settings;
    this._hostFacade = hostFacade;
};
OSF.InitializationHelper.prototype.getAppContext = function OSF_InitializationHelper$getAppContext(wnd, gotAppContext) {
};
OSF.InitializationHelper.prototype.setAgaveHostCommunication = function OSF_InitializationHelper$setAgaveHostCommunication() {
};
OSF.InitializationHelper.prototype.prepareRightBeforeWebExtensionInitialize = function OSF_InitializationHelper$prepareRightBeforeWebExtensionInitialize(appContext) {
};
OSF.InitializationHelper.prototype.loadAppSpecificScriptAndCreateOM = function OSF_InitializationHelper$loadAppSpecificScriptAndCreateOM(appContext, appReady, basePath) {
};
OSF._OfficeAppFactory = (function OSF__OfficeAppFactory() {
    var _setNamespace = function OSF_OUtil$_setNamespace(name, parent) {
        if(parent && name && !parent[name]) {
            parent[name] = {
            };
        }
    };
    _setNamespace("Office", window);
    _setNamespace("Microsoft", window);
    _setNamespace("Office", Microsoft);
    _setNamespace("WebExtension", Microsoft.Office);
    window.Office = Microsoft.Office.WebExtension;
    var _context = {
    };
    var _settings = {
    };
    var _hostFacade = {
    };
    var _WebAppState = {
        id: null,
        webAppUrl: null,
        conversationID: null,
        clientEndPoint: null,
        wnd: window.parent,
        focused: false
    };
    var _hostInfo = {
        isO15: true,
        isRichClient: true,
        hostType: "",
        hostPlatform: "",
        hostSpecificFileVersion: ""
    };
    var _initializationHelper = {
    };
    var _parseHostInfo = function OSF__OfficeAppFactory$_parseHostInfo() {
        var hostInfoValue;
        var hostInfo = "_host_Info=";
        var searchString = window.location.search;
        if(searchString) {
            var hostInfoParts = searchString.split(hostInfo);
            if(hostInfoParts.length > 1) {
                var hostInfoValueRestString = hostInfoParts[1];
                var separatorRegex = new RegExp("/[&#]/g");
                var hostInfoValueParts = hostInfoValueRestString.split(separatorRegex);
                if(hostInfoValueParts.length > 0) {
                    hostInfoValue = hostInfoValueParts[0];
                }
            }
        }
        return hostInfoValue;
    };
    var _loadScript = function OSF_OUtil$_loadScript(url, callback, timeoutInMs) {
        var loadedScripts = {
        };
        var defaultScriptLoadingTimeout = 30000;
        if(url && callback) {
            var doc = window.document;
            var loadedScriptEntry = loadedScripts[url];
            if(!loadedScriptEntry) {
                var script = doc.createElement("script");
                script.type = "text/javascript";
                loadedScriptEntry = {
                    loaded: false,
                    pendingCallbacks: [
                        callback
                    ],
                    timer: null
                };
                loadedScripts[url] = loadedScriptEntry;
                var onLoadCallback = function OSF_OUtil_loadScript$onLoadCallback() {
                    if(loadedScriptEntry.timer != null) {
                        clearTimeout(loadedScriptEntry.timer);
                        delete loadedScriptEntry.timer;
                    }
                    loadedScriptEntry.loaded = true;
                    var pendingCallbackCount = loadedScriptEntry.pendingCallbacks.length;
                    for(var i = 0; i < pendingCallbackCount; i++) {
                        var currentCallback = loadedScriptEntry.pendingCallbacks.shift();
                        currentCallback();
                    }
                };
                var onLoadError = function OSF_OUtil_loadScript$onLoadError() {
                    delete loadedScripts[url];
                    if(loadedScriptEntry.timer != null) {
                        clearTimeout(loadedScriptEntry.timer);
                        delete loadedScriptEntry.timer;
                    }
                    var pendingCallbackCount = loadedScriptEntry.pendingCallbacks.length;
                    for(var i = 0; i < pendingCallbackCount; i++) {
                        var currentCallback = loadedScriptEntry.pendingCallbacks.shift();
                        currentCallback();
                    }
                };
                if(script.readyState) {
                    script.onreadystatechange = function () {
                        if(script.readyState == "loaded" || script.readyState == "complete") {
                            script.onreadystatechange = null;
                            onLoadCallback();
                        }
                    };
                } else {
                    script.onload = onLoadCallback;
                }
                script.onerror = onLoadError;
                timeoutInMs = timeoutInMs || defaultScriptLoadingTimeout;
                loadedScriptEntry.timer = setTimeout(onLoadError, timeoutInMs);
                script.src = url;
                doc.getElementsByTagName("head")[0].appendChild(script);
            } else if(loadedScriptEntry.loaded) {
                callback();
            } else {
                loadedScriptEntry.pendingCallbacks.push(callback);
            }
        }
    };
    var _retrieveHostInfo = function OSF__OfficeAppFactory$_retrieveHostInfo() {
        var hostInfoValue = _parseHostInfo();
        var getSessionStorage = function OSF__OfficeAppFactory$_retrieveHostInfo$getSessionStorage() {
            var osfSessionStorage = null;
            try  {
                if(window.sessionStorage) {
                    osfSessionStorage = window.sessionStorage;
                }
            } catch (ex) {
            }
            return osfSessionStorage;
        };
        var osfSessionStorage = getSessionStorage();
        if(!hostInfoValue && osfSessionStorage && osfSessionStorage.getItem("hostInfoValue")) {
            hostInfoValue = osfSessionStorage.getItem("hostInfoValue");
        }
        if(hostInfoValue) {
            _hostInfo.isO15 = false;
            var items = hostInfoValue.split('|');
            _hostInfo.hostType = items[0];
            _hostInfo.hostPlatform = items[1];
            _hostInfo.hostSpecificFileVersion = items[2];
            var hostSpecificFileVersionValue = parseFloat(_hostInfo.hostSpecificFileVersion);
            if(hostSpecificFileVersionValue > OSF.ConstantNames.HostSpecificFallbackVersion) {
                _hostInfo.hostSpecificFileVersion = OSF.ConstantNames.HostSpecificFallbackVersion.toString();
            }
            if(osfSessionStorage) {
                osfSessionStorage.setItem("hostInfoValue", hostInfoValue);
            }
        } else {
            _hostInfo.isO15 = true;
        }
    };
    var getAppContextAsync = function OSF__OfficeAppFactory$getAppContextAsync(wnd, gotAppContext) {
        _initializationHelper.getAppContext(wnd, gotAppContext);
    };
    var initialize = function OSF__OfficeAppFactory$initialize() {
        _retrieveHostInfo();
        var getScriptBase = function OSF__OfficeAppFactory_initialize$getScriptBase(scriptSrc, scriptNameToCheck) {
            var scriptBase, indexOfJS;
            scriptSrc = scriptSrc.toLowerCase();
            scriptNameToCheck = scriptNameToCheck.toLowerCase();
            indexOfJS = scriptSrc.indexOf(scriptNameToCheck);
            if(indexOfJS >= 0 && indexOfJS === (scriptSrc.length - scriptNameToCheck.length) && (indexOfJS === 0 || scriptSrc.charAt(indexOfJS - 1) === '/' || scriptSrc.charAt(indexOfJS - 1) === '\\')) {
                scriptBase = scriptSrc.substring(0, indexOfJS);
            }
            return scriptBase;
        };
        var scripts = document.getElementsByTagName("script") || [];
        var scriptsCount = scripts.length;
        var officeScripts = [
            OSF.ConstantNames.OfficeJS, 
            OSF.ConstantNames.OfficeDebugJS
        ];
        var officeScriptsCount = officeScripts.length;
        var i, j, basePath;
        for(i = 0; !basePath && i < scriptsCount; i++) {
            if(scripts[i].src) {
                for(j = 0; !basePath && j < officeScriptsCount; j++) {
                    basePath = getScriptBase(scripts[i].src, officeScripts[j]);
                }
            }
        }
        if(!basePath) {
            throw "Office Web Extension script library file name should be " + OSF.ConstantNames.OfficeJS + " or " + OSF.ConstantNames.OfficeDebugJS + ".";
        }
        var numberOfTimeForMsAjaxTries = 500;
        var timerId;
        var loadLocaleStringsAndAppSpecificCode = function OSF__OfficeAppFactory_initialize$loadLocaleStringsAndAppSpecificCode() {
            if(typeof (Sys) !== 'undefined' && typeof (Type) !== 'undefined' && Sys.StringBuilder && typeof (Sys.StringBuilder) === "function" && Type.registerNamespace && typeof (Type.registerNamespace) === "function" && Type.registerClass && typeof (Type.registerClass) === "function") {
                _initializationHelper = new OSF.InitializationHelper(_hostInfo, _WebAppState, _context, _settings, _hostFacade);
                _initializationHelper.setAgaveHostCommunication();
                getAppContextAsync(_WebAppState.wnd, function (appContext) {
                    var postLoadLocaleStringInitialization = function OSF__OfficeAppFactory_initialize$postLoadLocaleStringInitialization() {
                        var retryNumber = 100;
                        var t;
                        function appReady() {
                            if(Microsoft.Office.WebExtension.initialize != undefined) {
                                _initializationHelper.prepareRightBeforeWebExtensionInitialize(appContext);
                                if(t != undefined) {
                                    window.clearTimeout(t);
                                }
                            } else if(retryNumber == 0) {
                                clearTimeout(t);
                                throw "Office.js has not been fully loaded yet. Please try again later or make sure to add your initialization code on the Office.initialize function.";
                            } else {
                                retryNumber--;
                                t = window.setTimeout(appReady, 100);
                            }
                        }
                        ;
                        _initializationHelper.loadAppSpecificScriptAndCreateOM(appContext, appReady, basePath);
                    };
                    var fallbackLocaleTried = false;
                    var loadLocaleStringCallback = function OSF__OfficeAppFactory_initialize$loadLocaleStringCallback() {
                        if(typeof Strings == 'undefined' || typeof Strings.OfficeOM == 'undefined') {
                            if(!fallbackLocaleTried) {
                                fallbackLocaleTried = true;
                                var fallbackLocaleStringFile = basePath + OSF.ConstantNames.DefaultLocale + "/" + OSF.ConstantNames.OfficeStringJS;
                                _loadScript(fallbackLocaleStringFile, loadLocaleStringCallback);
                            } else {
                                throw "Neither the locale, " + appContext.get_appUILocale().toLowerCase() + ", provided by the host app nor the fallback locale " + OSF.ConstantNames.DefaultLocale + " are supported.";
                            }
                        } else {
                            fallbackLocaleTried = false;
                            postLoadLocaleStringInitialization();
                        }
                    };
                    var localeStringFile = OSF.OUtil.formatString("{0}{1}/{2}", basePath, appContext.get_appUILocale().toLowerCase(), OSF.ConstantNames.OfficeStringJS);
                    _loadScript(localeStringFile, loadLocaleStringCallback, OSF.ConstantNames.LocaleStringLoadingTimeout);
                });
            } else if(numberOfTimeForMsAjaxTries === 0) {
                clearTimeout(timerId);
                throw "MicrosoftAjax.js is not loaded successfully.";
            } else {
                numberOfTimeForMsAjaxTries--;
                timerId = window.setTimeout(loadLocaleStringsAndAppSpecificCode, 100);
            }
        };
        if(_hostInfo.isO15) {
            _loadScript(basePath + OSF.ConstantNames.O15InitHelper, loadLocaleStringsAndAppSpecificCode);
        } else {
            var hostSpecificFileName;
            hostSpecificFileName = _hostInfo.hostType + "-" + _hostInfo.hostPlatform + "-" + _hostInfo.hostSpecificFileVersion + ".debug.js";
            _loadScript(basePath + hostSpecificFileName.toLowerCase(), loadLocaleStringsAndAppSpecificCode);
        }
        window.confirm = function OSF__OfficeAppFactory_initialize$confirm(message) {
            throw 'Function window.confirm is not supported.';
        };
        window.alert = function OSF__OfficeAppFactory_initialize$alert(message) {
            throw 'Function window.alert is not supported.';
        };
        window.prompt = function OSF__OfficeAppFactory_initialize$prompt(message, defaultvalue) {
            throw 'Function window.prompt is not supported.';
        };
    };
    initialize();
    return {
        getId: function OSF__OfficeAppFactory$getId() {
            return _WebAppState.id;
        },
        getClientEndPoint: function OSF__OfficeAppFactory$getClientEndPoint() {
            return _WebAppState.clientEndPoint;
        },
        getContext: function OSF__OfficeAppFactory$getContext() {
            return _context;
        },
        setContext: function OSF__OfficeAppFactory$setContext(context) {
            _context = context;
        },
        getHostFacade: function OSF__OfficeAppFactory$getHostFacade() {
            return _hostFacade;
        },
        setHostFacade: function setHostFacade(hostFacade) {
            _hostFacade = hostFacade;
        },
        getInitializationHelper: function OSF__OfficeAppFactory$getInitializationHelper() {
            return _initializationHelper;
        },
        getCachedSessionSettingsKey: function OSF__OfficeAppFactory$getCachedSessionSettingsKey() {
            return _WebAppState.conversationID != null ? _WebAppState.conversationID + "CachedSessionSettings" : "NoConversationIdCachedSessionSettings";
        }
    };
})();
