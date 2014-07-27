/* Office Web Widgets - Experimental */
/* Version: 0.1 */
/*
	Copyright (c) Microsoft Corporation.  All rights reserved.
*/

/*
	Your use of this file is governed by the following license http://go.microsoft.com/fwlink/?LinkId=392925
*/
if (window.Type && window.Type.registerNamespace) {
Type.registerNamespace('Office.Controls');} else {
if(typeof(window['Office']) == 'undefined') {
window['Office'] = new Object(); window['Office']. __namespace = true;
}
if(typeof(window['Office']['Controls']) == 'undefined') {
window['Office']['Controls'] = new Object(); window['Office']['Controls']. __namespace = true;
}

}


Office.Controls.Context = function Office_Controls_Context(parameterObject) {
    if (typeof(parameterObject) !== 'object') {
        Office.Controls.Utils.errorConsole('Invalid parameters type');
        return;
    }
    var sharepointHost = parameterObject[Office.Controls.Context._sharepointHostUrlFieldName$p];
    if (Office.Controls.Utils.isNullOrUndefined(sharepointHost)) {
        var param = Office.Controls.Utils.getQueryStringParameter(Office.Controls.Context._sharepointHostUrlQueryParameter$p);
        if (!Office.Controls.Utils.isNullOrEmptyString(param)) {
            param = decodeURIComponent(param);
        }
        this.sharePointHostUrl = param;
    }
    else {
        this.sharePointHostUrl = sharepointHost;
    }
    this.sharePointHostUrl = this.sharePointHostUrl.toLocaleLowerCase();
    var appWeb = parameterObject[Office.Controls.Context._appWebUrlFieldName$p];
    if (Office.Controls.Utils.isNullOrUndefined(appWeb)) {
        var param = Office.Controls.Utils.getQueryStringParameter(Office.Controls.Context._appWebUrlQueryParameter$p);
        if (!Office.Controls.Utils.isNullOrEmptyString(param)) {
            param = decodeURIComponent(param);
        }
        this.appWebUrl = param;
    }
    else {
        this.appWebUrl = appWeb;
    }
    this.appWebUrl = this.appWebUrl.toLocaleLowerCase();
    this.requestViaUrl = parameterObject[Office.Controls.Context._viaURLFieldName$p];
}
Office.Controls.Context.prototype = {
    _re$p$0: null,
    sharePointHostUrl: null,
    appWebUrl: null,
    requestViaUrl: null,
    
    getRequestExecutor: function Office_Controls_Context$getRequestExecutor() {
        if (!this._re$p$0) {
            if (!Office.Controls.Utils.isNullOrEmptyString(Office.Controls.Runtime.context.appWebUrl)) {
                if (!Office.Controls.Utils.isNullOrEmptyString(Office.Controls.Runtime.context.requestViaUrl)) {
                    var options = new SP.RequestExecutorOptions();
                    options.viaUrl = Office.Controls.Runtime.context.requestViaUrl;
                    this._re$p$0 = new SP.RequestExecutor(Office.Controls.Runtime.context.sharePointHostUrl, options);
                }
                else {
                    this._re$p$0 = new SP.RequestExecutor(Office.Controls.Runtime.context.appWebUrl);
                }
            }
            else {
                Office.Controls.Utils.errorConsole('Missing authentication informations.');
            }
        }
        return this._re$p$0;
    }
}


Office.Controls.Runtime = function Office_Controls_Runtime() {
}
Office.Controls.Runtime._registerControl = function Office_Controls_Runtime$_registerControl(root, control) {
    root[Office.Controls.Runtime._controlPropertyName$p] = control;
}
Office.Controls.Runtime.getControl = function Office_Controls_Runtime$getControl(root) {
    var control = root[Office.Controls.Runtime._controlPropertyName$p];
    return control;
}
Office.Controls.Runtime.initialize = function Office_Controls_Runtime$initialize(parameterObject) {
    Office.Controls.Runtime.context = new Office.Controls.Context(parameterObject);
}
Office.Controls.Runtime.renderAll = function Office_Controls_Runtime$renderAll() {
    var controlRoots = document.querySelectorAll(Office.Controls.Runtime._controlSelector$p);
    for (var i = 0; i < controlRoots.length; i++) {
        var controlRootElement = controlRoots[i];
        var optionsAttribute = controlRootElement.attributes.getNamedItem(Office.Controls.Runtime._optionAttributeName$p);
        var options;
        try {
            if (optionsAttribute) {
                options = eval('(' + optionsAttribute.value + ')');
            }
            else {
                options = {};
            }
        }
        catch (e) {
            Office.Controls.Utils.errorConsole('Error while deserializing options :' + e.message);
            return;
        }
        var controlNameAttribute = controlRootElement.attributes.getNamedItem(Office.Controls.Runtime._controlAttributeName$p);
        var controlToCreate = Office.Controls.Utils._getObjectFromFullyQualifiedName$i(controlNameAttribute.value);
        controlToCreate[Office.Controls.Runtime._creationMethodName$p](controlRootElement, options);
    }
}


