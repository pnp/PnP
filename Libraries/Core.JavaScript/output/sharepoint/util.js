(function (factory) {/* istanbul ignore next */
    if (typeof module === 'object' && typeof module.exports === 'object') {
        var v = factory(require, exports); if (v !== undefined) module.exports = v;
    }
    else if (typeof define === 'function' && define.amd) {
        define(["require", "exports"], factory);
    }
})(function (require, exports) {
    "use strict";
    /// <reference path="..\..\typings\main.d.ts" />
    /**
     * Retrieves the list ID of the current page from _spPageContextInfo
     */
    function getListId() {
        return _spPageContextInfo.hasOwnProperty("pageListId") ? _spPageContextInfo.pageListId.substring(1, 37) : "";
    }
    exports.getListId = getListId;
    /**
     * Make URL relative to host
     *
     * @param url The URL to make relative
     */
    function getRelativeUrl(url) {
        return url.replace(document.location.protocol + "//" + document.location.hostname, "");
    }
    exports.getRelativeUrl = getRelativeUrl;
    /**
     * Retrieves the node with the given title from a collection of SP.NavigationNode
     */
    function getNodeFromCollectionByTitle(nodeCollection, title) {
        var f = jQuery.grep(nodeCollection, function (val) {
            return val.get_title() === title;
        });
        return f[0] || null;
    }
    exports.getNodeFromCollectionByTitle = getNodeFromCollectionByTitle;
    ;
    /**
     * Replaces URL tokens in a string
     */
    function replaceUrlTokens(url) {
        return url.replace(/{site}/g, _spPageContextInfo.webAbsoluteUrl)
            .replace(/{sitecollection}/g, _spPageContextInfo.siteAbsoluteUrl);
    }
    exports.replaceUrlTokens = replaceUrlTokens;
    ;
    function encodePropertyKey(propKey) {
        var bytes = [];
        for (var i = 0; i < propKey.length; ++i) {
            bytes.push(propKey.charCodeAt(i));
            bytes.push(0);
        }
        var b64encoded = window.btoa(String.fromCharCode.apply(null, bytes));
        return b64encoded;
    }
    exports.encodePropertyKey = encodePropertyKey;
});