Office.Controls.Utils = function Office_Controls_Utils() {
}
Office.Controls.Utils.deserializeJSON = function Office_Controls_Utils$deserializeJSON(data) {
    if (Office.Controls.Utils.isNullOrEmptyString(data)) {
        return {};
    }
    else {
        return JSON.parse(data);
    }
}
Office.Controls.Utils.serializeJSON = function Office_Controls_Utils$serializeJSON(obj) {
    return JSON.stringify(obj);
}
Office.Controls.Utils.isNullOrEmptyString = function Office_Controls_Utils$isNullOrEmptyString(str) {
    var strNull = null;
    return str === strNull || typeof(str) === 'undefined' || !str.length;
}
Office.Controls.Utils.isNullOrUndefined = function Office_Controls_Utils$isNullOrUndefined(obj) {
    var objNull = null;
    return obj === objNull || typeof(obj) === 'undefined';
}
Office.Controls.Utils.getQueryStringParameter = function Office_Controls_Utils$getQueryStringParameter(paramToRetrieve) {
    if (document.URL.split('?').length < 2) {
        return null;
    }
    var queryParameters = document.URL.split('?')[1].split('#')[0].split('&');
    for (var i = 0; i < queryParameters.length; i = i + 1) {
        var singleParam = queryParameters[i].split('=');
        if (singleParam[0].toLowerCase() === paramToRetrieve.toLowerCase()) {
            return singleParam[1];
        }
    }
    return null;
}
Office.Controls.Utils.logConsole = function Office_Controls_Utils$logConsole(message) {
    console.log(message);
}
Office.Controls.Utils.warnConsole = function Office_Controls_Utils$warnConsole(message) {
    console.warn(message);
}
Office.Controls.Utils.errorConsole = function Office_Controls_Utils$errorConsole(message) {
    console.error(message);
}
Office.Controls.Utils._getObjectFromFullyQualifiedName$i = function Office_Controls_Utils$_getObjectFromFullyQualifiedName$i(objectName) {
    var currentObject = window.self;
    var controlNameParts = objectName.split('.');
    for (var i = 0; i < controlNameParts.length; i++) {
        currentObject = currentObject[controlNameParts[i]];
        if (Office.Controls.Utils.isNullOrUndefined(currentObject)) {
            return null;
        }
    }
    return currentObject;
}
Office.Controls.Utils.getStringFromResource = function Office_Controls_Utils$getStringFromResource(controlName, stringName) {
    var resourceObjectName = Office.Controls.Utils._namespace$p + '.' + controlName + 'Resources';
    var res;
    var nonPreserveCase = stringName.charAt(0).toString().toLowerCase() + stringName.substr(1);
    res = SP.RuntimeRes;
    var str;
    if (!Office.Controls.Utils.isNullOrUndefined(res)) {
        str = res[nonPreserveCase];
        if (!Office.Controls.Utils.isNullOrEmptyString(str)) {
            return str;
        }
    }
    resourceObjectName += 'Defaults';
    res = Office.Controls.Utils._getObjectFromFullyQualifiedName$i(resourceObjectName);
    if (!Office.Controls.Utils.isNullOrUndefined(res)) {
        return res[stringName];
    }
    return stringName;
}
Office.Controls.Utils._getUrlDomainPart$i = function Office_Controls_Utils$_getUrlDomainPart$i(fullUrl) {
    var index = fullUrl.indexOf('://');
    index = fullUrl.indexOf('/', index + 3);
    if (index > 0) {
        fullUrl = fullUrl.substr(0, index);
    }
    if (fullUrl.substr(0, 'https://'.length).toLowerCase() === 'https://' && fullUrl.substr(fullUrl.length - 4, 4) === ':443') {
        fullUrl = fullUrl.substr(0, fullUrl.length - 4);
    }
    else if (fullUrl.substr(0, 'http://'.length).toLowerCase() === 'http://' && fullUrl.substr(fullUrl.length - 3, 3) === ':80') {
        fullUrl = fullUrl.substr(0, fullUrl.length - 3);
    }
    return fullUrl;
}
Office.Controls.Utils._getUrlAuthorityPart$i = function Office_Controls_Utils$_getUrlAuthorityPart$i(fullUrl) {
    fullUrl = Office.Controls.Utils._getUrlDomainPart$i(fullUrl);
    if (fullUrl.substr(0, 'https://'.length).toLowerCase() === 'https://') {
        fullUrl = fullUrl.substr('https://'.length);
    }
    else if (fullUrl.substr(0, 'http://'.length).toLowerCase() === 'http://') {
        fullUrl = fullUrl.substr('http://'.length);
    }
    return fullUrl;
}
Office.Controls.Utils.addEventListener = function Office_Controls_Utils$addEventListener(element, eventName, handler) {
    if (!Office.Controls.Utils.isNullOrUndefined(element.addEventListener)) {
        element.addEventListener(eventName, handler, false);
    }
    else if (!Office.Controls.Utils.isNullOrUndefined(element.attachEvent)) {
        element.attachEvent('on' + eventName, handler);
    }
}
Office.Controls.Utils.getEvent = function Office_Controls_Utils$getEvent(e) {
    return (Office.Controls.Utils.isNullOrUndefined(e)) ? window.event : e;
}
Office.Controls.Utils.getTarget = function Office_Controls_Utils$getTarget(e) {
    return (Office.Controls.Utils.isNullOrUndefined(e.target)) ? e.srcElement : e.target;
}
Office.Controls.Utils.cancelEvent = function Office_Controls_Utils$cancelEvent(e) {
    if (!Office.Controls.Utils.isNullOrUndefined(e.cancelBubble)) {
        e.cancelBubble = true;
    }
    if (!Office.Controls.Utils.isNullOrUndefined(e.stopPropagation)) {
        e.stopPropagation();
    }
    if (!Office.Controls.Utils.isNullOrUndefined(e.preventDefault)) {
        e.preventDefault();
    }
    if (!Office.Controls.Utils.isNullOrUndefined(e.returnValue)) {
        e.returnValue = false;
    }
    if (!Office.Controls.Utils.isNullOrUndefined(e.cancel)) {
        e.cancel = true;
    }
}
Office.Controls.Utils.addClass = function Office_Controls_Utils$addClass(elem, className) {
    if (elem.className !== '') {
        elem.className += ' ';
    }
    elem.className += className;
}
Office.Controls.Utils.removeClass = function Office_Controls_Utils$removeClass(elem, className) {
    var regex = new RegExp('( |^)' + className + '( |$)');
    elem.className = elem.className.replace(regex, ' ').trim();
}
Office.Controls.Utils.containClass = function Office_Controls_Utils$containClass(elem, className) {
    return elem.className.indexOf(className) !== -1;
}
Office.Controls.Utils.cloneData = function Office_Controls_Utils$cloneData(obj) {
    return Office.Controls.Utils.deserializeJSON(Office.Controls.Utils.serializeJSON(obj));
}
Office.Controls.Utils.formatString = function Office_Controls_Utils$formatString(format) {
    var args = [];
    for (var $$pai_8 = 1; $$pai_8 < arguments.length; ++$$pai_8) {
        args[$$pai_8 - 1] = arguments[$$pai_8];
    }
    var result = '';
    var i = 0;
    while (i < format.length) {
        var open = Office.Controls.Utils._findPlaceHolder$p(format, i, '{');
        if (open < 0) {
            result = result + format.substr(i);
            break;
        }
        else {
            var close = Office.Controls.Utils._findPlaceHolder$p(format, open, '}');
            if (close > open) {
                result = result + format.substr(i, open - i);
                var position = format.substr(open + 1, close - open - 1);
                var pos = parseInt(position);
                result = result + args[pos];
                i = close + 1;
            }
            else {
                Office.Controls.Utils.errorConsole('Invalid Operation');
                return null;
            }
        }
    }
    return result;
}
Office.Controls.Utils._findPlaceHolder$p = function Office_Controls_Utils$_findPlaceHolder$p(format, start, ch) {
    var index = format.indexOf(ch, start);
    while (index >= 0 && index < format.length - 1 && format.charAt(index + 1) === ch) {
        start = index + 2;
        index = format.indexOf(ch, start);
    }
    return index;
}
Office.Controls.Utils.htmlEncode = function Office_Controls_Utils$htmlEncode(value) {
    value = value.replace(new RegExp('&', 'g'), '&amp;');
    value = value.replace(new RegExp('\"', 'g'), '&quot;');
    value = value.replace(new RegExp('\'', 'g'), '&#39;');
    value = value.replace(new RegExp('<', 'g'), '&lt;');
    value = value.replace(new RegExp('>', 'g'), '&gt;');
    return value;
}
Office.Controls.Utils.getLocalizedCountValue = function Office_Controls_Utils$getLocalizedCountValue(locText, intervals, count) {
    var ret = '';
    var locIndex = -1;
    var intervalsArray = intervals.split('||');
    for (var i = 0, lenght = intervalsArray.length; i < lenght; i++) {
        var interval = intervalsArray[i];
        if (Office.Controls.Utils.isNullOrEmptyString(interval)) {
            continue;
        }
        var subIntervalsArray = interval.split(',');
        for (var k = 0, subLenght = subIntervalsArray.length; k < subLenght; k++) {
            var subInterval = subIntervalsArray[k];
            if (Office.Controls.Utils.isNullOrEmptyString(subInterval)) {
                continue;
            }
            if (isNaN(Number(subInterval))) {
                var range = subInterval.split('-');
                if (Office.Controls.Utils.isNullOrUndefined(range) || range.length !== 2) {
                    continue;
                }
                var min;
                var max;
                if (range[0] === '') {
                    min = 0;
                }
                else {
                    if (isNaN(Number(range[0]))) {
                        continue;
                    }
                    else {
                        min = parseInt(range[0]);
                    }
                }
                if (count >= min) {
                    if (range[1] === '') {
                        locIndex = i;
                        break;
                    }
                    else {
                        if (isNaN(Number(range[1]))) {
                            continue;
                        }
                        else {
                            max = parseInt(range[1]);
                        }
                    }
                    if (count <= max) {
                        locIndex = i;
                        break;
                    }
                }
            }
            else {
                var exactNumber = parseInt(subInterval);
                if (count === exactNumber) {
                    locIndex = i;
                    break;
                }
            }
        }
        if (locIndex !== -1) {
            break;
        }
    }
    var locValues = locText.split('||');
    if (locIndex !== -1) {
        ret = locValues[locIndex];
    }
    return ret;
}
Office.Controls.Utils.NOP = function Office_Controls_Utils$NOP() {
}


if (Office.Controls.Context.registerClass) Office.Controls.Context.registerClass('Office.Controls.Context');
if (Office.Controls.Runtime.registerClass) Office.Controls.Runtime.registerClass('Office.Controls.Runtime');
if (Office.Controls.Utils.registerClass) Office.Controls.Utils.registerClass('Office.Controls.Utils');
Office.Controls.Context._sharepointHostUrlFieldName$p = 'sharePointHostUrl';
Office.Controls.Context._sharepointHostUrlQueryParameter$p = 'SPHostUrl';
Office.Controls.Context._appWebUrlFieldName$p = 'appWebUrl';
Office.Controls.Context._appWebUrlQueryParameter$p = 'SPAppWebUrl';
Office.Controls.Context._viaURLFieldName$p = 'requestsViaUrl';
Office.Controls.Runtime._controlAttributeName$p = 'data-office-control';
Office.Controls.Runtime._optionAttributeName$p = 'data-office-options';
Office.Controls.Runtime._creationMethodName$p = 'create';
Office.Controls.Runtime._controlPropertyName$p = '_officeControl';
Office.Controls.Runtime._controlSelector$p = '[data-office-control^=\"Office.Controls.\"]';
Office.Controls.Runtime.context = null;
Office.Controls.Utils.oDataJSONAcceptString = 'application/json;odata=verbose';
Office.Controls.Utils.clientTagHeaderName = 'X-ClientService-ClientTag';
Office.Controls.Utils._namespace$p = 'Office.Controls';
